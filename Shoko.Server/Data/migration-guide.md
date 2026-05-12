# EF Core Migration Guide for Production Use

This guide covers the production-ready EF Core migration system for Shoko Server's database client migration from NHibernate to Entity Framework Core.

**CRITICAL**: EF Core migrations are applied **automatically during normal server startup**. Users should **NEVER** manually run migration commands or switch between NHibernate and EF Core. The CLI commands documented in this guide are for **development, testing, and troubleshooting purposes only**.

**Feature**: Database Client Migration (001-database-client-migration)  
**Target Framework**: .NET 10.0  
**Supported Providers**: SQLite, MySQL/MariaDB (Pomelo), SQL Server  
**Activation Model**: Automatic at server boot (no manual user intervention required)

**Out of Scope**: Quartz scheduler storage (`Quartz.db` / Quartz-managed provider schema) is not migrated by this workflow.

**For Operators and Developers**:
- **Operators**: Simply backup your database and start the server normally. Migration happens automatically.
- **Developers**: Use CLI commands for creating migrations, testing, and troubleshooting only.

---

## Table of Contents

1. [Automatic Migration Activation](#automatic-migration-activation)
2. [Prerequisites](#prerequisites)
3. [Migration Commands](#migration-commands) — **Development/Testing Only**
4. [Provider-Specific Configuration](#provider-specific-configuration)
5. [Production Deployment Workflow](#production-deployment-workflow)
6. [Rollback Procedures](#rollback-procedures)
7. [Troubleshooting](#troubleshooting)

---

## Automatic Migration Activation

**CRITICAL**: EF Core migrations are applied **automatically during normal server startup**. No manual user intervention is required.

### Startup Migration Flow

When Shoko Server starts, the following sequence occurs automatically:

1. **Detect Provider/Database/Version**:
   - Read database configuration from `DatabaseSettings`
   - Identify provider type (SQLite, MySQL/MariaDB, SQL Server)
   - Check current database version from `Versions` table

2. **Run Required Legacy Update/Bootstrap Steps**:
   - Execute `DatabaseFixes.cs` for any pending NHibernate-era schema mutations
   - Apply legacy database version updates
   - Ensure database is in a consistent state before EF Core migration

3. **Register/Apply EF Migration Baseline**:
   - Check `__EFMigrationsHistory` table for existing EF Core migrations
   - If baseline not registered, call `BaselineRegistration.RegisterBaselineAsync()` to register NHibernate schema as EF Core baseline
   - Apply any pending EF Core migrations automatically

4. **Continue Startup Automatically**:
   - EF Core `DbContext` is fully initialized
   - All repositories and services are ready
   - Server continues normal startup without interruption

### User Experience

- **Zero Manual Steps**: Users simply start Shoko Server as usual
- **Transparent Migration**: Migration happens automatically in the background
- **No Configuration Changes**: No new settings or environment variables required
- **No Manual Switching**: Users do NOT manually switch between NHibernate and EF Core
- **Graceful Rollback**: If startup activation fails, restore from backup and use the rollback procedure rather than asking operators to manually switch ORM modes

### Internal Architecture Notes

- **Legacy NHibernate/Bootstrap**: Remains as internal compatibility infrastructure during transition
- **No User Exposure**: NHibernate bootstrap path is not exposed to users or operators
- **Automatic Resolution**: Server automatically determines whether to use EF Core or NHibernate bootstrap
- **Seamless Transition**: Existing databases migrate seamlessly without user intervention

### Development vs. Production

- **Production**: Automatic migration at startup (documented in this section)
- **Development/Testing**: Manual CLI commands for creating and testing migrations (documented in [Migration Commands](#migration-commands))

---

### Required Tools

```bash
# Install EF Core CLI tools (global)
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

### Required Packages

The following EF Core packages must be installed in `Shoko.Server/Shoko.Server.csproj`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
```

### Database Backup

**CRITICAL**: Always create a full database backup before running any migration commands.

```bash
# SQLite backup
cp Shoko.db3 Shoko.db3.backup.$(date +%Y%m%d_%H%M%S)

# MySQL/MariaDB backup
mysqldump -u root -p shoko > shoko_backup_$(date +%Y%m%d_%H%M%S).sql

# SQL Server backup
sqlcmd -S localhost -U sa -P YourPassword -Q "BACKUP DATABASE shokodb TO DISK = 'C:\backup\shokodb_backup.bak'"
```

---

## Migration Commands (Development/Testing Only)

**NOTE**: These commands are for **development, testing, and troubleshooting purposes only**. Production deployments use automatic migration at startup.

### Creating a New Migration

### Creating a New Migration

```bash
# Basic migration (default SQLite provider)
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Provider-specific migration
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --context ShokoDbContext

# With verbose output
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --verbose
```

**Migration Naming Conventions**:
- Use descriptive PascalCase names: `AddVideoLocalHashDigestTable`, `AddTmdbShowCrossReference`, `UpdatePlaylistConfiguration`
- For multi-provider changes: `AddTmdbSupportForAllProviders`
- For provider-specific changes: `AddSqlServerFullTextSearch`, `AddMySqlJsonSupport`

### Applying Migrations to Database (Development/Testing Only)

**WARNING**: Do not use these commands in production. Production deployments use automatic migration at startup.

```bash
# Apply all pending migrations (default SQLite provider)
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Apply to specific migration
dotnet ef database update MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# With verbose output
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --verbose
```

**Production Deployment**: See [Production Deployment Workflow](#production-deployment-workflow) for automatic migration at startup.

```bash
# Apply all pending migrations (default SQLite provider)
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Apply to specific migration
dotnet ef database update MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# With verbose output
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --verbose
```

### Listing Migrations

```bash
# List all migrations
dotnet ef migrations list --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Show SQL for a migration (without applying)
dotnet ef migrations script MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --idempotent

# Generate SQL script for all migrations
dotnet ef migrations script --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --output migrations.sql --idempotent
```

### Removing the Last Migration

```bash
# Remove last migration (not applied to database)
dotnet ef migrations remove --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Remove last migration (force, even if applied)
dotnet ef migrations remove --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --force
```

---

## Provider-Specific Configuration

### Provider Caveats

- **SQLite**: Normal startup activation is fully automatic. Large benchmark datasets may require longer execution windows for measured runs.
- **MySQL/MariaDB**: Use `utf8mb4`-capable configuration. Provider tests and troubleshooting commands require explicit `DB_*` environment variables.
- **SQL Server**: Trust the provider-specific connection string settings shown below. Local benchmark evidence produced under Rosetta or other translation layers should be treated as directional only.

### SQLite Provider

```bash
# Set environment variable for SQLite
export DB_TYPE=SQLite

# Create migration
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Apply migration
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj
```

**Connection String**:
```
Data Source=shoko.db3;Mode=ReadWriteCreate;Pooling=True
```

### MySQL/MariaDB Provider

```bash
# Set environment variables for MySQL/MariaDB
export DB_TYPE=MySQL
export DB_HOST=127.0.0.1
export DB_PORT=3306
export DB_USER=shoko
export DB_PASS=your_password
export DB_NAME=shoko

# Create migration
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Apply migration
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj
```

**Connection String**:
```
Server=127.0.0.1;Port=3306;Database=shoko;User=shoko;Password=your_password;CharSet=utf8mb4;SslMode=None;AllowPublicKeyRetrieval=True
```

### SQL Server Provider

```bash
# Set environment variables for SQL Server
export DB_TYPE=SQLServer
export DB_HOST=127.0.0.1
export DB_PORT=1433
export DB_USER=sa
export DB_PASS=YourStrong@Password
export DB_NAME=shokodb

# Create migration
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# Apply migration
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj
```

**Connection String**:
```
Server=127.0.0.1,1433;Database=shokodb;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True;MultipleActiveResultSets=true
```

---

## Production Deployment Workflow

**IMPORTANT**: Production deployments use **automatic migration at startup**. No manual `dotnet ef database update` commands are required.

### Pre-Deployment Checklist

1. **Database Backup**: Create full backup of production database
2. **Migration Review**: Review migration SQL script for potential breaking changes
3. **Staging Test**: Deploy and test automatic migration on staging environment
4. **Rollback Plan**: Prepare rollback procedure (see [Rollback Procedures](#rollback-procedures))
5. **Downtime Window**: Schedule maintenance window if migration requires downtime

### Deployment Steps

```bash
# 1. Create database backup
./scripts/backup-database.sh

# 2. Verify backup integrity
./scripts/verify-backup.sh

# 3. Deploy new Shoko Server version to staging
# (No manual migration commands required)

# 4. Start Shoko Server on staging
# Automatic migration will occur during startup:
# - Detect provider/database/version
# - Run required legacy update/bootstrap steps
# - Register/apply EF migration baseline as needed
# - Continue startup automatically

# 5. Verify staging deployment
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "SmokeTests" --configuration Release

# 6. Monitor application logs for migration completion
tail -f logs/shoko-server.log | grep -i "migration\|startup"

# 7. If staging tests pass, deploy to production
# (No manual migration commands required)

# 8. Start Shoko Server in production
# Automatic migration will occur during startup (same as staging)

# 9. Verify production deployment
# Check logs for successful migration and startup
```

### Zero-Downtime Migration Strategy

For large databases or critical production systems:

```bash
# 1. Create new migration with idempotent script (for review only)
dotnet ef migrations script --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --output migration.sql --idempotent

# 2. Review and test migration.sql on staging
# 3. Deploy new Shoko Server version during low-traffic period
# 4. Start Shoko Server — automatic migration occurs during startup
#    No manual migration commands required
# 5. Monitor application health metrics during migration
```

---

## Rollback Procedures

### Quick Rollback (Database Only)

```bash
# Restore from backup
# SQLite
cp Shoko.db3.backup.20260512_120000 Shoko.db3

# MySQL/MariaDB
mysql -u root -p shoko < shoko_backup_20260512_120000.sql

# SQL Server
sqlcmd -S localhost -U sa -P YourPassword -Q "RESTORE DATABASE shokodb FROM DISK = 'C:\backup\shokodb_backup.bak' WITH REPLACE"
```

### Full Rollback (Database + Code)

**NOTE**: This is for development/emergency rollback scenarios. Production operators should use database-only rollback. Do not ask normal users to manually flip between NHibernate and EF Core.

```bash
# 1. Restore database from backup (see above)
# 2. Revert git commit containing EF Core changes
git revert <commit-hash>
# 3. Rebuild and restart server from the reverted code state
dotnet build Shoko.Server/Shoko.Server.sln --configuration Release
# 4. Verify startup and provider smoke tests
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj --filter "DatabaseMigrationTests" --configuration Release
```

**For detailed rollback procedures, see [rollback.md](./rollback.md)**.

---

## Troubleshooting

### Common Issues

#### Migration Already Applied

```bash
# Error: The migration '20260512_AddVideoLocalHashDigestTable' has already been applied.

# Solution: Check current migration state
dotnet ef migrations list --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj

# If migration was incorrectly applied, use:
dotnet ef database update PreviousMigration --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj
```

#### Connection String Issues

```bash
# Error: A network-related or instance-specific error occurred while establishing a connection to SQL Server.

# Solution: Verify connection string and provider settings
echo $DB_TYPE
echo $DB_HOST
echo $DB_PORT
echo $DB_USER
echo $DB_NAME

# Test database connectivity manually
# SQLite
sqlite3 Shoko.db3 "SELECT 1;"

# MySQL/MariaDB
mysql -h 127.0.0.1 -P 3306 -u shoko -p shoko -e "SELECT 1;"

# SQL Server
sqlcmd -S 127.0.0.1,1433 -U sa -P YourPassword -Q "SELECT 1;"
```

#### Provider-Specific Migration Conflicts

```bash
# Error: The operation failed because an index or statistics with name \'IX_VideoLocal_ED2K\' already exists on table \'VideoLocal\'.

# Solution: Generate idempotent SQL script and review
dotnet ef migrations script --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --output migration.sql --idempotent

# Development/Testing Only: Manually edit migration.sql to handle conflicts
# Apply migration manually (development/testing only)
# SQLite
sqlite3 Shoko.db3 < migration.sql

# MySQL/MariaDB
mysql -u root -p shoko < migration.sql

# SQL Server
sqlcmd -S localhost -U sa -P YourPassword -i migration.sql
```

### Debug Mode

```bash
# Enable verbose logging
dotnet ef database update --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --verbose

# Enable detailed errors
dotnet ef migrations add MigrationName --project Shoko.Server/Shoko.Server.csproj --startup-project Shoko.CLI/Shoko.CLI.csproj --verbose --diagnostics
```

---

## Best Practices

1. **Always backup before migration**: Never skip the backup step
2. **Test on staging first**: Never apply untested migrations to production
3. **Review migration SQL**: Always review generated SQL for potential issues
4. **Use idempotent scripts**: Generate idempotent SQL for development/testing review
5. **Monitor after deployment**: Watch application logs and metrics after migration
6. **Document breaking changes**: Clearly document any breaking changes in release notes
7. **Version control migrations**: Never manually edit migration files after generation
8. **Keep migrations small**: Prefer multiple small migrations over one large migration
9. **Test on all providers**: Validate migrations work on SQLite, MySQL/MariaDB, and SQL Server
10. **Plan rollback strategy**: Always have a rollback plan ready before deployment
11. **Never manually switch**: Users should never manually switch between NHibernate and EF Core
12. **Trust automatic migration**: Let the server handle migration automatically at startup

---

## Additional Resources

- [EF Core Migrations Documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Shoko Server Rollback Guide](./rollback.md)
- [Shoko Server Pre-Migration Checklist](./pre-migration-checklist.md)
- [Shoko Server Data Inventory](./inventory.md)

---

**Last Updated**: 2026-05-12  
**Feature Branch**: 001-database-client-migration  
**Maintainer**: Shoko Server Development Team
