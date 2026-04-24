<%
' CarRetailSystem - Security Module
' WARNING: Basic security only - MANY VULNERABILITIES PRESENT
' This is legacy code - NOT suitable for production in modern standards

' ===== BASIC AUTHENTICATION =====

Function UserIsAuthenticated()
    If Session("UserID") <> "" Then
        UserIsAuthenticated = True
    Else
        UserIsAuthenticated = False
    End If
End Function

Function UserIsAdmin()
    If Session("Role") = "Admin" Then
        UserIsAdmin = True
    Else
        UserIsAdmin = False
    End If
End Function

Function UserIsSalesperson()
    If Session("Role") = "Salesperson" Then
        UserIsSalesperson = True
    Else
        UserIsSalesperson = False
    End If
End Function

' ===== INPUT SANITIZATION (MINIMAL) =====
' NOTE: This is NOT sufficient for modern security standards
' Legacy approach - does not prevent SQL injection

Function SanitizeInput(inputStr)
    ' Basic string trimming only
    SanitizeInput = Trim(inputStr)
End Function

Function SanitizeNumeric(inputStr)
    ' Attempt numeric validation
    If IsNumeric(inputStr) Then
        SanitizeNumeric = inputStr
    Else
        SanitizeNumeric = 0
    End If
End Function

' ===== LOGGING (BASIC) =====

Sub LogEvent(eventType, description)
    Dim logFile, fso
    On Error Resume Next
    
    Set fso = CreateObject("Scripting.FileSystemObject")
    Set logFile = fso.OpenTextFile(LOG_FILE_PATH & "events.log", 8, True)
    
    logFile.WriteLine Now() & " | " & eventType & " | " & description
    logFile.Close
    
    Set logFile = Nothing
    Set fso = Nothing
End Sub

Sub LogUserAction(action, details)
    Dim logEntry
    logEntry = Session("UserName") & " | " & action & " | " & details
    LogEvent "USER_ACTION", logEntry
End Sub

Sub LogSecurityEvent(incident, details)
    Dim logEntry
    logEntry = incident & " | " & details
    LogEvent "SECURITY", logEntry
End Sub

' ===== PASSWORD VALIDATION (LEGACY - NO HASHING) =====
' WARNING: Passwords stored in plain text - SECURITY RISK

Function ValidatePassword(password)
    ' Minimal requirements
    If Len(password) >= 6 Then
        ValidatePassword = True
    Else
        ValidatePassword = False
    End If
End Function

%>
