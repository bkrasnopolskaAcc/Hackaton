VERSION 5.00
Begin VB.Form frmSalesOrder
   Caption         =   "Sales Order - VB6 COM Component"
   ClientHeight    =   6000
   ClientLeft      =   60
   ClientTop       =   350
   ClientWidth     =   9240
End
Attribute VB_Name = "SalesOrder"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

' CarRetailSystem - Sales Order COM Component
' Handles transaction creation and validation
' VB6 Class Module compiled as DLL
' Registration: regsvr32 SalesOrder.dll

Option Explicit

Private conn As ADODB.Connection
Private Const DB_CONN_STRING = "Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;"

' Initialize
Public Function Initialize() As Boolean
    On Error GoTo ErrHandler
    
    Set conn = New ADODB.Connection
    conn.ConnectionString = DB_CONN_STRING
    conn.ConnectionTimeout = 30
    conn.Open
    
    Initialize = True
    Exit Function
    
ErrHandler:
    Initialize = False
End Function

' Create new sales order
Public Function CreateSalesOrder(customerID As Long, carID As Long, salePrice As Currency, salespersonID As Long) As Long
    On Error GoTo ErrHandler
    
    Dim sql As String
    Dim cmd As ADODB.Command
    Dim newSalesID As Long
    
    ' Validate inputs
    If customerID <= 0 Or carID <= 0 Or salePrice <= 0 Then
        CreateSalesOrder = 0
        Exit Function
    End If
    
    ' Start transaction
    conn.BeginTrans
    
    ' Insert sales order
    sql = "INSERT INTO Sales (CustomerID, CarID, SalesPrice, SalesDate, SalespersonID) VALUES (" & _
          customerID & ", " & carID & ", " & salePrice & ", GETDATE(), " & salespersonID & ")"
    
    conn.Execute sql
    
    ' Get inserted ID
    Set cmd = New ADODB.Command
    cmd.ActiveConnection = conn
    cmd.CommandText = "SELECT @@IDENTITY as NewID"
    
    Dim rs As ADODB.Recordset
    Set rs = cmd.Execute()
    
    If Not rs.EOF Then
        newSalesID = rs("NewID")
    End If
    rs.Close
    
    ' Update inventory
    sql = "UPDATE Cars SET Stock = Stock - 1 WHERE CarID = " & carID
    conn.Execute sql
    
    ' Log transaction
    sql = "INSERT INTO Inventory_Log (CarID, Action, Quantity, ChangedDate, ChangedBy) VALUES (" & carID & ", 'SALE', 1, GETDATE(), 'SalesOrder_" & newSalesID & "')"
    conn.Execute sql
    
    ' Commit
    conn.CommitTrans
    
    CreateSalesOrder = newSalesID
    Exit Function
    
ErrHandler:
    conn.RollbackTrans
    CreateSalesOrder = 0
End Function

' Get sales history
Public Function GetSalesHistory(customerID As Long) As ADODB.Recordset
    On Error Resume Next
    
    Dim sql As String
    Dim rs As ADODB.Recordset
    
    sql = "SELECT S.SalesID, S.SalesDate, S.SalesPrice, C.Make, C.Model, C.Year FROM Sales S " & _
          "INNER JOIN Cars C ON S.CarID = C.CarID " & _
          "WHERE S.CustomerID = " & customerID & _
          " ORDER BY S.SalesDate DESC"
    
    Set rs = New ADODB.Recordset
    rs.Open sql, conn, adOpenStatic, adLockReadOnly
    
    Set GetSalesHistory = rs
End Function

' Calculate sales tax
Public Function CalculateTax(salePrice As Currency) As Currency
    Const TAX_RATE = 0.10  ' 10%
    CalculateTax = salePrice * TAX_RATE
End Function

' Calculate total with tax
Public Function CalculateTotal(salePrice As Currency) As Currency
    CalculateTotal = salePrice + CalculateTax(salePrice)
End Function

' Cleanup
Public Sub Terminate()
    On Error Resume Next
    If Not conn Is Nothing Then
        conn.Close
    End If
    Set conn = Nothing
End Sub
