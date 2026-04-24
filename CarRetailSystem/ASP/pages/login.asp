<%@ Language=VBScript %>
<%
' CarRetailSystem - Login Page
' Legacy session-based authentication
' WARNING: Plain text password storage (SECURITY ISSUE)

Option Explicit
Response.Expires = 0
%>
<!--#include virtual="/includes/config.asp"-->

<%
    Dim username, password, conn, rs, userFound
    
    If Request.Form("submit") = "Login" Then
        username = Request.Form("username")
        password = Request.Form("password")
        
        If Len(username) > 0 And Len(password) > 0 Then
            Set conn = CreateObject("ADODB.Connection")
            Set rs = CreateObject("ADODB.Recordset")
            
            conn.ConnectionString = DB_CONNECTION_STRING
            conn.Open
            
            ' SECURITY ISSUE: Credentials in plain text, no password hashing
            Dim sql
            sql = "SELECT UserID, UserName, Role FROM Users WHERE UserName='" & username & "' AND Password='" & password & "'"
            
            Set rs = conn.Execute(sql)
            
            If Not rs.EOF Then
                Session("UserID") = rs("UserID")
                Session("UserName") = rs("UserName")
                Session("Role") = rs("Role")
                rs.Close
                conn.Close
                Set rs = Nothing
                Set conn = Nothing
                Response.Redirect "index.asp"
                Response.End
            Else
                userFound = False
            End If

            rs.Close
            conn.Close
            Set rs = Nothing
            Set conn = Nothing
        End If
    End If
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
    <title>CarRetailSystem - Login</title>
    <style>
        body { font-family: Arial, sans-serif; background-color: #f0f0f0; }
        .login-container { width: 300px; margin: 100px auto; background-color: white; padding: 20px; border: 1px solid #ccc; }
        .login-container h1 { text-align: center; color: #003366; }
        .login-form { margin: 20px 0; }
        .login-form input[type="text"], .login-form input[type="password"] { width: 100%; padding: 8px; margin: 8px 0; box-sizing: border-box; }
        .login-form input[type="submit"] { width: 100%; padding: 10px; background-color: #003366; color: white; border: none; cursor: pointer; }
        .login-form input[type="submit"]:hover { background-color: #005599; }
        .error { color: red; margin: 10px 0; }
    </style>
</head>
<body>

<div class="login-container">
    <h1>CarRetailSystem</h1>
    <p style="text-align: center;">Retail Car Sales Management</p>
    
    <% If userFound = False And Request.Form("submit") <> "" Then %>
        <p class="error">Login failed: Invalid username or password</p>
    <% End If %>
    
    <form method="POST" action="login.asp" class="login-form">
        <label for="username">Username:</label>
        <input type="text" id="username" name="username" required>
        
        <label for="password">Password:</label>
        <input type="password" id="password" name="password" required>
        
        <input type="submit" name="submit" value="Login">
    </form>
    
    <p style="font-size: 12px; color: #666;">
        Demo Credentials:<br>
        Username: admin<br>
        Password: admin123
    </p>
</div>

</body>
</html>
