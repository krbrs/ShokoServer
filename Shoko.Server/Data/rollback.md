# EF Core Migration Rollback Procedure

**Feature**: Database Client Migration (NHibernate → EF Core)
**Migration ID**: 001-database-client-migration
**Last Updated**: 2026-05-11
**Status**: Automatic EF startup activation implemented; NHibernate/bootstrap path retained for bootstrap/compatibility

---

## Overview

This document provides rollback procedures for the EF Core migration from NHibernate. The current architecture maintains both EF Core and legacy NHibernate/bootstrap paths, allowing for safe rollback in case of migration failures.

**Current State**:
- ✅ Automatic EF startup activation is implemented in normal startup
- ✅ EF Core migrations are validated through provider validation and startup activation tests
- ✅ Schema comparison utility confirms EF Core model matches NHibernate schema
- ✅ Provider-specific integration tests pass (SQLite, MariaDB, SQL Server)
- ✅ Cross-provider consistency verified
- ℹ️ NHibernate packages and infrastructure are retained for bootstrap/compatibility
- ℹ️ Legacy NHibernate/bootstrap path is still functional as internal infrastructure
- ℹ️ NHibernate removal (T180-T189) remains deferred
- ℹ️ Quartz scheduler storage (`Quartz.db` / Quartz-managed provider schema) is out of scope for this document

---

## Critical Safety Warnings

### ⚠️ Pre-Migration Backup Requirements

**ALWAYS take a database backup before attempting any migration or rollback**:

1. **Stop Shoko Server** completely before backup
2. **Backup the entire database file** (SQLite) or **full database dump** (MariaDB/SQL Server)
3. **Verify backup integrity** before proceeding
4. **Store backup in a safe location** separate from the production database
5. **Document backup location and timestamp** for rollback reference

### ⚠️ Benchmark/Test Dataset Safety

**Never mutate source benchmark databases or backups**:

- Use copied/restored working databases only for benchmark/test operations
- Source SQLite DBs and SQL Server backups must remain read-only
- Apply mutations only to working copies created from source data
- Verify source DB size/hash before and after benchmark operations
- Log row counts and cardinality summaries only (no raw path/filename logging)

### ⚠️ Schema Verification After Rollback

**Always verify database integrity after rollback**:

- Run `SchemaComparer.CompareAsync()` after restoration
- Verify `__EFMigrationsHistory` table state (should be empty or contain only valid migrations)
- Confirm all tables and columns match expected schema
- Run provider-specific validation tests (SQLite/MariaDB/SQL Server integration tests)

---

## Rollback Scenarios

### Scenario 1: Failed EF Core Migration Application

**Symptoms**:
- `dotnet ef database update` fails with errors
- Migration log shows partial application
- Database is in inconsistent state
- Application fails to start after migration

**Rollback Procedure**:

#### Step 1: Stop Shoko Server

```bash
# Stop the Shoko Server process
# Method varies by platform (systemd, service manager, or manual process kill)
```

#### Step 2: Restore Database from Backup

**SQLite**:
```bash
# Locate backup file (e.g., shoko_143_20260511_1920.db)
cp /path/to/backup/shoko_143_20260511_1920.db /path/to/production/shoko.db

# Verify backup restoration
sqlite3 /path/to/production/shoko.db "PRAGMA integrity_check;"
```

**MariaDB**:
```bash
# Stop MariaDB service
sudo systemctl stop mariadb

# Drop corrupted database
mysql -u root -p -e "DROP DATABASE IF EXISTS shoko;"

# Restore from backup
mysql -u root -p shoko < /path/to/backup/shoko_backup_20260511.sql

# Start MariaDB service
sudo systemctl start mariadb

# Verify restoration
mysql -u root -p -e "USE shoko; SHOW TABLES;"
```

**SQL Server**:
```sql
-- Connect to SQL Server
-- Drop corrupted database
DROP DATABASE IF EXISTS shoko;

-- Restore from backup
RESTORE DATABASE shoko
FROM DISK = '/path/to/backup/shoko_backup_20260511.bak'
WITH REPLACE;

-- Verify restoration
USE shoko;
SELECT name FROM sys.tables;
```

#### Step 3: Verify Database Integrity

```bash
# Run schema comparison verification
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "SchemaComparisonTests" --no-build

# Run provider-specific integration tests
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "SQLite" --no-build
```

#### Step 4: Restart Shoko Server

```bash
# Start Shoko Server
# Monitor logs for successful startup
```

**Expected Outcome**:
- Database restored to pre-migration state
- NHibernate/bootstrap path functional
- Application starts successfully
- All data accessible

---

### Scenario 2: Failed Baseline Registration

**Symptoms**:
- `BaselineRegistration.RegisterBaselineAsync()` fails
- `__EFMigrationsHistory` table contains invalid or partial entries
- Database schema is correct but EF Core migration history is corrupted
- Application fails to recognize existing database as migrated

**Rollback Procedure**:

#### Step 1: Stop Shoko Server

```bash
# Stop the Shoko Server process
```

#### Step 2: Clean Up EF Core Migration History

**SQLite**:
```sql
-- Connect to SQLite database
sqlite3 /path/to/shoko.db

-- Check current migration history
SELECT * FROM __EFMigrationsHistory;

-- Remove invalid migration entries
DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20260509114039_InitialCreate';

-- Verify cleanup
SELECT * FROM __EFMigrationsHistory;
```

**MariaDB**:
```sql
-- Connect to MariaDB
mysql -u root -p shoko

-- Check current migration history
SELECT * FROM __EFMigrationsHistory;

-- Remove invalid migration entries
DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20260509114039_InitialCreate';

-- Verify cleanup
SELECT * FROM __EFMigrationsHistory;
```

**SQL Server**:
```sql
-- Connect to SQL Server
USE shoko;

-- Check current migration history
SELECT * FROM __EFMigrationsHistory;

-- Remove invalid migration entries
DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20260509114039_InitialCreate';

-- Verify cleanup
SELECT * FROM __EFMigrationsHistory;
```

#### Step 3: Verify Database Schema

```bash
# Run schema comparison to verify NHibernate schema is intact
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "SchemaComparisonTests" --no-build
```

#### Step 4: Restart Shoko Server

```bash
# Start Shoko Server
# Application will use NHibernate/bootstrap path
```

**Expected Outcome**:
- `__EFMigrationsHistory` table cleaned up
- Database schema remains intact (NHibernate schema)
- Application uses NHibernate/bootstrap path successfully
- No data loss

---

### Scenario 3: Provider-Specific Rollback Issues

#### SQLite Provider Rollback

**Common Issues**:
- SQLite database corruption during migration
- Journal file conflicts
- Lock file issues

**Rollback Procedure**:

```bash
# Stop Shoko Server

# Remove lock files if present
rm -f /path/to/shoko.db-shm
rm -f /path/to/shoko.db-wal

# Restore from backup
cp /path/to/backup/shoko_143_20260511_1920.db /path/to/production/shoko.db

# Verify integrity
sqlite3 /path/to/production/shoko.db "PRAGMA integrity_check;"

# Restart Shoko Server
```

**SQLite-Specific Notes**:
- SQLite databases are single files, making backup/restore straightforward
- Journal files (`-shm`, `-wal`) are automatically managed by SQLite
- Corruption can occur if the process crashes during a write operation
- Always ensure the database file is not open during backup/restore

#### MariaDB Provider Rollback

**Common Issues**:
- Character set/collation mismatches
- Foreign key constraint violations
- Transaction rollback failures

**Rollback Procedure**:

```bash
# Stop Shoko Server

# Stop MariaDB service
sudo systemctl stop mariadb

# Drop corrupted database
mysql -u root -p -e "DROP DATABASE IF EXISTS shoko;"

# Restore from backup
mysql -u root -p shoko < /path/to/backup/shoko_backup_20260511.sql

# Start MariaDB service
sudo systemctl start mariadb

# Verify character set and collation
mysql -u root -p -e "USE shoko; SELECT DEFAULT_CHARACTER_SET_NAME, DEFAULT_COLLATION_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = 'shoko';"

# Restart Shoko Server
```

**MariaDB-Specific Notes**:
- Character set should be `utf8mb4` for full Unicode support
- Collation should be `utf8mb4_unicode_ci` for case-insensitive comparisons
- Foreign key constraints can cause restore failures if tables are restored in wrong order
- Use `SET FOREIGN_KEY_CHECKS=0;` before restore if needed

#### SQL Server Provider Rollback

**Common Issues**:
- Database file path changes
- Permission issues
- Transaction log corruption

**Rollback Procedure**:

```sql
-- Connect to SQL Server as sysadmin

-- Set database to single-user mode
ALTER DATABASE shoko SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

-- Drop corrupted database
DROP DATABASE IF EXISTS shoko;

-- Restore from backup
RESTORE DATABASE shoko
FROM DISK = '/path/to/backup/shoko_backup_20260511.bak'
WITH REPLACE,
MOVE 'shoko' TO '/var/opt/mssql/data/shoko.mdf',
MOVE 'shoko_log' TO '/var/opt/mssql/data/shoko_log.ldf';

-- Set database to multi-user mode
ALTER DATABASE shoko SET MULTI_USER;

-- Verify restoration
USE shoko;
SELECT name FROM sys.tables;
```

**SQL Server-Specific Notes**:
- Database files (`mdf`, `ldf`) have specific paths in `RESTORE DATABASE` command
- `WITH REPLACE` option overwrites existing database
- `SINGLE_USER` mode prevents other connections during restore
- Transaction log (`ldf`) must be restored with data file (`mdf`)

---

### Scenario 4: Restoring from Backup (General Procedure)

**When to Use**:
- Any migration failure that corrupts the database
- Complete system rollback to pre-migration state
- Data recovery after catastrophic failure

**Rollback Procedure**:

#### Step 1: Identify Correct Backup

```bash
# List available backups
ls -lh /path/to/backups/

# Choose backup with correct version and timestamp
# Example: shoko_143_20260511_1920.db (version 143, May 11, 2026, 19:20)
```

#### Step 2: Stop Shoko Server

```bash
# Stop the Shoko Server process
# Ensure no processes are accessing the database
```

#### Step 3: Backup Current State (Before Rollback)

```bash
# Backup current (corrupted) state for analysis
cp /path/to/production/shoko.db /path/to/corrupted/shoko_corrupted_$(date +%Y%m%d_%H%M%S).db
```

#### Step 4: Restore from Backup

**SQLite**:
```bash
cp /path/to/backup/shoko_143_20260511_1920.db /path/to/production/shoko.db
```

**MariaDB**:
```bash
mysql -u root -p shoko < /path/to/backup/shoko_backup_20260511.sql
```

**SQL Server**:
```sql
RESTORE DATABASE shoko
FROM DISK = '/path/to/backup/shoko_backup_20260511.bak'
WITH REPLACE;
```

#### Step 5: Verify Restoration

```bash
# Run schema comparison
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "SchemaComparisonTests" --no-build

# Run provider-specific tests
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "SQLite" --no-build
```

#### Step 6: Restart Shoko Server

```bash
# Start Shoko Server
# Monitor logs for successful startup
```

---

### Scenario 5: Reverting to a Known-Good Pre-EF Code/Database State

**When to Use**:
- Automatic EF startup activation fails and database-only restoration is not sufficient
- A development or emergency rollback needs both code and database reverted together
- You need to return to a known-good earlier build rather than manually switching ORM modes

**Rollback Procedure**:

#### Step 1: Verify NHibernate Infrastructure is Intact

```bash
# Check NHibernate packages are installed
grep -A 5 "FluentNHibernate\|NHibernate" Shoko.Server/Shoko.Server.csproj

# Check Mappings/ directory exists
ls -la Shoko.Server/Mappings/

# Check NHibernate converter files exist
ls -la Shoko.Server/Databases/NHIbernate/
```

#### Step 2: Verify Database Uses NHibernate Schema

```bash
# Run schema comparison to confirm NHibernate schema
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "SchemaComparisonTests" --no-build
```

#### Step 3: Restore a Known-Good Application Build

```bash
# Restore or redeploy the known-good application version
# Do not attempt to toggle ORM mode manually for normal operations
```

#### Step 4: Restart Shoko Server

```bash
# Start Shoko Server
# Application should start from the restored code + restored database state
```

**Expected Outcome**:
- Application starts successfully from the restored code state
- Database schema matches the restored backup
- Automatic startup activation either succeeds on the restored state or is not needed because the restored build predates the EF path
- No manual ORM toggle is required

**Limitations**:
- This is a code-and-database rollback, not an in-place ORM mode switch
- It should be treated as an emergency/development recovery path, not normal operator behavior

---

### Scenario 6: Benchmark/Test Dataset Rollback Caveats

**Special Considerations for Benchmark/Test Data**:

#### Benchmark Dataset Safety

**Never mutate source benchmark databases**:
- Source SQLite DBs must remain read-only
- Source SQL Server backups must remain read-only
- Always create working copies for benchmark operations

**Benchmark Dataset Preparation Workflow**:
```bash
# 1. Copy source DB to working directory (read-only source preserved)
cp /path/to/source/benchmark_source.db /path/to/work/benchmark_work.db

# 2. Verify source DB integrity before mutation
sqlite3 /path/to/source/benchmark_source.db "PRAGMA integrity_check;"

# 3. Record source DB hash and size
sha256sum /path/to/source/benchmark_source.db > /path/to/source/benchmark_source.sha256
ls -lh /path/to/source/benchmark_source.db > /path/to/source/benchmark_source.size

# 4. Apply mutations only to working copy
# (run benchmark operations on /path/to/work/benchmark_work.db)

# 5. Verify source DB unchanged after operations
sha256sum -c /path/to/source/benchmark_source.sha256
cmp /path/to/source/benchmark_source.db <(ls -l /path/to/source/benchmark_source.db)
```

#### Benchmark Dataset Rollback

**If working copy is corrupted**:
```bash
# Delete corrupted working copy
rm -f /path/to/work/benchmark_work.db

# Create fresh copy from source (read-only source preserved)
cp /path/to/source/benchmark_source.db /path/to/work/benchmark_work.db

# Verify working copy integrity
sqlite3 /path/to/work/benchmark_work.db "PRAGMA integrity_check;"
```

**If source DB is accidentally mutated**:
```bash
# This is a critical failure - restore from original backup
# Source DBs should never be mutated
# Restore from known-good backup taken before benchmark operations
cp /path/to/backup/benchmark_source_original.db /path/to/source/benchmark_source.db
```

**Benchmark Dataset Logging Requirements**:
- Log only row counts and cardinality summaries
- Never log raw file paths or filenames
- Never log user data or sensitive information
- Use relative paths or hashes only when necessary

---

## Verification Steps After Rollback

### 1. Database Integrity Check

**SQLite**:
```bash
sqlite3 /path/to/shoko.db "PRAGMA integrity_check;"
```

**MariaDB**:
```sql
-- Check all tables
USE shoko;
CHECK TABLE shoko.AnimeSeries;
CHECK TABLE shoko.AnimeEpisode;
-- Repeat for all critical tables
```

**SQL Server**:
```sql
-- Check database integrity
USE shoko;
DBCC CHECKDB WITH NO_INFOMSGS;
```

### 2. Schema Comparison

```bash
# Run schema comparison tests
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "SchemaComparisonTests" --no-build

# Expected: All tests pass, schema matches NHibernate baseline
```

### 3. Provider-Specific Validation

**SQLite**:
```bash
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "SQLite" --no-build
```

**MariaDB**:
```bash
DB_TYPE=MySQL DB_HOST=127.0.0.1 DB_USER=root DB_PASS=root DB_NAME=shoko \
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "MariaDB" --no-build
```

**SQL Server**:
```bash
DB_TYPE=SQLServer DB_HOST=127.0.0.1 DB_USER=sa DB_PASS='ShokoTest1!' DB_NAME=shoko \
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "SQLServer" --no-build
```

### 4. Application Startup Test

```bash
# Start Shoko Server
# Monitor logs for successful startup
# Verify database connection established
# Verify all services initialized successfully
```

### 5. Data Accessibility Test

```bash
# Verify critical data is accessible
# Example: Check anime series count via API or database query
sqlite3 /path/to/shoko.db "SELECT COUNT(*) FROM AnimeSeries;"
```

---

## Common Rollback Failures and Solutions

### Failure 1: Backup File is Corrupted

**Symptoms**:
- Restore operation fails
- Backup file is unreadable
- Database integrity check fails after restore

**Solution**:
- Use older backup if available
- If no backup exists, attempt database repair (provider-specific)
- Consider data recovery services for critical data loss

### Failure 2: Schema Mismatch After Restore

**Symptoms**:
- Application fails to start after restore
- Schema comparison tests fail
- Database version mismatch

**Solution**:
- Verify correct backup was restored (check version in `Versions` table)
- Run legacy NHibernate bootstrap to apply missing schema patches
- If schema is incompatible, may need to restore from earlier backup

### Failure 3: Permission Issues During Restore

**Symptoms**:
- Permission denied errors during restore
- Unable to write to database location
- File ownership issues

**Solution**:
- Check file permissions on database files
- Ensure Shoko Server process has read/write access
- Use appropriate user permissions for restore operation

### Failure 4: Lock/Connection Conflicts

**Symptoms**:
- Database is locked during restore
- Connection errors during rollback
- Unable to stop Shoko Server

**Solution**:
- Ensure Shoko Server is completely stopped
- Kill any lingering database connections
- Remove lock files (SQLite: `-shm`, `-wal`)
- Restart database service if necessary (MariaDB/SQL Server)

---

## Rollback Decision Tree

```
Migration Failure Detected
    │
    ├─ Is database corrupted?
    │   ├─ YES → Restore from backup (Scenario 4)
    │   └─ NO → Continue
    │
    ├─ Is EF Core migration history corrupted?
    │   ├─ YES → Clean up __EFMigrationsHistory (Scenario 2)
    │   └─ NO → Continue
    │
    ├─ Is provider-specific issue?
    │   ├─ YES → Use provider-specific rollback (Scenario 3)
    │   └─ NO → Continue
    │
    ├─ Is EF Core path completely broken?
    │   ├─ YES → Revert to NHibernate path (Scenario 5)
    │   └─ NO → Continue
    │
    └─ Is benchmark/test dataset corrupted?
        ├─ YES → Use benchmark rollback caveats (Scenario 6)
        └─ NO → Use general rollback procedure (Scenario 1 or 4)
```

---

## Post-Rollback Checklist

- [ ] Shoko Server stopped before rollback
- [ ] Correct backup identified and verified
- [ ] Database restored from backup
- [ ] Database integrity verified
- [ ] Schema comparison tests pass
- [ ] Provider-specific validation tests pass
- [ ] Application starts successfully
- [ ] Critical data accessible
- [ ] No lock files or connection conflicts
- [ ] Logs show successful startup
- [ ] Rollback documented with timestamp and backup used

---

## Contact and Support

**For rollback assistance**:
- Review this documentation carefully
- Check application logs for specific error messages
- Verify backup integrity before attempting restore
- Contact development team if rollback procedures fail

**Documentation Updates**:
- Update this document with new rollback scenarios as they are discovered
- Record lessons learned from rollback attempts
- Improve procedures based on real-world rollback experience

---

**End of Rollback Procedure**
