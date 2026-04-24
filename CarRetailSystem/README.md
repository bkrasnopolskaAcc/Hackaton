# CarRetailSystem - Legacy ASP/VB6 Monolith

## 📋 Project Overview

**CarRetailSystem** is a classic enterprise monolith application for managing car inventory and retail sales operations. Built with legacy Microsoft technologies (ASP, VB6, COM), it has served the business for 15+ years.

### Current Status: **LEGACY - SLATED FOR MODERNIZATION**

This project is being maintained in its original form while modernization efforts are planned.

---

## 🏗️ Architecture

### Architecture Type: **Monolithic**
- Single-tier application deployed on Windows Server with IIS
- All components tightly coupled
- Shared SQL Server database
- No service separation

### Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Web Frontend** | Classic ASP (VBScript) | IIS 5.0+ |
| **Business Logic** | VB6 COM Components | 6.0 |
| **Data Access** | ADO (ActiveX Data Objects) | 2.8 |
| **Database** | SQL Server | 7.0 |
| **Scripting** | Inline VBScript | - |

---

## 📁 Project Structure

```
CarRetailSystem/
├── ASP/                              # IIS Web Application (VBScript)
│   ├── pages/                        # ASP Web Pages
│   │   ├── index.asp                 # Landing page
│   │   ├── login.asp                 # Authentication
│   │   ├── dashboard.asp             # Admin dashboard
│   │   ├── inventory.asp             # Car inventory management
│   │   ├── sales.asp                 # Sales order processing
│   │   ├── customers.asp             # Customer management
│   │   ├── reports.asp               # Business reports
│   │   └── logout.asp                # Session termination
│   │
│   └── includes/                     # Shared ASP Includes
│       ├── config.asp                # Configuration & Connection strings
│       ├── session.asp               # Session management
│       ├── utils.asp                 # Helper functions
│       ├── db_connection.asp         # ADO connection manager
│       └── security.asp              # Basic authentication
│
├── VB6Components/                    # COM Components (Compiled DLLs)
│   ├── CarInventory.vb               # Car inventory business logic
│   ├── SalesOrder.vb                 # Sales transaction handling
│   ├── CustomerMgmt.vb               # Customer operations
│   ├── ReportGenerator.vb            # Report generation
│   └── Utilities.vb                  # Shared utilities
│
├── SQL/                              # Database Scripts
│   ├── schema.sql                    # Table definitions
│   ├── stored_procedures.sql         # T-SQL stored procedures
│   ├── sample_data.sql               # Initial data
│   └── database_setup.sql            # Database creation script
│
├── docs/                             # Documentation
│   ├── ARCHITECTURE.md               # Architecture diagrams & explanations
│   ├── DEPLOYMENT.md                 # Deployment instructions
│   ├── DATABASE_SCHEMA.md            # Database documentation
│   └── COM_COMPONENTS.md             # COM component reference
│
├── config/                           # Configuration files
│   └── iis_config.xml                # IIS 6.0 settings
│
└── README.md                         # This file

```

---

## 🗄️ Database Design (SQL Server 7.0)

### Core Tables

**Cars**
```sql
CarID (PK) | Make | Model | Year | Color | VIN | Price | Stock | CreatedDate
```

**Customers**
```sql
CustomerID (PK) | FirstName | LastName | Email | Phone | Address | RegisteredDate
```

**Sales**
```sql
SalesID (PK) | CustomerID (FK) | CarID (FK) | SalesPrice | SalesDate | SalespersonID
```

**Inventory_Log**
```sql
LogID (PK) | CarID (FK) | Action | Quantity | ChangedDate | ChangedBy
```

### Key Stored Procedures
- `sp_GetAvailableCars` - List inventory with filter
- `sp_CreateSalesOrder` - Transaction-aware sales creation
- `sp_UpdateInventory` - Atomic stock updates
- `sp_GetCustomerSalesHistory` - Historical data retrieval
- `sp_GenerateMonthlySalesReport` - Aggregated reporting

---

## 🔧 Technology Details

### ASP/VBScript Features
- **Inline VBScript:** Business logic embedded directly in .asp pages
- **Session Management:** Session-based authentication stored in global.asa
- **ADO Recordsets:** RecordSet objects for data manipulation
- **String Concatenation:** SQL queries built with string concatenation (security concern)
- **No URL Routing:** Direct page mapping (e.g., /pages/inventory.asp)

### VB6 COM Components
- **COM DLLs:** Pre-compiled binary components registered on server
- **Early Binding:** Used by ASP pages for type safety
- **Business Logic Isolation:** Core logic separated from presentation (partial)
- **State Management:** Some components maintain session state

### ADO Usage Pattern
```vbscript
' Typical ADO pattern in ASP
Set conn = CreateObject("ADODB.Connection")
conn.ConnectionString = "Provider=SQLOLEDB;Server=...;UID=...;PWD=..."
Set rs = conn.Execute("SELECT * FROM Cars WHERE Stock > 0")
Do While Not rs.EOF
    Response.Write rs("Make") & " - " & rs("Price") & "<BR>"
    rs.MoveNext
Loop
rs.Close
conn.Close
```

### SQL Server 7.0 Features Used
- T-SQL stored procedures for complex operations
- Triggers for audit logging
- Indexes on frequently queried columns
- No full-text search
- No XML support

---

## 🚀 Deployment

### Requirements
- **OS:** Windows Server 2003/2008
- **IIS:** 5.0 or 6.0
- **SQL Server:** 7.0 or 2000
- **.NET Framework:** Not required
- **COM:** Must be registered on production server

### Deployment Steps (Current)
1. Copy ASP files to IIS virtual directory
2. Register VB6 DLLs via `regsvr32`
3. Configure connection strings in `config.asp`
4. Run SQL scripts against database
5. Set IIS permissions appropriately
6. Restart IIS (`iisreset`)

---

## 🔐 Security Issues (Legacy)

⚠️ **CRITICAL CONCERNS FOR MODERNIZATION:**

1. **SQL Injection:** String concatenation in queries (not parameterized)
2. **Session Hijacking:** Session IDs stored in cookies without encryption
3. **No HTTPS:** All communication in plaintext
4. **Hardcoded Credentials:** Database passwords in config.asp
5. **No Input Validation:** Direct user input in SQL queries
6. **Weak Authentication:** Basic username/password in Session
7. **No CSRF Protection:** No token validation on state-changing operations
8. **No Password Hashing:** Passwords stored in plain text
9. **Cross-Site Scripting (XSS):** User input not sanitized before output

---

## 🐛 Known Limitations

| Issue | Severity | Impact |
|-------|----------|--------|
| No caching mechanism | Medium | High database load |
| Monolithic deployment | High | Cannot scale individual features |
| No logging framework | High | Difficult troubleshooting |
| Hard to test | High | No unit testing framework |
| Tight coupling | High | Changes require full regression testing |
| Performance bottlenecks | Medium | Slow page loads under high traffic |
| No API layer | High | Limited integration possibilities |
| Deprecated language | Medium | Hard to find new developers |
| No containerization | Medium | Difficult to manage environments |

---

## 📈 Modernization Roadmap

### Phase 1: Assessment & Documentation
- [ ] Map all ASP page dependencies
- [ ] Identify all COM component usages
- [ ] Document database schema completely
- [ ] Create data migration strategy
- [ ] Audit security vulnerabilities

### Phase 2: API Layer (Parallel Development)
- [ ] Build .NET Core REST API endpoints
- [ ] Implement proper authentication (JWT/OAuth)
- [ ] Add comprehensive API documentation
- [ ] Create API integration tests

### Phase 3: Data Migration
- [ ] Schema migration to modern SQL Server
- [ ] Data cleansing and validation
- [ ] Historical data archival strategy
- [ ] Zero-downtime migration plan

### Phase 4: Frontend Modernization
- [ ] Build React/Vue SPA frontend
- [ ] Implement responsive UI
- [ ] Migrate ASP pages to modern components
- [ ] User testing and feedback

### Phase 5: Full Cutover
- [ ] Blue-green deployment strategy
- [ ] Parallel run period
- [ ] Legacy system decommissioning
- [ ] Archive legacy code

---

## 🛠️ Development Setup

### Local Development Environment

**Prerequisites:**
- Windows 10/11
- IIS Express
- SQL Server Express LocalDB
- VB6 IDE (if component updates needed)

**Setup:**
```bash
# Clone/extract project
cd CarRetailSystem

# Configure IIS Express
iisexpress /path:. /port:8080

# Restore database (SQL Server Express)
sqlcmd -S localhost\SQLEXPRESS -i SQL\database_setup.sql

# Access application
# http://localhost:8080/ASP/pages/index.asp
```

### Running Tests
```bash
# Currently minimal - manual testing required
# Legacy system lacks automated test infrastructure
```

---

## 📋 File Descriptions

### ASP Pages

- **index.asp** - Landing page with login redirect
- **login.asp** - Basic session-based authentication
- **dashboard.asp** - Admin overview with key metrics
- **inventory.asp** - Car catalog management (CRUD)
- **sales.asp** - Sales order creation and tracking
- **customers.asp** - Customer database management
- **reports.asp** - Business intelligence dashboards
- **logout.asp** - Session destruction

### Includes

- **config.asp** - Global configuration (DB connection strings, settings)
- **session.asp** - Session cookie handling
- **utils.asp** - Utility functions (date formatting, string manipulation)
- **db_connection.asp** - ADO connection pooling (basic)
- **security.asp** - Authentication checks (basic)

### VB6 Components

- **CarInventory.com** - Stock management, availability checks
- **SalesOrder.com** - Order creation, validation, payment processing
- **CustomerMgmt.com** - Customer CRUD operations
- **ReportGenerator.com** - Report generation and PDF export
- **Utilities.com** - Shared math, string, and date utilities

### SQL Scripts

- **schema.sql** - Table creation, primary keys, foreign keys
- **stored_procedures.sql** - T-SQL business logic
- **sample_data.sql** - Test data (50 cars, 30 customers)
- **database_setup.sql** - Master setup script

---

## 📞 Support & Maintenance

### Current Maintenance Mode
- Bug fixes only
- Security patches applied
- No new feature development
- Backward compatibility maintained

### Known Workarounds
- Database connection timeouts: increase `CommandTimeout` in db_connection.asp
- Session expiration: restart browser to clear stale sessions
- Inventory discrepancies: run `sp_UpdateInventory` repair procedure

---

## 📚 Related Documentation

- [Architecture Details](docs/ARCHITECTURE.md)
- [Database Schema](docs/DATABASE_SCHEMA.md)
- [COM Components Reference](docs/COM_COMPONENTS.md)
- [Deployment Guide](docs/DEPLOYMENT.md)

---

## 🚫 Deprecation Notice

**This codebase represents legacy technology and is NOT recommended for new features or enhancements.**

- ASP/VB6 support is ending
- Security vulnerabilities cannot be fully remediated
- Performance limitations inherent to architecture
- Developer skills in these technologies are scarce
- Maintenance costs will increase over time

**Recommendation:** Plan migration to modern stack (.NET Core/React, SQL Server 2019+) within 12-24 months.

---

## 📄 License

Internal Business Use Only - Proprietary

---

## 👥 Contributors

- **Original Developer:** Unknown (Pre-2010)
- **Last Maintainer:** Legacy Systems Team
- **Modernization Lead:** [To be assigned]

---

**Last Updated:** April 2026  
**Next Review Date:** Q3 2026
