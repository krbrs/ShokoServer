# Quickstart: Database Client Migration

## Overview

This document provides a quick reference for implementing the NHibernate to EF Core migration. It covers the essential setup steps, configuration patterns, and common tasks.

## Prerequisites

1. **NHibernate inventory complete**: All custom `IUserType` converters (count TBD from inventory), the `NLogInterceptor`, and all 60+ mapping files have been audited.
2. **Branch**: Working on `001-database-client-migration`.
3. **Backup**: Production databases must be backed up before any migration work begins.

## Step 1: Add EF Core Packages

In `Shoko.Server/Shoko.Server.csproj`, add:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" PrivateAssets="All" />
```

## Step 2: Create DbContext

Create `Shoko.Server/Data/ShokoDbContext.cs`:

```csharp
public class ShokoDbContext : DbContext
{
    public ShokoDbContext(DbContextOptions<ShokoDbContext> options) : base(options) { }

    // DbSets for all entities — one per mapped class
    public DbSet<AnimeSeries> AnimeSeries => Set<AnimeSeries>();
    public DbSet<AnimeEpisode> AnimeEpisodes => Set<AnimeEpisode>();
    public DbSet<VideoLocal> VideoLocals => Set<VideoLocal>();
    // ... all other entities

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration<T> classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShokoDbContext).Assembly);

        // Global query filters (if any)
        // modelBuilder.Entity<Entity>().HasQueryFilter(...);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Provider selection is handled externally via DI
        // Do NOT configure the provider here
    }
}
```

## Step 3: Create Entity Configurations

Create `Shoko.Server/Data/Configurations/` directory with `IEntityTypeConfiguration<T>` classes:

```csharp
// Example: VideoLocalConfiguration.cs
public class VideoLocalConfiguration : IEntityTypeConfiguration<VideoLocal>
{
    public void Configure(EntityTypeBuilder<VideoLocal> builder)
    {
        builder.ToTable("VideoLocal");

        builder.HasKey(v => v.VideoLocalID);

        builder.Property(v => v.VideoLocalID).ValueGeneratedOnAdd();
        builder.Property(v => v.DateTimeUpdated).IsRequired();
        builder.Property(v => v.DateTimeCreated).IsRequired();
        builder.Property(v => v.DateTimeImported);
        builder.Property(v => v.FileName).IsRequired();
        builder.Property(v => v.FileSize).IsRequired();
        builder.Property(v => v.Hash).IsRequired();
        builder.Property(v => v.HashSource).IsRequired();
        builder.Property(v => v.IsIgnored).IsRequired();
        builder.Property(v => v.IsVariation).IsRequired();
        builder.Property(v => v.MediaVersion).IsRequired();

        // MessagePack value converter for MediaInfo
        builder.Property(v => v.MediaInfo)
            .HasColumnName("MediaBlob")
            .HasColumnType("varbinary")
            .HasConversion(
                v => v == null ? null : MessagePackSerializer.Serialize(v),
                v => v == null ? null : MessagePackSerializer.Deserialize<MediaContainer>(v));

        builder.Property(v => v.MyListID).IsRequired();
        builder.Property(v => v.LastAVDumped);
        builder.Property(v => v.LastAVDumpVersion);

        // Relationships
        builder.HasMany(v => v.VideoLocalPlaces)
            .WithOne(p => p.VideoLocal)
            .HasForeignKey(p => p.VideoLocalID);

        builder.HasMany(v => v.VideoLocalHashDigests)
            .WithOne(h => h.VideoLocal)
            .HasForeignKey(h => h.VideoLocalID);

        // Indexes (match NHibernate schema)
        builder.HasIndex(v => v.Hash);
    }
}
```

## Step 4: Provider Selection

Create `Shoko.Server/Databases/EFCoreDatabaseProvider.cs`:

```csharp
public enum EFCoreDatabaseProvider
{
    SQLite,
    MariaDB,
    SQLServer
}

public static class EFCoreOptionsExtensions
{
    public static void ConfigureShokoDbContext(this DbContextOptionsBuilder options, EFCoreDatabaseProvider provider, string connectionString)
    {
        switch (provider)
        {
            case EFCoreDatabaseProvider.SQLite:
                options.UseSqlite(connectionString);
                break;
            case EFCoreDatabaseProvider.MariaDB:
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                break;
            case EFCoreDatabaseProvider.SQLServer:
                options.UseSqlServer(connectionString);
                break;
        }
    }
}
```

## Step 5: Register in DI

In `Shoko.Server/Repositories/RepositoryStartup.cs`, add EF Core registration:

```csharp
// After existing DatabaseFactory registration
services.AddScoped<ShokoDbContext>(sp =>
{
    var settings = Utils.SettingsProvider.GetSettings();
    var connectionString = settings.Database.GetConnectionString(); // existing method
    var provider = settings.Database.Type switch
    {
        Constants.DatabaseType.SQLServer => EFCoreDatabaseProvider.SQLServer,
        Constants.DatabaseType.MySQL => EFCoreDatabaseProvider.MariaDB,
        _ => EFCoreDatabaseProvider.SQLite
    };

    var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
    optionsBuilder.ConfigureShokoDbContext(provider, connectionString);
    return new ShokoDbContext(optionsBuilder.Options);
});
```

## Step 6: Migrate Repositories

For each repository, replace NHibernate `ISession` with EF Core `DbContext`:

**Before (BaseCachedRepository.cs)**:
```csharp
using var session = _databaseFactory.SessionFactory.OpenSession();
using var transaction = session.BeginTransaction();
session.SaveOrUpdate(obj);
transaction.Commit();
```

**After**:
```csharp
using var scope = _serviceProvider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
using var transaction = await context.Database.BeginTransactionAsync();
context.Entry(obj).State = EntityState.Modified;
await context.SaveChangesAsync();
await transaction.CommitAsync();
```

## Step 7: Schema Comparison & Baseline Registration

**Do NOT apply `InitialCreate` directly to existing NHibernate databases.** Existing databases must be schema-validated and then marked as having the EF Core baseline applied, without creating already-existing tables.

### For Fresh Databases
```bash
dotnet ef migrations add InitialCreate --project Shoko.Server --context ShokoDbContext
dotnet ef database update --project Shoko.Server --context ShokoDbContext
```

### For Existing Databases
1. Run schema comparison utility (`SchemaComparer`) against the existing database
2. If schema matches EF Core model, run baseline registration to mark the database as compliant
3. The baseline registration adds an entry to `__EFMigrationsHistory` without modifying existing tables
4. Future schema changes use standard `dotnet ef migrations add` / `dotnet ef database update`

## Step 8: Run Integration Tests

```bash
dotnet test Shoko.IntegrationTests --filter "Category=EFCore"
```

Test against all three backends:
- SQLite (fast, local, default)
- MariaDB (requires Docker or local instance)
- SQL Server (requires Docker or local instance)

## Key Patterns

### Explicit Loading (No Lazy Loading)
```csharp
// Instead of lazy-loaded navigation:
// var episodes = series.AnimeEpisodes;

// Use explicit loading:
var series = await context.AnimeSeries
    .Include(s => s.AnimeEpisodes)
    .Include(s => s.AnimeGroup)
    .FirstOrDefaultAsync(s => s.AnimeSeriesID == id);
```

### Value Converters
All NHibernate `IUserType` converters (count TBD from inventory) map to EF Core `ValueConverter`:
- MessagePack → `HasConversion(byte[] serializer/deserializer)`
- Newtonsoft.Json → `HasConversion(string serializer/deserializer)`
- Int/Enum → `HasConversion(int converter)`
- DateOnly → `HasConversion<int>(DateOnlyConverter)` mapping `DateOnly ↔ int` (Unix epoch days)

### Transaction Handling
Preserve the existing repository-level transaction pattern:
- `Save()` / `Delete()` create their own transactions
- `SaveWithOpenTransaction()` accepts an external `IDbContextTransaction`

### In-Memory Cache
Preserve `PocoCache<S, T>` structure:
- Load all entities at startup: `context.Set<T>().AsNoTracking().ToList()`
- Use `ReaderWriterLockSlim` for thread-safe access
- Rebuild indexes via `PopulateIndexes()` per repository

### Folder Structure
```
Shoko.Server/Data/
├── ShokoDbContext.cs              # Main DbContext
├── Configurations/                # IEntityTypeConfiguration<T> classes
│   ├── VideoLocalConfiguration.cs
│   ├── AnimeSeriesConfiguration.cs
│   └── ...
├── Converters/                    # EF Core ValueConverter classes
│   ├── MessagePackConverter.cs
│   ├── DateOnlyConverter.cs
│   └── ...
├── Design/                        # Design-time support
│   └── ShokoDbContextDesignTimeFactory.cs
├── Migrations/                    # EF Core migration files
└── SchemaComparison/              # Schema validation utilities
    ├── SchemaComparer.cs
    └── BaselineRegistration.cs
```
