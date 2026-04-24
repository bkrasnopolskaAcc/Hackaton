<%@ Language=VBScript %>
<%
' CarRetailSystem - Inventory Management
' Manages car catalog with COM component integration

Option Explicit
Response.Expires = 0
%>
<!--#include virtual="/includes/config.asp"-->
<!--#include virtual="/includes/session.asp"-->
<!--#include virtual="/includes/security.asp"-->

<%
' Require login
If Session("UserID") = "" Then
    Response.Redirect "login.asp"
End If

' Check user role
If Not UserIsAdmin() Then
    Response.Write "<p>Access Denied: Admin privileges required</p>"
    Response.End
End If
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
    <title>Inventory Management</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 10px; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #003366; color: white; }
        .action-btn { padding: 5px 10px; background-color: #003366; color: white; text-decoration: none; cursor: pointer; }
        .action-btn:hover { background-color: #005599; }
    </style>
</head>
<body>

<h1>Car Inventory Management</h1>
<p><a href="index.asp">Back to Dashboard</a></p>

<%
    Dim conn, rs, carID, action
    Set conn = CreateObject("ADODB.Connection")
    Set rs = CreateObject("ADODB.Recordset")
    
    action = Request.QueryString("action")
    carID = Request.QueryString("id")
    
    conn.ConnectionString = DB_CONNECTION_STRING
    conn.Open
    
    ' Handle add/edit/delete
    If Request.Form("submit") = "Add Car" Then
        ' SQL Injection Vulnerability - String Concatenation (LEGACY ISSUE)
        Dim sql
        sql = "INSERT INTO Cars (Make, Model, Year, Color, VIN, Price, Stock, CreatedDate) VALUES ('" & _
              Request.Form("Make") & "', '" & _
              Request.Form("Model") & "', " & _
              Request.Form("Year") & ", '" & _
              Request.Form("Color") & "', '" & _
              Request.Form("VIN") & "', " & _
              Request.Form("Price") & ", " & _
              Request.Form("Stock") & ", GETDATE())"
        
        On Error Resume Next
        conn.Execute sql
        If Err.Number <> 0 Then
            Response.Write "<p style='color:red'>Error adding car: " & Err.Description & "</p>"
        Else
            Response.Write "<p style='color:green'>Car added successfully</p>"
        End If
        On Error GoTo 0
    End If
    
    ' Display form
    Response.Write "<h2>Add New Car</h2>"
    Response.Write "<form method='POST' action='inventory.asp'>"
    Response.Write "<table border='0'>"
    Response.Write "<tr><td>Make:</td><td><input type='text' name='Make' size='30'></td></tr>"
    Response.Write "<tr><td>Model:</td><td><input type='text' name='Model' size='30'></td></tr>"
    Response.Write "<tr><td>Year:</td><td><input type='text' name='Year' size='10'></td></tr>"
    Response.Write "<tr><td>Color:</td><td><input type='text' name='Color' size='30'></td></tr>"
    Response.Write "<tr><td>VIN:</td><td><input type='text' name='VIN' size='30'></td></tr>"
    Response.Write "<tr><td>Price:</td><td><input type='text' name='Price' size='10'></td></tr>"
    Response.Write "<tr><td>Stock:</td><td><input type='text' name='Stock' size='5'></td></tr>"
    Response.Write "<tr><td></td><td><input type='submit' name='submit' value='Add Car'></td></tr>"
    Response.Write "</table>"
    Response.Write "</form>"
    
    ' Display inventory list
    Response.Write "<h2>Current Inventory</h2>"
    
    Set rs = conn.Execute("SELECT CarID, Make, Model, Year, Color, VIN, Price, Stock, CreatedDate FROM Cars ORDER BY CreatedDate DESC")
    
    If rs.EOF Then
        Response.Write "<p>No cars in inventory</p>"
    Else
        Response.Write "<table>"
        Response.Write "<tr><th>ID</th><th>Make</th><th>Model</th><th>Year</th><th>Color</th><th>VIN</th><th>Price</th><th>Stock</th><th>Actions</th></tr>"
        
        Do While Not rs.EOF
            Response.Write "<tr>"
            Response.Write "<td>" & rs("CarID") & "</td>"
            Response.Write "<td>" & rs("Make") & "</td>"
            Response.Write "<td>" & rs("Model") & "</td>"
            Response.Write "<td>" & rs("Year") & "</td>"
            Response.Write "<td>" & rs("Color") & "</td>"
            Response.Write "<td>" & rs("VIN") & "</td>"
            Response.Write "<td>$" & FormatNumber(rs("Price"), 2) & "</td>"
            Response.Write "<td>" & rs("Stock") & "</td>"
            Response.Write "<td><a href='inventory.asp?action=edit&id=" & rs("CarID") & "' class='action-btn'>Edit</a> | <a href='inventory.asp?action=delete&id=" & rs("CarID") & "' class='action-btn'>Delete</a></td>"
            Response.Write "</tr>"
            rs.MoveNext
        Loop
        
        Response.Write "</table>"
    End If
    
    rs.Close
    conn.Close
    Set rs = Nothing
    Set conn = Nothing
%>

</body>
</html>
