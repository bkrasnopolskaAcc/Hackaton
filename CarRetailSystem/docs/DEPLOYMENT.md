# Deployment Guide

## System Requirements

### Minimum Requirements
- **OS**: Windows Server 2003 SP2 or Windows Server 2008
- **IIS**: 5.0 (Windows 2003) or 6.0 (Windows 2008)
- **SQL Server**: 7.0, 2000, or 2005
- **.NET Framework**: Not required
- **RAM**: 2 GB
- **Disk Space**: 10 GB

### Recommended
- **OS**: Windows Server 2008 R2
- **IIS**: 7.0
- **SQL Server**: 2005 SP3
- **RAM**: 4+ GB
- **Disk Space**: 20+ GB (for logs and backups)

## Pre-Deployment Checklist

- [ ] IIS installed and configured
- [ ] SQL Server database created and accessible
- [ ] VB6 runtime libraries installed
- [ ] VB6 COM DLLs compiled
- [ ] Database schema created (run database_setup.sql)
- [ ] Sample data loaded
- [ ] File permissions configured
- [ ] Connection strings updated
- [ ] Test environment verified

## Step 1: Database Setup

### Create Database
```sql
-- Run SQL\database_setup.sql against SQL Server instance
sqlcmd -S SERVER_NAME -i SQL\database_setup.sql
```

### Verify Tables Created
```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'
```

Expected tables:
- Users
- Cars
- Customers
- Sales
- Inventory_Log

## Step 2: Configure IIS

### Create Virtual Directory
1. Open IIS Manager
2. Create new virtual directory
3. Point to ASP folder
4. Set name to `CarRetailSystem`
5. Configure permissions

### IIS 5.0/6.0 Configuration
1. Enable Parent Paths: `ApplicationHost.config`
2. Set ASP Session Timeout: 30 minutes
3. Enable 32-bit compatibility (if needed)

### Set File Permissions
```powershell
# Grant IIS_IUSRS permissions to ASP folder
icacls "C:\Projects\Anthropic\Hackaton\LegacyOld\CarRetailSystem\ASP" /grant "IIS_IUSRS:F" /T
```

## Step 3: Register COM Components

### Compile VB6 Components (if source available)
```batch
REM Must be done on deployment machine
cd VB6Components
REM Use VB6 IDE to compile:
REM - Set to "Compile to Native Code"
REM - Set to "Create DLL"
REM - Output: CarInventory.dll, SalesOrder.dll, etc.
```

### Register DLLs
```batch
REM Run as Administrator
regsvr32 "C:\Program Files\CarRetailSystem\VB6Components\CarInventory.dll"
regsvr32 "C:\Program Files\CarRetailSystem\VB6Components\SalesOrder.dll"
regsvr32 "C:\Program Files\CarRetailSystem\VB6Components\CustomerMgmt.dll"
regsvr32 "C:\Program Files\CarRetailSystem\VB6Components\ReportGenerator.dll"
regsvr32 "C:\Program Files\CarRetailSystem\VB6Components\Utilities.dll"
```

### Verify Registration
```batch
REM Check Registry
reg query "HKEY_CLASSES_ROOT\CarRetailSystem.CarInventory"
```

## Step 4: Configure Connection Strings

### Update config.asp
Edit `ASP\includes\config.asp`:

```vbscript
' Change this line to match your SQL Server instance:
Const DB_CONNECTION_STRING = "Provider=SQLOLEDB;Server=YOUR_SERVER;Database=CarRetailDB;UID=sa;PWD=YOUR_PASSWORD;"
```

**IMPORTANT**: Update SQL Server name and password!

### Common Connection String Formats
```vbscript
' Local instance
"Provider=SQLOLEDB;Server=.\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;"

' Named instance
"Provider=SQLOLEDB;Server=SERVERNAME\SQLEXPRESS;Database=CarRetailDB;UID=sa;PWD=Admin@123;"

' Network instance
"Provider=SQLOLEDB;Server=192.168.1.100;Database=CarRetailDB;UID=sa;PWD=Admin@123;"
```

## Step 5: Verify Installation

### Test Database Connection
1. Open `http://localhost/CarRetailSystem/ASP/pages/index.asp`
2. Click "Dashboard" after login
3. Verify car count displays
4. Verify recent sales count displays

### Test Authentication
1. Login with username: `admin`, password: `admin123`
2. Verify redirect to dashboard
3. Logout and verify return to login

### Test Inventory Management
1. Click "Inventory" link
2. Verify cars display in table
3. Add new car and verify success message
4. Check database for new entry

## Deployment Checklist

- [ ] Database created and populated
- [ ] Virtual directory configured in IIS
- [ ] COM DLLs registered
- [ ] Connection strings updated
- [ ] File permissions set correctly
- [ ] IIS restarted
- [ ] Application accessible via browser
- [ ] Login functionality verified
- [ ] Inventory operations tested
- [ ] Sales operations tested

## Post-Deployment

### Create Logs Directory
```batch
mkdir C:\CarRetailSystem\logs
icacls "C:\CarRetailSystem\logs" /grant "IIS_IUSRS:F"
```

### Set Up Backup
```batch
REM Backup database daily
BACKUP DATABASE CarRetailDB TO DISK='C:\Backups\CarRetailDB.bak'
```

### Monitor Performance
- CPU usage on web server
- Memory utilization
- Database connection count
- IIS request/sec
- SQL Server response times

## Troubleshooting

### "COM object not found" Error
- Verify DLLs registered: `regsvr32 /l CarInventory.dll`
- Check DLL paths in config.asp
- Verify 32-bit/64-bit compatibility

### "Cannot connect to database"
- Verify SQL Server is running
- Check connection string in config.asp
- Verify firewall allows port 1433
- Check SQL authentication is enabled

### "Permission Denied" on files
- Verify IIS_IUSRS has read/write permissions
- Check file ownership
- Verify folder permissions inherited correctly

### Session timeouts
- Verify SESSION_TIMEOUT setting in config.asp
- Check IIS session timeout
- Review browser cookie settings

### Slow performance
- Check database indexes
- Review SQL Server query plans
- Monitor IIS worker process
- Check for database blocking

## Backup & Recovery

### Database Backup
```sql
BACKUP DATABASE CarRetailDB 
TO DISK='C:\Backups\CarRetailDB.bak' 
WITH INIT, COMPRESSION
```

### Restore Database
```sql
RESTORE DATABASE CarRetailDB 
FROM DISK='C:\Backups\CarRetailDB.bak' 
WITH REPLACE
```

### File Backup
```batch
REM Backup ASP files
xcopy "ASP\*" "C:\Backups\ASP\" /S /Y
```

## Decommissioning

When ready to retire this system:

1. Migrate data to new system
2. Run final data backup
3. Update DNS/routing to new application
4. Keep legacy system running read-only for 90 days
5. Archive database backups
6. Remove IIS virtual directory
7. Unregister COM DLLs
8. Archive source code
9. Document lessons learned

## Support Contacts

- **IIS Support**: Windows Server documentation
- **SQL Server**: Microsoft SQL Server support
- **VB6 Legacy**: Microsoft Legacy Products Support (limited)
- **Application**: Contact original development team (if available)
