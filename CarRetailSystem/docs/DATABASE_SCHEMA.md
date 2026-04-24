# Database Schema Documentation

## Database: CarRetailDB (SQL Server 7.0+)

### Overview
The CarRetailSystem database stores all data for a car retail sales operation including inventory, customers, sales transactions, and audit logs.

---

## Tables

### Users Table
**Purpose**: Application authentication and authorization

```sql
CREATE TABLE [Users] (
    [UserID] [int] PRIMARY KEY IDENTITY(1,1),
    [UserName] [varchar](50) NOT NULL UNIQUE,
    [Password] [varchar](50) NOT NULL,
    [FirstName] [varchar](50),
    [LastName] [varchar](50),
    [Email] [varchar](100),
    [Role] [varchar](20) DEFAULT 'Salesperson',
    [IsActive] [bit] DEFAULT 1,
    [CreatedDate] [datetime] DEFAULT GETDATE(),
    [LastLoginDate] [datetime]
)
```

**Columns**:
- `UserID`: Primary key, auto-increment
- `UserName`: Unique login identifier
- `Password`: Plain text (SECURITY CONCERN)
- `Role`: Admin, Salesperson, Manager
- `IsActive`: Soft delete flag

**Indexes**:
- PRIMARY KEY (UserID)
- UNIQUE (UserName)

**Sample Data**:
| UserID | UserName | Role | Email |
|--------|----------|------|-------|
| 1 | admin | Admin | admin@carretail.com |
| 2 | john | Salesperson | john@carretail.com |
| 3 | mary | Salesperson | mary@carretail.com |
| 4 | bob | Manager | bob@carretail.com |

---

### Cars Table
**Purpose**: Vehicle inventory master file

```sql
CREATE TABLE [Cars] (
    [CarID] [int] PRIMARY KEY IDENTITY(1,1),
    [Make] [varchar](50) NOT NULL,
    [Model] [varchar](50) NOT NULL,
    [Year] [int],
    [Color] [varchar](30),
    [VIN] [varchar](17) UNIQUE,
    [Price] [decimal](10, 2),
    [Stock] [int] DEFAULT 0,
    [Description] [text],
    [CreatedDate] [datetime] DEFAULT GETDATE(),
    [LastModifiedDate] [datetime],
    [IsActive] [bit] DEFAULT 1
)
```

**Columns**:
- `CarID`: Primary key, auto-increment
- `Make`: Vehicle manufacturer (Toyota, Honda, etc.)
- `Model`: Vehicle model name
- `Year`: Model year
- `VIN`: Vehicle Identification Number (unique)
- `Price`: Sale price in USD
- `Stock`: Available quantity
- `IsActive`: Soft delete flag

**Indexes**:
- PRIMARY KEY (CarID)
- UNIQUE (VIN)
- INDEX (Make) - for filtering by manufacturer
- INDEX (Stock) - for availability queries

**Sample Data**:
| CarID | Make | Model | Year | Color | VIN | Price | Stock |
|-------|------|-------|------|-------|-----|-------|-------|
| 1 | Toyota | Camry | 2020 | Silver | 12345678901234567 | 25000.00 | 5 |
| 2 | Honda | Civic | 2019 | Blue | 23456789012345678 | 22000.00 | 3 |
| 3 | Ford | F-150 | 2021 | Red | 34567890123456789 | 35000.00 | 2 |

---

### Customers Table
**Purpose**: Customer master file

```sql
CREATE TABLE [Customers] (
    [CustomerID] [int] PRIMARY KEY IDENTITY(1,1),
    [FirstName] [varchar](50) NOT NULL,
    [LastName] [varchar](50) NOT NULL,
    [Email] [varchar](100),
    [Phone] [varchar](20),
    [Address] [varchar](255),
    [City] [varchar](50),
    [State] [varchar](2),
    [ZipCode] [varchar](10),
    [RegisteredDate] [datetime] DEFAULT GETDATE(),
    [IsActive] [bit] DEFAULT 1
)
```

**Columns**:
- `CustomerID`: Primary key, auto-increment
- `FirstName`, `LastName`: Customer name
- `Email`, `Phone`: Contact information
- `Address`, `City`, `State`, `ZipCode`: Location
- `RegisteredDate`: Account creation date
- `IsActive`: Soft delete flag

**Indexes**:
- PRIMARY KEY (CustomerID)
- INDEX (Email) - for customer lookup

**Sample Data**:
| CustomerID | FirstName | LastName | Email | Phone |
|------------|-----------|----------|-------|-------|
| 1 | Michael | Brown | mbrown@email.com | 555-0101 |
| 2 | Sarah | Davis | sdavis@email.com | 555-0102 |
| 3 | James | Wilson | jwilson@email.com | 555-0103 |

---

### Sales Table
**Purpose**: Transaction history and sales records

```sql
CREATE TABLE [Sales] (
    [SalesID] [int] PRIMARY KEY IDENTITY(1,1),
    [CustomerID] [int] NOT NULL FOREIGN KEY REFERENCES [Customers]([CustomerID]),
    [CarID] [int] NOT NULL FOREIGN KEY REFERENCES [Cars]([CarID]),
    [SalesPrice] [decimal](10, 2),
    [SalesDate] [datetime] DEFAULT GETDATE(),
    [SalespersonID] [int] FOREIGN KEY REFERENCES [Users]([UserID]),
    [PaymentMethod] [varchar](20),
    [Notes] [text]
)
```

**Columns**:
- `SalesID`: Primary key, auto-increment
- `CustomerID`: Foreign key to Customers
- `CarID`: Foreign key to Cars
- `SalesPrice`: Sale price (may differ from catalog)
- `SalesDate`: Date of transaction
- `SalespersonID`: Employee who made the sale
- `PaymentMethod`: Cash, Credit, Finance

**Relationships**:
- FOREIGN KEY (CustomerID) → Customers(CustomerID)
- FOREIGN KEY (CarID) → Cars(CarID)
- FOREIGN KEY (SalespersonID) → Users(UserID)

**Indexes**:
- PRIMARY KEY (SalesID)
- INDEX (SalesDate) - for date-range queries
- INDEX (CustomerID) - for customer lookup

**Sample Data**:
| SalesID | CustomerID | CarID | SalesPrice | SalesDate | SalespersonID |
|---------|------------|-------|-----------|-----------|---------------|
| 1 | 1 | 1 | 25000.00 | 2024-03-01 | 2 |
| 2 | 2 | 2 | 22000.00 | 2024-03-05 | 3 |

---

### Inventory_Log Table
**Purpose**: Audit trail for inventory changes

```sql
CREATE TABLE [Inventory_Log] (
    [LogID] [int] PRIMARY KEY IDENTITY(1,1),
    [CarID] [int] NOT NULL FOREIGN KEY REFERENCES [Cars]([CarID]),
    [Action] [varchar](20),
    [Quantity] [int],
    [ChangedDate] [datetime] DEFAULT GETDATE(),
    [ChangedBy] [varchar](50),
    [Notes] [text]
)
```

**Columns**:
- `LogID`: Primary key, auto-increment
- `CarID`: Foreign key to Cars
- `Action`: ADD, SALE, UPDATE, DELETE, AUDIT
- `Quantity`: Number of units changed
- `ChangedDate`: Timestamp
- `ChangedBy`: User or system that made change
- `Notes`: Additional details

**Actions**:
- `ADD`: New inventory added
- `SALE`: Car sold (quantity negative)
- `UPDATE`: Price/details updated
- `DELETE`: Inventory deleted
- `AUDIT`: System audit triggered

---

## Stored Procedures

### sp_GetAvailableCars
**Purpose**: Retrieve cars in stock with optional filtering

```sql
CREATE PROCEDURE sp_GetAvailableCars
    @Make VARCHAR(50) = NULL
AS
BEGIN
    SELECT CarID, Make, Model, Year, Color, VIN, Price, Stock
    FROM Cars
    WHERE Stock > 0
        AND IsActive = 1
        AND (@Make IS NULL OR Make = @Make)
    ORDER BY Make, Model
END
```

**Parameters**:
- `@Make` (optional): Filter by vehicle make

**Returns**: RecordSet with available cars

**Example**: `EXEC sp_GetAvailableCars @Make='Toyota'`

---

### sp_CreateSalesOrder
**Purpose**: Create sales transaction with atomic operations

```sql
CREATE PROCEDURE sp_CreateSalesOrder
    @CustomerID INT,
    @CarID INT,
    @SalesPrice DECIMAL(10,2),
    @SalespersonID INT,
    @NewSalesID INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        -- Insert sale
        INSERT INTO Sales (CustomerID, CarID, SalesPrice, SalesDate, SalespersonID)
        VALUES (@CustomerID, @CarID, @SalesPrice, GETDATE(), @SalespersonID)
        
        -- Get ID
        SELECT @NewSalesID = @@IDENTITY
        
        -- Update inventory
        UPDATE Cars SET Stock = Stock - 1 WHERE CarID = @CarID
        
        -- Log change
        INSERT INTO Inventory_Log (CarID, Action, Quantity, ChangedDate, ChangedBy)
        VALUES (@CarID, 'SALE', 1, GETDATE(), 'SalesOrder_' + CAST(@NewSalesID AS VARCHAR))
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SET @NewSalesID = -1
    END CATCH
END
```

**Key Features**:
- Transaction wrapping (atomic)
- Rollback on error
- Automatic inventory update
- Audit log entry

**Parameters**:
- `@CustomerID`: Customer making purchase
- `@CarID`: Car being sold
- `@SalesPrice`: Sale amount
- `@SalespersonID`: Salesperson handling sale
- `@NewSalesID` (OUTPUT): Returned SalesID or -1 on error

---

### sp_UpdateInventory
**Purpose**: Update car inventory stock levels

```sql
CREATE PROCEDURE sp_UpdateInventory
    @CarID INT,
    @Quantity INT
AS
BEGIN
    UPDATE Cars
    SET Stock = Stock - @Quantity,
        LastModifiedDate = GETDATE()
    WHERE CarID = @CarID
    
    INSERT INTO Inventory_Log (CarID, Action, Quantity, ChangedDate, ChangedBy)
    VALUES (@CarID, 'UPDATE', @Quantity, GETDATE(), 'ADMIN')
END
```

**Parameters**:
- `@CarID`: Car to update
- `@Quantity`: Quantity adjustment (negative = decrease)

---

### sp_GetCustomerSalesHistory
**Purpose**: Retrieve all sales for a specific customer

```sql
CREATE PROCEDURE sp_GetCustomerSalesHistory
    @CustomerID INT
AS
BEGIN
    SELECT 
        S.SalesID,
        S.SalesDate,
        S.SalesPrice,
        C.Make,
        C.Model,
        C.Year,
        U.UserName
    FROM Sales S
    INNER JOIN Cars C ON S.CarID = C.CarID
    LEFT JOIN Users U ON S.SalespersonID = U.UserID
    WHERE S.CustomerID = @CustomerID
    ORDER BY S.SalesDate DESC
END
```

**Parameters**:
- `@CustomerID`: Customer to query

**Returns**: Sales history with vehicle details and salesperson

---

### sp_GenerateMonthlySalesReport
**Purpose**: Generate aggregated sales statistics by month

```sql
CREATE PROCEDURE sp_GenerateMonthlySalesReport
    @Year INT,
    @Month INT
AS
BEGIN
    SELECT 
        DATENAME(MONTH, S.SalesDate) AS Month,
        COUNT(S.SalesID) AS TotalSales,
        SUM(S.SalesPrice) AS TotalRevenue,
        AVG(S.SalesPrice) AS AvgPrice
    FROM Sales S
    WHERE YEAR(S.SalesDate) = @Year
        AND MONTH(S.SalesDate) = @Month
    GROUP BY DATENAME(MONTH, S.SalesDate)
END
```

**Parameters**:
- `@Year`: Year to report on
- `@Month`: Month (1-12)

**Returns**: Sales summary statistics

---

## Triggers

### TR_Cars_Audit
**Purpose**: Automatically log all updates and deletes to Cars table

```sql
CREATE TRIGGER TR_Cars_Audit
ON Cars
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON
    
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO Inventory_Log (CarID, Action, ChangedDate, ChangedBy)
        SELECT CarID, 'AUDIT', GETDATE(), 'TRIGGER'
        FROM deleted
    END
END
```

**Behavior**: Triggers whenever Cars row is modified or deleted

---

## Relationships Diagram

```
Users (UserID PK)
    ├── Sales (SalespersonID FK)
    
Customers (CustomerID PK)
    └── Sales (CustomerID FK)

Cars (CarID PK)
    ├── Sales (CarID FK)
    └── Inventory_Log (CarID FK)
```

---

## Data Integrity Rules

1. **Referential Integrity**: 
   - Sales.CustomerID must exist in Customers
   - Sales.CarID must exist in Cars
   - Sales.SalespersonID must exist in Users

2. **Unique Constraints**:
   - Users.UserName is unique
   - Cars.VIN is unique

3. **Check Constraints** (recommended, not yet implemented):
   - Cars.Stock >= 0
   - Cars.Price > 0
   - Sales.SalesPrice > 0

4. **Default Values**:
   - CreatedDate = GETDATE()
   - Stock = 0
   - IsActive = 1

---

## Performance Considerations

### Current Indexes
- CarID, Make, Stock on Cars
- SalesDate, CustomerID on Sales
- Email on Customers

### Recommended Additional Indexes
```sql
CREATE INDEX IX_Sales_CarID ON Sales(CarID)
CREATE INDEX IX_Cars_IsActive ON Cars(IsActive)
CREATE INDEX IX_Customers_IsActive ON Customers(IsActive)
CREATE INDEX IX_Inventory_Log_CarID ON Inventory_Log(CarID)
```

### Query Optimization
- Use sp_GetAvailableCars instead of SELECT *
- Add WHERE IsActive = 1 to all queries
- Use indexed columns for filtering
- Avoid calculations in WHERE clauses

---

## Maintenance

### Regular Tasks
- Reorganize indexes monthly: `ALTER INDEX ALL ON [TableName] REORGANIZE`
- Update statistics: `UPDATE STATISTICS [TableName]`
- Backup daily: `BACKUP DATABASE CarRetailDB`
- Archive old records to history table annually

### Data Cleanup
```sql
-- Soft delete inactive users
UPDATE Users SET IsActive = 0 WHERE LastLoginDate < DATEADD(YEAR, -1, GETDATE())

-- Archive old sales
INSERT INTO Sales_Archive SELECT * FROM Sales WHERE SalesDate < DATEADD(YEAR, -5, GETDATE())
DELETE FROM Sales WHERE SalesDate < DATEADD(YEAR, -5, GETDATE())
```

---

## Notes for Modernization

⚠️ **Issues to address during migration**:

1. **Password Storage**: Move to hashed passwords with salts
2. **Decimal Precision**: Money should use MONEY or NUMERIC(10,2) with proper type
3. **Timestamps**: Add UpdatedDate to all audit-relevant tables
4. **Soft Deletes**: Consider implementing proper archival instead of IsActive flags
5. **Audit Trail**: Implement proper change tracking (who changed what when)
6. **Referential Integrity**: Consider ON DELETE CASCADE policies
7. **Performance**: SQL 7.0 doesn't support many modern optimizations
