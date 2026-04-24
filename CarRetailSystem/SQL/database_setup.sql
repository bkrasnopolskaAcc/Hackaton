-- CarRetailSystem Database Setup Script
-- SQL Server 7.0 / 2000 / 2005
-- Run this script to create the complete database schema

-- Create Database
CREATE DATABASE CarRetailDB
GO

USE CarRetailDB
GO

-- ===== TABLES =====

-- Users Table
CREATE TABLE [Users] (
    [UserID] [int] PRIMARY KEY IDENTITY(1,1),
    [UserName] [varchar](50) NOT NULL UNIQUE,
    [Password] [varchar](50) NOT NULL,  -- SECURITY ISSUE: Plain text storage
    [FirstName] [varchar](50),
    [LastName] [varchar](50),
    [Email] [varchar](100),
    [Role] [varchar](20) DEFAULT 'Salesperson',  -- Admin, Salesperson, Manager
    [IsActive] [bit] DEFAULT 1,
    [CreatedDate] [datetime] DEFAULT GETDATE(),
    [LastLoginDate] [datetime]
)
GO

-- Cars Table (Inventory)
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
GO

-- Customers Table
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
GO

-- Sales Table (Order History)
CREATE TABLE [Sales] (
    [SalesID] [int] PRIMARY KEY IDENTITY(1,1),
    [CustomerID] [int] NOT NULL FOREIGN KEY REFERENCES [Customers]([CustomerID]),
    [CarID] [int] NOT NULL FOREIGN KEY REFERENCES [Cars]([CarID]),
    [SalesPrice] [decimal](10, 2),
    [SalesDate] [datetime] DEFAULT GETDATE(),
    [SalespersonID] [int] FOREIGN KEY REFERENCES [Users]([UserID]),
    [PaymentMethod] [varchar](20),  -- Cash, Credit, Finance
    [Notes] [text]
)
GO

-- Inventory Log (Audit Trail)
CREATE TABLE [Inventory_Log] (
    [LogID] [int] PRIMARY KEY IDENTITY(1,1),
    [CarID] [int] NOT NULL FOREIGN KEY REFERENCES [Cars]([CarID]),
    [Action] [varchar](20),  -- ADD, SALE, UPDATE, DELETE
    [Quantity] [int],
    [ChangedDate] [datetime] DEFAULT GETDATE(),
    [ChangedBy] [varchar](50),
    [Notes] [text]
)
GO

-- ===== INDEXES =====

CREATE INDEX IX_Cars_Make ON [Cars]([Make])
GO

CREATE INDEX IX_Cars_Stock ON [Cars]([Stock])
GO

CREATE INDEX IX_Customers_Email ON [Customers]([Email])
GO

CREATE INDEX IX_Sales_Date ON [Sales]([SalesDate])
GO

CREATE INDEX IX_Sales_CustomerID ON [Sales]([CustomerID])
GO

-- ===== STORED PROCEDURES =====

-- Get available cars
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
GO

-- Create sales order (with transaction)
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
        INSERT INTO Sales (CustomerID, CarID, SalesPrice, SalesDate, SalespersonID)
        VALUES (@CustomerID, @CarID, @SalesPrice, GETDATE(), @SalespersonID)
        
        SELECT @NewSalesID = @@IDENTITY
        
        UPDATE Cars SET Stock = Stock - 1 WHERE CarID = @CarID
        
        INSERT INTO Inventory_Log (CarID, Action, Quantity, ChangedDate, ChangedBy)
        VALUES (@CarID, 'SALE', 1, GETDATE(), 'SalesOrder_' + CAST(@NewSalesID AS VARCHAR))
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SET @NewSalesID = -1
    END CATCH
END
GO

-- Update inventory
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
GO

-- Get customer sales history
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
GO

-- Monthly sales report
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
GO

-- ===== TRIGGERS =====

-- Audit trigger for Cars table
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
GO

-- ===== SAMPLE DATA =====

-- Insert users
INSERT INTO [Users] (UserName, Password, FirstName, LastName, Email, Role)
VALUES 
    ('admin', 'admin123', 'System', 'Admin', 'admin@carretail.com', 'Admin'),
    ('john', 'pass123', 'John', 'Smith', 'john@carretail.com', 'Salesperson'),
    ('mary', 'pass123', 'Mary', 'Johnson', 'mary@carretail.com', 'Salesperson'),
    ('bob', 'pass123', 'Bob', 'Manager', 'bob@carretail.com', 'Manager')
GO

-- Insert sample cars
INSERT INTO [Cars] (Make, Model, Year, Color, VIN, Price, Stock, Description)
VALUES
    ('Toyota', 'Camry', 2020, 'Silver', '12345678901234567', 25000.00, 5, '2020 Toyota Camry, Automatic, 45K Miles'),
    ('Honda', 'Civic', 2019, 'Blue', '23456789012345678', 22000.00, 3, '2019 Honda Civic, Manual, 52K Miles'),
    ('Ford', 'F-150', 2021, 'Red', '34567890123456789', 35000.00, 2, '2021 Ford F-150 Pickup, 4WD, 28K Miles'),
    ('BMW', 'X5', 2020, 'Black', '45678901234567890', 55000.00, 1, '2020 BMW X5, Luxury, 35K Miles'),
    ('Mazda', 'CX-5', 2019, 'White', '56789012345678901', 28000.00, 4, '2019 Mazda CX-5, AWD, 48K Miles'),
    ('Chevrolet', 'Malibu', 2018, 'Gray', '67890123456789012', 18000.00, 6, '2018 Chevrolet Malibu, Auto, 65K Miles'),
    ('Volkswagen', 'Golf', 2020, 'Green', '78901234567890123', 24000.00, 2, '2020 VW Golf, Sporty, 32K Miles'),
    ('Audi', 'A4', 2019, 'Black', '89012345678901234', 42000.00, 1, '2019 Audi A4, Premium Sound, 44K Miles')
GO

-- Insert sample customers
INSERT INTO [Customers] (FirstName, LastName, Email, Phone, Address, City, State, ZipCode)
VALUES
    ('Michael', 'Brown', 'mbrown@email.com', '555-0101', '123 Main St', 'New York', 'NY', '10001'),
    ('Sarah', 'Davis', 'sdavis@email.com', '555-0102', '456 Oak Ave', 'Los Angeles', 'CA', '90001'),
    ('James', 'Wilson', 'jwilson@email.com', '555-0103', '789 Pine Rd', 'Chicago', 'IL', '60601'),
    ('Emily', 'Moore', 'emoore@email.com', '555-0104', '321 Elm St', 'Houston', 'TX', '77001'),
    ('David', 'Taylor', 'dtaylor@email.com', '555-0105', '654 Maple Dr', 'Phoenix', 'AZ', '85001')
GO

-- Insert sample sales
INSERT INTO [Sales] (CustomerID, CarID, SalesPrice, SalesDate, SalespersonID, PaymentMethod)
VALUES
    (1, 1, 25000.00, '2024-03-01', 2, 'Finance'),
    (2, 2, 22000.00, '2024-03-05', 3, 'Cash'),
    (3, 3, 35000.00, '2024-03-10', 2, 'Finance'),
    (4, 4, 55000.00, '2024-03-15', 3, 'Credit'),
    (5, 5, 28000.00, '2024-03-20', 2, 'Finance')
GO

PRINT 'Database creation completed successfully!'
PRINT 'Sample data has been inserted.'
PRINT 'Connect with: Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;'
