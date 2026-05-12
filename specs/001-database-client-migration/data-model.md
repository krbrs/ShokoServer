# Data Model: Database Client Migration

## Entity Inventory

### Core Shoko Entities (Shoko.Server.Models.Shoko)

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `AnimeSeries` | `AnimeSeries` | `int` (AnimeSeriesID) | Identity | `AnimeGroup`, `AnimeEpisodes` (ICollection), `AnimeSeriesUsers` (ICollection), `AniDB_Anime` (single) |
| `AnimeEpisode` | `AnimeEpisode` | `int` (AnimeEpisodeID) | Identity | `AnimeSeries`, `AniDB_Episode`, `VideoLocals` (ICollection), `AnimeEpisodeUsers` (ICollection) |
| `VideoLocal` | `VideoLocal` | `int` (VideoLocalID) | Identity | `VideoLocalPlaces` (ICollection), `VideoLocalUsers` (ICollection), `VideoLocalHashDigests` (ICollection) |
| `VideoLocal_Place` | `VideoLocal_Place` | `int` (ID → `VideoLocal_Place_ID`) | Identity | `VideoLocal`, `ShokoManagedFolder` |
| `ShokoManagedFolder` | `ShokoManagedFolder` | `int` (ShokoManagedFolderID) | Identity | `VideoLocalPlaces` (ICollection) |
| `FilterPreset` | `FilterPreset` | `int` (FilterPresetID) | Identity | `ParentFilterPreset`, `ChildFilterPresets` (ICollection) |
| `CustomTag` | `CustomTag` | `int` (CustomTagID) | Identity | — |
| `StoredReleaseInfo` | `StoredReleaseInfo` | `int` (StoredReleaseInfoID) | Identity | `StoredReleaseInfoMatchAttempts` (ICollection) |
| `StoredReleaseInfo_MatchAttempt` | `StoredReleaseInfo_MatchAttempt` | `int` (ID) | Identity | `StoredReleaseInfo` |
| `StoredRelocationPipe` | `StoredRelocationPipe` | `int` (StoredRelocationPipeID) | Identity | — |
| `ScanFile` | `ScanFile` | `int` (ScanFileID) | Identity | — |
| `Scan` | `Scan` | `int` (ScanID) | Identity | — |
| `FileNameHash` | `FileNameHash` | `int` (FileNameHashID) | Identity | — (unique index on `FileName` + `FileSize`) |

### AniDB Entities (Shoko.Server.Models.AniDB)

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `AniDB_Anime` | `AniDB_Anime` | `int` (AnimeID) | Identity | `AnimeSeries`, `AniDB_Episodes` (ICollection), `AniDB_AnimeTags` (ICollection), `AniDB_AnimeCharacters` (ICollection), `AniDB_AnimeStaff` (ICollection), `AniDB_AnimeTitles` (ICollection), `AniDB_AnimePreferredImages` (ICollection), `AniDB_AnimeRelations` (ICollection), `AniDB_AnimeSimilars` (ICollection) |
| `AniDB_Episode` | `AniDB_Episode` | `int` (EpisodeID) | Identity | `AnimeEpisode` (single), `AniDB_EpisodeTitles` (ICollection), `AniDB_EpisodePreferredImages` (ICollection) |
| `AniDB_Tag` | `AniDB_Tag` | `int` (TagID) | Identity | `AniDB_AnimeTags` (ICollection), `AnimeSeriesTags` (ICollection) |
| `AniDB_Creator` | `AniDB_Creator` | `int` (CreatorID) | Identity | — |
| `AniDB_Anime_Tag` | `AniDB_Anime_Tag` | `int` (AniDB_Anime_TagID) | Identity | `AniDB_Anime`, `AniDB_Tag` |
| `AniDB_Anime_Character` | `AniDB_Anime_Character` | `int` (ID) | Identity | `AniDB_Anime`, `AniDB_Character` |
| `AniDB_Anime_Character_Creator` | `AniDB_Anime_Character_Creator` | `int` (ID) | Identity | `AniDB_Anime_Character`, `AniDB_Creator` |
| `AniDB_Anime_Staff` | `AniDB_Anime_Staff` | `int` (ID) | Identity | `AniDB_Anime`, `AniDB_Creator` |
| `AniDB_Anime_Title` | `AniDB_Anime_Title` | `int` (ID) | Identity | `AniDB_Anime` |
| `AniDB_Anime_PreferredImage` | `AniDB_Anime_PreferredImage` | `int` (ID) | Identity | `AniDB_Anime` |
| `AniDB_AnimeUpdate` | `AniDB_AnimeUpdate` | `int` (AniDB_AnimeUpdateID) | Identity | — (unique index on `AnimeID` for one-row-per-anime semantics) |
| `AniDB_Anime_Relation` | `AniDB_Anime_Relation` | `int` (ID) | Identity | `AniDB_Anime` |
| `AniDB_Anime_Similar` | `AniDB_Anime_Similar` | `int` (ID) | Identity | `AniDB_Anime` |
| `AniDB_Episode_Title` | `AniDB_Episode_Title` | `int` (ID) | Identity | `AniDB_Episode` |
| `AniDB_Episode_PreferredImage` | `AniDB_Episode_PreferredImage` | `int` (ID) | Identity | `AniDB_Episode` |
| `AniDB_GroupStatus` | `AniDB_GroupStatus` | `int` (AniDB_GroupStatusID) | Identity | — (unique index on `AnimeID` for one-row-per-anime semantics) |
| `AniDB_NotifyQueue` | `AniDB_NotifyQueue` | `int` (ID) | Identity | — |
| `AniDB_Message` | `AniDB_Message` | `int` (ID) | Identity | — |
| `AniDB_Character` | `AniDB_Character` | `int` (CharacterID) | Identity | — |

### TMDB Entities (Shoko.Server.Models.TMDB)

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `TMDB_Show` | `TMDB_Show` | `int` (Id) | Identity | `TMDB_Seasons` (ICollection), `TMDB_Episodes` (ICollection), `TMDB_ImageEntities` (ICollection), `TMDB_ShowNetworks` (ICollection) |
| `TMDB_Movie` | `TMDB_Movie` | `int` (Id) | Identity | `TMDB_MovieCast` (ICollection), `TMDB_MovieCrew` (ICollection), `TMDB_ImageEntities` (ICollection) |
| `TMDB_Episode` | `TMDB_Episode` | `int` (Id) | Identity | `TMDB_EpisodeCast` (ICollection), `TMDB_EpisodeCrew` (ICollection) |
| `TMDB_Season` | `TMDB_Season` | `int` (Id) | Identity | `TMDB_Episodes` (ICollection) |
| `TMDB_Person` | `TMDB_Person` | `int` (TmdbPersonID) | Identity | — |
| `TMDB_Image` | `TMDB_Image` | `int` (Id) | Identity | — |
| `TMDB_Image_Entity` | `TMDB_Image_Entity` | `int` (ID) | Identity | `TMDB_Show`, `TMDB_Movie`, `TMDB_Episode` |
| `TMDB_Company` | `TMDB_Company` | `int` (Id) | Identity | `TMDB_CompanyEntities` (ICollection) |
| `TMDB_Company_Entity` | `TMDB_Company_Entity` | `int` (ID) | Identity | `TMDB_Show`, `TMDB_Movie`, `TMDB_Company` |
| `TMDB_Collection` | `TMDB_Collection` | `int` (Id) | Identity | `TMDB_CollectionMovies` (ICollection) |
| `TMDB_Collection_Movie` | `TMDB_Collection_Movie` | `int` (ID) | Identity | `TMDB_Collection`, `TMDB_Movie` |
| `TMDB_Network` | `TMDB_Network` | `int` (Id) | Identity | — |
| `TMDB_Show_Network` | `TMDB_Show_Network` | `int` (ID) | Identity | `TMDB_Show`, `TMDB_Network` |
| `TMDB_AlternateOrdering` | `TMDB_AlternateOrdering` | `int` (Id) | Identity | `TMDB_AlternateOrderingSeasons` (ICollection), `TMDB_AlternateOrderingEpisodes` (ICollection) |
| `TMDB_AlternateOrdering_Season` | `TMDB_AlternateOrdering_Season` | `int` (ID) | Identity | `TMDB_AlternateOrdering`, `TMDB_Season` |
| `TMDB_AlternateOrdering_Episode` | `TMDB_AlternateOrdering_Episode` | `int` (ID) | Identity | `TMDB_AlternateOrdering`, `TMDB_Episode` |
| `TMDB_Movie_Cast` | `TMDB_Movie_Cast` | `int` (ID) | Identity | `TMDB_Movie`, `TMDB_Person` |
| `TMDB_Movie_Crew` | `TMDB_Movie_Crew` | `int` (ID) | Identity | `TMDB_Movie`, `TMDB_Person` |
| `TMDB_Episode_Cast` | `TMDB_Episode_Cast` | `int` (ID) | Identity | `TMDB_Episode`, `TMDB_Person` |
| `TMDB_Episode_Crew` | `TMDB_Episode_Crew` | `int` (ID) | Identity | `TMDB_Episode`, `TMDB_Person` |
| `TMDB_Title` | `TMDB_Title` | `int` (ID) | Identity | `TMDB_Show`, `TMDB_Movie`, `TMDB_Episode` |
| `TMDB_Overview` | `TMDB_Overview` | `int` (ID) | Identity | `TMDB_Show`, `TMDB_Movie`, `TMDB_Episode` |

### Cross-Reference Entities

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `CrossRef_File_Episode` | `CrossRef_File_Episode` | `int` (CrossRef_File_EpisodeID) | Identity | `AniDB_Episode`, `AniDB_Anime` |
| `CrossRef_AniDB_TMDB_Show` | `CrossRef_AniDB_TMDB_Show` | `int` (ID) | Identity | `AnimeSeries`, `TMDB_Show` |
| `CrossRef_AniDB_TMDB_Movie` | `CrossRef_AniDB_TMDB_Movie` | `int` (ID) | Identity | `AnimeSeries` or `AnimeEpisode`, `TMDB_Movie` |
| `CrossRef_AniDB_TMDB_Episode` | `CrossRef_AniDB_TMDB_Episode` | `int` (ID) | Identity | `AnimeEpisode`, `TMDB_Episode` |
| `CrossRef_AniDB_MAL` | `CrossRef_AniDB_MAL` | `int` (ID) | Identity | `AnimeSeries` |
| `CrossRef_CustomTag` | `CrossRef_CustomTag` | `int` (ID) | Identity | `AnimeEpisode`, `CustomTag` |
| `CrossRef_AniDB_TraktV2` | `CrossRef_AniDB_TraktV2` | `int` (ID) | Identity | `AnimeSeries` |

### User & Auth Entities

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `JMMUser` | `JMMUser` | `int` (JMMUserID) | Identity | `AnimeSeriesUsers` (ICollection), `AnimeEpisodeUsers` (ICollection), `VideoLocalUsers` (ICollection), `AuthTokens` (ICollection), `AnimeGroupUsers` (ICollection) |
| `AuthTokens` | `AuthTokens` | `int` (AuthTokensID) | Identity | `JMMUser` |

### User Data Entities

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `AnimeSeries_User` | `AnimeSeries_User` | `int` (ID) | Identity | `AnimeSeries`, `JMMUser` |
| `AnimeEpisode_User` | `AnimeEpisode_User` | `int` (ID) | Identity | `AnimeEpisode`, `JMMUser` |
| `VideoLocal_User` | `VideoLocal_User` | `int` (ID) | Identity | `VideoLocal`, `JMMUser` |
| `AnimeGroup_User` | `AnimeGroup_User` | `int` (ID) | Identity | `AnimeGroup`, `JMMUser` |

### Internal & Tracking Entities

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `ScheduledUpdate` | `ScheduledUpdate` | `int` (ID) | Identity | — |
| `Versions` | `Versions` | `int` (ID) | Identity | — |
| `VideoLocal_HashDigest` | `VideoLocal_HashDigest` | `int` (ID) | Identity | `VideoLocal` |
| `AnimeGroup` | `AnimeGroup` | `int` (AnimeGroupID) | Identity | `ParentGroup`, `ChildGroups` (ICollection), `AnimeSeries` (ICollection), `AnimeGroupUsers` (ICollection) |

### Trakt Entities

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `Trakt_Show` | `Trakt_Show` | `int` (ID) | Identity | — |
| `Trakt_Season` | `Trakt_Season` | `int` (ID) | Identity | — |
| `Trakt_Episode` | `Trakt_Episode` | `int` (ID) | Identity | — |

### Image Entity

| Entity | Table | Key Type | Key Generation | Navigation Properties |
|--------|-------|----------|----------------|----------------------|
| `Image` | `Image` | `int` (ImageID) | Identity | — |

## Custom Value Conversions Required

The following NHibernate `IUserType` converters are known candidates for EF Core `ValueConverter<T, TProvider>` replacement. The exact inventory count will be confirmed during Phase 1 (T-005 through T-010) by scanning all mapping files and the `Shoko.Server/Databases/NHIbernate/` directory.

**Known converters** (subject to inventory confirmation):

| Converter | Entity | Property | Storage Type | Serializer |
|-----------|--------|----------|--------------|------------|
| `MessagePackConverter<MediaContainer>` | `VideoLocal` | `MediaInfo` | VARBINARY/BLOB | MessagePack |
| `MessagePackConverter<FilterPreset>` | `FilterPreset` | `Expression` | TEXT | MessagePack |
| `MessagePackConverter<FilterPreset>` | `FilterPreset` | `SortingExpression` | TEXT | MessagePack |
| `FilterExpressionConverter` | `FilterPreset` | `Expression` (legacy) | TEXT | Newtonsoft.Json |
| `DateOnlyConverter` | Various date fields | Various | INTEGER | `DateOnly` ↔ `int` (Unix epoch days) |
| `TitleLanguageConverter` | `AnimeSeries` | `DefaultSubtitleLanguage` | TEXT | String |
| `TitleTypeConverter` | Various | Various | INTEGER | Int |
| `StringListConverter` | Various | Various | TEXT | JSON |
| `TmdbContentRatingConverter` | `TMDB_Show` | `ContentRating` | TEXT | String |
| `TmdbProductionCountryConverter` | `TMDB_Show` | `ProductionCountries` | TEXT | JSON |
| `DisabledAutoMatchFlagConverter` | `AnimeSeries` | `DisableAutoMatchFlags` | INTEGER | Int (flags) |
| `TypeNameSerializationBinder`-based | Various | Various | TEXT/VARBINARY | Newtonsoft.Json + binder |

## Relationship Summary

### One-to-Many
- `AnimeGroup` → `AnimeSeries` (1 → many)
- `AnimeSeries` → `AnimeEpisode` (1 → many)
- `AniDB_Anime` → `AniDB_Episode` (1 → many)
- `AnimeSeries` → `AnimeSeries_User` (1 → many)
- `AnimeEpisode` → `AnimeEpisode_User` (1 → many)
- `VideoLocal` → `VideoLocal_Place` (1 → many)
- `ShokoManagedFolder` → `VideoLocal_Place` (1 → many)
- `VideoLocal` → `VideoLocal_HashDigest` (1 → many)
- `VideoLocal` → `VideoLocal_User` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Tag` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Character` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Staff` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Title` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_PreferredImage` (1 → many)
- `AniDB_Episode` → `AniDB_Episode_Title` (1 → many)
- `AniDB_Episode` → `AniDB_Episode_PreferredImage` (1 → many)
- `TMDB_Show` → `TMDB_Season` (1 → many)
- `TMDB_Show` → `TMDB_Episode` (1 → many)
- `TMDB_Season` → `TMDB_Episode` (1 → many)
- `TMDB_Show` → `TMDB_Image_Entity` (1 → many)
- `TMDB_Movie` → `TMDB_Image_Entity` (1 → many)
- `TMDB_Episode` → `TMDB_Image_Entity` (1 → many)
- `AniDB_Creator` → `AniDB_Anime_Staff` (1 → many)
- `AniDB_Creator` → `AniDB_Anime_Character_Creator` (1 → many)
- `JMMUser` → `AuthTokens` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Relation` (1 → many)
- `AniDB_Anime` → `AniDB_Anime_Similar` (1 → many)

### Many-to-Many (via join tables)
- `AniDB_Anime` ↔ `AniDB_Tag` (via `AniDB_Anime_Tag`)
- `AniDB_Anime` ↔ `AniDB_Creator` (via `AniDB_Anime_Staff`)
- `AniDB_Anime` ↔ `AniDB_Character` (via `AniDB_Anime_Character`)
- `AniDB_Anime_Character` ↔ `AniDB_Creator` (via `AniDB_Anime_Character_Creator`)
- `CrossRef_File_Episode` links `VideoLocal` (via hash) ↔ `AniDB_Episode`
- `CrossRef_AniDB_TMDB_Show` links `AnimeSeries` ↔ `TMDB_Show`
- `CrossRef_AniDB_TMDB_Movie` links `AnimeSeries`/`AnimeEpisode` ↔ `TMDB_Movie`
- `CrossRef_AniDB_TMDB_Episode` links `AnimeEpisode` ↔ `TMDB_Episode`
- `CrossRef_CustomTag` links `AnimeEpisode` ↔ `CustomTag`
- `TMDB_Movie_Cast` links `TMDB_Movie` ↔ `TMDB_Person`
- `TMDB_Movie_Crew` links `TMDB_Movie` ↔ `TMDB_Person`
- `TMDB_Episode_Cast` links `TMDB_Episode` ↔ `TMDB_Person`
- `TMDB_Episode_Crew` links `TMDB_Episode` ↔ `TMDB_Person`
- `TMDB_Company_Entity` links `TMDB_Show`/`TMDB_Movie` ↔ `TMDB_Company`
- `TMDB_Show_Network` links `TMDB_Show` ↔ `TMDB_Network`
- `TMDB_AlternateOrdering_Season` links `TMDB_AlternateOrdering` ↔ `TMDB_Season`
- `TMDB_AlternateOrdering_Episode` links `TMDB_AlternateOrdering` ↔ `TMDB_Episode`
- `TMDB_Collection_Movie` links `TMDB_Collection` ↔ `TMDB_Movie`

### Self-Referential
- `AnimeGroup` → `AnimeGroup` (parent → children via `AnimeGroupParentID`)
- `FilterPreset` → `FilterPreset` (parent → children via `ParentFilterPresetID`)

### Key Generation Strategy
All entities use **Identity** (auto-increment integer) keys. No entities use **Assigned** key generation.

**Important EF Core note**: Three entities have unique business-key columns that are NOT the primary key:
- `FileNameHash`: PK is `FileNameHashID` (Identity); unique constraint on `FileName` + `FileSize`
- `AniDB_AnimeUpdate`: PK is `AniDB_AnimeUpdateID` (Identity); unique constraint on `AnimeID` (one-row-per-anime)
- `AniDB_GroupStatus`: PK is `AniDB_GroupStatusID` (Identity); unique constraint on `AnimeID` (one-row-per-anime)
