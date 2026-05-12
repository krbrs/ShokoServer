# Feature Specification: Database Client Migration

**Feature Branch**: `001-database-client-migration`  
**Created**: 2026-05-06  
**Status**: Draft  
**Input**: User description: "Migrate the application's database access layer from NHibernate to Entity Framework Core while preserving support for SQLite, MariaDB, and Microsoft SQL Server."

## User Scenarios & Testing

### User Story 1 - Existing User with Local Data (Priority: P1)

An existing Shoko Server installation with imported anime files and watch history starts after the migration. All their data — series, episodes, files, watch records, user settings, and plugin state — remains intact and accessible without any manual intervention.

**Why this priority**: This is the highest-risk scenario. If existing data becomes inaccessible or corrupted after migration, users lose trust in the application and may abandon it. The migration must be transparent to end users.

**Independent Test**: Install the migrated application against an existing database file (SQLite) or connection string (MariaDB/SQL Server) and verify all data is accessible through the API without errors.

**Acceptance Scenarios**:

1. **Given** an existing Shoko Server installation with imported content and watch history, **When** the user starts the application and accesses the API, **Then** all series, episodes, files, user data, and settings are returned correctly with no data loss.
2. **Given** an existing Shoko Server installation with content imported across multiple managed folders, **When** the user scans for new files, **Then** the file import pipeline works correctly and new files are processed without errors.
3. **Given** an existing Shoko Server installation with custom filter presets and user overrides, **When** the user accesses the API, **Then** all customizations are preserved and functional.

---

### User Story 2 - New Installation Setup (Priority: P2)

A new user installs Shoko Server for the first time. The application initializes the database, creates all required tables, and is ready to accept imports. The user can configure the database backend (SQLite, MariaDB, or SQL Server) through the existing configuration system.

**Why this priority**: New installations must work flawlessly for the application to be usable. This is the baseline functionality that every user encounters.

**Independent Test**: Perform a fresh install for each supported database backend and verify the application runs correctly with no pre-existing database.

**Acceptance Scenarios**:

1. **Given** a fresh installation with no existing database, **When** the user starts the application, **Then** the database is automatically created with all required tables and the application is fully operational.
2. **Given** a fresh installation configured for MariaDB or SQL Server, **When** the user provides valid connection credentials, **Then** the application connects successfully and initializes the database schema.
3. **Given** a fresh installation configured for SQLite, **When** the user starts the application, **Then** a local SQLite database file is created in the configured directory.

---

### User Story 3 - Administrator Configuring Database Backend (Priority: P3)

An administrator manages an existing Shoko Server deployment and needs to switch the database backend (e.g., from SQLite to MariaDB for multi-machine access). The migration tool or process handles schema conversion and data transfer without data loss.

**Why this priority**: Backend switching is an advanced but supported use case. It enables scaling deployments from single-machine to shared hosting scenarios.

**Independent Test**: Execute a backend migration from SQLite to MariaDB (or vice versa) and verify all data transfers correctly.

**Acceptance Scenarios**:

1. **Given** an existing SQLite database with data, **When** the administrator reconfigures the application to use MariaDB, **Then** all data is transferred to the new backend and the application continues operating normally.
2. **Given** an existing MariaDB or SQL Server database, **When** the administrator reconfigures to use SQLite, **Then** all data is exported and imported into the local SQLite database without loss.

---

### Edge Cases

- What happens when a database migration script encounters a schema element that already exists (e.g., re-running migration on a partially-migrated database)?
- How does the system handle concurrent access during a backend migration?
- What happens if a migration fails mid-way — is there a rollback mechanism? (Answer: Manual backup + restore from backup is the documented rollback procedure.)
- How does the system handle existing databases with custom modifications to schema elements?
- What happens when a database backend is unavailable during startup — does the application fail gracefully?
- What happens if the user attempts to migrate without first creating a backup? (Answer: Migration tool refuses to proceed until backup is confirmed.)
- How does the system handle the transition period where NHibernate and EF Core coexist during phased dependency removal?

## Clarifications

### Session 2026-05-06

- Q: Who owns schema evolution after migration? → A: EF Core migrations own schema evolution; existing NHibernate schema becomes the EF Core baseline via a starting migration.
- Q: Should a full inventory of NHibernate advanced features be conducted before implementation? → A: Yes — inventory all NHibernate-specific features (custom types, interceptors, filters, conventions, second-level caching) before implementation begins.
- Q: Should repository/unit-of-work abstractions remain stable or be redesigned? → A: Keep repository interfaces stable. Existing repository interfaces from `Shoko.Abstractions` remain unchanged; EF Core is used internally behind the same contracts.
- Q: Should EF Core lazy loading be enabled or should loading be explicit only? → A: Explicit loading only. No lazy loading proxies. All related data loaded via `Include`, `ThenInclude`, or explicit loading. Requires audit of existing lazy loading usage.
- Q: What is the production migration and rollback approach? → A: Require explicit backup before migration + documented rollback procedure. Migration tool refuses to proceed without confirming a backup exists.

## Requirements

### Functional Requirements

- **FR-001**: The application MUST persist all domain data (series, episodes, files, user data, settings, plugin state, cross-references) using EF Core instead of NHibernate.
- **FR-002**: The application MUST support SQLite, MariaDB, and Microsoft SQL Server as database backends, with provider selection configurable via the existing settings system.
- **FR-003**: The application MUST read and write all existing data in the current database schemas without requiring manual data transformation by the user.
- **FR-004**: The application MUST provide a database migration mechanism that converts existing NHibernate-persisted databases to EF Core-compatible schemas automatically on first startup.
- **FR-005**: The application MUST preserve all existing repository patterns and service interfaces from `Shoko.Abstractions` so that higher-level application code requires minimal changes. EF Core is used internally behind the same contracts.
- **FR-006**: The application MUST support transaction semantics equivalent to existing NHibernate behavior, including commit, rollback, and isolation level handling.
- **FR-007**: The application MUST support eager-loading and explicit loading patterns used by existing application code. Lazy loading via proxies is NOT supported — all related data must be loaded via `Include`, `ThenInclude`, or explicit loading. An audit of existing lazy loading usage is required before implementation.
- **FR-008**: The application MUST initialize the database schema automatically on first run for each supported backend.
- **FR-009**: The application MUST be able to select and configure the correct EF Core provider based on the user's database configuration setting.
- **FR-010**: The application MUST verify database connectivity and schema compatibility on startup and report clear errors if the database is unreachable or incompatible.
- **FR-011**: The application MUST include automated tests that verify persistence behavior against SQLite, MariaDB, and Microsoft SQL Server.
- **FR-012**: The application MUST document the migration process for existing installations, including step-by-step instructions and rollback procedures.

### Key Entities

- **AnimeSeries**: Core series entity with title, description, overrides, language preferences, TMDB match flags, and missing episode counts. Maps to `AnimeSeries` table.
- **AnimeEpisode**: Episode entity wrapping an AniDB episode with Shoko-specific state (hidden flag, title override). Maps to `AnimeEpisode` table.
- **VideoLocal**: File entity identified by ED2K hash and file size, containing hash digests and MediaInfo. Maps to `VideoLocal` table.
- **VideoLocal_Place**: Location entity linking a file to a managed folder and relative path. Maps to `VideoLocal_Place` table.
- **ShokoManagedFolder**: Root directory entity with watched/drop flags. Maps to `ShokoManagedFolder` table.
- **StoredReleaseInfo**: Release provider response cache with ED2K hash, file size, codec flags, and episode cross-references. Maps to `StoredReleaseInfo` table.
- **JMMUser**: User entity with role, device name, and auth tokens. Maps to `JMMUser` table.
- **CrossRef_File_Episode**: Join table linking files to episodes via ED2K hash. Maps to `CrossRef_File_Episode` table.
- **CrossRef_AniDB_TMDB_Show**: Join table linking AniDB series to TMDB shows. Maps to `CrossRef_AniDB_TMDB_Show` table.
- **AniDB_Anime**: Cached AniDB metadata for a series. Maps to `AniDB_Anime` table.
- **AniDB_Episode**: Cached AniDB episode data. Maps to `AniDB_Episode` table.
- **FilterPreset**: User-defined filter expressions and sorting rules. Maps to `FilterPreset` table.
- **AuthTokens**: API authentication tokens per user. Maps to `AuthTokens` table.
- **ScheduledUpdate**: Tracking entity for periodic background task timestamps. Maps to `ScheduledUpdate` table.
- **AniDB_NotifyQueue**: Staging table for raw AniDB notification IDs. Maps to `AniDB_NotifyQueue` table.
- **AniDB_Message**: Stored message body from AniDB notifications. Maps to `AniDB_Message` table.

## Success Criteria

### Measurable Outcomes

- **SC-001**: All existing database-backed features continue to function correctly after migration with zero data loss, verified by running the full API test suite against each supported backend.
- **SC-002**: A fresh installation initializes the database and becomes operational within 30 seconds on SQLite and within 60 seconds on MariaDB/SQL Server on standard hardware.
- **SC-003**: An existing SQLite database with 10,000+ files migrates to EF Core schema within 5 minutes without manual intervention.
- **SC-004**: Query performance for the top 20 most frequently executed database queries does not degrade by more than 10% compared to the NHibernate baseline.
- **SC-005**: 100% of existing unit tests pass after migration, or are updated with documented justification for any intentional behavior changes.
- **SC-006**: Migration documentation enables a first-time operator to perform a backend migration (SQLite to MariaDB or vice versa) without requiring developer assistance.

## Assumptions

- The existing database schemas (as defined by NHibernate mappings) will serve as the baseline for the initial EF Core migration. All future schema changes will be managed through EF Core migrations.
- The `RepoFactory` static accessor pattern will be preserved during migration to minimize changes to existing code, though new code should prefer DI.
- NHibernate dependencies will be removed only after EF Core coverage is complete and all tests pass — a phased approach.
- The existing configuration system (`ServerSettings` persisted to `settings-server.json`) will be extended to support EF Core provider selection without requiring new configuration files.
- The existing plugin interface (`Shoko.Abstractions`) does not change as part of this migration; plugins continue to depend on the same domain interfaces.
- MediaInfo data, currently serialized via MessagePack in NHibernate, will continue to use the same MessagePack serialization within EF Core's property storage. A custom EF Core value converter will replace the NHibernate MessagePack converter.
- The Quartz scheduler's custom in-memory job store and the existing database lock mechanisms will continue to function with the EF Core replacement.
- Existing database indexes and constraints will be preserved in the EF Core schema to maintain query performance characteristics.
- Before implementation begins, a full inventory of NHibernate-specific features (custom user types, interceptors, filters, naming conventions, second-level caching) will be conducted to identify all features requiring EF Core equivalents.
- Repository interfaces from `Shoko.Abstractions` remain stable — EF Core replaces NHibernate internally without changing the public repository contract.
- Lazy loading via proxies is not supported. All loading is explicit (`Include`, `ThenInclude`, explicit loading). Existing lazy loading usage will be audited and converted.
- Production migration requires an explicit manual backup before the migration proceeds. The migration tool will refuse to run without confirming a backup exists. A documented rollback procedure (restore from backup) is required.
