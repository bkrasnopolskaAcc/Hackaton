<%
' CarRetailSystem - Global Configuration
' Contains database connection strings and application settings
' WARNING: Credentials stored in plain text (SECURITY ISSUE)

' ===== DATABASE CONFIGURATION =====
Const DB_CONNECTION_STRING = "Provider=SQLNCLI11;Server=.;Database=CarRetailDB;UID=sa;PWD=Admin@123;Connect Timeout=30;"

' Windows Auth alternative (requires IIS running as domain account with SQL access):
' Const DB_CONNECTION_STRING = "Provider=SQLOLEDB;Server=(local);Database=CarRetailDB;Integrated Security=SSPI;"

' ===== APPLICATION SETTINGS =====
Const APP_NAME = "CarRetailSystem"
Const APP_VERSION = "3.2"
Const APP_ENV = "Production"  ' Development, Staging, Production

' ===== SESSION SETTINGS =====
Const SESSION_TIMEOUT = 30  ' Minutes
Const SESSION_COOKIE_NAME = "CarRetailSID"

' ===== BUSINESS RULES =====
Const MIN_CAR_PRICE = 5000
Const MAX_CAR_PRICE = 150000
Const TAX_RATE = 0.10  ' 10% sales tax
Const DISCOUNT_PERCENTAGE = 0.05  ' 5% discount for bulk orders

' ===== EMAIL SETTINGS =====
Const SMTP_SERVER = "smtp.gmail.com"
Const SMTP_PORT = 587
Const SMTP_USERNAME = "noreply@carretail.com"
Const SMTP_PASSWORD = "EmailPassword123"  ' SECURITY ISSUE: Hard-coded credentials

' ===== LOGGING SETTINGS =====
Const LOG_FILE_PATH = "C:\CarRetailSystem\logs\"
Const ENABLE_DEBUG_LOG = True

' ===== HELPER FUNCTIONS =====

Function GetConnectionString()
    GetConnectionString = DB_CONNECTION_STRING
End Function

Function GetAppInfo()
    Dim info
    info = APP_NAME & " v" & APP_VERSION & " (" & APP_ENV & ")"
    GetAppInfo = info
End Function

Function FormatCurrency(amount)
    FormatCurrency = Format(amount, "$#,##0.00")
End Function

' VB6 COM Component Registration
' These must be registered on the production server
' Run: regsvr32 CarInventory.dll
' Run: regsvr32 SalesOrder.dll
' Run: regsvr32 CustomerMgmt.dll
' Run: regsvr32 ReportGenerator.dll
' Run: regsvr32 Utilities.dll

%>
