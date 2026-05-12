# Implementation Plan: Database Client Migration

**Branch**: `001-database-client-migration` | **Date**: 2026-05-06 | **Spec**: [spec.md](../spec.md)
**Input**: Feature specification from `/specs/001-database-client-migration/spec.md`

## Summary

Replace NHibernate (5.6.0) + FluentNHibernate (3.4.1) persistence layer with Entity Framework Core across all three supported database backends (SQLite, MariaDB, Microsoft SQL Server). The migration preserves all existing repository interfaces, table/column names, relationships, indexes, and constraints. NHibernate dependencies are removed only after EF Core coverage is complete and verified. The approach is incremental: EF Core infrastructure is introduced first, then mappings are ported entity-by-entity, then data access is migrated, then NHibernate is removed.

## Technical Context

**Language/Version**: C# 13 / .NET 10.0  
**Primary Dependencies**: Microsoft.Extensions.DependencyInjection (DI), Quartz (scheduling), NLog (logging), Newtonsoft.Json (JSON serialization), MessagePack (binary serialization), NutzCode.InMemoryIndex (in-memory caching)  
**Storage**: SQLite (default), MariaDB (via MySqlConnector), Microsoft SQL Server — all via NHibernate currently  
**Testing**: xUnit 2.7.0, Moq 4.20.7, coverlet 6.0.2, Microsoft.NET.Test.Sdk 17.9.0 — unit tests in `Shoko.Tests/`, integration tests in `Shoko.IntegrationTests/`  
**Target Platform**: Windows, Linux, macOS (cross-platform server application via Avalonia tray app and headless CLI)  
**Project Type**: Server application (ASP.NET Core web API with background services)  
**Performance Goals**: Top 20 queries must not degrade >10% vs NHibernate baseline (from spec SC-004)  
**Constraints**: 
- Table names, column names, keys, indexes, relationships, nullability, enum storage, value conversions, cascade behavior, and constraints must match the existing NHibernate-generated schema exactly.
- Repository interfaces from `Shoko.Abstractions` must remain stable.
- No lazy loading proxies — all loading must be explicit.
- No provider-specific code in domain or application layers.
- SQLite behavior is NOT representative of MariaDB or MSSQL — provider-specific behavior must be tested against each.
- Do not remove NHibernate until EF Core tests pass for SQLite, MariaDB, and MSSQL.

**Scale/Scope**: 60+ NHibernate mapping files, 16+ entity types across 7 model namespaces (Shoko, AniDB, TMDB, CrossReference, Release, Trakt, Internal), ~50 repository classes (cached and direct), ~20 custom NHibernate IUserType converters (exact count TBD from inventory), existing database schema with version-based migration system in `DatabaseFixes.cs`.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The constitution file (`.specify/memory/constitution.md`) is a template with placeholder values and has not been populated with project-specific principles. This migration does not conflict with any established constitution rules since none are defined. The migration adheres to the project's existing code conventions (`AGENTS.md`): `var` preferred everywhere, 160-char line length, ReSharper-style braces, and DI-first approach (preserving `RepoFactory` static accessor for compatibility).

**Gates**: No constitution violations to justify. Proceeding.

## Project Structure

### Documentation (this feature)

```text
specs/001-database-client-migration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── checklists/
│   └── requirements.md  # Specification quality checklist
└── tasks.md             # Phase 2 output (created by /speckit.tasks)
```

### Source Code (repository root)

```text
Shoko.Server/
├── Data/                            # NEW: EF Core infrastructure
│   ├── ShokoDbContext.cs            # EF Core DbContext
│   ├── Configurations/              # EF Core IEntityTypeConfiguration<T> classes
│   ├── Converters/                  # EF Core ValueConverter classes
│   ├── Design/                      # Design-time support (IDesignTimeDbContextFactory)
│   ├── Migrations/                  # EF Core migration files
│   └── SchemaComparison/            # Schema validation utilities
├── Databases/
│   ├── DatabaseFactory.cs           # Current: creates ISessionFactory per backend
│   ├── IDatabase.cs                 # Current: ISessionFactory + schema management interface
│   ├── BaseDatabase.cs              # Current: abstract base with migration logic
│   ├── SQLite.cs / MySQL.cs / SQLServer.cs  # Current: backend-specific implementations
│   ├── DatabaseFixes.cs             # Current: version-based migration system
│   ├── DatabaseCommand.cs           # Current: migration command definitions
│   └── NHIbernate/                  # Current: NHibernate custom IUserType converters (TBD count from inventory)
│       ├── MessagePackConverter.cs
│       ├── FilterExpressionConverter.cs
│       ├── DateOnlyConverter.cs
│       └── ... (TBD more)
├── Mappings/                        # Current: FluentNHibernate ClassMap<T> files (TBD count from inventory)
│   ├── VideoLocalMap.cs
│   ├── AnimeSeriesMap.cs
│   ├── AniDB/
│   ├── TMDB/
│   └── CrossReference/
├── Repositories/
│   ├── BaseCachedRepository.cs      # Current: PocoCache + ReaderWriterLockSlim + NHibernate ISession
│   ├── BaseDirectRepository.cs      # Current: direct NHibernate access
│   ├── Cached/                      # ~30 cached repositories
│   └── Direct/                      # ~20 direct repositories
├── Models/                          # Current: NHibernate-mapped entities
│   ├── Shoko/
│   ├── AniDB/
│   ├── TMDB/
│   ├── CrossReference/
│   ├── Release/
│   ├── Trakt/
│   ├── Image/
│   ├── Internal/
│   └── Legacy/
├── API/                             # Controllers (no changes expected)
├── Services/                        # Business logic (no changes expected)
└── Shoko.Abstractions/              # Plugin interfaces (no changes expected)

Shoko.Tests/                         # Unit tests — must pass after migration
Shoko.IntegrationTests/              # Integration tests — must be extended for EF Core
```

**Structure Decision**: The existing repository structure is preserved. EF Core infrastructure lives in `Shoko.Server/Data/` (with `Configurations/`, `Converters/`, `Design/`, `Migrations/` subdirectories) separate from the current NHibernate infrastructure in `Shoko.Server/Databases/`. NHibernate code is not deleted until Phase 7 (final cleanup). The `Data/Configurations/` directory holds EF Core `IEntityTypeConfiguration<T>` classes. The `Repositories/` directory's cached/direct structure is preserved with EF Core implementations.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Dual ORM coexistence (NHibernate + EF Core) | Required for incremental migration — NHibernate cannot be removed until EF Core coverage is complete | Big-bang replacement would be higher risk: no rollback path, all queries must work simultaneously |
| Preserving RepoFactory static accessor | Existing code (60+ files) uses `RepoFactory` — changing it would touch every repository consumer | DI-only approach would require changes across all services, jobs, and tasks |
| In-memory PocoCache preservation | `NutzCode.InMemoryIndex` caches all rows at startup for hot data — replacing it would change performance characteristics | Removing cache would cause DB hit on every read, degrading performance significantly |
| 60+ entity mappings ported individually | Each entity has unique relationships, custom types, and constraints that must be verified per-provider | Bulk port without verification risks schema incompatibility across backends |
