# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet build Shoko.Server.sln
dotnet test Shoko.Tests/Shoko.Tests.csproj --filter "FullyQualifiedName~ClassName.Method"
dotnet test Shoko.IntegrationTests/Shoko.IntegrationTests.csproj
```

Target framework: `.NET 10.0`. Configurations: `Debug`, `Release`, `ApiLogging`, `Benchmarks`.

## Code Style

`.editorconfig` with ReSharper enforcement:
- Line length: 160 characters
- Modifier order: `private, protected, public, internal, sealed, new, override, virtual, abstract, static, extern, async, unsafe, volatile, readonly, required, file`
- `var` when type is apparent; braces on new lines (`csharp_new_line_before_open_brace = all`)

## Architecture

### Project Layout

- **`Shoko.Abstractions`** — NuGet package for plugin authors. Defines `IPlugin`, `IShoko`, and all service/metadata/video/user interfaces. No implementation logic lives here. Update this before `Shoko.Server` when changing public contracts.
- **`Shoko.Server`** — All implementation: API, database, repositories, services, scheduling, providers, models.
- **`Shoko.CLI`** — Headless server entry point. Hosts `SystemService` as `IHostedService`.
- **`Shoko.TrayService`** — Windows tray app (Avalonia) embedding the server.
- **`Plugins/`** — Built-in plugins (`ReleaseExporter`, `RelocationPlus`, `OfflineImporter`) built as separate projects and loaded at runtime.

### Startup Sequence

`Program.cs` → `SystemService` constructor (NLog, `PluginManager`, `ConfigurationService`, `SettingsProvider`) → `SystemService.StartAsync()` (DB migrations via `DatabaseFixes`, build `IHost`, init Quartz scheduler) → ASP.NET Core listens on port 8111.

Global service container is exposed via `Utils.ServiceContainer = _webHost.Services` for legacy code that predates DI.

### API Pipeline

**Middleware order** (configured in `Shoko.Server/API/APIExtensions.cs`, `UseAPI()`):

1. Sentry exception handling (if not opted out)
2. `DeveloperExceptionPage` (DEBUG / `AlwaysUseDeveloperExceptions`)
3. Static files — WebUI served via `WebUiFileProvider` at configurable path
4. Swagger UI (if enabled)
5. `UseRouting`
6. `UseAuthentication` — custom "ShokoServer" scheme
7. `UseAuthorization` — policies: `"admin"` (IsAdmin == 1), `"init"` (setup user only)
8. `UseEndpoints` — SignalR hubs registered here: `/signalr/logging`, `/signalr/aggregate`
9. Plugin middleware registration
10. `UseCors` (any origin/method/header)
11. `UseMvc` (legacy, `EnableEndpointRouting = false`)

**Global action filters** (registered on all MVC controllers):
- `DatabaseBlockedFilter` — returns 400 if DB is blocked, exempted via `[DatabaseBlockedExempt]`
- `ServerNotRunningFilter` — returns 503 until server is started, exempted via `[InitFriendly]`

**Action constraints**:
- `RedirectConstraint` — redirects root `/` to WebUI public path if configured

**Authentication** (`Shoko.Server/API/Authentication/`):
- `CustomAuthHandler` extracts API key from: `apikey` header, `apikey` query param, `Bearer` token (SignalR), or `access_token` query param
- Validates against `AuthTokensRepository`; builds `ClaimsPrincipal` with user ID, role, device name
- During first-run setup, `InitUser` (synthetic admin) is used — no real auth required
- No sessions; every request is authenticated by API key

**API versioning**: `v0` (Plex webhooks + legacy), `v1`/`v2` (legacy REST, can be kill-switched), `v3` (current, all new endpoints). Version resolved from query string, `api-version` header, or custom `ShokoApiReader`. `ApiVersionControllerFeatureProvider` excludes disabled versions at startup via `Web.DisabledAPIVersions`.

### SignalR (Real-time Events)

Two hubs, both require `[Authorize]`:

- **`LoggingHub`** (`/signalr/logging`) — streams buffered server logs to connecting clients
- **`AggregateHub`** (`/signalr/aggregate`) — subscription model; clients call `feed.join_single` / `feed.join_many` etc. to subscribe to event categories

Event emitters bridge internal domain events to SignalR: `AnidbEventEmitter`, `AvdumpEventEmitter`, `ConfigurationEventEmitter`, `FileEventEmitter`, `ManagedFolderEventEmitter`, `MetadataEventEmitter`, `NetworkEventEmitter`, `QueueEventEmitter`, `ReleaseEventEmitter`, `UserDataEventEmitter`, `UserEventEmitter`.

### Model Layers and Separation

Three distinct model layers; **do not mix them**.

**1. Persistence models** (`Shoko.Server/Models/`)
NHibernate-mapped entities. Organized by source:
- `Shoko.Server.Models.Shoko` — core domain: `AnimeSeries`, `AnimeGroup`, `AnimeEpisode`, `VideoLocal`, `JMMUser`, `FilterPreset`, etc.
- `Shoko.Server.Models.AniDB` — AniDB metadata cache: `AniDB_Anime`, `AniDB_Episode`, `AniDB_Character`, `AniDB_Creator`, `AniDB_Tag`, etc.
- `Shoko.Server.Models.TMDB` — TMDB metadata cache: `TMDB_Show`, `TMDB_Movie`, `TMDB_Episode`, `TMDB_Image`, etc.
- `Shoko.Server.Models.CrossReference` — cross-reference tables linking providers (AniDB↔TMDB, AniDB↔MAL, AniDB↔Trakt)
- `Shoko.Server.Models.Release` — release/video file associations

NHibernate mappings live in `Shoko.Server/Mappings/` as `*Map.cs` files. Schemas should be maintained to match, as they will be migrated to Entity Framework Code-First in a future version.

**2. API response DTOs** (`Shoko.Server/API/v*/Models/`)
Never persisted; built from persistence models in controllers/services.
- `v1/Models/` — legacy `CL_*` contract classes (50+ files), kept for backward compatibility
- `v3/Models/Shoko/` — modern response models (`Series`, `Episode`, `Group`, `File`, `User`, …) extending `BaseModel`
- `v3/Models/AniDB/` and `v3/Models/TMDB/` — provider-specific response shapes
- `v3/Models/Common/` — shared types (`Images`, `Rating`, `Tag`, `Title`, etc.)

**3. Abstractions interfaces** (`Shoko.Abstractions/`)
`IShokoSeries`, `IShokoEpisode`, `IVideo`, `IUser`, etc. — implemented by persistence models, consumed by plugins and services. Plugin code should depend only on these, never on concrete `Shoko.Server` types.

### Repository Pattern

Two variants in `Shoko.Server/Repositories/`:
- **`Cached/`** — `BaseCachedRepository<T, S>` loads all rows at startup into a `PocoCache` (from `NutzCode.InMemoryIndex`). Reads are `ReaderWriterLockSlim`-protected. Each repository builds typed indexes via `PopulateIndexes()` (e.g., `_animeIDs = Cache.CreateIndex(a => a.AnimeID)`). All writes go to DB then invalidate/update the in-memory cache. Use for hot data.
- **`Direct/`** — no cache; hits DB on every call. Use for infrequently accessed or large data.

Always prefer a cached repository over a direct one when both exist for the same entity.

### Scheduling

Quartz.NET with a custom in-memory `ThreadPooledJobStore` (`Shoko.Server/Scheduling/`). Jobs in `Jobs/` are DI-resolved via `JobFactory`. `QueueStateEventHandler` fires domain events (job added/started/completed) consumed by `QueueEventEmitter` → SignalR clients. `DatabaseLocks/` provides named locks to prevent concurrent conflicting DB operations.

### Plugin System

`PluginManager` scans the `/plugins/` directory, loads assemblies, finds `IPlugin` implementations via reflection, and registers them with the DI container. Plugins call `IPluginManager.RegisterPlugins(services)` to add their own services. `CorePlugin` is the built-in plugin that ships with the server.

### Database Migrations

All schema migrations and data fixups are in `Shoko.Server/Databases/DatabaseFixes.cs`. Append new migrations; never modify existing ones. `Versions` class tracks the applied migration level. Supported backends: SQLite (default), MySQL/MariaDB, SQL Server — selected via `DatabaseFactory`.

## Domain Model Relationships

### File → Location

**`VideoLocal`** is the canonical record for a unique file, identified by its ED2K hash + file size. It holds hashes, `MediaInfo`, import date, and AniDB MyList ID. It does not store a path.

**`VideoLocal_Place`** stores where a `VideoLocal` physically lives: a `ManagedFolderID` + `RelativePath`. One `VideoLocal` can have multiple places (the same file duplicated across folders). The absolute path is computed at runtime as `folder.Path + place.RelativePath`.

**`ShokoManagedFolder`** (formerly `ImportFolder`) is a root directory Shoko monitors. Each folder has `IsWatched`, `IsDropSource`, and `IsDropDestination` flags used by the file relocation system.

```
ShokoManagedFolder (1) ──< VideoLocal_Place >── (1) VideoLocal
                                                      │
                                              VideoLocal_HashDigest (CRC32/MD5/SHA1)
```

### File → Episode

**`CrossRef_File_Episode`** is the join between a file and an AniDB episode, keyed by ED2K hash + file size (not VideoLocalID). It stores:
- `Percentage` — what fraction of the episode this file covers (100 for a single-episode file, 50 for a 2-part release)
- `EpisodeOrder` — which part this file is in a multi-file episode

A single file can map to multiple episodes (e.g., a combined OVA file). Multiple files can map to the same episode (alternative releases). The `IsManuallyLinked` flag distinguishes user-created links from AniDB-sourced ones.

**`StoredReleaseInfo`** hangs off `CrossRef_File_Episode` and holds the release group, video/audio codec, language, and quality information returned by AniDB's FILE command.

```
VideoLocal (hash+size) ──< CrossRef_File_Episode >── AniDB_Episode
                                │
                          StoredReleaseInfo
```

### Episode → Series → Group

**`AniDB_Episode`** is the raw AniDB cache (episode number, type, air date, synopsis, rating). It has no Shoko-specific data.

**`AnimeEpisode`** wraps one `AniDB_Episode` and adds Shoko state: hidden flag, title override, and the FK to `AnimeSeries`. All user watch data is stored in `AnimeEpisode_User`.

**`AniDB_Anime`** is the raw AniDB cache for a series (titles, synopsis, ratings, episode counts, external IDs for streaming services). One `AnimeSeries` maps to exactly one `AniDB_Anime` via `AniDB_ID`.

**`AnimeSeries`** is Shoko's local wrapper around an AniDB anime. Adds name/description overrides, language preferences, TMDB auto-match flags, and missing episode counts. All user ratings live in `AnimeSeries_User`.

**`AnimeGroup`** is a container for series, supporting arbitrary nesting (groups within groups via `AnimeGroupParentID`). Groups can be auto-named from their main series or manually named. `AllSeries` and `AllChildren` are recursive traversals.

```
AnimeGroup (self-referential parent ──< children)
  └──< AnimeSeries (1:1 AniDB_Anime)
         └──< AnimeEpisode (1:1 AniDB_Episode)
                └──< CrossRef_File_Episode >── VideoLocal
```

### Series/Episode → TMDB

AniDB and TMDB entities are connected through cross-reference tables, not direct FKs:

- `CrossRef_AniDB_TMDB_Show` — `AnimeSeries` ↔ `TMDB_Show`
- `CrossRef_AniDB_TMDB_Movie` — `AnimeSeries` or `AnimeEpisode` ↔ `TMDB_Movie` (OVAs/movies often link at episode level)
- `CrossRef_AniDB_TMDB_Episode` — `AnimeEpisode` ↔ `TMDB_Episode`

One anime can match multiple TMDB shows (e.g., split-cour series on TMDB) and one TMDB show can match multiple anime. TMDB models (`TMDB_Show`, `TMDB_Movie`, `TMDB_Episode`, `TMDB_Season`, `TMDB_Image`) are read-only caches of TMDB API data, structured identically to the TMDB response schema.

## Import Pipeline

### Job Chain

When a file appears, jobs execute in sequence — each job enqueues the next upon completion:

```
File appears on disk
        │
        ▼
ScanFolderJob  (Shoko.Server/Scheduling/Jobs/Shoko/ScanFolderJob.cs)
  Walks managed folder, creates VideoLocal + VideoLocal_Place stubs for new files
        │
        ▼
HashFileJob  (Shoko.Server/Scheduling/Jobs/Shoko/HashFileJob.cs)
  Computes ED2K (primary), MD5, SHA1, CRC32 via IVideoHashingService
  Stores hashes, populates VideoLocal.Hash
        │
        ▼
ProcessFileJob  (Shoko.Server/Scheduling/Jobs/Shoko/ProcessFileJob.cs)
  Sends FILE command to AniDB UDP API (hash + size)
  Creates CrossRef_File_Episode + StoredReleaseInfo
  Adds file to AniDB MyList (unless skipped)
        │  [on new AnimeID]
        ▼
GetAniDBAnimeJob  (Shoko.Server/Scheduling/Jobs/AniDB/GetAniDBAnimeJob.cs)
  Fetches full AniDB_Anime + all AniDB_Episode records via AniDB HTTP API
  Creates AnimeSeries + AnimeGroup if they don't exist (CreateSeriesEntry=true)
        │  [unless SkipTmdbUpdate]
        ▼
SearchTmdbJob  (Shoko.Server/Scheduling/Jobs/TMDB/SearchTmdbJob.cs)
  Auto-searches TMDB for matching show/movie by title + episode count
  Creates CrossRef_AniDB_TMDB_Show / CrossRef_AniDB_TMDB_Movie
        │
        ▼
UpdateTmdbShowJob / UpdateTmdbMovieJob  (Shoko.Server/Scheduling/Jobs/TMDB/)
  Fetches TMDB_Show/Movie/Episode/Season/Image records
  Fetches titles + overviews in all configured languages
        │
        ▼
Image download jobs  (DownloadAniDBImageJob, DownloadTmdbImageJob)
  Download poster/backdrop/thumbnail files to local image cache
```

### Orchestration Pattern

Jobs do not use a central orchestrator. Each job enqueues its successor directly via `IJobFactory` / `IScheduler`. `ProcessFileJob` is the pivot: it reads the `AnimeID` from the AniDB response and checks whether `AniDB_Anime` already exists before deciding to enqueue `GetAniDBAnimeJob`.

**`ImportJob`** (`Shoko.Server/Scheduling/Jobs/Actions/ImportJob.cs`) is a periodic sweep that catches anything the live pipeline missed: it calls `ActionService.ScheduleMissingAnidbAnimeForFiles()` to queue `GetAniDBAnimeJob` for any file whose anime was never fetched, and `IVideoService.ScheduleScanForManagedFolders()` to rescan all watched folders.

### Intermediate Cache Models

Several models exist solely to avoid redundant I/O or external API calls. Jobs check these before making outbound requests.

**`FileNameHash`** (`Models/Shoko/FileNameHash.cs`) — maps `FileName + FileSize → ED2K hash`. Written by `VideoHashingService.SaveFileNameHash()` and `VideoRelocationService` after a file is successfully hashed. Read by `AnidbReleaseProvider` as a last-resort local lookup when checking for creditless/variant files before going to AniDB.

**`VideoLocal_HashDigest`** (`Models/Shoko/VideoLocal_HashDigest.cs`) — stores all computed hash types (ED2K, CRC32, MD5, SHA1) for a `VideoLocal` as `Type + Value` rows. Written by `VideoHashingService` during `HashFileJob`. Read when displaying or cross-referencing file hashes without recomputing.

**`StoredReleaseInfo`** (`Models/Release/StoredReleaseInfo.cs`) — caches the full AniDB FILE command response: ED2K + FileSize, provider ID, release URI, source (BluRay/Web/etc.), codec flags (`IsCensored`, `IsCreditless`, `IsChaptered`), version, and cross-references to anime/episodes. Written by `IVideoReleaseService.FindReleaseForVideo()` inside `ProcessFileJob`. `ProcessFileJob` calls `GetCurrentReleaseForVideo()` first — if a `StoredReleaseInfo` already exists for the hash, the AniDB UDP call is skipped entirely. Queried by the API via `GetByEd2kAndFileSize()`, `GetByReleaseURI()`, `GetByAnidbEpisodeID()`.

**`AniDB_AnimeUpdate`** (`Models/AniDB/AniDB_AnimeUpdate.cs`) — one row per `AnimeID`, storing only `UpdatedAt`. Written by `RequestGetAnime.UpdateAccessTime()` after every successful AniDB HTTP response. Read by the same method to decide whether the local `AniDB_Anime` record is stale enough to warrant a new fetch. `GetAniDBAnimeJob` respects `IgnoreTimeCheck` to force a refresh past this gate.

**`AniDB_GroupStatus`** (`Models/AniDB/AniDB_GroupStatus.cs`) — caches AniDB GROUPSTATUS UDP responses: release group name, completion state, episode range, rating. Written by `GetAniDBReleaseGroupStatusJob` after a UDP `RequestReleaseGroupStatus` call. `GetAniDBReleaseGroupStatusJob.ShouldSkip()` bypasses the fetch entirely if the anime ended more than 50 days ago.

**`AniDB_NotifyQueue`** + **`AniDB_Message`** — `AniDB_NotifyQueue` (`Models/AniDB/`) is a staging table for raw AniDB notification IDs (type + ID) written by `GetAniDBNotifyJob`. `AniDB_Message` stores the fully fetched message body (sender, title, body, `FileMoved`/`FileMoveHandled` flags). `AcknowledgeAniDBNotifyJob` and `ProcessFileMovedMessageJob` drain these two tables in sequence.

**`ScheduledUpdate`** (`Models/Internal/ScheduledUpdate.cs`) — tracks `LastUpdate` timestamps for periodic background tasks (one row per `UpdateType`). Written after each scheduled job completes. Read at job start to determine whether enough time has elapsed to run again.

### Unrecognized Files

If AniDB's FILE command returns no match, `ProcessFileJob` marks the file as unrecognized. It will not automatically retry until the user either manually links the file to an episode via the API, or triggers an AVDump (`AvdumpFileJob`) to submit the file's hash to AniDB for future recognition.

### Concurrency Limits

| Job | Concurrency | Reason |
|-----|-------------|--------|
| `HashFileJob` | 2 | I/O bound |
| `ProcessFileJob` | 4 | AniDB UDP rate limit |
| `GetAniDBAnimeJob` | 1 per anime ID (lock) | Prevent duplicate fetches |
| `SearchTmdbJob` / `UpdateTmdbShowJob` | 8–24 | TMDB allows higher throughput |
