# Pre-Migration Checklist for EF Core Database Migration

**Feature**: Database Client Migration (001-database-client-migration)  
**Purpose**: Ensure safe migration from NHibernate to Entity Framework Core  
**Activation Model**: Automatic at server boot (no manual user intervention required)

**IMPORTANT**: This checklist is for operators deploying Shoko Server with EF Core migrations. Migration happens automatically during server startup. This checklist ensures you have proper backups and verification procedures in place.

---

## Table of Contents

1. [Pre-Deployment Checklist](#pre-deployment-checklist)
2. [Backup Procedures](#backup-procedures)
3. [Backup Verification](#backup-verification)
4. [Rollback Preparation](#rollback-preparation)
5. [Post-Migration Verification](#post-migration-verification)
6. [Emergency Contacts](#emergency-contacts)

---

## Pre-Deployment Checklist

### Database Backup

- [ ] **Identify Database Location**
  - [ ] SQLite: Locate `Shoko.db3` file(s) in application data directory
  - [ ] MySQL/MariaDB: Identify server host, port, database name, credentials
  - [ ] SQL Server: Identify server host, port, database name, credentials

- [ ] **Stop Shoko Server**
  - [ ] Stop server process completely
  - [ ] Verify no lingering connections to database
  - [ ] Wait 30 seconds for all operations to complete

- [ ] **Create Full Database Backup**
  - [ ] Follow backup procedures below for your provider
  - [ ] Verify backup file exists and has reasonable size
  - [ ] Record backup location and timestamp

- [ ] **Verify Backup Integrity**
  - [ ] Follow backup verification procedures below
  - [ ] Confirm backup is readable and not corrupted
  - [ ] Document any verification issues

### Environment Verification

- [ ] **Check System Requirements**
  - [ ] .NET 10.0 runtime installed
  - [ ] Sufficient disk space for backup files (2x database size minimum)
  - [ ] Network connectivity for remote databases (MySQL/MariaDB, SQL Server)

- [ ] **Review Configuration**
  - [ ] Verify `DatabaseSettings` in `settings-server.json` is correct
  - [ ] Confirm provider type (SQLite, MySQL/MariaDB, SQL Server)
  - [ ] Verify connection string is valid
  - [ ] Check for any custom environment variables

### Staging Test (Recommended)

- [ ] **Test on Staging Environment First**
  - [ ] Create staging environment copy of production database
  - [ ] Deploy new Shoko Server version to staging
  - [ ] Start server and monitor automatic migration
  - [ ] Verify all data is accessible
  - [ ] Run smoke tests (API endpoints, basic operations)
  - [ ] Check logs for migration errors or warnings

- [ ] **Document Staging Results**
  - [ ] Record migration duration
  - [ ] Note any warnings or errors
  - [ ] Document any issues and resolutions
  - [ ] Confirm staging test passed before proceeding to production

---

## Backup Procedures

### SQLite Backup

```bash
#!/bin/bash
# backup-sqlite.sh
# Usage: ./backup-sqlite.sh /path/to/Shoko.db3

DB_PATH="$1"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="./backups"
BACKUP_FILE="${BACKUP_DIR}/Shoko.db3.backup.${TIMESTAMP}"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Stop Shoko Server
echo "Stopping Shoko Server..."
# systemctl stop shoko-server  # Linux
# Stop-Service -Name "Shoko Server"  # Windows

# Wait for server to stop
sleep 30

# Create backup
echo "Creating backup of ${DB_PATH}..."
cp "$DB_PATH" "$BACKUP_FILE"

# Verify backup
if [ -f "$BACKUP_FILE" ]; then
    BACKUP_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null)
    ORIGINAL_SIZE=$(stat -f%z "$DB_PATH" 2>/dev/null || stat -c%s "$DB_PATH" 2>/dev/null)
    echo "Backup created: ${BACKUP_FILE}"
    echo "Backup size: ${BACKUP_SIZE} bytes"
    echo "Original size: ${ORIGINAL_SIZE} bytes"
    
    if [ "$BACKUP_SIZE" -eq "$ORIGINAL_SIZE" ]; then
        echo "✓ Backup size matches original"
    else
        echo "✗ WARNING: Backup size does not match original"
    fi
else
    echo "✗ ERROR: Backup file not created"
    exit 1
fi

# Start Shoko Server (optional)
echo "Starting Shoko Server..."
# systemctl start shoko-server  # Linux
# Start-Service -Name "Shoko Server"  # Windows

echo "Backup completed successfully"
```

**Windows PowerShell Version**:

```powershell
# backup-sqlite.ps1
# Usage: .\backup-sqlite.ps1 "C:\Path\To\Shoko.db3"

param(
    [Parameter(Mandatory=$true)]
    [string]$DbPath
)

$Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupDir = ".\backups"
$BackupFile = "${BackupDir}\Shoko.db3.backup.${Timestamp}"

# Create backup directory
New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null

# Stop Shoko Server
Write-Host "Stopping Shoko Server..."
# Stop-Service -Name "Shoko Server"

# Wait for server to stop
Start-Sleep -Seconds 30

# Create backup
Write-Host "Creating backup of ${DbPath}..."
Copy-Item -Path $DbPath -Destination $BackupFile -Force

# Verify backup
if (Test-Path $BackupFile) {
    $BackupSize = (Get-Item $BackupFile).Length
    $OriginalSize = (Get-Item $DbPath).Length
    Write-Host "Backup created: ${BackupFile}"
    Write-Host "Backup size: ${BackupSize} bytes"
    Write-Host "Original size: ${OriginalSize} bytes"
    
    if ($BackupSize -eq $OriginalSize) {
        Write-Host "✓ Backup size matches original"
    } else {
        Write-Host "✗ WARNING: Backup size does not match original"
    }
} else {
    Write-Host "✗ ERROR: Backup file not created"
    exit 1
}

# Start Shoko Server (optional)
Write-Host "Starting Shoko Server..."
# Start-Service -Name "Shoko Server"

Write-Host "Backup completed successfully"
```

### MySQL/MariaDB Backup

```bash
#!/bin/bash
# backup-mysql.sh
# Usage: ./backup-mysql.sh shoko

DB_NAME="$1"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="./backups"
BACKUP_FILE="${BACKUP_DIR}/shoko_backup.${TIMESTAMP}.sql"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Get MySQL credentials from environment or prompt
if [ -z "$DB_HOST" ]; then
    read -p "MySQL Host (default: 127.0.0.1): " DB_HOST
    DB_HOST=${DB_HOST:-127.0.0.1}
fi

if [ -z "$DB_PORT" ]; then
    read -p "MySQL Port (default: 3306): " DB_PORT
    DB_PORT=${DB_PORT:-3306}
fi

if [ -z "$DB_USER" ]; then
    read -p "MySQL User (default: root): " DB_USER
    DB_USER=${DB_USER:-root}
fi

read -s -p "MySQL Password: " DB_PASS
echo

# Stop Shoko Server
echo "Stopping Shoko Server..."
# systemctl stop shoko-server  # Linux

# Wait for server to stop
sleep 30

# Create backup
echo "Creating backup of ${DB_NAME}..."
mysqldump -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$DB_PASS" \
    --single-transaction \
    --routines \
    --triggers \
    --events \
    --add-drop-database \
    --databases "$DB_NAME" > "$BACKUP_FILE"

# Verify backup
if [ -f "$BACKUP_FILE" ]; then
    BACKUP_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null)
    echo "Backup created: ${BACKUP_FILE}"
    echo "Backup size: ${BACKUP_SIZE} bytes"
    
    # Verify backup is valid SQL
    if grep -q "CREATE DATABASE" "$BACKUP_FILE" && grep -q "Dump completed" "$BACKUP_FILE"; then
        echo "✓ Backup appears to be valid"
    else
        echo "✗ WARNING: Backup may be incomplete or corrupted"
    fi
else
    echo "✗ ERROR: Backup file not created"
    exit 1
fi

# Start Shoko Server (optional)
echo "Starting Shoko Server..."
# systemctl start shoko-server  # Linux

echo "Backup completed successfully"
```

### SQL Server Backup

```bash
#!/bin/bash
# backup-sqlserver.sh
# Usage: ./backup-sqlserver.sh shokodb

DB_NAME="$1"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="./backups"
BACKUP_FILE="${BACKUP_DIR}/shokodb_backup_${TIMESTAMP}.bak"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Get SQL Server credentials from environment or prompt
if [ -z "$DB_HOST" ]; then
    read -p "SQL Server Host (default: 127.0.0.1): " DB_HOST
    DB_HOST=${DB_HOST:-127.0.0.1}
fi

if [ -z "$DB_PORT" ]; then
    read -p "SQL Server Port (default: 1433): " DB_PORT
    DB_PORT=${DB_PORT:-1433}
fi

if [ -z "$DB_USER" ]; then
    read -p "SQL Server User (default: sa): " DB_USER
    DB_USER=${DB_USER:-sa}
fi

read -s -p "SQL Server Password: " DB_PASS
echo

# Stop Shoko Server
echo "Stopping Shoko Server..."
# systemctl stop shoko-server  # Linux

# Wait for server to stop
sleep 30

# Create backup
echo "Creating backup of ${DB_NAME}..."
sqlcmd -S "${DB_HOST},${DB_PORT}" -U "$DB_USER" -P "$DB_PASS" \
    -Q "BACKUP DATABASE [${DB_NAME}] TO DISK = '${BACKUP_FILE}' WITH FORMAT, STATS = 10"

# Verify backup
if [ -f "$BACKUP_FILE" ]; then
    BACKUP_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null)
    echo "Backup created: ${BACKUP_FILE}"
    echo "Backup size: ${BACKUP_SIZE} bytes"
    
    # Verify backup is valid
    if sqlcmd -S "${DB_HOST},${DB_PORT}" -U "$DB_USER" -P "$DB_PASS" \
        -Q "RESTORE VERIFYONLY FROM DISK = '${BACKUP_FILE}'" | grep -q "The backup set on file 1 is valid"; then
        echo "✓ Backup is valid"
    else
        echo "✗ WARNING: Backup verification failed"
    fi
else
    echo "✗ ERROR: Backup file not created"
    exit 1
fi

# Start Shoko Server (optional)
echo "Starting Shoko Server..."
# systemctl start shoko-server  # Linux

echo "Backup completed successfully"
```

---

## Backup Verification

### SQLite Verification

```bash
#!/bin/bash
# verify-sqlite-backup.sh
# Usage: ./verify-sqlite-backup.sh /path/to/Shoko.db3 /path/to/backup.db3

ORIGINAL_DB="$1"
BACKUP_DB="$2"

echo "Verifying SQLite backup..."

# Check file existence
if [ ! -f "$ORIGINAL_DB" ]; then
    echo "✗ ERROR: Original database not found: ${ORIGINAL_DB}"
    exit 1
fi

if [ ! -f "$BACKUP_DB" ]; then
    echo "✗ ERROR: Backup database not found: ${BACKUP_DB}"
    exit 1
fi

# Compare file sizes
ORIGINAL_SIZE=$(stat -f%z "$ORIGINAL_DB" 2>/dev/null || stat -c%s "$ORIGINAL_DB" 2>/dev/null)
BACKUP_SIZE=$(stat -f%z "$BACKUP_DB" 2>/dev/null || stat -c%s "$BACKUP_DB" 2>/dev/null)

echo "Original size: ${ORIGINAL_SIZE} bytes"
echo "Backup size: ${BACKUP_SIZE} bytes"

if [ "$ORIGINAL_SIZE" -eq "$BACKUP_SIZE" ]; then
    echo "✓ File sizes match"
else
    echo "✗ WARNING: File sizes do not match"
fi

# Verify database integrity
echo "Verifying original database integrity..."
sqlite3 "$ORIGINAL_DB" "PRAGMA integrity_check;" | grep -q "ok"
if [ $? -eq 0 ]; then
    echo "✓ Original database is valid"
else
    echo "✗ ERROR: Original database is corrupted"
    exit 1
fi

echo "Verifying backup database integrity..."
sqlite3 "$BACKUP_DB" "PRAGMA integrity_check;" | grep -q "ok"
if [ $? -eq 0 ]; then
    echo "✓ Backup database is valid"
else
    echo "✗ ERROR: Backup database is corrupted"
    exit 1
fi

# Compare schema
echo "Comparing schema..."
ORIGINAL_TABLES=$(sqlite3 "$ORIGINAL_DB" ".tables")
BACKUP_TABLES=$(sqlite3 "$BACKUP_DB" ".tables")

if [ "$ORIGINAL_TABLES" = "$BACKUP_TABLES" ]; then
    echo "✓ Schema matches"
else
    echo "✗ WARNING: Schema does not match"
    echo "Original tables: ${ORIGINAL_TABLES}"
    echo "Backup tables: ${BACKUP_TABLES}"
fi

echo "Verification completed successfully"
```

### MySQL/MariaDB Verification

```bash
#!/bin/bash
# verify-mysql-backup.sh
# Usage: ./verify-mysql-backup.sh shoko /path/to/backup.sql

DB_NAME="$1"
BACKUP_FILE="$2"

echo "Verifying MySQL backup..."

# Check file existence
if [ ! -f "$BACKUP_FILE" ]; then
    echo "✗ ERROR: Backup file not found: ${BACKUP_FILE}"
    exit 1
fi

# Check file size
BACKUP_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null)
echo "Backup size: ${BACKUP_SIZE} bytes"

if [ "$BACKUP_SIZE" -lt 1000 ]; then
    echo "✗ WARNING: Backup file is suspiciously small"
    exit 1
fi

# Verify backup contains valid SQL
echo "Verifying backup SQL..."
if grep -q "CREATE DATABASE" "$BACKUP_FILE" && \
   grep -q "CREATE TABLE" "$BACKUP_FILE" && \
   grep -q "Dump completed" "$BACKUP_FILE"; then
    echo "✓ Backup contains valid SQL"
else
    echo "✗ WARNING: Backup may be incomplete or corrupted"
    exit 1
fi

# Count tables
TABLE_COUNT=$(grep -c "CREATE TABLE" "$BACKUP_FILE")
echo "Tables found: ${TABLE_COUNT}"

if [ "$TABLE_COUNT" -lt 10 ]; then
    echo "✗ WARNING: Backup contains suspiciously few tables"
fi

echo "Verification completed successfully"
```

### SQL Server Verification

```bash
#!/bin/bash
# verify-sqlserver-backup.sh
# Usage: ./verify-sqlserver-backup.sh shokodb /path/to/backup.bak

DB_NAME="$1"
BACKUP_FILE="$2"

echo "Verifying SQL Server backup..."

# Check file existence
if [ ! -f "$BACKUP_FILE" ]; then
    echo "✗ ERROR: Backup file not found: ${BACKUP_FILE}"
    exit 1
fi

# Get SQL Server credentials from environment or prompt
if [ -z "$DB_HOST" ]; then
    read -p "SQL Server Host (default: 127.0.0.1): " DB_HOST
    DB_HOST=${DB_HOST:-127.0.0.1}
fi

if [ -z "$DB_PORT" ]; then
    read -p "SQL Server Port (default: 1433): " DB_PORT
    DB_PORT=${DB_PORT:-1433}
fi

if [ -z "$DB_USER" ]; then
    read -p "SQL Server User (default: sa): " DB_USER
    DB_USER=${DB_USER:-sa}
fi

read -s -p "SQL Server Password: " DB_PASS
echo

# Check file size
BACKUP_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null)
echo "Backup size: ${BACKUP_SIZE} bytes"

if [ "$BACKUP_SIZE" -lt 1000 ]; then
    echo "✗ WARNING: Backup file is suspiciously small"
    exit 1
fi

# Verify backup using RESTORE VERIFYONLY
echo "Verifying backup integrity..."
if sqlcmd -S "${DB_HOST},${DB_PORT}" -U "$DB_USER" -P "$DB_PASS" \
    -Q "RESTORE VERIFYONLY FROM DISK = '${BACKUP_FILE}'" | grep -q "The backup set on file 1 is valid"; then
    echo "✓ Backup is valid"
else
    echo "✗ ERROR: Backup verification failed"
    exit 1
fi

# Get backup information
echo "Backup information:"
sqlcmd -S "${DB_HOST},${DB_PORT}" -U "$DB_USER" -P "$DB_PASS" \
    -Q "RESTORE HEADERONLY FROM DISK = '${BACKUP_FILE}'" | grep -E "DatabaseName|BackupSize|BackupStartDate|BackupFinishDate"

echo "Verification completed successfully"
```

---

## Rollback Preparation

### Rollback Decision Tree

1. **Migration Failed During Startup**
   - Server logs show migration error
   - Database is in inconsistent state
   - **Action**: Restore from backup (see rollback procedures below)

2. **Migration Completed but Data Issues**
   - Server starts but data is missing or corrupted
   - API endpoints return errors
   - **Action**: Restore from backup and investigate issue

3. **Performance Degradation**
   - Server starts but performance is unacceptable
   - Queries are slow or timeout
   - **Action**: Monitor for 24 hours, restore if not improving

### Rollback Procedures

**For detailed rollback procedures, see [rollback.md](./rollback.md)**

Quick rollback summary:
1. Stop Shoko Server
2. Restore database from backup (use scripts above)
3. Verify database integrity
4. Restart Shoko Server
5. Verify data accessibility

---

## Post-Migration Verification

### Startup Verification

- [ ] **Server Starts Successfully**
  - [ ] No errors in startup logs
  - [ ] Database connection established
  - [ ] All services initialized

- [ ] **Migration Logs Review**
  - [ ] Check for migration completion message
  - [ ] Review any warnings or errors
  - [ ] Document migration duration

### Data Verification

- [ ] **Critical Data Accessible**
  - [ ] Anime series count matches expected
  - [ ] Episode count matches expected
  - [ ] Video files accessible
  - [ ] User data intact

- [ ] **API Endpoints Working**
  - [ ] List series endpoint works
  - [ ] Search functionality works
  - [ ] User authentication works
  - [ ] File operations work

### Performance Verification

- [ ] **Response Times Acceptable**
  - [ ] API response times within normal range
  - [ ] Database queries performant
  - [ ] No significant degradation

- [ ] **Resource Usage Normal**
  - [ ] CPU usage normal
  - [ ] Memory usage normal
  - [ ] Disk I/O normal

---

## Getting Help

Shoko is a community-driven hobby project for anime collection management. If you encounter issues during migration:

- **Read the Documentation**: Review this checklist, the migration guide, and the rollback guide thoroughly
- **Check Release Notes**: Always read the release notes for the version you're upgrading to
- **Search the Documentation**: Check the official Shoko documentation for known issues and solutions
- **Join the Community**: Visit the Shoko Discord server or forums for help from other users
- **Report Issues**: If you believe you've found a bug, report it on the Shoko GitHub issues page with detailed information

**Important**: Shoko is developed and maintained by volunteers in their spare time. Please be patient and respectful when seeking help from the community.

---

## Additional Resources

- **Migration Guide**: `migration-guide.md` — Production deployment and CLI commands
- **Rollback Guide**: `rollback.md` — Detailed rollback procedures
- **System Documentation**: `AGENTS.md` — Architecture and EF Core migration details
- **Official Documentation**: https://shokoanime.com/ — Official Shoko website and documentation
- **Community Support**: Discord server and forums — Community help and discussion
- **GitHub Issues**: https://github.com/ShokoAnime/ShokoServer/issues — Bug reports and feature requests

---

**Last Updated**: 2026-05-12  
**Feature Branch**: 001-database-client-migration  
**Version**: 1.0

**Last Updated**: 2026-05-12  
**Feature Branch**: 001-database-client-migration  
**Version**: 1.0