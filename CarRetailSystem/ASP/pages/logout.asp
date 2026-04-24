<%@ Language=VBScript %>
<%
' CarRetailSystem - Logout
Option Explicit

Session("UserID") = ""
Session("UserName") = ""
Session("Role") = ""
Session.Abandon()

Response.Redirect "login.asp"
%>
