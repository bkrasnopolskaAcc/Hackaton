# CLAUDE.md — CarRetailSystem Modernization

This file provides all context needed to work on the CarRetailSystem migration project.

## What This Project Is

Legacy monolithic car retail sales system (15+ years old, ~2,000 LOC) being modernized from Classic ASP + VB6 COM + SQL Server 7.0 to ASP.NET Core 8 + React + SQL Server 2022.

Full documentation is in `CarRetailSystem/Documentation/`.

---

## Repository Structure

```
Hackaton/
├── CLAUDE.md                          ← you are here
├── README.md                          ← project overview
└── CarRetailSystem/
    ├── ASP/
    │   ├── pages/
    │   │   ├── login.asp              ← auth (SQL injection at line 28)
    │   │   ├── inventory.asp          ← car CRUD (SQL injection at lines 59-66)
    │   │   └── index.asp              ← dashboard (XSS at line 40)
    │   └── includes/
    │       ├── config.asp             ← hardcoded sa/Admin@123 at line 7
    │       ├── security.asp           ← auth helpers, weak sanitization
    │       ├── session.asp            ← session lifecycle
    │       └── utils.asp              ← 40+ helper functions (HTMLEncode exists here)
    ├── VB6Components/
    │   ├── CarInventory.vb            ← stock queries, SQL injection at lines 50,70,83
    │   └── SalesOrder.vb             ← transactions, SQL injection at lines 59-60,78,105
    ├── SQL/
    │   └── database_setup.sql        ← full schema + stored procedures + sample data
    ├── docs/                          ← legacy docs (architecture, DB schema, COM ref)
    └── Documentation/                 ← modernization analysis (read these first)
        ├── 01_CURRENT_STATE.md
        ├── 02_SECURITY_VULNERABILITIES.md
        ├── 03_TARGET_ARCHITECTURE.md
        ├── 04_MIGRATION_STRATEGY.md
        └── 05_QUICK_WINS.md
```

---

## Business Domain

**Five core entities:**

| Entity | Legacy Table | Key Fields |
|--------|-------------|-----------|
| Cars / Inventory | `Cars` | CarID, Make, Model, Year, VIN (UNIQUE), Price, Stock, IsActive |
| Customers | `Customers` | CustomerID, FirstName, LastName, Email, Phone, IsActive |
| Sales | `Sales` | SalesID, CustomerID (FK), CarID (FK), SalesPrice, SalesDate, SalespersonID (FK) |
| Users | `Users` | UserID, UserName, Password (plain text!), Role (Admin/Salesperson/Manager) |
| Audit Log | `Inventory_Log` | LogID, CarID (FK), Action (ADD/SALE/UPDATE/DELETE/AUDIT), Quantity, ChangedBy |

**Five business workflows to preserve:**
1. Login with role-based access (Admin, Salesperson, Manager)
2. Inventory browse — filterable by Make and Price, only Stock > 0
3. Sales order — atomic: insert Sale + decrement Cars.Stock + insert Inventory_Log
4. Customer management — CRUD
5. Reports — monthly sales summary, inventory status, top sellers

**Business rules that must survive migration:**
- Tax rate = 10% (hardcoded in `SalesOrder.vb:CalculateTax`, move to `appsettings.json`)
- Sales order is a single atomic transaction (legacy uses `BeginTrans/CommitTrans/RollbackTrans`)
- Inventory changes must always write an audit record to `Inventory_Log`
- Soft-delete pattern: `IsActive` flag on Cars and Customers (do not hard-delete)
- VIN must be unique (enforced at DB level)

---

## Target Architecture

**Stack:** Modular Monolith — ASP.NET Core 8 Web API + React 18 + TypeScript

```
React 18 (SPA)
    │ HTTPS + JWT Bearer
ASP.NET Core 8 Web API
    ├── Auth Module       → /auth/login, /auth/refresh, /auth/logout
    ├── Inventory Module  → /cars/**
    ├── Sales Module      → /sales/**
    ├── Customers Module  → /customers/**
    └── Reports Module    → /reports/**
    │
    ├── EF Core 8 → SQL Server 2022
    ├── Redis (cache, 5-min TTL on reports + dashboard stats)
    └── Serilog → Azure Application Insights
```

**New solution projects:**
- `CarRetailSystem.Api` — ASP.NET Core 8 Web API
- `CarRetailSystem.Web` — React + TypeScript + Vite + Tailwind CSS

**Project layout for the API:**
```
CarRetailSystem.Api/
├── Modules/{Auth,Inventory,Sales,Customers,Reports}/
│   ├── *Controller.cs
│   ├── *Service.cs
│   ├── Dtos/
│   └── Models/
├── Infrastructure/
│   ├── Data/AppDbContext.cs + Migrations/
│   ├── Auth/JwtTokenService.cs
│   └── Caching/CacheService.cs
├── Common/Middleware/{ExceptionHandler,Audit}Middleware.cs
└── Program.cs
```

---

## Migration Strategy

**Pattern:** Strangler Fig — legacy stays running, new system grows beside it, traffic shifts gradually.

| Phase | Weeks | Goal |
|-------|-------|------|
| 1 — Foundation | 1–2 | Project scaffolding, EF Core baseline migration, legacy hotfixes |
| 2 — Auth | 3–4 | JWT auth, bcrypt passwords, on-login password re-hash |
| 3 — Core API | 5–8 | All endpoints, service layer, tests (xUnit + Testcontainers) |
| 4 — Frontend | 9–12 | React SPA, all pages, E2E tests (Playwright) |
| 5 — Cutover | 13–14 | Blue-green deploy, DNS switch, decommission legacy |

**Password migration (no forced reset):**
```
Add columns: Users.PasswordHash (bcrypt), Users.PasswordMigrated (bit)
On login:
  if PasswordMigrated = 0 → validate plain text → if valid: bcrypt hash + set flag
  if PasswordMigrated = 1 → validate bcrypt hash
After all users migrated: drop plain-text Password column
```

**Rollback:** At every phase the legacy system remains deployed. Rollback = point DNS back. After the 48h monitoring window in Week 14, legacy is archived.

---

## Stored Procedures → Service Methods Mapping

Every stored procedure must have a direct equivalent in the new service layer:

| Legacy SP | New Method |
|-----------|-----------|
| `sp_GetAvailableCars(@Make, @MaxPrice)` | `InventoryService.GetAvailableCarsAsync(make, maxPrice)` |
| `sp_CreateSalesOrder(@CustomerID, @CarID, @SalePrice, @SalespersonID)` | `SalesService.CreateOrderAsync(dto)` with `IDbContextTransaction` |
| `sp_UpdateInventory(@CarID, @Quantity)` | `InventoryService.UpdateStockAsync(carID, quantity)` |
| `sp_GetCustomerSalesHistory(@CustomerID)` | `SalesService.GetCustomerHistoryAsync(customerId)` |
| `sp_GenerateMonthlySalesReport(@Year, @Month)` | `ReportService.GetMonthlySummaryAsync(year, month)` |

The `TR_Cars_Audit` trigger (fires on Cars UPDATE/DELETE) must be replaced by `AuditMiddleware` writing to `Inventory_Log`.

---

## API Endpoints

### Auth
| Method | Path | Auth | Notes |
|--------|------|------|-------|
| POST | `/auth/login` | None | Returns `{ accessToken, refreshToken }` |
| POST | `/auth/refresh` | None | Exchanges refresh token |
| POST | `/auth/logout` | Bearer | Revokes refresh token |

### Inventory
| Method | Path | Auth | Notes |
|--------|------|------|-------|
| GET | `/cars` | Bearer | Paginated, filter: `?make=&maxPrice=&inStock=true` |
| GET | `/cars/{id}` | Bearer | — |
| POST | `/cars` | Admin | — |
| PUT | `/cars/{id}` | Admin | — |
| DELETE | `/cars/{id}` | Admin | Soft-delete (sets IsActive = false) |
| GET | `/cars/{id}/stock` | Bearer | — |

### Sales
| Method | Path | Auth | Notes |
|--------|------|------|-------|
| POST | `/sales` | Salesperson/Admin | Atomic transaction |
| GET | `/sales/{id}` | Bearer | — |
| GET | `/customers/{id}/sales` | Bearer | Purchase history |

### Customers
| Method | Path | Auth | Notes |
|--------|------|------|-------|
| GET | `/customers` | Bearer | Paginated |
| GET | `/customers/{id}` | Bearer | — |
| POST | `/customers` | Salesperson/Admin | — |
| PUT | `/customers/{id}` | Salesperson/Admin | — |
| DELETE | `/customers/{id}` | Admin | Soft-delete |

### Reports
| Method | Path | Auth | Notes |
|--------|------|------|-------|
| GET | `/reports/monthly-sales` | Manager/Admin | Redis cached 5 min |
| GET | `/reports/inventory` | Bearer | Redis cached 5 min |
| GET | `/reports/top-sellers` | Bearer | Redis cached 5 min |

---

## Security Requirements

Every item below maps to a vulnerability in `Documentation/02_SECURITY_VULNERABILITIES.md`.

| Requirement | Implementation |
|-------------|---------------|
| No SQL injection | EF Core only — never raw string SQL |
| No hardcoded credentials | All secrets from env vars or Azure Key Vault; nothing in `appsettings.json` |
| Hashed passwords | ASP.NET Identity bcrypt, cost factor 12 |
| HTTPS only | `app.UseHttpsRedirection()` + `app.UseHsts()` |
| No XSS | React escapes by default; never use `dangerouslySetInnerHTML` |
| No CSRF | JWT in `Authorization` header (not cookie) — CSRF does not apply |
| Rate limiting | `app.UseRateLimiter()` on `POST /auth/login` — max 5/min per IP |
| Structured logging | Serilog; never log passwords or tokens |
| Global error handling | `ExceptionHandlerMiddleware` — never expose stack traces to client |
| Minimum password policy | 12 chars, uppercase, lowercase, digit, symbol (ASP.NET Identity options) |

---

## Coding Conventions (New Codebase)

- **No raw SQL strings** — EF Core LINQ or `FromSqlInterpolated` (parameterized) only
- **Async everywhere** — all service and repository methods are `async Task<T>`
- **FluentValidation** for all request DTOs — no manual validation in controllers
- **No comments** unless the WHY is non-obvious; well-named identifiers are sufficient
- **Soft-delete** — set `IsActive = false`, never `DELETE FROM`
- **No feature flags** or backwards-compat shims — just change the code
- **Tests:** xUnit for units (Moq for dependencies), Testcontainers for integration tests
- Secrets: never in source — use `dotnet user-secrets` locally, Key Vault in production
- Docker: every service has a `Dockerfile`; local dev uses `docker-compose.yml`

---

## Critical Gotchas

1. **Three VB6 DLLs have no source** (`CustomerMgmt.dll`, `ReportGenerator.dll`, `Utilities.dll`) — behavior must be reverse-engineered from `docs/COM_COMPONENTS.md` and tested against the live legacy system before decommissioning.

2. **The existing stored procedures are correctly parameterized** — the SQL injection exists only in the ASP/VB6 layer that bypasses them. The SPs themselves are safe to reference as the source of truth for business logic.

3. **`sp_CreateSalesOrder` uses an explicit DB transaction** — the new `SalesService.CreateOrderAsync()` must also wrap the entire operation in `IDbContextTransaction`, not rely on EF Core's default auto-commit.

4. **Session timeout is 30 minutes** in the legacy system (`config.asp`). JWT access tokens should be 15 minutes; refresh tokens 7 days.

5. **The `Inventory_Log` table is both written by the SP and by the `TR_Cars_Audit` trigger** — the trigger handles UPDATE/DELETE on Cars; the SP handles SALE events. Both paths must exist in the new system (service + audit middleware).

6. **Sample data credentials** (`admin/admin123`, `john/pass123`) exist in `SQL/database_setup.sql`. These are for development seed data only — remove from any production deployment.

---

## Definition of Done

Migration is complete when all of the following are true:

- [ ] All 5 ASP pages replaced by React equivalents
- [ ] All 5 stored procedures replaced by service layer methods
- [ ] All 17 security vulnerabilities resolved (see `Documentation/02_SECURITY_VULNERABILITIES.md`)
- [ ] Unit test coverage ≥ 80% on service layer
- [ ] E2E tests (Playwright) cover: login, add car, create sale, view monthly report
- [ ] Zero raw string SQL in new codebase
- [ ] Zero hardcoded credentials
- [ ] Application Insights monitoring active with alert policies
- [ ] IIS Classic ASP application pool decommissioned
- [ ] VB6 DLL registrations removed (`regsvr32 /u`)
