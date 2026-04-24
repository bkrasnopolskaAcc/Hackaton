# COM Components Reference

## Overview

The CarRetailSystem uses five VB6 COM components (DLLs) for business logic isolation from presentation layer. These are compiled binary components that must be registered on the production server.

---

## CarInventory Component

**File**: `CarInventory.dll`  
**Purpose**: Manage car inventory operations  
**ProgID**: `CarRetailSystem.CarInventory`

### Methods

#### Initialize()
```vbscript
Function Initialize() As Boolean
```
Initializes database connection.

**Returns**: Boolean (success/failure)

**Usage**:
```vbscript
Dim objInventory
Set objInventory = CreateObject("CarRetailSystem.CarInventory")
If objInventory.Initialize() Then
    Response.Write "Connected"
End If
```

---

#### GetAvailableCars()
```vbscript
Function GetAvailableCars(Optional make As String, Optional maxPrice As Long) As ADODB.Recordset
```
Retrieves cars available in stock with optional filtering.

**Parameters**:
- `make` (optional): Filter by vehicle make (e.g., "Toyota")
- `maxPrice` (optional): Maximum price filter

**Returns**: ADODB.RecordSet with columns:
- CarID
- Make
- Model
- Year
- Color
- VIN
- Price
- Stock

**Usage**:
```vbscript
Set rs = objInventory.GetAvailableCars("Toyota", 30000)
Do While Not rs.EOF
    Response.Write rs("Make") & " - $" & rs("Price") & "<BR>"
    rs.MoveNext
Loop
rs.Close
```

---

#### GetCarDetails()
```vbscript
Function GetCarDetails(carID As Long) As ADODB.Recordset
```
Get complete details for a specific car.

**Parameters**:
- `carID`: Car ID to retrieve

**Returns**: RecordSet with all car columns

---

#### UpdateStock()
```vbscript
Function UpdateStock(carID As Long, quantity As Long) As Boolean
```
Decrease stock after sale.

**Parameters**:
- `carID`: Car ID to update
- `quantity`: Quantity to subtract

**Returns**: Boolean (success/failure)

**Side Effects**: Automatically logs change to Inventory_Log

---

#### GetStockLevel()
```vbscript
Function GetStockLevel(carID As Long) As Long
```
Get current stock level for a car.

**Parameters**:
- `carID`: Car ID

**Returns**: Stock quantity

---

#### Terminate()
```vbscript
Sub Terminate()
```
Close database connection and cleanup resources.

**Usage**:
```vbscript
Call objInventory.Terminate()
Set objInventory = Nothing
```

---

## SalesOrder Component

**File**: `SalesOrder.dll`  
**Purpose**: Handle sales transactions  
**ProgID**: `CarRetailSystem.SalesOrder`

### Methods

#### Initialize()
```vbscript
Function Initialize() As Boolean
```
Initializes database connection.

**Returns**: Boolean

---

#### CreateSalesOrder()
```vbscript
Function CreateSalesOrder(customerID As Long, carID As Long, salePrice As Currency, salespersonID As Long) As Long
```
Create new sales order with transaction handling.

**Parameters**:
- `customerID`: Buying customer ID
- `carID`: Car being purchased
- `salePrice`: Sale price
- `salespersonID`: Salesperson employee ID

**Returns**: New SalesID or 0 on failure

**Behavior**:
- Wrapped in SQL transaction
- Atomically updates: Sales, Cars (stock), Inventory_Log
- Rolls back all changes on any error
- Returns -1 on error

**Usage**:
```vbscript
Dim newOrderID
newOrderID = objSales.CreateSalesOrder(1, 5, 28000, 2)
If newOrderID > 0 Then
    Response.Write "Order created: " & newOrderID
Else
    Response.Write "Order creation failed"
End If
```

---

#### GetSalesHistory()
```vbscript
Function GetSalesHistory(customerID As Long) As ADODB.Recordset
```
Get all purchases by a customer.

**Parameters**:
- `customerID`: Customer ID

**Returns**: RecordSet with columns:
- SalesID
- SalesDate
- SalesPrice
- Make
- Model
- Year

---

#### CalculateTax()
```vbscript
Function CalculateTax(salePrice As Currency) As Currency
```
Calculate sales tax (10%).

**Parameters**:
- `salePrice`: Amount before tax

**Returns**: Tax amount

---

#### CalculateTotal()
```vbscript
Function CalculateTotal(salePrice As Currency) As Currency
```
Calculate total price including tax.

**Parameters**:
- `salePrice`: Amount before tax

**Returns**: Total with tax applied

**Usage**:
```vbscript
Dim basePrice, totalPrice
basePrice = 25000
totalPrice = objSales.CalculateTotal(basePrice)
Response.Write "Total: " & FormatCurrency(totalPrice)
' Output: Total: $27,500.00
```

---

## CustomerMgmt Component

**File**: `CustomerMgmt.dll`  
**Purpose**: Customer management operations  
**ProgID**: `CarRetailSystem.CustomerMgmt`

### Methods

#### Initialize()
```vbscript
Function Initialize() As Boolean
```
Initialize database connection.

---

#### GetCustomer()
```vbscript
Function GetCustomer(customerID As Long) As ADODB.Recordset
```
Retrieve customer details.

**Returns**: RecordSet with all customer columns

---

#### CreateCustomer()
```vbscript
Function CreateCustomer(firstName As String, lastName As String, email As String, phone As String) As Long
```
Create new customer.

**Returns**: New CustomerID

---

#### UpdateCustomer()
```vbscript
Function UpdateCustomer(customerID As Long, email As String, phone As String) As Boolean
```
Update customer contact info.

**Returns**: Boolean (success/failure)

---

#### GetCustomerSalesCount()
```vbscript
Function GetCustomerSalesCount(customerID As Long) As Long
```
Count total purchases by customer.

**Returns**: Sale count

---

#### SearchCustomers()
```vbscript
Function SearchCustomers(searchTerm As String) As ADODB.Recordset
```
Search customers by name or email.

**Returns**: RecordSet matching criteria

---

## ReportGenerator Component

**File**: `ReportGenerator.dll`  
**Purpose**: Generate business reports  
**ProgID**: `CarRetailSystem.ReportGenerator`

### Methods

#### Initialize()
```vbscript
Function Initialize() As Boolean
```
Initialize component.

---

#### GenerateMonthlySalesReport()
```vbscript
Function GenerateMonthlySalesReport(year As Integer, month As Integer) As ADODB.Recordset
```
Generate monthly sales summary.

**Parameters**:
- `year`: Year (YYYY)
- `month`: Month (1-12)

**Returns**: RecordSet with:
- Month
- TotalSales (count)
- TotalRevenue
- AvgPrice

---

#### GenerateInventoryReport()
```vbscript
Function GenerateInventoryReport() As ADODB.Recordset
```
Current inventory levels by make/model.

**Returns**: RecordSet with inventory summary

---

#### GenerateTopSalesReport()
```vbscript
Function GenerateTopSalesReport(topN As Integer) As ADODB.Recordset
```
Top N best-selling cars.

**Parameters**:
- `topN`: Number of results (default 10)

**Returns**: RecordSet with top sellers

---

#### ExportToCSV()
```vbscript
Function ExportToCSV(reportData As ADODB.Recordset, filePath As String) As Boolean
```
Export report data to CSV file.

**Parameters**:
- `reportData`: RecordSet to export
- `filePath`: Output file path

**Returns**: Boolean (success/failure)

---

#### ExportToPDF()
```vbscript
Function ExportToPDF(reportData As ADODB.Recordset, filePath As String) As Boolean
```
Export report to PDF (requires PDF library).

**Parameters**:
- `reportData`: RecordSet to export
- `filePath`: Output file path

**Returns**: Boolean (success/failure)

**Note**: Requires third-party PDF component

---

## Utilities Component

**File**: `Utilities.dll`  
**Purpose**: Shared utility functions  
**ProgID**: `CarRetailSystem.Utilities`

### Methods

#### FormatCurrency()
```vbscript
Function FormatCurrency(amount As Currency) As String
```
Format number as currency string.

**Returns**: String like "$1,234.56"

---

#### FormatDate()
```vbscript
Function FormatDate(dateValue As Date) As String
```
Format date as MM/DD/YYYY.

**Returns**: Formatted date string

---

#### ValidateEmail()
```vbscript
Function ValidateEmail(email As String) As Boolean
```
Basic email validation.

**Returns**: Boolean

---

#### ValidatePhone()
```vbscript
Function ValidatePhone(phone As String) As Boolean
```
Basic phone number validation.

**Returns**: Boolean

---

#### GenerateGUID()
```vbscript
Function GenerateGUID() As String
```
Generate unique GUID string.

**Returns**: GUID string

---

#### EncryptPassword()
```vbscript
Function EncryptPassword(password As String) As String
```
Simple password encoding (NOT cryptographically secure).

**Returns**: Encoded string

---

## Registration

### Register COM Components
```batch
REM Run as Administrator
regsvr32 CarInventory.dll
regsvr32 SalesOrder.dll
regsvr32 CustomerMgmt.dll
regsvr32 ReportGenerator.dll
regsvr32 Utilities.dll
```

### Unregister Components
```batch
regsvr32 /u CarInventory.dll
regsvr32 /u SalesOrder.dll
regsvr32 /u CustomerMgmt.dll
regsvr32 /u ReportGenerator.dll
regsvr32 /u Utilities.dll
```

### Verify Registration
```batch
REM Check if registered
reg query "HKEY_CLASSES_ROOT\CarRetailSystem.CarInventory"
```

---

## Usage Pattern

Typical usage in ASP pages:

```vbscript
<%
' Initialize components
Dim objInventory, objSales, objCustomer
Set objInventory = CreateObject("CarRetailSystem.CarInventory")
Set objSales = CreateObject("CarRetailSystem.SalesOrder")
Set objCustomer = CreateObject("CarRetailSystem.CustomerMgmt")

objInventory.Initialize()
objSales.Initialize()
objCustomer.Initialize()

' Use components
Dim availableCars
Set availableCars = objInventory.GetAvailableCars("Toyota")

While Not availableCars.EOF
    Response.Write availableCars("Make") & " " & availableCars("Model")
    availableCars.MoveNext
Wend

' Cleanup
Call objInventory.Terminate()
Call objSales.Terminate()
Call objCustomer.Terminate()

Set objInventory = Nothing
Set objSales = Nothing
Set objCustomer = Nothing
%>
```

---

## Error Handling

### Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| 429 - ActiveX component not available | DLL not registered | Run regsvr32 |
| 80070005 - Access denied | Permissions issue | Check NTFS permissions |
| 80004005 - Database connection failed | SQL Server down | Verify SQL Server running |
| 80040e21 - ADO error | Invalid SQL syntax | Check stored procedure |

### Debugging

Enable debug logging in components (if available):
```vbscript
objComponent.EnableDebugLogging True
```

Check Windows Event Viewer for COM errors:
- Event Viewer → Windows Logs → System
- Look for errors from "COM+"

---

## Modernization Notes

⚠️ **Issues with current architecture**:

1. **No Error Propagation**: Errors silently fail
2. **No Async Support**: All operations blocking
3. **Limited Transactions**: Only at SP level
4. **No Logging**: No centralized error logging
5. **COM Overhead**: Performance penalty for COM interop
6. **32-bit limitation**: Cannot run on 64-bit-only systems
7. **No API**: Components only accessible from COM
8. **No Versioning**: Components must be recompiled for updates

**Modernization Strategy**:
- Move to .NET assemblies (removes COM overhead)
- Implement structured logging
- Add async/await support
- Create REST API layer
- Implement dependency injection
- Add unit testing
