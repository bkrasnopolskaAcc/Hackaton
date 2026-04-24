# Current Behaviour Baseline — CarRetailSystem

This document records **what the system actually does today**, not what it should do. It serves as a regression baseline: after any change, run through these cases to confirm no unintended behaviour was altered.

Statuses:
- **CORRECT** — behaviour matches the specification in `test-cases.md`
- **GAP** — specification says one thing; current system does another (documented gaps from `test-cases.md`)
- **RISK** — security or data-integrity concern, but currently accepted behaviour

---

## 1. Authentication & Login

| ID | Scenario | Input | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| AUTH-01 | Valid admin login | username=`admin`, password=`admin123` | Session created with UserID=1, Role=Admin; redirected to dashboard | CORRECT |
| AUTH-02 | Valid salesperson login | username=`john`, password=`pass123` | Session created with UserID=2, Role=Salesperson; redirected to dashboard | CORRECT |
| AUTH-03 | Wrong password | username=`admin`, password=`wrongpass` | Generic "Invalid username or password" shown; no session created | CORRECT |
| AUTH-04 | Non-existent user | username=`ghost`, password=`any` | Same generic message as AUTH-03; no user enumeration | CORRECT |
| AUTH-05 | Empty username | username=``, password=`admin123` | Form rejected before credential lookup; error shown | CORRECT |
| AUTH-06 | Empty password | username=`admin`, password=`` | Form rejected before credential lookup; error shown | CORRECT |
| AUTH-07 | Both fields empty | username=``, password=`` | Form rejected before credential lookup; error shown | CORRECT |
| AUTH-08 | SQL injection in username | username=`' OR '1'='1`, password=`x` | Authentication rejected (AND password clause prevents bypass with this payload) | CORRECT |
| AUTH-09 | SQL injection in password | username=`admin`, password=`' OR '1'='1' --` | **Authentication succeeds**; logged in as admin (UserID=1) — SQL comment neutralises password check | RISK / GAP |

---

## 2. Session Management

| ID | Scenario | Setup | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| SESS-01 | Access protected resource without session | No active session | Redirected to login page; protected content not served | CORRECT |
| SESS-02 | Session expires after inactivity | LastActivity > 30 minutes ago | Session treated as expired; user redirected to login | CORRECT |
| SESS-03 | Session valid within timeout window | LastActivity < 30 minutes ago | Session accepted; access granted | CORRECT |
| SESS-04 | Activity resets inactivity timer | Any authenticated action taken | `LastActivity` reset to `Now()`; 30-minute window restarts | CORRECT |
| SESS-05 | Logout clears session | User logs out | All session variables cleared; `Session.Abandon()` called; subsequent requests treated as unauthenticated | CORRECT |
| SESS-06 | Time remaining — active session | LastActivity 20 min ago, timeout 30 min | Returns 10 minutes remaining | CORRECT |
| SESS-07 | Time remaining — expired session | LastActivity 35 min ago, timeout 30 min | Returns 0 (clamped; no negative value returned) | CORRECT |

---

## 3. Authorization / Role Checks

| ID | Scenario | Role | Current Actual Behaviour | Status |
|----|----------|------|--------------------------|--------|
| AUTHZ-01 | Admin accesses inventory management | Admin | Full access; add/edit/delete actions visible | CORRECT |
| AUTHZ-02 | Salesperson accesses inventory management | Salesperson | Access denied; redirected to login (same redirect as unauthenticated) | CORRECT |
| AUTHZ-03 | Admin role check — Admin user | Admin | `UserIsAdmin()` returns true | CORRECT |
| AUTHZ-04 | Admin role check — non-Admin user | Salesperson | `UserIsAdmin()` returns false | CORRECT |
| AUTHZ-05 | Salesperson role check — Salesperson user | Salesperson | `UserIsSalesperson()` returns true | CORRECT |

---

## 4. Inventory Management

| ID | Scenario | Input | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| INV-01 | Add valid car | Make=Toyota, Model=Camry, Year=2020, Color=Blue, VIN=12345678901234567, Price=25000, Stock=5 | Car inserted; appears in inventory list | CORRECT |
| INV-02 | Add car with duplicate VIN | VIN already in database | INSERT fails; database UNIQUE constraint rejects it; error shown | CORRECT |
| INV-03 | Add car with zero price | Price=0 | **Car is inserted successfully**; no price range check performed | GAP |
| INV-04 | Add car below minimum price | Price=4999 | **Car is inserted successfully**; `MIN_CAR_PRICE` constant defined but not enforced | GAP |
| INV-05 | Add car above maximum price | Price=200000 | **Car is inserted successfully**; `MAX_CAR_PRICE` constant defined but not enforced | GAP |
| INV-06 | Add car with invalid VIN length | VIN ≠ 17 characters | **Car is inserted successfully**; `ValidateVIN()` exists but is not called on insert | GAP |
| INV-07 | View inventory — cars present | Records in Cars table | All cars listed with CarID, Make, Model, Year, Color, VIN, Price, Stock | CORRECT |
| INV-08 | View inventory — empty | No cars in table | Empty table rendered; no error or unhandled exception | CORRECT |
| INV-09 | List available cars — no filters | — | Returns all records where `Stock > 0` AND `IsActive = 1` | CORRECT |
| INV-10 | List available cars — filter by make | make=Toyota | Returns only in-stock Toyota cars | CORRECT |
| INV-11 | List available cars — filter by max price | maxPrice=30000 | Returns only in-stock cars with Price ≤ 30000 | CORRECT |
| INV-12 | Get stock level — existing car | Valid CarID | Returns current integer stock count | CORRECT |
| INV-13 | Get stock level — nonexistent car | Invalid CarID | Returns 0 | CORRECT |
| INV-14 | Decrease stock — valid operation | Valid CarID, quantity=2 | `Stock` reduced by 2; audit log row inserted with action=`SALE` | CORRECT |
| INV-15 | Decrease stock — invalid car ID | CarID not in database | SQL UPDATE affects 0 rows; no error raised; function returns without failure | GAP |
| INV-16 | Decrease stock below zero | Stock=1, quantity=3 | **Stock goes negative** (e.g. Stock=-2); no lower-bound check; audit log still written | GAP |

---

## 5. Sales Order Processing

| ID | Scenario | Input | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| SALE-01 | Create valid sales order | customerID=1, carID=1, salePrice=25000, salespersonID=2 | Order row inserted; new SalesID returned (≥ 1); `Cars.Stock` decremented by 1; `Inventory_Log` row inserted with action=`SALE` | CORRECT |
| SALE-02 | Create order — zero customerID | customerID=0 | Rejected by input validation; returns 0; no data written | CORRECT |
| SALE-03 | Create order — zero carID | carID=0 | Rejected by input validation; returns 0; no data written | CORRECT |
| SALE-04 | Create order — negative price | salePrice=-100 | Rejected by input validation; returns 0; no data written | CORRECT |
| SALE-05 | Create order — zero price | salePrice=0 | Rejected by input validation; returns 0; no data written | CORRECT |
| SALE-06 | Create order — out-of-stock car | Car with Stock=0 | **Order is created**; Stock decremented to -1; no pre-check on Stock before sale | GAP |
| SALE-07 | Stock atomicity on sale | Stock starts at 3; 1 sale created | Final Stock=2; exactly one `Inventory_Log` entry with action=`SALE` | CORRECT |
| SALE-08 | Transaction rollback on mid-operation failure | Simulated error after Sales INSERT | `ROLLBACK TRAN` executed; both Sales row and Stock update are undone; data integrity preserved | CORRECT |
| SALE-09 | Get sales history — customer with sales | Existing CustomerID | Returns list of SalesID, SalesDate, SalesPrice, Make, Model, Year; ordered by SalesDate DESC | CORRECT |
| SALE-10 | Get sales history — customer with no sales | New CustomerID | Returns empty recordset; no error | CORRECT |

---

## 6. Tax & Pricing Calculations

| ID | Scenario | Input | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| TAX-01 | Tax on standard price | salePrice=25000 | Returns 2500.00 | CORRECT |
| TAX-02 | Total price with tax | salePrice=25000 | Returns 27500.00 | CORRECT |
| TAX-03 | Tax rate is exactly 10% | Any positive price | Tax = price × 0.10 (hard-coded; no external config) | CORRECT |
| TAX-04 | Tax on zero price | salePrice=0 | Returns 0.00 | CORRECT |
| TAX-05 | Currency formatting — whole number | amount=25000 | Displays `$25,000.00` | CORRECT |
| TAX-06 | Currency formatting — decimal amount | amount=25000.5 | Displays `$25,000.50` | CORRECT |

---

## 7. Input Validation

| ID | Scenario | Input | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| UTIL-01 | Email — valid format | `user@example.com` | Accepted | CORRECT |
| UTIL-02 | Email — missing domain | `user@` | Rejected by regex | CORRECT |
| UTIL-03 | Email — missing TLD | `user@example` | Rejected by regex | CORRECT |
| UTIL-04 | Email — empty string | `` | Rejected by regex | CORRECT |
| UTIL-05 | Phone — 10 digits, no formatting | `1234567890` | Accepted | CORRECT |
| UTIL-06 | Phone — formatted with dashes | `123-456-7890` | Non-numeric chars stripped; 10 digits remain; accepted | CORRECT |
| UTIL-07 | Phone — fewer than 10 digits | `123456789` | Rejected (length < 10 after stripping) | CORRECT |
| UTIL-08 | VIN — exactly 17 characters | `12345678901234567` | Accepted | CORRECT |
| UTIL-09 | VIN — 16 characters | `1234567890123456` | Rejected by `ValidateVIN()` | CORRECT |
| UTIL-10 | VIN — 18 characters | `123456789012345678` | Rejected by `ValidateVIN()` | CORRECT |
| UTIL-11 | Numeric rounding | `Round(2.555, 2)` | Returns 2.56 (uses `Int(value * 10^decimals + 0.5) / 10^decimals`) | CORRECT |
| UTIL-12 | Date formatting | Any valid date | Output in `MM/DD/YYYY` format | CORRECT |
| UTIL-13 | Input sanitisation — trims whitespace | `  hello  ` | Returns `hello`; no other sanitisation applied | CORRECT |
| UTIL-14 | Numeric parse — valid number string | `"42"` | Returns 42 | CORRECT |
| UTIL-15 | Numeric parse — non-numeric string | `"abc"` | Returns 0 | CORRECT |

---

## 8. Dashboard

| ID | Scenario | Setup | Current Actual Behaviour | Status |
|----|----------|-------|--------------------------|--------|
| DASH-01 | Authenticated user views dashboard | Valid session | Displays: total count of all Cars rows, total count of all Customers rows, count of Sales rows where `MONTH(SalesDate) = MONTH(GETDATE())` | CORRECT |
| DASH-02 | Unauthenticated access | No session | Login prompt shown; no data queries executed | CORRECT |
| DASH-03 | Database unavailable | DB connection fails | "Cannot connect to database" message shown; no unhandled exception or stack trace | CORRECT |

---

## 9. Data Layer / Persistence

| ID | Scenario | Operation | Current Actual Behaviour | Status |
|----|----------|-----------|--------------------------|--------|
| DB-01 | Fetch available cars — no filter | `sp_GetAvailableCars` (no param) | Returns all rows where `Stock > 0` AND `IsActive = 1`; ordered by Make, Model | CORRECT |
| DB-02 | Fetch available cars — filter by make | `sp_GetAvailableCars @Make='Toyota'` | Returns only active, in-stock Toyota rows | CORRECT |
| DB-03 | Create sales order — success | Valid customerID, carID, price, salespersonID | Sales row inserted; Stock decremented; Inventory_Log row added; new SalesID returned | CORRECT |
| DB-04 | Create sales order — referential integrity failure | Non-existent carID | Foreign key violation causes transaction rollback; returns -1; no partial data committed | CORRECT |
| DB-05 | Update inventory quantity | `sp_UpdateInventory @CarID, @Quantity=2` | Stock reduced by 2; Inventory_Log row inserted; **no transaction wrapping this procedure** | CORRECT |
| DB-06 | Fetch customer sales history | `sp_GetCustomerSalesHistory @CustomerID` | Returns SalesID, SalesDate, SalesPrice, Make, Model, Year, UserName; ordered by SalesDate DESC | CORRECT |
| DB-07 | Audit log on record update | UPDATE any column on Cars row | `TR_Cars_Audit` trigger fires; Inventory_Log row inserted with action=`AUDIT` | CORRECT |
| DB-08 | Audit log on record delete | DELETE Cars row | `TR_Cars_Audit` trigger fires; Inventory_Log row inserted with action=`AUDIT` | CORRECT |
| DB-09 | Monthly sales report | `sp_GenerateMonthlySalesReport @Year=2024, @Month=3` | Returns: total order count, total revenue (SUM), average sale price for that month | CORRECT |

---

## Summary of Known Gaps (Current Broken Behaviours)

These cases are the ones where current behaviour deviates from the specification. Any change to the system should either preserve this broken behaviour knowingly (legacy compatibility) or fix it deliberately — and both outcomes must be explicit.

| ID | Gap | What Happens Now | What Should Happen |
|----|-----|------------------|--------------------|
| INV-03 | Zero price accepted | Car inserted at Price=0 | Reject; minimum price is $5,000 |
| INV-04 | Below minimum price accepted | Car inserted at any price ≥ 0 | Reject prices below $5,000 |
| INV-05 | Above maximum price accepted | Car inserted at any price | Reject prices above $150,000 |
| INV-06 | Invalid VIN length accepted | Any string stored as VIN | Reject if length ≠ 17 |
| INV-15 | Update stock on missing car silently succeeds | 0 rows affected, no error | Return failure indicator |
| INV-16 | Stock goes negative | No lower-bound check | Reject if result would be < 0 |
| SALE-06 | Sale created against out-of-stock car | Order created, stock goes to -1 | Pre-check Stock > 0; reject if not |
| AUTH-09 | SQL injection in password bypasses auth | Attacker authenticated as admin | Parameterise queries |
