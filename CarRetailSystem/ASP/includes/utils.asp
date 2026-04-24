<%
' CarRetailSystem - Utilities Module
' Common helper functions used throughout the application

Option Explicit

' ===== STRING UTILITIES =====

Function TrimString(inputStr)
    TrimString = Trim(inputStr)
End Function

Function UpperCase(inputStr)
    UpperCase = UCase(inputStr)
End Function

Function LowerCase(inputStr)
    LowerCase = LCase(inputStr)
End Function

Function LeftString(inputStr, length)
    LeftString = Left(inputStr, length)
End Function

Function RightString(inputStr, length)
    RightString = Right(inputStr, length)
End Function

Function ReverseString(inputStr)
    Dim result, i
    result = ""
    For i = Len(inputStr) To 1 Step -1
        result = result & Mid(inputStr, i, 1)
    Next
    ReverseString = result
End Function

Function ReplaceString(inputStr, findStr, replaceStr)
    ReplaceString = Replace(inputStr, findStr, replaceStr)
End Function

Function RemoveSpaces(inputStr)
    RemoveSpaces = Replace(inputStr, " ", "")
End Function

' ===== NUMERIC UTILITIES =====

Function IsNumeric(inputStr)
    On Error Resume Next
    Dim result
    result = CDbl(inputStr)
    If Err.Number = 0 Then
        IsNumeric = True
    Else
        IsNumeric = False
    End If
    On Error GoTo 0
End Function

Function FormatCurrency(amount)
    FormatCurrency = Format(amount, "$#,##0.00")
End Function

Function FormatPercent(value)
    FormatPercent = Format(value, "0.00%")
End Function

Function Round(value, decimals)
    Round = Int(value * (10 ^ decimals) + 0.5) / (10 ^ decimals)
End Function

Function AbsoluteValue(value)
    AbsoluteValue = Abs(value)
End Function

' ===== DATE/TIME UTILITIES =====

Function FormatDate(dateValue)
    If IsDate(dateValue) Then
        FormatDate = Format(dateValue, "MM/DD/YYYY")
    Else
        FormatDate = ""
    End If
End Function

Function FormatDateTime(dateTimeValue)
    If IsDate(dateTimeValue) Then
        FormatDateTime = Format(dateTimeValue, "MM/DD/YYYY HH:MM:SS")
    Else
        FormatDateTime = ""
    End If
End Function

Function GetCurrentDate()
    GetCurrentDate = Date()
End Function

Function GetCurrentTime()
    GetCurrentTime = Time()
End Function

Function GetCurrentDateTime()
    GetCurrentDateTime = Now()
End Function

Function AddDays(dateValue, days)
    AddDays = DateAdd("d", days, dateValue)
End Function

Function AddMonths(dateValue, months)
    AddMonths = DateAdd("m", months, dateValue)
End Function

Function AddYears(dateValue, years)
    AddYears = DateAdd("yyyy", years, dateValue)
End Function

Function GetDayOfWeek(dateValue)
    Dim dayNames(6)
    dayNames(0) = "Sunday"
    dayNames(1) = "Monday"
    dayNames(2) = "Tuesday"
    dayNames(3) = "Wednesday"
    dayNames(4) = "Thursday"
    dayNames(5) = "Friday"
    dayNames(6) = "Saturday"
    
    GetDayOfWeek = dayNames(Weekday(dateValue) - 1)
End Function

' ===== ARRAY UTILITIES =====

Function ArrayLength(arr)
    On Error Resume Next
    ArrayLength = UBound(arr) - LBound(arr) + 1
    If Err.Number <> 0 Then
        ArrayLength = 0
    End If
    On Error GoTo 0
End Function

Function InArray(needle, haystack)
    Dim i
    InArray = False
    For i = LBound(haystack) To UBound(haystack)
        If haystack(i) = needle Then
            InArray = True
            Exit Function
        End If
    Next
End Function

' ===== VALIDATION UTILITIES =====

Function ValidateEmail(email)
    ' Basic email validation - not RFC compliant
    Dim emailPattern
    Dim re
    Set re = New RegExp
    emailPattern = "^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
    re.Pattern = emailPattern
    ValidateEmail = re.Test(email)
End Function

Function ValidatePhone(phone)
    ' Remove non-numeric characters
    Dim cleanPhone
    cleanPhone = Replace(phone, "-", "")
    cleanPhone = Replace(cleanPhone, " ", "")
    cleanPhone = Replace(cleanPhone, "(", "")
    cleanPhone = Replace(cleanPhone, ")", "")
    
    ' Check if 10+ digits
    If Len(cleanPhone) >= 10 Then
        ValidatePhone = True
    Else
        ValidatePhone = False
    End If
End Function

Function ValidateVIN(vin)
    ' VIN should be 17 characters
    If Len(vin) = 17 Then
        ValidateVIN = True
    Else
        ValidateVIN = False
    End If
End Function

' ===== URL UTILITIES =====

Function URLEncode(inputStr)
    URLEncode = Server.URLEncode(inputStr)
End Function

Function URLDecode(inputStr)
    ' Manual URL decoding
    Dim result
    result = inputStr
    result = Replace(result, "+", " ")
    result = Replace(result, "%20", " ")
    URLDecode = result
End Function

' ===== HTML UTILITIES =====

Function HTMLEncode(inputStr)
    HTMLEncode = Server.HTMLEncode(inputStr)
End Function

Function StripHTML(inputStr)
    ' Remove HTML tags (basic implementation)
    Dim re
    Set re = New RegExp
    re.Pattern = "<[^>]+>"
    re.Global = True
    StripHTML = re.Replace(inputStr, "")
End Function

' ===== MISC UTILITIES =====

Function Nz(value, nullValue)
    ' Null-zero function - return alternative if null
    If IsNull(value) Or value = "" Then
        Nz = nullValue
    Else
        Nz = value
    End If
End Function

Function IIF(condition, trueValue, falseValue)
    ' Immediate if function
    If condition Then
        IIF = trueValue
    Else
        IIF = falseValue
    End If
End Function

Function GetRandomNumber(minVal, maxVal)
    Randomize
    GetRandomNumber = Int((maxVal - minVal + 1) * Rnd + minVal)
End Function

%>
