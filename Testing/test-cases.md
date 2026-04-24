# Test Cases for CarRetailSystem

Test cases are written against **business behavior**, not implementation. They apply regardless of the underlying tech stack (Classic ASP/VB6, .NET, Node.js, etc.). References to specific files or components are replaced with functional layer names.

---

## 1. Authentication & Login

| ID | Test Case | Input | Expected Result |
|----|-----------|-------|-----------------|
| AUTH-01 | Valid admin login | username=`admin`, password=`admin123` | Authenticated; user context reflects Role=Admin |
| AUTH-02 | Valid salesperson login | username=`john`, password=`pass123` | Authenticated; user context reflects Role=Salesperson |
| AUTH-03 | Wrong password | username=`admin`, password=`wrongpass` | Authentication rejected; generic failure message shown; no session created |
| AUTH-04 | Non-existent user | username=`ghost`, password=`any` | Same failure message as AUTH-03 (no user enumeration) |
| AUTH-05 | Empty username | username=``, password=`admin123` | Rejected before any credential lookup |
| AUTH-06 | Empty password | username=`admin`, password=`` | Rejected before any credential lookup |
| AUTH-07 | Both fields empty | username=``, password=`` | Rejected before any credential lookup |
| AUTH-08 | SQL injection in username | username=`' OR '1'='1`, password=`x` | Authentication rejected; no data leak |
| AUTH-09 | SQL injection in password | username=`admin`, password=`' OR '1'='1' --` | Authentication rejected; no data leak |

---

## 2. Session Management

| ID | Test Case | Scenario | Expected Result |
|----|-----------|----------|-----------------|
| SESS-01 | Access protected resource without login | Request any authenticated route with no active session | Redirected to login; resource not served |
| SESS-02 | Session expires after inactivity | Last activity > 30 minutes ago | Session treated as expired; user redirected to login |
| SESS-03 | Session valid within timeout window | Last activity < 30 minutes ago | Session accepted; access granted |
| SESS-04 | Activity resets inactivity timer | Any authenticated action taken | Inactivity clock resets to zero |
| SESS-05 | Logout clears session | User logs out | All session state destroyed; subsequent requests treated as unauthenticated |
| SESS-06 | Time remaining calculation — active session | Last activity 20 min ago, timeout 30 min | Returns 10 minutes remaining |
| SESS-07 | Time remaining calculation — expired session | Last activity 35 min ago, timeout 30 min | Returns 0 (not a negative value) |

---

## 3. Authorization / Role Checks

| ID | Test Case | Role | Expected Result |
|----|-----------|------|-----------------|
| AUTHZ-01 | Admin accesses inventory management | Admin | Full access; create/edit/delete actions available |
| AUTHZ-02 | Salesperson accesses inventory management | Salesperson | Access denied; appropriate error message shown |
| AUTHZ-03 | Admin role check — Admin user | Admin | Role check returns true |
| AUTHZ-04 | Admin role check — non-Admin user | Salesperson | Role check returns false |
| AUTHZ-05 | Salesperson role check — Salesperson user | Salesperson | Role check returns true |

---

## 4. Inventory Management

| ID | Test Case | Input | Expected Result |
|----|-----------|-------|-----------------|
| INV-01 | Add valid car | Make=Toyota, Model=Camry, Year=2020, Color=Blue, VIN=12345678901234567, Price=25000, Stock=5 | Car created successfully; appears in inventory list |
| INV-02 | Add car with duplicate VIN | VIN already exists in system | Rejected; uniqueness constraint enforced; no duplicate created |
| INV-03 | Add car with zero price | Price=0 | **Should be rejected** (below minimum $5,000) |
| INV-04 | Add car below minimum price | Price=4999 | **Should be rejected** (minimum allowed: $5,000) |
| INV-05 | Add car above maximum price | Price=200000 | **Should be rejected** (maximum allowed: $150,000) |
| INV-06 | Add car with invalid VIN length | VIN shorter or longer than 17 characters | **Should be rejected**; VIN must be exactly 17 characters |
| INV-07 | View inventory — cars present | Inventory has records | All cars listed with: ID, Make, Model, Year, Color, VIN, Price, Stock |
| INV-08 | View inventory — empty | No cars in system | Empty state message displayed; no error |
| INV-09 | List available cars — no filters | — | Returns all cars with Stock > 0 |
| INV-10 | List available cars — filter by make | make=Toyota | Returns only in-stock Toyota cars |
| INV-11 | List available cars — filter by max price | maxPrice=30000 | Returns only in-stock cars priced at or below $30,000 |
| INV-12 | Get stock level — existing car | Valid car ID | Returns correct current stock count |
| INV-13 | Get stock level — nonexistent car | Invalid car ID | Returns 0 or appropriate not-found response |
| INV-14 | Decrease stock — valid operation | Valid car ID, quantity=2 | Stock reduced by 2; change recorded in audit log |
| INV-15 | Decrease stock — invalid car ID | car ID not in system | Stock unchanged; operation fails gracefully |
| INV-16 | Decrease stock below zero | Current stock=1, quantity=3 | **Should be rejected**; stock must not go negative |

---

## 5. Sales Order Processing

| ID | Test Case | Input | Expected Result |
|----|-----------|-------|-----------------|
| SALE-01 | Create valid sales order | customerID=1, carID=1, salePrice=25000, salespersonID=2 | Order created; new order ID returned; stock decremented by 1; audit log entry created |
| SALE-02 | Create order — invalid customer | customerID=0 or non-existent | Rejected; no data changes |
| SALE-03 | Create order — invalid car | carID=0 or non-existent | Rejected; no data changes |
| SALE-04 | Create order — negative price | salePrice=-100 | Rejected; no data changes |
| SALE-05 | Create order — zero price | salePrice=0 | Rejected; no data changes |
| SALE-06 | Create order — out-of-stock car | Car with Stock=0 | **Should be rejected**; sale not permitted without available stock |
| SALE-07 | Stock atomicity on sale | Stock starts at 3; 1 sale created | Final stock=2; exactly one audit log entry with action SALE |
| SALE-08 | Transaction rollback on mid-operation failure | Error occurs after order insert but before stock update | No partial data committed; stock and order table both unchanged |
| SALE-09 | Get sales history — customer with sales | Existing customer ID | Returns list with order ID, date, price, car make/model/year |
| SALE-10 | Get sales history — customer with no sales | New customer ID | Returns empty list; no error |

---

## 6. Tax & Pricing Calculations

| ID | Test Case | Input | Expected Result |
|----|-----------|-------|-----------------|
| TAX-01 | Tax on standard price | salePrice=25000 | Tax amount = 2500.00 |
| TAX-02 | Total price with tax | salePrice=25000 | Total = 27500.00 |
| TAX-03 | Tax rate is exactly 10% | Any positive price | Tax = price × 0.10 |
| TAX-04 | Tax on zero price | salePrice=0 | Tax = 0.00 |
| TAX-05 | Currency formatting — whole number | amount=25000 | Displayed as "$25,000.00" |
| TAX-06 | Currency formatting — decimal amount | amount=25000.5 | Displayed as "$25,000.50" |

---

## 7. Input Validation

| ID | Test Case | Input | Expected Result |
|----|-----------|-------|-----------------|
| UTIL-01 | Email — valid format | `user@example.com` | Accepted |
| UTIL-02 | Email — missing domain | `user@` | Rejected |
| UTIL-03 | Email — missing TLD | `user@example` | Rejected |
| UTIL-04 | Email — empty string | `` | Rejected |
| UTIL-05 | Phone — 10 digits, no formatting | `1234567890` | Accepted |
| UTIL-06 | Phone — formatted with dashes | `123-456-7890` | Accepted (formatting stripped before validation) |
| UTIL-07 | Phone — fewer than 10 digits | `123456789` | Rejected |
| UTIL-08 | VIN — exactly 17 characters | `12345678901234567` | Accepted |
| UTIL-09 | VIN — 16 characters | `1234567890123456` | Rejected |
| UTIL-10 | VIN — 18 characters | `123456789012345678` | Rejected |
| UTIL-11 | Numeric rounding | Round(2.555, 2) | Returns 2.56 |
| UTIL-12 | Date formatting | Any valid date | Output in MM/DD/YYYY format |
| UTIL-13 | Input sanitization — trims whitespace | `  hello  ` | Returns `hello` |
| UTIL-14 | Numeric parse — valid number string | `"42"` | Returns 42 |
| UTIL-15 | Numeric parse — non-numeric string | `"abc"` | Returns 0 |

---

## 8. Dashboard

| ID | Test Case | Scenario | Expected Result |
|----|-----------|----------|-----------------|
| DASH-01 | Authenticated user views dashboard | Valid session | Displays: total cars in inventory, total customers, sales count for current month |
| DASH-02 | Unauthenticated access | No active session | Login prompt shown; no data displayed |
| DASH-03 | Database unavailable | DB connection fails | Graceful error message shown; no unhandled exception |

---

## 9. Data Layer / Persistence

| ID | Test Case | Operation | Expected Result |
|----|-----------|-----------|-----------------|
| DB-01 | Fetch available cars — no filter | — | Returns all cars with Stock > 0 and active status |
| DB-02 | Fetch available cars — filter by make | make=Toyota | Returns only active, in-stock Toyota cars |
| DB-03 | Create sales order — success | Valid customer, car, price, salesperson | Order row created; stock decremented; audit log row inserted; new order ID returned |
| DB-04 | Create sales order — referential integrity failure | Non-existent car ID | Transaction rolled back; no partial data; error indicated |
| DB-05 | Update inventory quantity | Valid car ID, quantity=2 | Stock reduced by 2; audit log entry with action UPDATE |
| DB-06 | Fetch customer sales history | Valid customer ID | Returns joined order + car + salesperson data ordered by date descending |
| DB-07 | Audit log on record update | Car record updated | Audit log entry created with action AUDIT |
| DB-08 | Audit log on record delete | Car record deleted | Audit log entry created with action AUDIT |
| DB-09 | Monthly sales report | Year=2024, Month=3 | Returns: total orders count, total revenue, average sale price for that month |

---

## Known Business Rule Gaps (Baseline for Migration)

The following rules are **specified but not currently enforced**. Tests against the legacy system will confirm broken behavior; the migrated system must fix them:

| Gap | Business Rule | Current Behavior |
|-----|--------------|-----------------|
| Price range | Cars must be priced between $5,000 and $150,000 | Any price accepted, including zero and negative |
| VIN validation | VIN must be exactly 17 characters | Any string accepted on insert |
| Stock floor | Stock must not go below zero | No lower-bound check; negative stock possible |
| Stock pre-check on sale | Sale requires Stock > 0 before proceeding | No pre-check; sale can be created against an out-of-stock car |
