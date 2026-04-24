# HOW TO RUN - CarRetailSystem

Local setup guide for running this legacy Classic ASP application on Windows 10/11.

## Prerequisites

| Requirement | Notes |
|-------------|-------|
| **Windows 10/11** | 64-bit |
| **IIS Express** | Download from [Microsoft](https://www.microsoft.com/en-us/download/details.aspx?id=48264) or via Visual Studio installer |
| **SQL Server** | Any edition: SQL Server Express, Developer, or full. Instance name: `(local)` or `.\SQLEXPRESS` |
| **sqlcmd** | Included with SQL Server or [standalone download](https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility) |

> **Note:** IIS Express is typically installed at `C:\Program Files\IIS Express\iisexpress.exe`

---

## Step 1 – Enable IIS Express ASP Parent Paths

By default IIS Express blocks `../` in ASP includes. Enable it once:

1. Open: `%USERPROFILE%\Documents\IISExpress\config\applicationhost.config`
   - On OneDrive-synced machines: `%USERPROFILE%\OneDrive - <Company>\Documents\IISExpress\config\applicationhost.config`
2. Find the `<asp ...>` element and add `enableParentPaths="true"`:
   ```xml
   <asp scriptErrorSentToBrowser="true" enableParentPaths="true">
   ```

---

## Step 2 – Configure SQL Server

### 2a. Enable Mixed Authentication Mode (SQL + Windows)

In SQL Server Management Studio or via sqlcmd (run as Administrator):

```sql
-- Check current mode (0 = Mixed, 1 = Windows only)
SELECT SERVERPROPERTY('IsIntegratedSecurityOnly');

-- Enable Mixed Mode via registry (requires SQL Server restart)
EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE',
  N'Software\Microsoft\MSSQLServer\MSSQLServer',
  N'LoginMode', REG_DWORD, 2;
```

Then restart SQL Server service.

### 2b. Enable SA Login

```sql
sqlcmd -S "(local)" -E -Q "ALTER LOGIN sa ENABLE; ALTER LOGIN sa WITH PASSWORD = 'Admin@123';"
```

### 2c. Create the Database

```cmd
sqlcmd -S "(local)" -U sa -P "Admin@123" -i "CarRetailSystem\SQL\database_setup.sql"
```

This creates `CarRetailDB` with all tables, stored procedures, and sample data (8 cars, 5 customers, 4 users).

---

## Step 3 – Configure Connection String

The file [CarRetailSystem/ASP/includes/config.asp](CarRetailSystem/ASP/includes/config.asp) contains the connection string.

For `(local)` SQL Server with SA:
```vbscript
Const DB_CONNECTION_STRING = "Provider=SQLOLEDB;Server=(local);Database=CarRetailDB;UID=sa;PWD=Admin@123;"
```

For named instance (e.g. `.\SQLEXPRESS`):
```vbscript
Const DB_CONNECTION_STRING = "Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;"
```

---

## Step 4 – Start IIS Express

Open a terminal and run:

```cmd
"C:\Program Files\IIS Express\iisexpress.exe" /path:"C:\projects\Hackaton\CarRetailSystem\ASP" /port:8080
```

Keep the terminal open while using the app.

---

## Step 5 – Open in Browser

```
http://localhost:8080/pages/index.asp
```

### Login Credentials

| Username | Password | Role |
|----------|----------|------|
| `admin` | `admin123` | Admin |
| `john` | `pass123` | Salesperson |
| `mary` | `pass123` | Salesperson |
| `bob` | `pass123` | Manager |

---

## Available Pages

| URL | Description |
|-----|-------------|
| `/pages/index.asp` | Landing page / dashboard with stats |
| `/pages/login.asp` | Login form |
| `/pages/inventory.asp` | Car inventory CRUD (Admin only) |
| `/pages/logout.asp` | End session |

---

## Troubleshooting

### HTTP 500 – "Disallowed Parent Path"
Enable `enableParentPaths="true"` in applicationhost.config (see Step 1).

### HTTP 500 – "Login timeout expired" / "Cannot connect to database"
- SQL Server not running → start the SQL Server service
- SA login disabled → run Step 2b
- Wrong server name → update `DB_CONNECTION_STRING` in config.asp

### HTTP 500 – "Expected statement" on `Option Explicit`
Multiple `Option Explicit` statements in merged ASP includes. Remove `Option Explicit` from all `includes/*.asp` files (keep it only in the main `.asp` pages).

### Port 8080 already in use
Change the port in the IIS Express command:
```cmd
"C:\Program Files\IIS Express\iisexpress.exe" /path:"..." /port:8090
```

### IIS Express "Cannot create a file when that file already exists"
A previous IIS Express instance is still running. Kill it:
```cmd
taskkill /F /IM iisexpress.exe
```

---

## What Was Fixed to Make It Run

The original generated code had several issues that prevented it from running. These were fixed:

1. **`#include` parent paths** – Changed `<!--#include file="../includes/..."-->` to `<!--#include virtual="/includes/..."-->` in all pages. Classic ASP blocks `../` by default; `virtual` paths from the web root work without that restriction.

2. **Duplicate `Option Explicit`** – Removed `Option Explicit` from `includes/session.asp`. ASP merges all includes into one script at compile time; a second `Option Explicit` causes "Expected statement" error.

3. **Duplicate function definitions** – `security.asp` and `session.asp` both defined `IsSessionExpired()` and `UpdateSessionActivity()`. Removed the duplicates from `security.asp`.

4. **Missing `Response.End` after redirect** – In `login.asp`, after `Response.Redirect` the code continued executing and called `.Close` on an already-closed ADO connection, causing a 500 error. Fixed by closing resources before the redirect and adding `Response.End`.

5. **Missing `dashboard.asp`** – Login redirected to `dashboard.asp` which did not exist. Changed redirect target to `index.asp`.

6. **Database connection string** – Original used `.\SQLEXPRESS` (SQL Server Express named instance). Updated to `(local)` which matches the SQL Server instance available on this machine.

---

## Architecture Reminder

```
Browser → IIS Express (port 8080) → Classic ASP (.asp pages) → SQLOLEDB → SQL Server
```

This is a legacy monolith (circa 2000s) kept as-is for educational purposes. See `CarRetailSystem/README.md` for the full modernization roadmap.
