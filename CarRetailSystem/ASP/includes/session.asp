<%
' CarRetailSystem - Session Management Module
' Handles user session initialization and validation
' WARNING: Basic implementation - prone to hijacking

' Session timeout configuration (inherited from config.asp)
' SESSION_TIMEOUT constant should be defined in config.asp

Function InitSession()
    Session.Timeout = SESSION_TIMEOUT
    Session("CreatedDate") = Now()
    Session("LastActivity") = Now()
    Session("IPAddress") = Request.ServerVariables("REMOTE_ADDR")
End Function

Function ValidateSession()
    ' Check if session exists
    If Session("UserID") = "" Then
        ValidateSession = False
        Exit Function
    End If
    
    ' Check if session expired
    If IsSessionExpired() Then
        ClearSession()
        ValidateSession = False
        Exit Function
    End If
    
    ' Update activity timestamp
    UpdateSessionActivity()
    ValidateSession = True
End Function

Function IsSessionExpired()
    Dim expirationTime
    expirationTime = DateAdd("n", SESSION_TIMEOUT, Session("LastActivity"))
    
    If Now() > expirationTime Then
        IsSessionExpired = True
    Else
        IsSessionExpired = False
    End If
End Function

Function UpdateSessionActivity()
    Session("LastActivity") = Now()
End Function

Sub ClearSession()
    ' Clear all session variables
    Session("UserID") = ""
    Session("UserName") = ""
    Session("Role") = ""
    Session("CreatedDate") = ""
    Session("LastActivity") = ""
    Session("IPAddress") = ""
    Session.Abandon()
End Sub

Function GetSessionDuration()
    ' Get time user has been logged in
    If Session("CreatedDate") <> "" Then
        GetSessionDuration = DateDiff("n", Session("CreatedDate"), Now())
    Else
        GetSessionDuration = 0
    End If
End Function

Function GetSessionTimeRemaining()
    ' Get minutes until session expiration
    Dim remaining
    remaining = SESSION_TIMEOUT - DateDiff("n", Session("LastActivity"), Now())
    If remaining < 0 Then
        GetSessionTimeRemaining = 0
    Else
        GetSessionTimeRemaining = remaining
    End If
End Function

%>
