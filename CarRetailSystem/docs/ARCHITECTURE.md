# Architecture Documentation

## System Architecture Overview

### Monolithic Architecture Pattern

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT BROWSERS                           │
│  (IE6/IE7 - No modern browser support)                      │
└─────────────────────────────────────────────────────────────┘
                           │
                           │ HTTP/HTTPS
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    IIS WEB SERVER (5.0/6.0)                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │         Classic ASP Application (.asp files)         │  │
│  │  - index.asp                                         │  │
│  │  - login.asp                                         │  │
│  │  - inventory.asp                                     │  │
│  │  - sales.asp                                         │  │
│  │  - customers.asp                                     │  │
│  │  - dashboard.asp                                     │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  VBScript Includes (Server-side processing)         │  │
│  │  - config.asp (Database connections)                │  │
│  │  - session.asp (Session management)                 │  │
│  │  - security.asp (Authentication)                    │  │
│  │  - utils.asp (Helper functions)                     │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                           │
                           │ COM Runtime
                           ▼
┌─────────────────────────────────────────────────────────────┐
│              VB6 COM COMPONENTS (Registered DLLs)            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  CarInventory.dll - Inventory management logic       │  │
│  │  SalesOrder.dll - Sales transaction processing      │  │
│  │  CustomerMgmt.dll - Customer operations             │  │
│  │  ReportGenerator.dll - Report generation            │  │
│  │  Utilities.dll - Shared utilities                   │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                           │
                           │ ADO/OLE DB
                           ▼
┌─────────────────────────────────────────────────────────────┐
│           SQL SERVER 7.0/2000/2005 DATABASE                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Tables:                                             │  │
│  │  - Users (Authentication/Authorization)             │  │
│  │  - Cars (Inventory)                                 │  │
│  │  - Customers (Customer Master)                      │  │
│  │  - Sales (Transaction History)                      │  │
│  │  - Inventory_Log (Audit Trail)                      │  │
│  │                                                      │  │
│  │  Stored Procedures:                                 │  │
│  │  - sp_GetAvailableCars                              │  │
│  │  - sp_CreateSalesOrder                              │  │
│  │  - sp_UpdateInventory                               │  │
│  │  - sp_GenerateMonthlySalesReport                    │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow

### Login Process
1. User enters credentials on login.asp
2. ASP retrieves password from database (plain text comparison)
3. Session variables created if credentials match
4. Redirects to dashboard.asp

### Inventory Browsing
1. User requests inventory.asp
2. ASP checks session authentication
3. Creates ADO RecordSet from Cars table
4. Builds HTML table dynamically
5. For each row, calls VB6 COM method to get availability

### Sales Order Creation
1. User initiates order from sales.asp
2. ASP validates customer and car IDs
3. Calls SalesOrder.dll COM component
4. COM component starts SQL transaction
5. Inserts to Sales, updates Cars stock, logs to Inventory_Log
6. Returns order ID or error code

## Key Architectural Characteristics

### Monolithic
- **Single Deployable Unit**: All code deployed together
- **Tight Coupling**: ASP pages directly reference COM objects
- **Shared Database**: All components use same database
- **No Separation of Concerns**: Business logic mixed with presentation

### Stateful
- **Session-Based**: Session ID stored in cookies
- **In-Memory Sessions**: User state in IIS memory
- **No Distributed Sessions**: Cannot scale horizontally

### Synchronous
- **Blocking Calls**: ASP waits for COM method completion
- **Request-Response**: No async processing
- **Real-time Constraints**: Long operations block user

## Deployment Topology

```
Production Environment:
├── Web Server (Windows Server 2003/2008)
│   ├── IIS 5.0/6.0
│   ├── ASP Engine
│   └── VB6 COM Components (registered)
│
└── Database Server (Windows Server)
    └── SQL Server 7.0/2000/2005
```

**Note**: Current architecture requires COM components to be registered on the same server as IIS.

## Communication Patterns

### ASP → COM (Early Binding)
```vbscript
Set objInventory = CreateObject("CarRetailSystem.CarInventory")
Call objInventory.Initialize()
Dim rs = objInventory.GetAvailableCars("Toyota")
```

### COM → SQL Server (ADO Connection String)
```
Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;
```

## Session Management

- **Cookie Name**: CarRetailSID
- **Timeout**: 30 minutes of inactivity
- **Storage**: IIS In-Process Session Store
- **Security**: Unencrypted cookie value

## Error Handling

Current error handling uses:
- `On Error Resume Next` - Suppresses errors (antipattern)
- `On Error GoTo ErrorLabel` - Minimal error recovery
- No centralized logging
- No structured exception handling

## Performance Characteristics

### Bottlenecks
1. **Database Connections**: Each ASP request creates new ADO connection
2. **No Caching**: All data queries hit database
3. **String Concatenation**: SQL queries built inefficiently
4. **COM Overhead**: Runtime cost of COM interop

### Scalability Issues
1. Cannot scale beyond single server
2. Session affinity required
3. Database becomes bottleneck
4. No load balancing possible

## Security Architecture (Legacy)

**WARNING**: This is NOT secure by modern standards.

- **Authentication**: Basic username/password (plain text storage)
- **Authorization**: Simple role-based (Admin/Salesperson/Manager)
- **Encryption**: None - all traffic in plaintext
- **Input Validation**: Minimal - prone to SQL injection
- **Session Security**: No CSRF tokens, vulnerable to hijacking
