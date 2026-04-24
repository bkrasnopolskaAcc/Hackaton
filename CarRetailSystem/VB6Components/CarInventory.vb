VERSION 5.00
Begin VB.Form frmCarInventory
   Caption         =   "Car Inventory - VB6 COM Component"
   ClientHeight    =   6000
   ClientLeft      =   60
   ClientTop       =   350
   ClientWidth     =   9240
   OleObjectBlocking=   False
   ShowInTaskbar   =   0   'False
End
Attribute VB_Name = "CarInventory"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

' CarRetailSystem - Car Inventory COM Component
' VB6 Class Module compiled as DLL
' Registration: regsvr32 CarInventory.dll

Option Explicit

Private conn As ADODB.Connection
Private rs As ADODB.Recordset
Private Const DB_CONN_STRING = "Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;"

' Initialize connection
Public Function Initialize() As Boolean
    On Error GoTo ErrHandler
    
    Set conn = New ADODB.Connection
    conn.ConnectionString = DB_CONN_STRING
    conn.Open
    
    Initialize = True
    Exit Function
    
ErrHandler:
    Initialize = False
End Function

' Get available cars with filter
Public Function GetAvailableCars(Optional make As String = "", Optional maxPrice As Long = 0) As ADODB.Recordset
    On Error Resume Next
    
    Dim sql As String
    sql = "SELECT CarID, Make, Model, Year, Color, VIN, Price, Stock FROM Cars WHERE Stock > 0"
    
    If Len(make) > 0 Then
        sql = sql & " AND Make = '" & make & "'"
    End If
    
    If maxPrice > 0 Then
        sql = sql & " AND Price <= " & maxPrice
    End If
    
    sql = sql & " ORDER BY Make, Model"
    
    Set rs = New ADODB.Recordset
    rs.Open sql, conn, adOpenStatic, adLockReadOnly
    
    Set GetAvailableCars = rs
End Function

' Get specific car details
Public Function GetCarDetails(carID As Long) As ADODB.Recordset
    On Error Resume Next
    
    Dim sql As String
    sql = "SELECT * FROM Cars WHERE CarID = " & carID
    
    Set rs = New ADODB.Recordset
    rs.Open sql, conn, adOpenStatic, adLockReadOnly
    
    Set GetCarDetails = rs
End Function

' Update stock
Public Function UpdateStock(carID As Long, quantity As Long) As Boolean
    On Error Resume Next
    
    Dim sql As String
    sql = "UPDATE Cars SET Stock = Stock - " & quantity & " WHERE CarID = " & carID
    
    conn.Execute sql
    
    If Err.Number = 0 Then
        ' Log to audit table
        LogInventoryChange carID, quantity
        UpdateStock = True
    Else
        UpdateStock = False
    End If
End Function

' Get stock level
Public Function GetStockLevel(carID As Long) As Long
    On Error Resume Next
    
    Dim sql As String
    Dim rs As ADODB.Recordset
    
    sql = "SELECT Stock FROM Cars WHERE CarID = " & carID
    
    Set rs = New ADODB.Recordset
    rs.Open sql, conn, adOpenStatic, adLockReadOnly
    
    If Not rs.EOF Then
        GetStockLevel = rs("Stock")
    Else
        GetStockLevel = 0
    End If
    
    rs.Close
End Function

' Private helper to log changes
Private Sub LogInventoryChange(carID As Long, quantity As Long)
    Dim sql As String
    sql = "INSERT INTO Inventory_Log (CarID, Action, Quantity, ChangedDate, ChangedBy) VALUES (" & carID & ", 'SALE', " & quantity & ", GETDATE(), 'SYSTEM')"
    conn.Execute sql
End Sub

' Cleanup
Public Sub Terminate()
    On Error Resume Next
    If Not conn Is Nothing Then
        conn.Close
    End If
    Set conn = Nothing
End Sub
