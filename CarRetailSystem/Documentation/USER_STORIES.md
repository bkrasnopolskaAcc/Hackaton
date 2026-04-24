# CarRetailSystem — User Stories
**IT Modernization Project**
Author: Michal Szymczyk | Date: 24/04/2026

---

---

# US-001 — CI/CD Pipeline & Project Skeleton Setup

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-001 |
| **Story Title** | CI/CD Pipeline & Project Skeleton Setup |
| **Epic / Module** | Phase 1 — Foundation |
| **Sprint / Release** | Sprint 1 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT / Development Infrastructure |
| **Business Description** | The current CarRetailSystem has no automated build or deployment process — releases are performed manually by copying files and running scripts, with no rollback capability. This story establishes the foundational project structure and automated pipeline so that all subsequent modernization work can be delivered safely and repeatably. |
| **Business Value** | Eliminates manual deployment errors, enables fast and safe delivery of all subsequent modernization stories, and provides a rollback mechanism that the legacy system currently lacks. |

## 3. USER STORY

> **As a** development team lead,
> **I want** a version-controlled project with an automated build, test, and deployment pipeline,
> **so that** every code change is validated and deployed consistently without manual intervention.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | New project scaffolding | ASP.NET Core 8 Web API project and React + TypeScript frontend project created and committed to the repository |
| 2 | Docker Compose environment | Local development environment with API, frontend, SQL Server, and Redis containers defined in a single compose file |
| 3 | GitHub Actions CI/CD pipeline | Automated pipeline that builds, tests, packages Docker images, and deploys to the target environment on every push to main |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A developer pushes code to the main branch | The GitHub Actions pipeline is triggered | The pipeline builds the API and frontend, runs all tests, and reports pass/fail within 10 minutes |
| Must Have | All pipeline checks pass | The pipeline completes successfully | Docker images are published to the container registry and deployed to the staging environment automatically |
| Must Have | A pipeline run fails | The failure is detected | No deployment occurs and the team is notified of the failure with a link to the build log |
| Should Have | A developer runs `docker compose up` locally | The command completes | The API, frontend, SQL Server, and Redis containers start and the application is accessible at localhost |
| Could Have | A new developer joins the project | They clone the repository | A single README command brings up the full local environment without additional configuration steps |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Pipeline successfully deploys a hello-world health check endpoint | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Technical | GitHub repository | Source repository must exist with branch protection rules enabled | DevOps / Pending |
| Technical | Container registry | Docker image registry must be provisioned (Azure Container Registry or equivalent) | DevOps / Pending |
| Technical | Staging environment | Target deployment environment must be available | DevOps / Pending |

## 8. ADDITIONAL NOTES

This story is a prerequisite for all other stories in the backlog. No development work should begin on any other story until this pipeline is operational. The legacy system remains fully operational and untouched during this sprint.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 24 |
| Testing & Validation | 8 |
| **Total** | **40** |

**Confidence:** Medium

**Notes:** Estimate assumes the team has prior experience with GitHub Actions and Docker. If the container registry or cloud environment requires procurement or access approvals, this could extend by several days outside the development estimate.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-002 — Database Schema Under Migration Control

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-002 |
| **Story Title** | Database Schema Under Migration Control |
| **Epic / Module** | Phase 1 — Foundation |
| **Sprint / Release** | Sprint 1 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT / Data Management |
| **Business Description** | The current database schema is managed entirely through manual SQL scripts with no version tracking, no rollback capability, and no automated validation. Any schema change carries the risk of data loss. This story brings the existing CarRetailDB schema under automated migration control so that all future schema changes are safe, traceable, and reversible. |
| **Business Value** | Reduces risk of data loss during modernization, enables schema changes to be tested before reaching production, and provides a full audit trail of every structural change made to the database. |

## 3. USER STORY

> **As a** system administrator,
> **I want** the database schema to be versioned and deployable automatically,
> **so that** schema changes can be applied, tested, and rolled back safely throughout the migration.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Entity Framework Core integration | ORM layer added to the new API project with entities matching the existing five database tables (Users, Cars, Customers, Sales, Inventory_Log) |
| 2 | Baseline migration | An initial EF Core migration that exactly captures the current CarRetailDB schema, enabling the new system to connect to the existing database |
| 3 | Migration applied to dev environment | The baseline migration runs successfully against a development SQL Server instance, confirming schema parity |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | The new API project is configured with a database connection | The EF Core migration command is run against a clean database | All five tables (Users, Cars, Customers, Sales, Inventory_Log) are created with correct columns, data types, and indexes |
| Must Have | The baseline migration has been applied | A developer queries the new and legacy databases side by side | Both databases have identical schema structure |
| Must Have | A schema change is needed in a future sprint | A developer creates a new EF Core migration | The migration applies cleanly on top of the baseline without manual SQL editing |
| Should Have | The migration runs in the CI/CD pipeline | A pull request is merged | The migration is applied automatically to the staging database |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Schema comparison between legacy and new DB confirms zero differences | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-001 | CI/CD pipeline must exist before automated migration deployment | Dev Team / Pending |
| Legacy System | CarRetailDB | Access to the existing SQL Server database for schema extraction | DBA / Pending |
| Technical | SQL Server 2022 | Target database engine must be provisioned in the development environment | DevOps / Pending |

## 8. ADDITIONAL NOTES

The baseline migration must not alter or destroy any existing data. It is a read-only schema snapshot. No password columns or sensitive data are modified in this story — that is handled in US-007.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 8 |
| Testing & Validation | 4 |
| **Total** | **16** |

**Confidence:** High

**Notes:** Well-defined scope. The five-table schema is small and the EF Core scaffolding tooling is mature. The main risk is obtaining DBA access to the production database for schema inspection; coordinate this before the sprint begins.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-003 — Fix SQL Injection in Login and Inventory Pages

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-003 |
| **Story Title** | Fix SQL Injection in Login and Inventory Pages (Legacy Hotfix) |
| **Epic / Module** | Phase 1 — Foundation / Security Hotfix |
| **Sprint / Release** | Sprint 1 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security / All Business Areas |
| **Business Description** | The existing login page and inventory management page build database queries by directly joining user input into SQL strings. This is a critical vulnerability that allows any attacker to log in without a valid password, read all customer and sales data, and potentially delete the entire database. This hotfix applies parameterized queries to the legacy ASP code immediately, without waiting for the full migration to complete. |
| **Business Value** | Eliminates the highest-severity attack vector in the system. Prevents unauthorized access, data exfiltration, and potential regulatory penalties under data protection legislation. Buys safe operating time during the 14-week migration. |

## 3. USER STORY

> **As a** security officer,
> **I want** the login and inventory pages to use safe database queries,
> **so that** attackers cannot gain unauthorized access or extract data by manipulating the login or inventory forms.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Parameterized login query | The login page query is rewritten to use ADODB.Command with explicit parameters instead of string concatenation |
| 2 | Parameterized inventory insert | The add-car form in the inventory page is rewritten to use parameterized inserts |
| 3 | VB6 component query hardening | All SQL string concatenation in CarInventory.dll and SalesOrder.dll is replaced with parameterized ADODB.Command calls |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user is on the login page | They enter `' OR '1'='1` as the username and any password | Login fails and no session is created |
| Must Have | A valid username and password are entered | The user submits the login form | The user is authenticated and redirected to the dashboard as normal |
| Must Have | An admin is on the inventory page | They submit the add-car form with a VIN containing a SQL special character (e.g. `'; DROP TABLE Cars;--`) | The car is either saved with the literal value or rejected by validation; no database error occurs and no tables are affected |
| Must Have | All hotfixes are applied | A security scan is run against the login and inventory endpoints | No SQL injection vulnerabilities are reported for these endpoints |
| Should Have | An invalid login attempt is made | The event is recorded | The login failure is written to the application event log with the username (not the password) |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Manual SQL injection test performed and confirmed blocked on all patched endpoints | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Legacy System | CarRetailSystem ASP | Direct modification to login.asp, inventory.asp, CarInventory.vb, SalesOrder.vb | Dev Team / Pending |
| Technical | IIS staging environment | Patched ASP files must be testable in a staging IIS instance before production deployment | DevOps / Pending |

## 8. ADDITIONAL NOTES

This fix applies to the **legacy** ASP/VB6 codebase only and is intentionally minimal — no refactoring, no new features. The stored procedures `sp_GetAvailableCars` and `sp_CreateSalesOrder` are already parameterized and should be used where possible. An emergency change request may be required if production deployment is needed before the next scheduled release window.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 12 |
| Testing & Validation | 8 |
| **Total** | **24** |

**Confidence:** High

**Notes:** The 8 vulnerable locations are fully documented with exact file and line references. Development covers login.asp, inventory.asp, CarInventory.vb, and SalesOrder.vb. VB6 requires a recompile and DLL re-registration, which adds time to the testing cycle. An IIS staging environment must be available before testing can begin.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-004 — Remove Hardcoded Credentials from Source Code

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-004 |
| **Story Title** | Remove Hardcoded Credentials from Source Code (Legacy Hotfix) |
| **Epic / Module** | Phase 1 — Foundation / Security Hotfix |
| **Sprint / Release** | Sprint 1 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security |
| **Business Description** | The database connection string (including the SQL Server system administrator password) and the email account password are stored in plain text in three source code files. Any developer, contractor, or attacker who gains access to the source code immediately has full administrative control over the database server and access to the company email account. This hotfix moves all credentials out of source code and into environment variables. |
| **Business Value** | Eliminates credential exposure risk. Prevents a source code breach from escalating into a full database compromise. Aligns with basic secrets management compliance requirements. |

## 3. USER STORY

> **As a** system administrator,
> **I want** database and email credentials to be loaded from secure environment variables rather than source code,
> **so that** credentials are never exposed through the codebase, version control history, or compiled DLL files.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Database connection string externalized | `config.asp` reads the connection string from the `CAR_RETAIL_DB_CONN` environment variable instead of the hardcoded `sa` password |
| 2 | SMTP password externalized | `config.asp` reads the email password from the `CAR_RETAIL_SMTP_PWD` environment variable |
| 3 | VB6 component credentials externalized | `CarInventory.vb` and `SalesOrder.vb` read the connection string from an environment variable rather than the hardcoded constant |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | The environment variables are set correctly in IIS | The application starts | The application connects to the database and functions normally |
| Must Have | The source code is inspected | A developer searches for `Admin@123` or `EmailPassword123` in all files | No matches are found in any source file or compiled output |
| Must Have | The environment variable is missing or empty | The application attempts to start | The application logs a clear error stating the missing variable name and does not start with an empty connection string |
| Should Have | Credentials are rotated | The IIS environment variable is updated and the application pool is recycled | The application connects using the new credentials without a code change or redeployment |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Git history scanned and confirmed to contain no credentials in any commit | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Legacy System | config.asp | File must be modified to remove hardcoded values | Dev Team / Pending |
| Legacy System | CarInventory.vb / SalesOrder.vb | VB6 source must be modified and DLLs recompiled | Dev Team / Pending |
| Technical | IIS Application Pool | Environment variables must be set in Windows System or IIS application pool settings | SysAdmin / Pending |

## 8. ADDITIONAL NOTES

After deployment, the old `sa` password should be rotated immediately. If the credentials were previously committed to any shared repository, treat them as fully compromised and rotate before deploying this fix. Git history should be audited for past credential exposure.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 2 |
| Development | 4 |
| Testing & Validation | 2 |
| **Total** | **8** |

**Confidence:** High

**Notes:** Small and well-scoped change across three files. The majority of effort is coordination — obtaining SysAdmin access to set environment variables in IIS, and scheduling the `sa` password rotation with the DBA. VB6 recompile adds approximately 1 hour to the development estimate.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-005 — Enforce HTTPS and Fix XSS Output Encoding

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-005 |
| **Story Title** | Enforce HTTPS and Fix XSS Output Encoding (Legacy Hotfix) |
| **Epic / Module** | Phase 1 — Foundation / Security Hotfix |
| **Sprint / Release** | Sprint 1 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security / All Business Areas |
| **Business Description** | All traffic to the CarRetailSystem is currently transmitted over unencrypted HTTP, meaning usernames, passwords, and session tokens are visible to anyone on the same network. Additionally, user-controlled data from the database is written directly to web pages without encoding, allowing malicious content to execute in users' browsers. These fixes close two high-severity attack vectors with minimal code changes. |
| **Business Value** | Protects employee credentials and session tokens from interception. Prevents cross-site scripting attacks that could steal sessions or display fraudulent content to staff. Demonstrates baseline compliance with data protection requirements. |

## 3. USER STORY

> **As a** system administrator,
> **I want** all web traffic to be encrypted and all page output to be safely encoded,
> **so that** employee credentials cannot be intercepted over the network and malicious scripts cannot be injected into the application.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | HTTP to HTTPS redirect | IIS is configured to redirect all HTTP requests permanently to HTTPS, with a valid TLS certificate installed |
| 2 | XSS output encoding | All `Response.Write` calls in `index.asp`, `inventory.asp`, and `login.asp` that output user-controlled or database values are wrapped with the existing `HTMLEncode()` function |
| 3 | Demo credentials removed | The hardcoded `admin / admin123` hints are removed from the login page HTML source |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user navigates to `http://` (plain HTTP) | The browser sends the request | The server returns a 301 redirect to the `https://` equivalent and the page loads securely |
| Must Have | A car record contains a name with HTML special characters (e.g. `<script>alert(1)</script>`) | The inventory page is loaded | The browser displays the literal text, not a popup — no script executes |
| Must Have | The login page source is viewed | A developer inspects the HTML | No credentials, passwords, or account hints appear anywhere in the page source |
| Should Have | A security scanner runs against the patched pages | The scan completes | No XSS or mixed-content warnings are reported for the patched pages |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | TLS certificate validity and HTTPS redirect confirmed by browser and automated scan | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Technical | TLS Certificate | A valid certificate must be obtained and installed on IIS before the redirect can be enforced | SysAdmin / Pending |
| Legacy System | index.asp, inventory.asp, login.asp | Source files require modification to apply HTMLEncode to all outputs | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The `HTMLEncode()` function already exists in `utils.asp` — this is a wrapping exercise, not a new development task. Estimated effort: 1 hour for HTTPS configuration, 1 hour for XSS encoding, 5 minutes for removing demo credentials.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 2 |
| Development | 4 |
| Testing & Validation | 2 |
| **Total** | **8** |

**Confidence:** High

**Notes:** The XSS fix is purely mechanical — wrapping existing `Response.Write` calls with an already-available function. HTTPS configuration depends on certificate procurement; if an internal CA or Let's Encrypt is already available this is straightforward. Allow extra time if a certificate request process requires approval.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-006 — Secure Login with JWT Authentication

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-006 |
| **Story Title** | Secure Login with JWT Authentication |
| **Epic / Module** | Phase 2 — Authentication |
| **Sprint / Release** | Sprint 2 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security / All Staff |
| **Business Description** | The current login system stores sessions in server memory with no token expiry enforcement, no brute-force protection, and transmits credentials without secure token management. The new authentication module replaces this with industry-standard JWT tokens, rate-limited login, and secure refresh token handling — providing the security foundation that all other new API modules depend on. |
| **Business Value** | Delivers a login experience that is secure by default, protects against credential theft and brute-force attacks, and enables the stateless API design that allows the new system to scale horizontally — something the legacy system cannot do. |

## 3. USER STORY

> **As a** dealership employee,
> **I want** to log in securely with my username and password,
> **so that** I can access only the features my role permits without risking account compromise.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | JWT login endpoint | `POST /auth/login` validates credentials and returns a short-lived access token (15 minutes) and a long-lived refresh token (7 days) |
| 2 | Token refresh endpoint | `POST /auth/refresh` exchanges a valid refresh token for a new access token without requiring re-login |
| 3 | Logout endpoint | `POST /auth/logout` revokes the refresh token, ending the session |
| 4 | Login rate limiting | A maximum of 5 failed login attempts per IP address per minute is enforced; excess attempts receive a 429 response |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A valid username and password are submitted | `POST /auth/login` is called | The response contains a JWT access token and a refresh token; HTTP 200 is returned |
| Must Have | An invalid username or wrong password is submitted | `POST /auth/login` is called | HTTP 401 is returned with a generic error message; no detail about which field was wrong |
| Must Have | A valid access token is included in a request header | Any protected API endpoint is called | The request is processed and the response is returned normally |
| Must Have | An expired or invalid token is used | A protected endpoint is called | HTTP 401 is returned and the client is prompted to refresh or re-login |
| Must Have | 5 failed login attempts occur from one IP within 1 minute | A 6th attempt is made | HTTP 429 (Too Many Requests) is returned and the attempt is logged |
| Should Have | A valid refresh token is submitted | `POST /auth/refresh` is called | A new access token is returned and the old refresh token is invalidated |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Penetration test confirms token cannot be forged or replayed after logout | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-002 | Database schema must be under EF Core control before Identity tables can be added | Dev Team / Pending |
| Story | US-001 | CI/CD pipeline needed to deploy auth service to staging | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The legacy ASP login (`login.asp`) remains operational in parallel during this sprint. Both systems authenticate against the same Users table. The JWT secret key must be stored in Azure Key Vault or environment variables — never in source code.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 24 |
| Testing & Validation | 8 |
| **Total** | **40** |

**Confidence:** Medium

**Notes:** JWT authentication with refresh token rotation and rate limiting is well-understood but has several moving parts (token generation, revocation store, middleware, rate limiter). The design session should confirm token storage strategy (refresh token in DB vs Redis) before development begins, as the choice affects architecture.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-007 — Password Hash Migration (Plain Text to bcrypt)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-007 |
| **Story Title** | Password Hash Migration — Plain Text to bcrypt |
| **Epic / Module** | Phase 2 — Authentication |
| **Sprint / Release** | Sprint 2 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security / HR |
| **Business Description** | All user passwords in the CarRetailSystem database are currently stored as plain text. Any database backup, query result, or log file exposes every employee's password in readable form. Because staff frequently reuse passwords across systems, a single database breach could compromise email accounts, HR systems, and other business tools. This story migrates passwords to bcrypt hashing transparently — users log in as normal and their password is silently secured on next login. |
| **Business Value** | Eliminates the risk of password exposure from a database breach. Protects employees whose passwords may be reused on other systems. Demonstrates compliance with basic credential security standards without disrupting normal operations. |

## 3. USER STORY

> **As a** security officer,
> **I want** all employee passwords to be stored as cryptographic hashes,
> **so that** a database breach does not expose any employee's readable password.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Schema extension for hashed passwords | Two new columns added to the Users table: `PasswordHash` (bcrypt hash) and `PasswordMigrated` (flag indicating whether the user has been migrated) |
| 2 | Transparent migration on login | On each successful login, if the user's password is still plain text, it is hashed with bcrypt and the migration flag is set — the user notices no change |
| 3 | Legacy plain text column removal | Once all users have logged in and been migrated, the plain text `Password` column is dropped from the database |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user with a legacy plain text password logs in for the first time | The login succeeds | Their password is hashed with bcrypt and stored; `PasswordMigrated` is set to true; the plain text value is cleared |
| Must Have | The same user logs in again after migration | They enter the correct password | Login succeeds using the bcrypt hash — no plain text comparison occurs |
| Must Have | The database is queried for user records | A DBA inspects the Password column | Only bcrypt hashes are visible; no readable passwords exist in any row |
| Must Have | All active users have logged in at least once | An admin reviews the migration status | `PasswordMigrated = true` for all active user accounts |
| Should Have | An inactive user account has not yet migrated | An admin exports a user report | The account is flagged as requiring a password reset before the legacy column is dropped |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Database audit confirms zero plain text passwords in Users table after migration window closes | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-006 | JWT login endpoint must exist before migration logic can be embedded in the login flow | Dev Team / Pending |
| Story | US-002 | EF Core schema control required to add migration columns safely | Dev Team / Pending |
| Legacy System | CarRetailDB / Users table | Schema change requires a coordinated deployment with a database backup | DBA / Pending |

## 8. ADDITIONAL NOTES

A database backup must be taken immediately before this migration is deployed to production. Define a migration window (e.g. 30 days) after which any unmigrated accounts are locked and require a password reset. The bcrypt cost factor should be set to 12.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 16 |
| Testing & Validation | 16 |
| **Total** | **40** |

**Confidence:** Medium

**Notes:** Testing is weighted heavily because this story touches live credential data. The dual-path login flow (plain text vs. bcrypt) requires thorough regression testing to ensure no user is locked out. The 30-day migration window monitoring and the dormant-account handling process add coordination overhead beyond pure development.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-008 — Role-Based Access Control (Admin / Salesperson / Manager)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-008 |
| **Story Title** | Role-Based Access Control |
| **Epic / Module** | Phase 2 — Authentication |
| **Sprint / Release** | Sprint 2 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT Security / Operations |
| **Business Description** | The legacy system has a role field in the database (Admin, Salesperson, Manager) but role checks are applied inconsistently across ASP pages, making it possible for a salesperson to access admin-only functions by directly navigating to a URL. The new system enforces roles at the API level, ensuring that no matter how a request is made, permissions are always checked before any action is taken. |
| **Business Value** | Ensures that employees can only perform actions appropriate to their job role, reducing the risk of accidental or intentional unauthorized changes to inventory, pricing, or user accounts. |

## 3. USER STORY

> **As a** dealership manager,
> **I want** the system to enforce access permissions based on each employee's role,
> **so that** salespeople cannot modify inventory or access admin functions they are not authorised to use.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Role claims in JWT | The JWT access token includes the user's role (Admin / Salesperson / Manager) as a claim, issued at login |
| 2 | API-level role enforcement | All API endpoints that modify inventory or manage users require the Admin role; sales creation requires Salesperson or Admin; reports require Manager or Admin |
| 3 | Forbidden response | Requests made with an insufficient role receive HTTP 403 (Forbidden) with a clear error message |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user is logged in as Salesperson | They attempt to call `POST /cars` (add a car — Admin only) | HTTP 403 is returned; no car is created |
| Must Have | A user is logged in as Admin | They call `POST /cars` | The request is processed and the car is created |
| Must Have | A user is logged in as Manager | They call `GET /reports/monthly-sales` | The report data is returned successfully |
| Must Have | A user is logged in as Salesperson | They call `GET /reports/monthly-sales` | HTTP 403 is returned |
| Should Have | A user's role is updated by an Admin | The user logs out and back in | The new role is reflected in their JWT and access permissions update accordingly |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Role permission matrix reviewed and signed off by business stakeholders | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-006 | JWT token must be issued at login before role claims can be embedded | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The three legacy roles (Admin, Salesperson, Manager) map directly to the new system. No new roles are introduced in this story. A role permission matrix should be reviewed and approved by the business before development begins.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 8 |
| Testing & Validation | 4 |
| **Total** | **16** |

**Confidence:** High

**Notes:** Straightforward implementation once the JWT infrastructure from US-006 is in place. The main risk is misalignment between IT's interpretation of role permissions and the business's expectations — the role permission matrix sign-off must happen before coding begins.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-009 — Car Inventory Management API

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-009 |
| **Story Title** | Car Inventory Management API |
| **Epic / Module** | Phase 3 — Core API / Inventory |
| **Sprint / Release** | Sprint 3 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Sales / Inventory Management |
| **Business Description** | The current inventory management is handled through a single ASP page with no pagination, no filtering beyond a simple make filter, and no API that other systems could consume. All changes to inventory are performed through the web page only. This story replaces the legacy inventory module with a full REST API that supports paginated browsing, filtering by make and price, and complete create/update/delete operations with proper role enforcement. |
| **Business Value** | Delivers a reliable, scalable inventory API that the new React frontend and any future integrations (e.g. stock feeds, partner portals) can consume. Pagination prevents slow page loads for large inventories. |

## 3. USER STORY

> **As a** dealership administrator,
> **I want** to manage the car inventory through a secure API,
> **so that** I can add, update, and remove vehicles reliably, with all changes logged for audit purposes.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Paginated inventory list | `GET /cars` returns a paginated, filterable list of available vehicles (filter by make and/or maximum price) |
| 2 | Car detail and stock | `GET /cars/{id}` and `GET /cars/{id}/stock` return full vehicle details and current stock level |
| 3 | Add, update, and soft-delete | `POST /cars`, `PUT /cars/{id}`, and `DELETE /cars/{id}` allow Admins to manage the catalogue; deletion sets IsActive = false rather than removing the record |
| 4 | Automatic audit logging | Every create, update, and delete action writes a record to Inventory_Log, preserving the full change history |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | 50 cars exist in the database | `GET /cars?page=1&pageSize=10` is called | 10 cars are returned with pagination metadata (total count, current page, total pages) |
| Must Have | A filter is applied | `GET /cars?make=Toyota&maxPrice=30000` is called | Only Toyota cars priced at or below £30,000 are returned |
| Must Have | An Admin submits a valid new car | `POST /cars` is called with all required fields | The car is saved, an Inventory_Log entry is created, and HTTP 201 is returned with the new car ID |
| Must Have | A Salesperson attempts to add a car | `POST /cars` is called | HTTP 403 is returned; no car is created |
| Must Have | An Admin deletes a car | `DELETE /cars/{id}` is called | `IsActive` is set to false; the car no longer appears in the available list but the record is preserved |
| Should Have | A car with invalid data is submitted | `POST /cars` is called with a missing VIN or negative price | HTTP 400 is returned with a clear field-level validation message |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Swagger / OpenAPI documentation complete and accurate for all inventory endpoints | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-006 | Auth endpoint required; all inventory endpoints must be protected by JWT | Dev Team / Pending |
| Story | US-008 | Role enforcement required before Admin-only endpoints can be validated | Dev Team / Pending |
| Story | US-002 | EF Core entities for Cars and Inventory_Log must exist | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The business logic of the legacy `CarInventory.dll` (stock queries, stock updates, audit logging) must be fully replicated in the new `InventoryService`. The legacy COM component is decommissioned only after the full frontend cutover in Phase 5.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 24 |
| Testing & Validation | 8 |
| **Total** | **40** |

**Confidence:** High

**Notes:** All business logic is documented with exact line references in the legacy `CarInventory.dll`. Six endpoints with CRUD operations, pagination, filtering, and audit logging. The 8-hour testing allocation includes unit tests for the service layer and integration tests against a test database instance.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-010 — Sales Order API

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-010 |
| **Story Title** | Sales Order API |
| **Epic / Module** | Phase 3 — Core API / Sales |
| **Sprint / Release** | Sprint 3 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Sales |
| **Business Description** | Creating a sale in the current system involves a single ASP page that calls a VB6 component, which in turn runs a database transaction to record the sale, reduce vehicle stock, and log the change. If any step fails silently (due to `On Error Resume Next`), the sale may be recorded without reducing stock — creating an inventory discrepancy. The new Sales API performs this transaction with proper error handling and guaranteed atomicity. |
| **Business Value** | Eliminates the risk of stock discrepancies caused by silent transaction failures. Provides salespeople with immediate confirmation of a completed sale including total price with tax. |

## 3. USER STORY

> **As a** salesperson,
> **I want** to create a sales order that atomically records the sale, reduces vehicle stock, and logs the transaction,
> **so that** inventory counts are always accurate and I receive immediate confirmation of every completed sale.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Create sales order | `POST /sales` records the sale, deducts one unit from car stock, and writes an Inventory_Log entry — all within a single atomic database transaction |
| 2 | Tax calculation | Sale total is calculated as sale price plus 10% tax; both values are returned in the response |
| 3 | Sale detail retrieval | `GET /sales/{id}` returns the full sale record including customer, vehicle, salesperson, price, tax, and date |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A car has stock of 2 and a valid customer exists | `POST /sales` is called with the car ID and customer ID | The sale is recorded, car stock becomes 1, an Inventory_Log entry is created, and the response includes the total price with tax |
| Must Have | A car has stock of 0 | `POST /sales` is called for that car | HTTP 400 is returned stating the car is out of stock; no sale record is created |
| Must Have | The database fails mid-transaction | An error occurs after the sale is inserted but before stock is updated | The entire transaction is rolled back; no partial data is saved |
| Must Have | A Salesperson creates a sale | `POST /sales` is called with their JWT | The sale is recorded and the salesperson ID is captured from their token |
| Should Have | An Admin retrieves a completed sale | `GET /sales/{id}` is called | Full sale details including customer name, car details, sale price, tax, and total are returned |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Transaction rollback tested and confirmed — no orphaned records created on failure | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-009 | Inventory API must exist; stock availability check depends on it | Dev Team / Pending |
| Story | US-011 | Customer must exist in the database before a sale can reference them | Dev Team / Pending |
| Story | US-008 | Salesperson role must be enforced on the sales endpoint | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The tax rate of 10% is currently hardcoded in the legacy `SalesOrder.dll`. In the new system, this rate should be configurable via `appsettings.json` so it can be changed without a code deployment. Confirm the correct tax rate with the Finance team before go-live.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 16 |
| Testing & Validation | 16 |
| **Total** | **40** |

**Confidence:** Medium

**Notes:** Testing is weighted equally with development because the atomic transaction — sale insert, stock update, audit log — must be tested under failure conditions (simulated database errors, concurrent requests, out-of-stock race conditions). Confirm the tax rate with Finance before the sprint begins to avoid a last-minute change.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-011 — Customer Management API

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-011 |
| **Story Title** | Customer Management API |
| **Epic / Module** | Phase 3 — Core API / Customers |
| **Sprint / Release** | Sprint 3 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Sales / Customer Relations |
| **Business Description** | Customer records are currently managed through an ASP page with no source code preserved in the repository. Customer data — including names, addresses, and email addresses — is stored and retrieved with no input validation and no pagination. The new Customer API provides a fully validated, paginated, and role-secured interface for managing the customer database. |
| **Business Value** | Provides a reliable customer data foundation for the sales process. Input validation reduces bad data entering the system. Pagination supports growth in the customer base without performance degradation. |

## 3. USER STORY

> **As a** salesperson,
> **I want** to search, view, create, and update customer records through the new system,
> **so that** I can quickly find an existing customer or register a new one when processing a sale.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Paginated customer list | `GET /customers` returns a paginated list of active customers, searchable by name or email |
| 2 | Customer detail and purchase history | `GET /customers/{id}` returns full customer details; `GET /customers/{id}/sales` returns their full purchase history |
| 3 | Create, update, and soft-delete | `POST /customers`, `PUT /customers/{id}`, and `DELETE /customers/{id}` with input validation on all fields |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | Customers exist in the database | `GET /customers?page=1&pageSize=20` is called | A paginated list of active customers is returned |
| Must Have | A new customer is submitted with valid data | `POST /customers` is called | The customer is created and HTTP 201 is returned with the new customer ID |
| Must Have | A duplicate email address is submitted | `POST /customers` is called | HTTP 400 is returned indicating the email already exists; no duplicate is created |
| Must Have | A customer's purchase history is requested | `GET /customers/{id}/sales` is called | All sales linked to that customer are returned in reverse chronological order |
| Should Have | A customer is soft-deleted | `DELETE /customers/{id}` is called | `IsActive` is set to false; the customer no longer appears in the list but their sales history is preserved |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | All customer fields validated (email format, phone format, required fields) | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-006 | JWT authentication required for all customer endpoints | Dev Team / Pending |
| Story | US-002 | EF Core Customers entity must exist | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The source code for the legacy `CustomerMgmt.dll` is missing. Behaviour must be reverse-engineered from the database schema and any available documentation before development begins. Confirm with the business whether any additional customer fields are required beyond those in the current schema.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 16 |
| Testing & Validation | 8 |
| **Total** | **32** |

**Confidence:** Medium

**Notes:** The 8-hour analysis allocation is higher than comparable stories because the source code for the legacy `CustomerMgmt.dll` is missing and behaviour must be reverse-engineered. If the analysis uncovers unexpected business rules, the development estimate may increase. Confirm field requirements with the business in the analysis phase.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-012 — Reports and Dashboard API

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-012 |
| **Story Title** | Reports and Dashboard API |
| **Epic / Module** | Phase 3 — Core API / Reports |
| **Sprint / Release** | Sprint 4 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Management / Finance |
| **Business Description** | Managers currently have no dedicated reporting interface — the legacy `ReportGenerator.dll` source code is missing and the reports page was never fully implemented. Monthly sales totals, inventory summaries, and top-selling vehicles are business-critical data that management needs regularly. This story delivers a reporting API with caching to ensure report queries do not impact day-to-day operational performance. |
| **Business Value** | Gives managers on-demand access to accurate business performance data. Caching ensures report generation does not slow down the system for salespeople during peak hours. |

## 3. USER STORY

> **As a** dealership manager,
> **I want** to retrieve sales performance and inventory reports via the new system,
> **so that** I can make informed business decisions based on up-to-date data without impacting system performance.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Monthly sales report | `GET /reports/monthly-sales` returns total sales count and revenue grouped by month for a given year |
| 2 | Inventory status report | `GET /reports/inventory` returns current stock levels grouped by make and model |
| 3 | Top sellers report | `GET /reports/top-sellers` returns the best-selling vehicles by unit count for a given date range |
| 4 | Report caching | All report endpoints are cached for 5 minutes to prevent repeated heavy database queries |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | Sales data exists for the current year | `GET /reports/monthly-sales?year=2026` is called | A list of months with sale count and total revenue per month is returned |
| Must Have | A Manager calls a report endpoint | The request is made with a valid Manager JWT | The report data is returned successfully |
| Must Have | A Salesperson calls a report endpoint | The request is made with a Salesperson JWT | HTTP 403 is returned |
| Must Have | A report endpoint is called twice within 5 minutes | The second call is made | The second response is served from cache; no additional database query is executed |
| Should Have | No sales exist for a given month | The monthly report is requested | The month appears in the response with a count of zero and revenue of zero |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Report output validated against equivalent manual SQL query run against the same dataset | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-009 | Inventory data must be available via the new system | Dev Team / Pending |
| Story | US-010 | Sales data must be available via the new system | Dev Team / Pending |
| Technical | Redis | Caching service must be provisioned in the environment | DevOps / Pending |

## 8. ADDITIONAL NOTES

The legacy `sp_GenerateMonthlySalesReport` stored procedure contains the reference business logic for the monthly report. Confirm with management whether the report grouping, date range parameters, and currency format match current expectations before sign-off.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 8 |
| Development | 20 |
| Testing & Validation | 8 |
| **Total** | **36** |

**Confidence:** Medium

**Notes:** The legacy `ReportGenerator.dll` source is missing, so report logic must be derived from the stored procedure `sp_GenerateMonthlySalesReport` and aligned with management's expectations. The stakeholder sign-off session on report format should be completed during analysis to prevent rework after development.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-013 — Login Page (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-013 |
| **Story Title** | Login Page — React Frontend |
| **Epic / Module** | Phase 4 — Frontend |
| **Sprint / Release** | Sprint 5 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | All Staff |
| **Business Description** | The current login page is a plain HTML form hosted on IIS. It provides no feedback on invalid credentials other than reloading the page, shows demo credentials in the page source, and cannot enforce modern browser security policies. The new React login page connects to the secure JWT API built in Phase 2 and provides a professional, accessible authentication experience. |
| **Business Value** | Delivers the entry point to the new system for all staff. A clear, validated login form reduces support calls from users who receive no feedback on failed login attempts. |

## 3. USER STORY

> **As a** dealership employee,
> **I want** to log in through a clear, modern interface that tells me when my credentials are wrong,
> **so that** I can access the system quickly without confusion or the need to contact IT support.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Login form | Username and password fields with client-side validation, a submit button, and a loading state while the request is in flight |
| 2 | Error feedback | Clear inline error messages for invalid credentials or account lockout, distinct from general network errors |
| 3 | Secure token storage | Access token stored in memory (not localStorage); refresh token stored in an HttpOnly cookie to prevent JavaScript access |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user navigates to the login page | The page loads | A username field, password field, and login button are displayed; no credentials or hints appear in the page source |
| Must Have | Valid credentials are submitted | The user clicks login | The user is authenticated, the access token is stored, and they are redirected to the dashboard |
| Must Have | Invalid credentials are submitted | The user clicks login | An error message "Invalid username or password" is displayed; the password field is cleared |
| Must Have | The login button is clicked | The API request is in flight | A loading indicator is shown and the button is disabled to prevent double submission |
| Should Have | The user's session expires | They attempt to access a protected page | They are automatically redirected to the login page with a message explaining the session has expired |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Accessibility check passed (keyboard navigation, screen reader labels on form fields) | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-006 | JWT login API endpoint must be live | Dev Team / Pending |
| Story | US-001 | Frontend build pipeline must be operational | Dev Team / Pending |

## 8. ADDITIONAL NOTES

The legacy login page (`login.asp`) remains accessible during Phase 4 as a fallback. Both pages authenticate against the same backend. The legacy page is decommissioned in Phase 5 (US-020).

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 8 |
| Testing & Validation | 4 |
| **Total** | **16** |

**Confidence:** High

**Notes:** A well-understood, single-purpose page. The token storage strategy (memory + HttpOnly cookie) is the only technically nuanced aspect; this should be confirmed with the security team during design. Includes accessibility validation as part of the testing allocation.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-014 — Dashboard Page (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-014 |
| **Story Title** | Dashboard Page — React Frontend |
| **Epic / Module** | Phase 4 — Frontend |
| **Sprint / Release** | Sprint 5 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | All Staff |
| **Business Description** | The legacy landing page (`index.asp`) shows a basic summary of cars, customers, and monthly sales, but hits the database on every single page load with no caching, causing unnecessary load. The new dashboard displays the same summary statistics sourced from the cached reports API, with navigation to all functional areas of the system. |
| **Business Value** | Gives all users an immediate at-a-glance view of business health on login. Cached data means fast load times and no database pressure from dashboard refreshes. |

## 3. USER STORY

> **As a** dealership employee,
> **I want** to see a summary of key business metrics on my home screen after logging in,
> **so that** I have an immediate overview of inventory, customer, and sales status before starting my work.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Summary statistics cards | Cards displaying total available cars, total active customers, and total sales this month |
| 2 | Navigation menu | Links to Inventory, Sales, Customers, and Reports pages, with items hidden based on the user's role |
| 3 | Cached data | Statistics are loaded from the cached reports API and display a loading state while fetching |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A user logs in successfully | They are redirected to the dashboard | Three summary cards are visible: total cars in stock, total customers, and sales this month |
| Must Have | A Salesperson is logged in | They view the navigation | The Reports section is not visible in the menu |
| Must Have | A Manager is logged in | They view the navigation | All sections including Reports are visible |
| Should Have | The dashboard is loaded | The statistics API is slow to respond | A loading skeleton is shown in place of the cards; the page does not appear blank |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Dashboard loads in under 2 seconds on a standard connection with cached data | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-013 | Login page must exist to redirect authenticated users to the dashboard | Dev Team / Pending |
| Story | US-012 | Reports API must provide the summary statistics | Dev Team / Pending |

## 8. ADDITIONAL NOTES

Role-based navigation hiding is a UI convenience — actual access control is enforced at the API level (US-008). The dashboard must be responsive and usable on tablet-sized screens as salespeople may use tablets on the showroom floor.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 12 |
| Testing & Validation | 4 |
| **Total** | **20** |

**Confidence:** High

**Notes:** Slightly more development time than the login page due to the navigation layout, role-based menu rendering, and loading skeleton states. The responsive/tablet requirement should be confirmed with salespeople or their manager during design to agree on supported screen sizes.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-015 — Inventory Management UI (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-015 |
| **Story Title** | Inventory Management UI — React Frontend |
| **Epic / Module** | Phase 4 — Frontend / Inventory |
| **Sprint / Release** | Sprint 5 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Inventory Management |
| **Business Description** | The current inventory page shows all vehicles in a single unfiltered, unpaginated HTML table. Adding or editing a vehicle requires a full page reload with no validation feedback. The new inventory UI provides a searchable, paginated table with inline confirmation dialogs for destructive actions, and modal forms for adding and editing vehicles. |
| **Business Value** | Faster, more accurate inventory management for administrators. Pagination prevents browser slowdowns as the vehicle catalogue grows. Validation feedback reduces data entry errors. |

## 3. USER STORY

> **As a** dealership administrator,
> **I want** to browse, search, add, edit, and remove vehicles through a modern interface,
> **so that** I can keep the inventory catalogue accurate and up to date without the frustration of page reloads and missing feedback.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Paginated inventory table | Displays vehicles in a paginated table with columns for make, model, year, price, stock, and status; 20 rows per page |
| 2 | Search and filter | Filter bar allowing filtering by make and maximum price, applying instantly without a full page reload |
| 3 | Add / edit vehicle form | Modal form for creating a new car or editing an existing one, with field-level validation and success/error toast notifications |
| 4 | Delete with confirmation | Soft-delete triggered by a button with a confirmation dialog to prevent accidental removal |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | Cars exist in the system | An Admin opens the inventory page | A paginated table of cars is displayed with pagination controls |
| Must Have | An Admin clicks "Add Car" | The modal opens and they fill in all required fields | On submit, the car appears in the list and a success message is displayed |
| Must Have | A required field is left empty | The Admin attempts to submit the add form | The specific field is highlighted with an error message; no API call is made |
| Must Have | An Admin clicks delete on a car | A confirmation dialog appears and they confirm | The car is removed from the list and a success notification is shown |
| Must Have | A Salesperson logs in | They view the inventory page | Add, edit, and delete controls are not visible |
| Should Have | A make filter is applied | The Admin types "Ford" in the filter field | The table updates to show only Ford vehicles without a full page reload |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | E2E test covers add car, edit car, and delete car flows | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-009 | Inventory API must be live and tested | Dev Team / Pending |
| Story | US-013 | Auth and routing must be in place | Dev Team / Pending |

## 8. ADDITIONAL NOTES

N/A

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 24 |
| Testing & Validation | 8 |
| **Total** | **36** |

**Confidence:** High

**Notes:** The highest-development frontend story due to the combination of paginated table, real-time filter, modal add/edit form, confirmation dialog, toast notifications, and role-conditional controls. E2E tests (Playwright) for add/edit/delete add to the testing allocation. Reusable table and modal components built here will accelerate US-016 and US-017.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-016 — Sales Order UI (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-016 |
| **Story Title** | Sales Order UI — React Frontend |
| **Epic / Module** | Phase 4 — Frontend / Sales |
| **Sprint / Release** | Sprint 6 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Sales |
| **Business Description** | The legacy sales page was never fully implemented. Salespeople currently have no dedicated UI for creating a sale through the new system. This story delivers a guided sales order form where a salesperson selects a customer and a vehicle, sees the calculated total with tax, and receives a printable confirmation on completion. |
| **Business Value** | Enables salespeople to process transactions through the new system with clear price visibility and a confirmation receipt, replacing the unreliable legacy COM-based transaction flow. |

## 3. USER STORY

> **As a** salesperson,
> **I want** to create a sales order by selecting a customer and a vehicle and reviewing the total cost before confirming,
> **so that** I can process transactions accurately and provide the customer with a clear price breakdown.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Sales order form | Searchable customer selector, searchable available-cars selector (showing price and stock), and a payment method selector |
| 2 | Live price calculation | Tax (10%) and total are calculated and displayed in real time as the salesperson selects a vehicle |
| 3 | Sale confirmation view | On successful submission, a printable confirmation page shows the sale ID, customer name, vehicle details, and total paid |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A salesperson opens the new sale form | They select a customer and a vehicle | The price, tax (10%), and total are displayed before submission |
| Must Have | The salesperson submits the form | The API processes the order | A confirmation screen shows the sale ID, customer, vehicle, and total; stock is reduced by one |
| Must Have | The selected vehicle has no stock | The salesperson attempts to submit | An error message states the vehicle is out of stock; no sale is created |
| Must Have | A network error occurs during submission | The salesperson submits the form | An error message is displayed and the form remains populated so they can retry |
| Should Have | The confirmation screen is displayed | The salesperson clicks "Print" | A print-friendly view is rendered containing the key sale details |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | E2E test covers full sale creation flow including stock reduction verification | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-010 | Sales API must be live | Dev Team / Pending |
| Story | US-011 | Customer list must be accessible to populate the customer selector | Dev Team / Pending |
| Story | US-009 | Inventory API needed for available cars list | Dev Team / Pending |

## 8. ADDITIONAL NOTES

Confirm with Finance whether the 10% tax rate shown on screen matches the rate used in billing. This must be consistent with US-010 configuration.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 16 |
| Testing & Validation | 8 |
| **Total** | **28** |

**Confidence:** High

**Notes:** The searchable customer and car selectors are the most complex UI elements. Reusable components from US-015 (table, modal, toast) will reduce development time. The E2E test must verify the full flow including stock reduction, which requires coordination with test data setup.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-017 — Customer Management UI (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-017 |
| **Story Title** | Customer Management UI — React Frontend |
| **Epic / Module** | Phase 4 — Frontend / Customers |
| **Sprint / Release** | Sprint 6 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Sales / Customer Relations |
| **Business Description** | The legacy customer management page has no preserved source code and was rebuilt with minimal functionality. The new customer UI provides salespeople and admins with a searchable customer directory, detailed customer profiles, and a view of each customer's purchase history — all essential for building customer relationships and managing repeat business. |
| **Business Value** | Enables faster customer lookup during the sales process. Purchase history gives salespeople context about the customer before a conversation, improving service quality and identifying upsell opportunities. |

## 3. USER STORY

> **As a** salesperson,
> **I want** to view, create, and update customer records and see each customer's purchase history,
> **so that** I can manage customer relationships and quickly find the right customer when processing a new sale.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Customer list with search | Paginated customer list with a name/email search bar |
| 2 | Customer detail page | Full contact details view with an edit option and a tab showing the customer's purchase history |
| 3 | Add and edit forms | Modal forms for creating a new customer or editing an existing one, with field validation |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | Customers exist in the system | A salesperson opens the customers page | A paginated list of customers is displayed |
| Must Have | A new customer is added | The form is submitted with valid data | The customer appears in the list and a success notification is shown |
| Must Have | A customer's detail page is opened | The salesperson views their profile | Contact information and a list of past purchases are displayed |
| Must Have | An email that already exists is entered | A new customer is submitted | An error message indicates the email is already registered; no duplicate is created |
| Should Have | A search term is entered | The salesperson types in the search bar | The list filters to matching customers in real time |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | E2E test covers create customer and view purchase history flows | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-011 | Customer API must be live | Dev Team / Pending |
| Story | US-013 | Auth and routing must be in place | Dev Team / Pending |

## 8. ADDITIONAL NOTES

N/A

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 16 |
| Testing & Validation | 8 |
| **Total** | **28** |

**Confidence:** High

**Notes:** Structurally similar to the inventory UI (US-015), benefiting from the reusable paginated table, modal form, and search components already built. The customer detail page with the purchase history tab is the additional complexity over a basic CRUD list. E2E tests cover the create and history flows.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-018 — Reports and Dashboard UI (React Frontend)

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-018 |
| **Story Title** | Reports and Dashboard UI — React Frontend |
| **Epic / Module** | Phase 4 — Frontend / Reports |
| **Sprint / Release** | Sprint 6 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | Management / Finance |
| **Business Description** | The legacy reports page was never implemented. Managers currently have no self-service reporting capability and must request data extracts from IT. The new reports UI exposes monthly sales trends, inventory status, and top-selling vehicles in a clear, visual format that managers can access independently. |
| **Business Value** | Empowers managers to access business performance data on demand, reducing IT dependency and enabling faster business decisions. |

## 3. USER STORY

> **As a** dealership manager,
> **I want** to view visual reports of monthly sales performance, inventory levels, and top-selling vehicles,
> **so that** I can monitor business health and make data-driven decisions without needing to request reports from IT.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Monthly sales chart | Bar or line chart showing sale count and total revenue by month for the selected year, with a summary table below |
| 2 | Inventory status report | Table showing current stock levels grouped by make and model |
| 3 | Top sellers report | Ranked list of best-selling vehicles for a selectable date range |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | A Manager opens the reports page | The page loads | Monthly sales chart, inventory summary, and top sellers are displayed |
| Must Have | A year is selected in the sales report | The Manager changes the year filter | The chart updates to show data for the selected year |
| Must Have | A Salesperson navigates to the reports URL | They attempt to access the reports page | They are redirected with an access-denied message |
| Should Have | The report is loading | A Manager opens the page | A loading state is shown while data is fetched; the page does not appear blank |
| Could Have | A Manager wants to share a report | They click "Export" | A CSV file of the report data is downloaded |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | N/A |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Report figures cross-validated against a direct SQL query on the same dataset | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-012 | Reports API must be live | Dev Team / Pending |
| Story | US-008 | Manager-only access must be enforced | Dev Team / Pending |

## 8. ADDITIONAL NOTES

Confirm with management which chart library is approved for use in the organisation (e.g. Recharts, Chart.js). The CSV export is a Could Have — descope it if Sprint 6 capacity is insufficient.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 16 |
| Testing & Validation | 8 |
| **Total** | **28** |

**Confidence:** Medium

**Notes:** Confidence is Medium due to the chart library dependency — if a preferred library is not already approved, a selection and evaluation step adds time. The report figure cross-validation (DoD criterion 8) requires a DBA or analyst to run comparative SQL queries, which must be coordinated in the testing phase.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-019 — Blue-Green Deployment and Traffic Cutover

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-019 |
| **Story Title** | Blue-Green Deployment and Traffic Cutover |
| **Epic / Module** | Phase 5 — Cutover |
| **Sprint / Release** | Sprint 7 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT / All Business Areas |
| **Business Description** | With the new system fully built and tested, all production traffic must be switched from the legacy IIS/ASP system to the new ASP.NET Core + React platform. A blue-green deployment approach is used: both systems run simultaneously against the same database, allowing a fast rollback if any issue is detected. Full monitoring is activated before the switch is made. |
| **Business Value** | Minimises downtime and risk during the most critical point of the migration. A fast DNS-level rollback means that if any issue is detected after cutover, the legacy system can be restored within minutes — protecting business continuity. |

## 3. USER STORY

> **As a** system administrator,
> **I want** to switch all user traffic to the new system with both systems running in parallel and full monitoring in place,
> **so that** the cutover can be completed safely and reversed immediately if any issue is detected.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Parallel operation | New system deployed alongside the legacy system, both reading and writing to the same CarRetailDB database, with beta users testing the new system |
| 2 | Application Insights monitoring | Error rate, response time, and failed login alerts configured and validated before traffic is switched |
| 3 | DNS / load balancer cutover | Traffic switched to the new system at the load balancer level; legacy system remains on standby for 48 hours |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | Both systems are running in parallel | A sale is created in the new system | The sale record appears correctly in CarRetailDB and stock is reduced; legacy system reads the same updated stock |
| Must Have | Monitoring alerts are configured | A simulated error rate spike occurs | An alert fires within 5 minutes and notifies the on-call team |
| Must Have | DNS is switched to the new system | A user navigates to the application URL | They are served the new React frontend without any action on their part |
| Must Have | A critical issue is detected within 48 hours of cutover | The rollback decision is made | DNS is pointed back to the legacy system within 15 minutes |
| Should Have | All primary workflows are tested by beta users | Beta testing is complete | Sign-off is given by at least one representative from Sales, Management, and IT before full cutover |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Rollback procedure documented, tested, and timed in staging — must complete within 15 minutes | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-013 through US-018 | All frontend stories must be complete and signed off | Dev Team / Pending |
| Story | US-006 through US-012 | All API stories must be complete and signed off | Dev Team / Pending |
| Technical | Application Insights | Monitoring service must be provisioned and alert policies configured | DevOps / Pending |
| Technical | DNS / Load Balancer | Access to DNS settings required to perform the traffic switch | DevOps / Pending |

## 8. ADDITIONAL NOTES

The 48-hour monitoring window after cutover is mandatory before proceeding to US-020 (legacy decommission). Business stakeholder sign-off is required before the DNS switch is executed. Schedule the cutover during a low-traffic period (e.g. early morning on a weekday).

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 16 |
| Development | 16 |
| Testing & Validation | 16 |
| **Total** | **48** |

**Confidence:** Low

**Notes:** This is the highest-risk story in the backlog. The analysis allocation covers cutover runbook creation, rollback procedure definition, monitoring alert configuration, and stakeholder communication planning. Testing covers full regression across all workflows plus rollback rehearsal. Low confidence reflects the coordination complexity across IT, business stakeholders, DNS/networking teams, and the DBA — any delay in one area can push the cutover date.

**Product Owner sign-off:** _________________________ **Date:** ___________

---

---

# US-020 — Legacy System Decommission

## 1. STORY IDENTIFICATION

| Field | Value |
|---|---|
| **Story ID** | US-020 |
| **Story Title** | Legacy System Decommission |
| **Epic / Module** | Phase 5 — Cutover |
| **Sprint / Release** | Sprint 7 |
| **Author** | Michal Szymczyk |
| **Date** | 24/04/2026 |
| **Last Updated** | 24/04/2026 |
| **Status** | [x] Draft [ ] Ready [ ] In Progress [ ] Done |

## 2. BUSINESS CONTEXT

| Field | Value |
|---|---|
| **Business Area** | IT |
| **Business Description** | Following a successful 48-hour monitoring window after the traffic cutover (US-019), the legacy Classic ASP application, VB6 COM components, and IIS Classic ASP application pool are formally decommissioned. Legacy source files are archived rather than deleted. This story formally closes the 14-week modernization programme. |
| **Business Value** | Eliminates ongoing operational cost and security risk of maintaining a 15-year-old unsupported technology stack. Removes the last remaining attack surfaces of the legacy system (unpatched IIS, COM DLLs, old SQL connections). |

## 3. USER STORY

> **As a** system administrator,
> **I want** to formally decommission the legacy IIS application and COM components after a stable cutover,
> **so that** the organisation no longer carries the security risk and maintenance burden of the unsupported legacy system.

## 4. MAIN FEATURES COVERED

| # | Feature Name | Description |
|---|---|---|
| 1 | Legacy ASP application pool disabled | IIS Classic ASP application pool is stopped and disabled; legacy ASP pages are no longer served |
| 2 | COM DLL deregistration | All VB6 COM DLLs (`CarInventory.dll`, `SalesOrder.dll`, etc.) are unregistered from the Windows server |
| 3 | Legacy source archive | All legacy ASP and VB6 source files are archived to a designated storage location and removed from the active codebase |

## 5. ACCEPTANCE CRITERIA

| Priority | Given (Context) | When (Action) | Then (Outcome) |
|---|---|---|---|
| Must Have | The 48-hour monitoring window has passed with no critical issues | Decommission is approved | The legacy ASP application pool is stopped and disabled in IIS |
| Must Have | Decommission is complete | A user navigates to the old legacy URL | They receive a redirect to the new system or a clear "service unavailable" message — no legacy page is served |
| Must Have | COM DLLs are deregistered | The server is inspected | `CarInventory.dll` and `SalesOrder.dll` are no longer registered in the Windows Registry |
| Must Have | Legacy source files are archived | A developer checks the repository | Legacy files are in an archive branch or folder; the main branch contains only the new system code |
| Should Have | All 17 identified security vulnerabilities are verified closed | A final security scan is run | The scan report confirms zero open vulnerabilities from the original list |

## 6. DEFINITION OF DONE

| # | Criterion | Validated |
|---|---|---|
| 1 | Code reviewed and approved by at least one peer | [ ] Yes [ ] No |
| 2 | All acceptance criteria tested and passed | [ ] Yes [ ] No |
| 3 | Unit tests written and passing (coverage ≥ 80%) | [ ] Yes [ ] No |
| 4 | Integration / regression tests passed | [ ] Yes [ ] No |
| 5 | Legacy system data migration validated (if applicable) | [ ] Yes [ ] No |
| 6 | Documentation updated (technical & user-facing) | [ ] Yes [ ] No |
| 7 | Deployed and validated in staging environment | [ ] Yes [ ] No |
| 8 | Final security scan completed and all 17 original vulnerabilities confirmed closed | [ ] Yes [ ] No |

## 7. DEPENDENCIES

| Type | Dependency Name | Description | Owner / Status |
|---|---|---|---|
| Story | US-019 | Traffic cutover must be complete with a successful 48-hour monitoring window | Dev Team / Pending |
| Technical | Windows Server | Administrator access required to disable IIS app pool and run regsvr32 /u | SysAdmin / Pending |

## 8. ADDITIONAL NOTES

Do not delete legacy source files — archive them. They may be needed for compliance, audit, or reference purposes. The decommission must be approved in writing by the IT Director before execution. This story marks the completion of the full modernization programme.

## 9. EFFORT ESTIMATES

| Activity | Estimate (hours) |
|---|---|
| Analysis & Design | 4 |
| Development | 8 |
| Testing & Validation | 16 |
| **Total** | **28** |

**Confidence:** Medium

**Notes:** Testing is weighted heavily because this story is irreversible past the 48-hour window — thorough post-decommission validation of all 17 security vulnerabilities, all primary workflows, and all legacy URL redirects must be completed before sign-off. Written IT Director approval must be obtained and recorded before any decommission steps are executed.

**Product Owner sign-off:** _________________________ **Date:** ___________
