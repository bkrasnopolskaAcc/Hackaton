<%@ Language=VBScript %>
<%
' CarRetailSystem - Landing Page
' Legacy ASP/VBScript Application
' Created: Unknown | Last Modified: 2020

Option Explicit
Response.Expires = 0
Response.ExpiresAbsolute = Now() - 1
Response.AddHeader "pragma","no-cache"
Response.AddHeader "cache-control","no-cache"
%>
<!--#include virtual="/includes/config.asp"-->
<!--#include virtual="/includes/session.asp"-->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
    <title>CarRetailSystem - Welcome</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f0f0f0; }
        .header { background-color: #003366; color: white; padding: 20px; text-align: center; }
        .container { max-width: 800px; margin: 20px auto; background-color: white; padding: 20px; border: 1px solid #ccc; }
        .menu { margin: 20px 0; }
        .menu a { display: inline-block; margin-right: 15px; padding: 10px 20px; background-color: #003366; color: white; text-decoration: none; }
        .menu a:hover { background-color: #005599; }
        .warning { color: red; font-weight: bold; }
    </style>
</head>
<body>

<div class="header">
    <h1>CarRetailSystem</h1>
    <p>Retail Car Sales Management Platform</p>
</div>

<div class="container">
    <%
        If Session("UserID") <> "" Then
            Response.Write "<p>Welcome, " & Session("UserName") & "!</p>"
            Response.Write "<div class='menu'>"
            Response.Write "<a href='dashboard.asp'>Dashboard</a>"
            Response.Write "<a href='inventory.asp'>Inventory</a>"
            Response.Write "<a href='sales.asp'>Sales</a>"
            Response.Write "<a href='customers.asp'>Customers</a>"
            Response.Write "<a href='reports.asp'>Reports</a>"
            Response.Write "<a href='logout.asp'>Logout</a>"
            Response.Write "</div>"
        Else
            Response.Write "<p>Please log in to access the system.</p>"
            Response.Write "<p><a href='login.asp'>Click here to login</a></p>"
        End If
    %>
    
    <hr>
    
    <h3>System Information</h3>
    <p>
        <strong>Version:</strong> 3.2 (Legacy)<br>
        <strong>Technology:</strong> Classic ASP with VB6 COM Components<br>
        <strong>Database:</strong> SQL Server 7.0<br>
        <strong class="warning">Status:</strong> Maintenance Mode - Scheduled for Modernization
    </p>
    
    <h3>Quick Stats</h3>
    <%
        Dim conn, rs
        Set conn = CreateObject("ADODB.Connection")
        On Error Resume Next
        conn.ConnectionString = DB_CONNECTION_STRING
        conn.Open
        
        If Err.Number <> 0 Then
            Response.Write "<p class='warning'>Error: Cannot connect to database</p>"
        Else
            ' Get car count
            Set rs = conn.Execute("SELECT COUNT(*) as cnt FROM Cars")
            If Not rs.EOF Then
                Response.Write "<p>Total Cars in Inventory: " & rs("cnt") & "</p>"
            End If
            rs.Close
            
            ' Get customer count
            Set rs = conn.Execute("SELECT COUNT(*) as cnt FROM Customers")
            If Not rs.EOF Then
                Response.Write "<p>Total Customers: " & rs("cnt") & "</p>"
            End If
            rs.Close
            
            ' Get recent sales
            Set rs = conn.Execute("SELECT COUNT(*) as cnt FROM Sales WHERE MONTH(SalesDate) = MONTH(GETDATE())")
            If Not rs.EOF Then
                Response.Write "<p>Sales This Month: " & rs("cnt") & "</p>"
            End If
            rs.Close
            
            conn.Close
        End If
        Set rs = Nothing
        Set conn = Nothing
    %>
    
</div>

</body>
</html>
