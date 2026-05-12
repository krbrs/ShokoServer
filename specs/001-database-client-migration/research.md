# Research: NHibernate to EF Core Migration

## Research Findings

### 1. EF Core Provider Selection for MariaDB

**Decision**: Use `Pomelo.EntityFrameworkCore.MySql` for MariaDB support.

**Rationale**: 
- Pomelo is the most widely used and actively maintained EF Core provider for MySQL/MariaDB.
- The existing NHibernate driver (`NHibernate.Driver.MySqlConnector`) uses MySqlConnector under the hood, so the connection string format is compatible.
- Pomelo supports EF Core 9+ (compatible with .NET 10).
- Alternatives evaluated:
  - `MySqlConnector` (Pomelo's underlying driver) — does not provide EF Core provider functionality.
  - `Devart.Data.MySql.EFCore` — commercial license required.
  - `ZMySqlEFCore` — less actively maintained, limited EF Core version support.

**Alternatives considered**:
- Using `Microsoft.EntityFrameworkCore.MySQL` (Oracle's official provider) — less mature EF Core feature support compared to Pomelo.
- Creating a custom provider — not feasible given scope and timeline.

### 2. EF Core Value Converters for NHibernate IUserType Replacements

**Decision**: Replace all NHibernate `IUserType` converters with EF Core `ValueConverter<T, TProvider>` classes. The exact count is not known until the inventory (Phase 1, T-005 through T-010) confirms it. The inventory will document all converters found in `Shoko.Server/Databases/NHIbernate/` and any other NHibernate custom types used across mapping files.

**Current NHibernate custom types identified**:
| NHibernate Type | Storage | EF Core Replacement |
|-----------------|---------|---------------------|
| `MessagePackConverter<T>` | `varbinary`/`BLOB` | `ValueConverter<T, byte[]>` using MessagePack serializer |
| `TypelessMessagePackConverter` | `varbinary`/`BLOB` | `ValueConverter<object, byte[]>` using MessagePack.Typeless |
| `FilterExpressionConverter` | `TEXT`/`VARCHAR` | `ValueConverter<FilterExpression<bool>, string>` using Newtonsoft.Json |
| `DateOnlyConverter` | `INTEGER` | `ValueConverter<DateOnly, int>` (`DateOnly` ↔ `int` Unix epoch days) |
| `TitleLanguageConverter` | `TEXT`/`VARCHAR` | `ValueConverter<TitleLanguage, string>` |
| `TitleTypeConverter` | `INTEGER` | `ValueConverter<TitleType, int>` |
| `StringListConverter` | `TEXT`/`VARCHAR` | `ValueConverter<List<string>, string>` (JSON serialized) |
| `TmdbContentRatingConverter` | `TEXT`/`VARCHAR` | `ValueConverter<ContentRating?, string>` |
| `TmdbProductionCountryConverter` | `TEXT`/`VARCHAR` | `ValueConverter<List<ProductionCountry>, string>` (JSON) |
| `TypeNameSerializationBinder`-based converters | Various | `ValueConverter<T, string>` or `byte[]` with Newtonsoft.Json serialization binder |

**Rationale**: EF Core's `ValueConverter` API is the direct equivalent of NHibernate's `IUserType`. The same serialization libraries (MessagePack, Newtonsoft.Json) can be reused, minimizing new dependencies.

**Alternatives considered**:
- Storing complex types as JSON columns — not viable because the existing schema stores these as TEXT/VARBINARY columns with specific serialization formats.
- Creating separate columns for each sub-field — would require schema changes, violating the compatibility constraint.

### 3. Schema Migration Strategy: Baseline Migration

**Decision**: The EF Core baseline migration creates schema only for fresh databases. Existing databases use a compatibility/baseline-registration path — they are schema-validated against the EF Core model and then marked as having the EF Core baseline applied, without creating already-existing tables.

**Approach**:
1. Create the EF Core `DbContext` with entity configurations that exactly match the current NHibernate-generated schema.
2. Run `dotnet ef migrations add InitialCreate` against an empty database to generate the creation migration.
3. Create a schema comparison utility (`SchemaComparer`) that compares the EF Core model against actual database schemas for SQLite, MariaDB, and SQL Server.
4. Create a baseline registration utility (`BaselineRegistration`) that:
   - For **fresh databases**: allows `InitialCreate` migration to run normally (creates all tables)
   - For **existing databases**: verifies schema matches EF Core model, then registers baseline in `__EFMigrationsHistory` as a no-op migration (does NOT create tables that already exist)
5. On first startup, the application checks whether the database has EF Core migration history. If not, it runs the schema comparison and baseline registration.

**Rationale**: This approach:
- Preserves existing data without transformation.
- Does NOT modify existing NHibernate-created databases (no duplicate table creation).
- Provides a clean migration history for future schema changes.
- Allows `dotnet ef migrations add` to work for all future changes.
- Explicitly supports all three backends (SQLite, MariaDB, SQL Server) with provider-specific schema comparison.

**Alternatives considered**:
- Applying `InitialCreate` to existing databases — risky because it may attempt to create tables that already exist, causing errors on some backends.
- Skipping EF Core Migrations entirely and using `context.Database.EnsureCreated()` — loses migration history and version tracking.
- Creating a separate migration tool — adds complexity and maintenance burden.
- Using `DatabaseFixes.cs` as the migration system — would duplicate the existing version-based system rather than consolidating.

### 4. Lazy Loading Audit Requirement

**Decision**: Confirm that NHibernate mappings use `Not.LazyLoad()` (verified in `VideoLocalMap.cs` and `AnimeSeriesMap.cs`), so EF Core lazy loading is NOT needed.

**Evidence found**:
- `VideoLocalMap.cs:13`: `Not.LazyLoad()`
- `AnimeSeriesMap.cs:12`: `Not.LazyLoad()`
- All mappings follow the same pattern (verified by grep of mapping files).

**Implication**: No lazy loading proxy setup is required. EF Core will use explicit loading (`Include`, `ThenInclude`) exclusively. The audit should confirm that all 60+ mapping files use `Not.LazyLoad()` or equivalent.

### 5. Transaction and Unit-of-Work Pattern

**Decision**: Preserve the existing repository-level transaction pattern. Each `BaseCachedRepository.Save()` and `Delete()` creates its own transaction via `session.BeginTransaction()` → `session.SaveOrUpdate()` → `transaction.Commit()`. The `SaveWithOpenTransaction()` methods support external transaction coordination.

**EF Core equivalent**:
- Replace `session.BeginTransaction()` with `context.Database.BeginTransaction()`.
- Replace `session.SaveOrUpdate()` with `context.Entry(entity).State = EntityState.Modified` or `context.Set<T>().AddOrUpdate()`.
- Replace `session.Delete()` with `context.Set<T>().Remove()`.
- Preserve `SaveWithOpenTransaction()` pattern using `IDbContextTransaction`.

**Rationale**: This preserves the existing transaction semantics and minimizes changes to repository consumers.

### 6. In-Memory Cache Replacement Strategy

**Decision**: Replace `NutzCode.InMemoryIndex.PocoCache<S, T>` with an EF Core-friendly in-memory cache. The cache loads all rows at startup into memory and uses `ReaderWriterLockSlim` for thread-safe reads.

**Approach**:
- Keep the same cache structure (PocoCache with indexes).
- Replace NHibernate `ISession.CreateCriteria<T>().List<T>()` with `context.Set<T>().AsNoTracking().ToList()`.
- Preserve `PopulateIndexes()` method for each repository to build typed indexes.
- The cache population happens once at application startup (in `DatabaseFixes` initialization).

**Rationale**: The cache is critical for performance. Removing it would cause a DB hit on every read operation, significantly degrading performance.

### 7. NLogInterceptor Replacement

**Decision**: Replace `NLogInterceptor : EmptyInterceptor` (which logs SQL via `OnPrepareStatement`) with EF Core's built-in logging.

**Approach**: Configure EF Core's `ILogger` sink to capture SQL commands at the `Debug` or `Trace` log level. This provides the same SQL logging capability without a custom interceptor.

**Rationale**: EF Core's logging is more flexible and configurable than NHibernate's interceptor pattern.

### 8. Database Fix System Migration

**Decision**: Migrate `DatabaseFixes.cs` migration logic from NHibernate `ISession` to EF Core `DbContext`. The version-based migration system (`Versions` table, `DatabaseCommand` objects, `ExecuteDatabaseFixes()`) should be preserved but use EF Core for data access.

**Approach**:
- The `VersionsRepository` (Direct) and `DatabaseFixes` class use NHibernate `ISession` for raw SQL queries and entity operations.
- Replace `ISession` with `DbContext` in `DatabaseFixes` and related classes.
- Keep the version tracking mechanism (`Versions` table with version/revision) — this is application logic, not ORM-specific.
- After EF Core coverage is complete, the `Versions` table can be used for EF Core migration history instead of `__EFMigrationsHistory`.

### 9. Test Strategy

**Decision**: Use SQLite for fast unit tests and integration tests. Add MariaDB and MSSQL integration tests using test containers or in-memory Docker instances.

**Approach**:
- `Shoko.Tests/`: Existing unit tests should pass with EF Core (they mock repositories, not the ORM directly).
- `Shoko.IntegrationTests/`: Add new integration tests that run against SQLite, MariaDB, and MSSQL.
- Use the existing `IDatabase.GetTestConnectionString()` pattern for test setup.
- Test categories: CRUD operations, complex queries, transactions, cascading deletes, concurrency, schema initialization, migration from NHibernate schema.

## Open Questions Resolved

| Question | Decision |
|----------|----------|
| Which MariaDB provider? | Pomelo.EntityFrameworkCore.MySql |
| How to handle NHibernate custom types? | EF Core ValueConverter with same serializers (count TBD from inventory) |
| EF Core Migrations or EnsureCreated? | Baseline migration for fresh DBs + schema comparison + baseline registration for existing DBs |
| Lazy loading needed? | No — all NHibernate mappings use Not.LazyLoad() |
| Transaction pattern? | Preserve repository-level transactions |
| In-memory cache? | Preserve PocoCache with EF Core queries |
| SQL logging? | EF Core ILogger sink |
| DatabaseFixes migration? | Preserve version system, use DbContext |
| Test strategy? | Provider-specific integration tests for SQLite, MariaDB, and SQL Server |
| DateOnlyConverter mapping? | `DateOnly` ↔ `int` (Unix epoch days), not `DateOnly` ↔ `DateTime` |
| Existing database migration? | Schema validation + baseline registration, NOT InitialCreate application |

### 10. EF Core Package Version Strategy

**Decision**: EF Core packages are intentionally pinned to 9.x during migration work, even though the project targets/runs on .NET 10.

**Rationale**:
- Stable `Pomelo.EntityFrameworkCore.MySql` support currently targets EF Core 9.x.
- MariaDB compatibility is a hard migration requirement.
- The current `NU1608` warnings are understood dependency-range mismatches caused by .NET 10 transitive resolution behavior (Pomelo 9.x declares `Microsoft.EntityFrameworkCore.Relational (>= 9.0.0 && <= 9.0.999)`, which .NET 10 resolves to 10.0.6 as a superset).
- This strategy is operationally acceptable during migration development.

**Re-evaluation triggers**:
- Entity configuration migration stabilizes
- Provider integration tests exist
- Pomelo stable EF Core 10 support is available

**Do not auto-upgrade EF Core packages during migration work.**
