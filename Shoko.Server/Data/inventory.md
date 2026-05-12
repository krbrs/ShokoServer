# Mapping Inventory

**Task**: T005 — Catalog FluentNHibernate mapping files  
**Generated**: 2026-05-06  
**Location**: `Shoko.Server/Mappings/`  
**Total**: 75 mapping files (verified via `find *Map.cs | wc -l`)

Each entry records: entity name, table name (explicit/implicit), key expression with column name and generated strategy, all mapped properties with custom column names, all CustomType usages, indexes/unique constraints, References/HasMany/HasManyToMany relationships, Not.LazyLoad/LazyLoad, ReadOnly, cascade behavior, and EF Core parity notes/risks.

---

## Root Mappings (47 files)

### 1. AniDB_AnimeMap.cs
- **Entity**: `AniDB_Anime`
- **Table**: `AniDB_Anime` (explicit)
- **Key**: `Id(x => x.AniDB_AnimeID)` — column `AniDB_AnimeID`, generated Identity
- **Properties**: `AirDate`, `AllCinemaID`, `AllTitles`, `AllTags`, `AnimeID` (Not.Nullable), `AnimeType` (Not.Nullable, CustomType `AnimeType`), `ANNID`, `AnisonID`, `SyoboiID`, `VNDBID`, `BangumiID`, `LainID`, `Site_EN`, `Site_JP`, `Wikipedia_ID`, `WikipediaJP_ID`, `CrunchyrollID`, `FunimationID`, `HiDiveID`, `AvgReviewRating` (Not.Nullable), `BeginYear` (Not.Nullable), `DateTimeDescUpdated` (Not.Nullable), `DateTimeUpdated` (Not.Nullable, deprecated), `Description` (CustomType `StringClob`, Not.Nullable), `EndDate`, `EndYear` (Not.Nullable), `EpisodeCount` (Not.Nullable), `EpisodeCountNormal` (Not.Nullable), `EpisodeCountSpecial` (Not.Nullable), `ImageEnabled` (Not.Nullable), `LatestEpisodeNumber`, `MainTitle` (Not.Nullable), `Picname`, `Rating` (Not.Nullable), `Restricted` (Not.Nullable), `ReviewCount` (Not.Nullable), `TempRating` (Not.Nullable), `TempVoteCount` (Not.Nullable), `URL`, `VoteCount` (Not.Nullable)
- **Custom Types**: `AnimeType`, `StringClob`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: All columns map directly. `Description` needs `HasColumnType("nvarchar(max)")`. `AnimeType` enum needs ValueConverter. No relationships defined at mapping level — FK from `AnimeSeries.AniDB_ID` is the logical 1:1.

### 2. AniDB_Anime_CharacterMap.cs
- **Entity**: `AniDB_Anime_Character`
- **Table**: `AniDB_Anime_Character` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_CharacterID)` — column `AniDB_Anime_CharacterID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `CharacterID` (Not.Nullable), `Appearance` (Not.Nullable), `AppearanceType` (Not.Nullable, CustomType `CharacterAppearanceType`), `Ordering` (Not.Nullable)
- **Custom Types**: `CharacterAppearanceType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Simple join-table-like entity. `AnimeID` and `CharacterID` are FK columns. `AppearanceType` enum needs ValueConverter.

### 3. AniDB_Anime_Character_CreatorMap.cs
- **Entity**: `AniDB_Anime_Character_Creator`
- **Table**: `AniDB_Anime_Character_Creator` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_Character_CreatorID)` — column `AniDB_Anime_Character_CreatorID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `CharacterID` (Not.Nullable), `CreatorID` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Simple join-table-like entity. All FK columns. No custom types.

### 4. AniDB_Anime_PreferredImageMap.cs
- **Entity**: `AniDB_Anime_PreferredImage`
- **Table**: `AniDB_Anime_PreferredImage` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_PreferredImageID)` — column `AniDB_Anime_PreferredImageID`, generated Identity
- **Properties**: `AnidbAnimeID` (Not.Nullable), `ImageID` (Not.Nullable), `ImageSource` (Not.Nullable, CustomType `DataSource`), `ImageType` (Not.Nullable, CustomType `ImageEntityType`)
- **Custom Types**: `DataSource`, `ImageEntityType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `ImageSource` and `ImageType` enums need ValueConverters. `AnidbAnimeID` and `ImageID` are FK columns.

### 5. AniDB_Anime_RelationMap.cs
- **Entity**: `AniDB_Anime_Relation`
- **Table**: `AniDB_Anime_Relation` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_RelationID)` — column `AniDB_Anime_RelationID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `RelatedAnimeID` (Not.Nullable), `RelationType` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Self-referential relation table. `RelationType` is likely an int/enum.

### 6. AniDB_Anime_SimilarMap.cs
- **Entity**: `AniDB_Anime_Similar`
- **Table**: `AniDB_Anime_Similar` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_SimilarID)` — column `AniDB_Anime_SimilarID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `Approval` (Not.Nullable), `SimilarAnimeID` (Not.Nullable), `Total` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Self-referential similarity table. `SimilarAnimeID` is FK to another anime.

### 7. AniDB_Anime_StaffMap.cs
- **Entity**: `AniDB_Anime_Staff`
- **Table**: `AniDB_Anime_Staff` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_StaffID)` — column `AniDB_Anime_StaffID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `CreatorID` (Not.Nullable), `RoleType` (Not.Nullable, CustomType `CreatorRoleType`), `Role` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: `CreatorRoleType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `RoleType` enum needs ValueConverter. `AnimeID` and `CreatorID` are FK columns.

### 8. AniDB_Anime_TagMap.cs
- **Entity**: `AniDB_Anime_Tag`
- **Table**: `AniDB_Anime_Tag` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_TagID)` — column `AniDB_Anime_TagID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `LocalSpoiler` (Not.Nullable), `Weight` (Not.Nullable), `TagID` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Join table between anime and tags. `TagID` is FK to `AniDB_Tag`.

### 9. AniDB_Anime_TitleMap.cs
- **Entity**: `AniDB_Anime_Title`
- **Table**: `AniDB_Anime_Title` (explicit)
- **Key**: `Id(x => x.AniDB_Anime_TitleID)` — column `AniDB_Anime_TitleID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `Language` (Not.Nullable, CustomType `TitleLanguageConverter`), `Title` (Not.Nullable), `TitleType` (Not.Nullable, CustomType `TitleTypeConverter`)
- **Custom Types**: `TitleLanguageConverter`, `TitleTypeConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Multi-language title table. `Language` and `TitleType` enums need ValueConverters.

### 10. AniDB_AnimeUpdateMap.cs
- **Entity**: `AniDB_AnimeUpdate`
- **Table**: `AniDB_AnimeUpdate` (explicit)
- **Key**: `Id(x => x.AniDB_AnimeUpdateID)` — column `AniDB_AnimeUpdateID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `UpdatedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Simple tracking table. Key is `AniDB_AnimeUpdateID` (Identity), NOT `AnimeID`. `AnimeID` is a regular column. EF Core config should add a unique index on `AnimeID` if enforcing one-row-per-anime semantics.

### 11. AniDB_CharacterMap.cs
- **Entity**: `AniDB_Character`
- **Table**: `AniDB_Character` (explicit)
- **Key**: `Id(x => x.AniDB_CharacterID)` — column `AniDB_CharacterID`, generated Identity
- **Properties**: `Description` (Not.Nullable, CustomType `StringClob`), `CharacterID` (Not.Nullable), `ImagePath` (Not.Nullable), `OriginalName` (Not.Nullable), `Name` (Not.Nullable), `Gender` (Not.Nullable, CustomType `PersonGender`), `Type` (Not.Nullable, CustomType `CharacterType`), `LastUpdated` (Not.Nullable)
- **Custom Types**: `StringClob`, `PersonGender`, `CharacterType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Description` needs `HasColumnType("nvarchar(max)")`. `Gender` and `Type` enums need ValueConverters.

### 12. AniDB_CreatorMap.cs
- **Entity**: `AniDB_Creator`
- **Table**: `AniDB_Creator` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.AniDB_CreatorID)` — column `AniDB_CreatorID`, generated Identity
- **Properties**: `CreatorID` (Not.Nullable), `Name` (Not.Nullable), `OriginalName`, `Type` (Not.Nullable, CustomType `CreatorType`), `ImagePath`, `EnglishHomepageUrl`, `JapaneseHomepageUrl`, `EnglishWikiUrl`, `JapaneseWikiUrl`, `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: `CreatorType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name must be explicitly set to `AniDB_Creator` since NHibernate falls back to class name. `Type` enum needs ValueConverter.

### 13. AniDB_EpisodeMap.cs
- **Entity**: `AniDB_Episode`
- **Table**: `AniDB_Episode` (explicit)
- **Key**: `Id(x => x.AniDB_EpisodeID)` — column `AniDB_EpisodeID`, generated Identity
- **Properties**: `AirDate` (Not.Nullable), `AnimeID` (Not.Nullable), `DateTimeUpdated` (Not.Nullable), `Description` (Not.Nullable, CustomType `StringClob`), `EpisodeID` (Not.Nullable), `EpisodeNumber` (Not.Nullable), `EpisodeType` (Not.Nullable, CustomType `EpisodeType`), `LengthSeconds` (Not.Nullable), `Rating` (Not.Nullable), `Votes` (Not.Nullable)
- **Custom Types**: `StringClob`, `EpisodeType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Description` needs `HasColumnType("nvarchar(max)")`. `EpisodeType` enum needs ValueConverter.

### 14. AniDB_Episode_PreferredImageMap.cs
- **Entity**: `AniDB_Episode_PreferredImage`
- **Table**: `AniDB_Episode_PreferredImage` (explicit)
- **Key**: `Id(x => x.AniDB_Episode_PreferredImageID)` — column `AniDB_Episode_PreferredImageID`, generated Identity
- **Properties**: `AnidbAnimeID` (Not.Nullable), `AnidbEpisodeID` (Not.Nullable), `ImageID` (Not.Nullable), `ImageSource` (Not.Nullable, CustomType `DataSource`), `ImageType` (Not.Nullable, CustomType `ImageEntityType`)
- **Custom Types**: `DataSource`, `ImageEntityType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `ImageSource` and `ImageType` enums need ValueConverters. Links episode to preferred image.

### 15. AniDB_Episode_TitleMap.cs
- **Entity**: `AniDB_Episode_Title`
- **Table**: `AniDB_Episode_Title` (explicit)
- **Key**: `Id(x => x.AniDB_Episode_TitleID)` — column `AniDB_Episode_TitleID`, generated Identity
- **Properties**: `AnidbEpisodeID` (Not.Nullable), `Language` (Not.Nullable, CustomType `TitleLanguageConverter`), `Title` (Not.Nullable)
- **Custom Types**: `TitleLanguageConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Multi-language episode title table. `Language` enum needs ValueConverter.

### 16. AniDB_GroupStatusMap.cs
- **Entity**: `AniDB_GroupStatus`
- **Table**: `AniDB_GroupStatus` (explicit)
- **Key**: `Id(x => x.AniDB_GroupStatusID)` — column `AniDB_GroupStatusID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `CompletionState` (Not.Nullable), `EpisodeRange`, `GroupID` (Not.Nullable), `GroupName`, `LastEpisodeNumber` (Not.Nullable), `Rating` (Not.Nullable), `Votes` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Simple cache table. Key is `AniDB_GroupStatusID` (Identity), NOT `AnimeID`. `AnimeID` is a regular column. EF Core config should add a unique index on `AnimeID` if enforcing one-row-per-anime semantics.

### 17. AniDB_MessageMap.cs
- **Entity**: `AniDB_Message`
- **Table**: `AniDB_Message` (explicit)
- **Key**: `Id(x => x.AniDB_MessageID)` — column `AniDB_MessageID`, generated Identity
- **Properties**: `MessageID` (Not.Nullable), `FromUserId` (Not.Nullable), `FromUserName` (Not.Nullable), `SentAt` (Not.Nullable), `FetchedAt` (Not.Nullable), `Type` (Not.Nullable, CustomType `AniDBMessageType`), `Title` (Not.Nullable), `Body` (Not.Nullable), `Flags` (Not.Nullable, CustomType `AniDBMessageFlags`)
- **Custom Types**: `AniDBMessageType`, `AniDBMessageFlags`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Type` and `Flags` enums need ValueConverters.

### 18. AniDB_NotifyQueueMap.cs
- **Entity**: `AniDB_NotifyQueue`
- **Table**: `AniDB_NotifyQueue` (explicit)
- **Key**: `Id(x => x.AniDB_NotifyQueueID)` — column `AniDB_NotifyQueueID`, generated Identity
- **Properties**: `Type` (Not.Nullable, CustomType `AniDBNotifyType`), `ID` (Not.Nullable), `AddedAt` (Not.Nullable)
- **Custom Types**: `AniDBNotifyType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Type` enum needs ValueConverter. `ID` maps to column `ID` (same name, no custom column).

### 19. AniDB_TagMap.cs
- **Entity**: `AniDB_Tag`
- **Table**: `AniDB_Tag` (explicit)
- **Key**: `Id(x => x.AniDB_TagID)` — column `AniDB_TagID`, generated Identity
- **Properties**: `TagID` (Not.Nullable), `ParentTagID`, `TagNameSource` (Not.Nullable, column: `TagName`), `TagNameOverride`, `TagDescription` (Not.Nullable, CustomType `StringClob`), `GlobalSpoiler` (Not.Nullable), `Verified` (Not.Nullable), `LastUpdated`
- **Custom Types**: `StringClob`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `TagNameSource` maps to column `TagName` (custom column name). `TagDescription` needs `HasColumnType("nvarchar(max)")`. `ParentTagID` is self-referential FK.

### 20. AnimeEpisodeMap.cs
- **Entity**: `AnimeEpisode`
- **Table**: `AnimeEpisode` (explicit)
- **Key**: `Id(x => x.AnimeEpisodeID)` — column `AnimeEpisodeID`, generated Identity
- **Properties**: `AniDB_EpisodeID` (Not.Nullable), `AnimeSeriesID` (Not.Nullable), `DateTimeCreated` (Not.Nullable), `DateTimeUpdated` (Not.Nullable), `IsHidden` (Not.Nullable), `EpisodeNameOverride`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Wraps `AniDB_Episode`. `AniDB_EpisodeID` and `AnimeSeriesID` are FK columns.

### 21. AnimeEpisode_UserMap.cs
- **Entity**: `AnimeEpisode_User`
- **Table**: `AnimeEpisode_User` (explicit)
- **Key**: `Id(x => x.AnimeEpisode_UserID)` — column `AnimeEpisode_UserID`, generated Identity
- **Properties**: `AnimeEpisodeID` (Not.Nullable), `AnimeSeriesID` (Not.Nullable), `JMMUserID` (Not.Nullable), `PlayedCount` (Not.Nullable), `StoppedCount` (Not.Nullable), `WatchedCount` (Not.Nullable), `WatchedDate`, `IsFavorite` (Not.Nullable), `AbsoluteUserRating`, `UserTags` (Not.Nullable, CustomType `StringListConverter`), `LastUpdated` (Not.Nullable)
- **Custom Types**: `StringListConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `UserTags` needs JSON ValueConverter. `AnimeEpisodeID`, `AnimeSeriesID`, `JMMUserID` are FK columns.

### 22. AnimeGroupMap.cs
- **Entity**: `AnimeGroup`
- **Table**: `AnimeGroup` (explicit)
- **Key**: `Id(x => x.AnimeGroupID)` — column `AnimeGroupID`, generated Identity
- **Properties**: `AnimeGroupParentID`, `DefaultAnimeSeriesID`, `MainAniDBAnimeID`, `DateTimeCreated` (Not.Nullable), `DateTimeUpdated` (Not.Nullable), `Description` (CustomType `StringClob`, CustomSqlType `nvarchar(max)`), `GroupName`, `IsManuallyNamed` (Not.Nullable), `OverrideDescription` (Not.Nullable), `EpisodeAddedDate`, `LatestEpisodeAirDate`, `MissingEpisodeCount` (Not.Nullable), `MissingEpisodeCountGroups` (Not.Nullable)
- **Custom Types**: `StringClob` (nvarchar(max))
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Description` needs `HasColumnType("nvarchar(max)")`. `AnimeGroupParentID` is self-referential FK.

### 23. AnimeGroup_UserMap.cs
- **Entity**: `AnimeGroup_User`
- **Table**: `AnimeGroup_User` (explicit)
- **Key**: `Id(x => x.AnimeGroup_UserID)` — column `AnimeGroup_UserID`, generated Identity
- **Properties**: `JMMUserID`, `AnimeGroupID`, `PlayedCount` (Not.Nullable), `StoppedCount` (Not.Nullable), `UnwatchedEpisodeCount` (Not.Nullable), `WatchedCount` (Not.Nullable), `WatchedDate`, `WatchedEpisodeCount`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `JMMUserID` and `AnimeGroupID` are FK columns.

### 24. AnimeSeriesMap.cs
- **Entity**: `AnimeSeries`
- **Table**: `AnimeSeries` (explicit)
- **Key**: `Id(x => x.AnimeSeriesID)` — column `AnimeSeriesID`, generated Identity
- **Properties**: `AniDB_ID` (Not.Nullable), `AnimeGroupID` (Not.Nullable), `DateTimeCreated` (Not.Nullable), `DateTimeUpdated` (Not.Nullable), `DefaultAudioLanguage`, `DefaultSubtitleLanguage`, `LatestLocalEpisodeNumber` (Not.Nullable), `EpisodeAddedDate`, `LatestEpisodeAirDate`, `MissingEpisodeCount` (Not.Nullable), `MissingEpisodeCountGroups` (Not.Nullable), `HiddenMissingEpisodeCount` (Not.Nullable), `HiddenMissingEpisodeCountGroups` (Not.Nullable), `SeriesNameOverride`, `AirsOn`, `UpdatedAt` (Not.Nullable), `DisableAutoMatchFlags` (Not.Nullable, CustomType `DisabledAutoMatchFlag`)
- **Custom Types**: `DisabledAutoMatchFlag`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `DisableAutoMatchFlags` needs ValueConverter for bitmask enum flag. `AniDB_ID` is FK to `AniDB_Anime`. `AnimeGroupID` is FK to `AnimeGroup`.

### 25. AnimeSeries_UserMap.cs
- **Entity**: `AnimeSeries_User`
- **Table**: `AnimeSeries_User` (explicit)
- **Key**: `Id(x => x.AnimeSeries_UserID)` — column `AnimeSeries_UserID`, generated Identity
- **Properties**: `JMMUserID` (Not.Nullable), `AnimeSeriesID` (Not.Nullable), `PlayedCount` (Not.Nullable), `StoppedCount` (Not.Nullable), `UnwatchedEpisodeCount` (Not.Nullable), `WatchedCount` (Not.Nullable), `WatchedDate`, `WatchedEpisodeCount` (Not.Nullable), `LastEpisodeUpdate`, `LastVideoUpdate`, `HiddenUnwatchedEpisodeCount` (Not.Nullable), `IsFavorite` (Not.Nullable), `AbsoluteUserRating`, `UserRatingVoteType` (CustomType `SeriesVoteType`), `UserTags` (Not.Nullable, CustomType `StringListConverter`), `LastUpdated` (Not.Nullable)
- **Custom Types**: `SeriesVoteType`, `StringListConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `UserRatingVoteType` and `UserTags` need ValueConverters. `JMMUserID` and `AnimeSeriesID` are FK columns.

### 26. AuthTokensMap.cs
- **Entity**: `AuthTokens`
- **Table**: `AuthTokens` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.AuthID)` — column `AuthID`, generated Identity
- **Properties**: `UserID` (Not.Nullable), `DeviceName` (Not.Nullable), `Token` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `AuthTokens`). `UserID` is FK to `JMMUser`.

### 27. CrossRef_AniDB_MALMap.cs
- **Entity**: `CrossRef_AniDB_MAL`
- **Table**: `CrossRef_AniDB_MAL` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.CrossRef_AniDB_MALID)` — column `CrossRef_AniDB_MALID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `MALID` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Cross-reference between AniDB and MAL. `AnimeID` and `MALID` are FK columns.

### 28. CrossRef_AniDB_TraktV2Map.cs
- **Entity**: `CrossRef_AniDB_TraktV2`
- **Table**: `CrossRef_AniDB_TraktV2` (explicit)
- **Key**: `Id(x => x.CrossRef_AniDB_TraktV2ID)` — column `CrossRef_AniDB_TraktV2ID`, generated Identity
- **Properties**: `AnimeID` (Not.Nullable), `CrossRefSource` (Not.Nullable), `TraktID`, `TraktSeasonNumber` (Not.Nullable), `AniDBStartEpisodeType` (Not.Nullable), `AniDBStartEpisodeNumber` (Not.Nullable), `TraktStartEpisodeNumber` (Not.Nullable), `TraktTitle`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Cross-reference between AniDB and Trakt. `AnimeID` is FK.

### 29. CrossRef_CustomTagMap.cs
- **Entity**: `CrossRef_CustomTag`
- **Table**: `CrossRef_CustomTag` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.CrossRef_CustomTagID)` — column `CrossRef_CustomTagID`, generated Identity
- **Properties**: `CustomTagID` (Not.Nullable), `CrossRefID` (Not.Nullable), `CrossRefType` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Custom tag cross-reference. `CustomTagID` and `CrossRefID` are FK columns.

### 30. CrossRef_File_EpisodeMap.cs
- **Entity**: `CrossRef_File_Episode`
- **Table**: `CrossRef_File_Episode` (explicit)
- **Key**: `Id(x => x.CrossRef_File_EpisodeID)` — column `CrossRef_File_EpisodeID`, generated Identity
- **Properties**: `EpisodeID` (Not.Nullable), `EpisodeOrder` (Not.Nullable), `Hash` (Not.Nullable), `Percentage` (Not.Nullable), `FileName` (Not.Nullable), `FileSize` (Not.Nullable), `AnimeID` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: File-to-episode mapping. `EpisodeID`, `AnimeID` are FK columns. `Hash` + `FileSize` identifies the ED2K hash pair.

### 31. CustomTagMap.cs
- **Entity**: `CustomTag`
- **Table**: `CustomTag` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.CustomTagID)` — column `CustomTagID`, generated Identity
- **Properties**: `TagName`, `TagDescription`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `CustomTag`).

### 32. FileNameHashMap.cs
- **Entity**: `FileNameHash`
- **Table**: `FileNameHash` (explicit)
- **Key**: `Id(x => x.FileNameHashID)` — column `FileNameHashID`, generated Identity
- **Properties**: `Hash`, `FileName`, `FileSize` (Not.Nullable), `DateTimeUpdated` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Filename-to-hash cache. `FileName` + `FileSize` should have a unique index for EF Core to match NHibernate semantics. Key is `FileNameHashID` (Identity), NOT composite.

### 33. FilterPresetMap.cs
- **Entity**: `FilterPreset`
- **Table**: `FilterPreset` (explicit)
- **Key**: `Id(x => x.FilterPresetID)` — column `FilterPresetID`, generated Identity
- **Properties**: `ParentFilterPresetID` (nullable), `Name` (Not.Nullable), `FilterType` (Not.Nullable, CustomType `FilterPresetType`), `Locked` (Not.Nullable), `Hidden` (Not.Nullable), `ApplyAtSeriesLevel` (Not.Nullable), `Expression` (nullable, CustomType `FilterExpressionConverter`), `SortingExpression` (nullable, CustomType `FilterExpressionConverter`)
- **Custom Types**: `FilterPresetType`, `FilterExpressionConverter` (2 usages)
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: `References(a => a.Parent).Column("ParentFilterPresetID").ReadOnly()` — self-referential FK to `FilterPreset`; `HasMany(x => x.Children).Fetch.Join().KeyColumn("ParentFilterPresetID").ReadOnly()` — child collection
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: Yes (both `References` and `HasMany` are `ReadOnly()`)
- **Cascade**: None
- **EF Core Notes**: Parent/children self-referential relationship. `ParentFilterPresetID` is FK column. `Expression` and `SortingExpression` use `FilterExpressionConverter` (JSON serialization). Both relationships are ReadOnly (no cascade). `FilterType` enum needs ValueConverter.

### 34. JMMUserMap.cs
- **Entity**: `JMMUser`
- **Table**: `JMMUser` (explicit)
- **Key**: `Id(x => x.JMMUserID)` — column `JMMUserID`, generated Identity
- **Properties**: `HideCategories`, `IsAniDBUser` (Not.Nullable), `IsTraktUser` (Not.Nullable), `IsAdmin` (Not.Nullable), `Password`, `Username`, `CanEditServerSettings`, `PlexUsers`, `PlexToken`, `AvatarImageBlob` (nullable), `RawAvatarImageMetadata` (column: `AvatarImageMetadata`, nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `RawAvatarImageMetadata` maps to column `AvatarImageMetadata` (custom column name).

### 35. PlaylistMap.cs
- **Entity**: `Playlist`
- **Table**: `Playlist` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.PlaylistID)` — column `PlaylistID`, generated Identity
- **Properties**: `PlaylistName`, `PlaylistItems`, `DefaultPlayOrder` (Not.Nullable), `PlayWatched` (Not.Nullable), `PlayUnwatched` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `Playlist`).

### 36. ScanFileMap.cs
- **Entity**: `ScanFile`
- **Table**: `ScanFile` (explicit)
- **Key**: `Id(x => x.ScanFileID)` — column `ScanFileID`, generated Identity
- **Properties**: `ScanID` (Not.Nullable), `ImportFolderID` (Not.Nullable), `VideoLocal_Place_ID` (Not.Nullable), `FullName` (Not.Nullable), `FileSize` (Not.Nullable), `Status` (Not.Nullable, CustomType `ScanFileStatus`), `CheckDate`, `Hash` (Not.Nullable), `HashResult`
- **Custom Types**: `ScanFileStatus`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Status` enum needs ValueConverter. `ScanID`, `ImportFolderID`, `VideoLocal_Place_ID` are FK columns.

### 37. ScanMap.cs
- **Entity**: `Scan`
- **Table**: `Scan` (explicit)
- **Key**: `Id(x => x.ScanID)` — column `ScanID`, generated Identity
- **Properties**: `CreationTIme` (Not.Nullable), `ImportFolders` (Not.Nullable), `Status` (Not.Nullable, CustomType `ScanStatus`)
- **Custom Types**: `ScanStatus`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Status` enum needs ValueConverter. Note property name typo: `CreationTIme`.

### 38. ScheduledUpdateMap.cs
- **Entity**: `ScheduledUpdate`
- **Table**: `ScheduledUpdate` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.ScheduledUpdateID)` — column `ScheduledUpdateID`, generated Identity
- **Properties**: `LastUpdate` (Not.Nullable), `UpdateDetails`, `UpdateType` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `ScheduledUpdate`). One row per `UpdateType`.

### 39. ShokoManagedFolderMap.cs
- **Entity**: `ShokoManagedFolder`
- **Table**: `ImportFolder` (explicit — table name differs from class name)
- **Key**: `Id(x => x.ID).Column("ImportFolderID")` — column `ImportFolderID`, generated Identity
- **Properties**: `Path` (column: `ImportFolderLocation`, Not.Nullable), `Name` (column: `ImportFolderName`, Not.Nullable), `IsDropDestination` (Not.Nullable), `IsDropSource` (Not.Nullable), `IsWatched` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is `ImportFolder` (not `ShokoManagedFolder`). Key property is `ID` mapping to column `ImportFolderID`. `Path` maps to `ImportFolderLocation`, `Name` maps to `ImportFolderName`.

### 40. StoredReleaseInfoMap.cs
- **Entity**: `StoredReleaseInfo`
- **Table**: `StoredReleaseInfo` (explicit)
- **Key**: `Id(x => x.StoredReleaseInfoID)` — column `StoredReleaseInfoID`, generated Identity
- **Properties**: `ED2K` (Not.Nullable), `FileSize` (Not.Nullable), `ID`, `ProviderName` (Not.Nullable), `ReleaseURI`, `Version` (Not.Nullable), `ProvidedFileSize`, `Comment`, `OriginalFilename`, `IsCensored`, `IsChaptered`, `IsCreditless`, `IsCorrupted` (Not.Nullable), `Source` (CustomType `ReleaseSource`, Not.Nullable), `GroupID`, `GroupSource`, `GroupName`, `GroupShortName`, `EmbeddedHashes` (column: `Hashes`), `EmbeddedAudioLanguages` (column: `AudioLanguages`), `EmbeddedSubtitleLanguages` (column: `SubtitleLanguages`), `EmbeddedCrossReferences` (column: `CrossReferences`, Not.Nullable), `ReleasedAt` (CustomType `DateOnlyConverter`), `LastUpdatedAt` (Not.Nullable), `CreatedAt` (Not.Nullable)
- **Custom Types**: `ReleaseSource`, `DateOnlyConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Many custom column names: `Hashes`, `AudioLanguages`, `SubtitleLanguages`, `CrossReferences`. `Source` enum needs ValueConverter. `ReleasedAt` uses `DateOnlyConverter`.

### 41. StoredReleaseInfo_MatchAttemptMap.cs
- **Entity**: `StoredReleaseInfo_MatchAttempt`
- **Table**: `StoredReleaseInfo_MatchAttempt` (explicit)
- **Key**: `Id(x => x.StoredReleaseInfo_MatchAttemptID)` — column `StoredReleaseInfo_MatchAttemptID`, generated Identity
- **Properties**: `ProviderName`, `ProviderID`, `ED2K` (Not.Nullable), `FileSize` (Not.Nullable), `EmbeddedAttemptProviderNames` (column: `AttemptProviderNames`, Not.Nullable), `AttemptStartedAt` (Not.Nullable), `AttemptEndedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `EmbeddedAttemptProviderNames` maps to column `AttemptProviderNames` (custom column name).

### 42. StoredRelocationPipeMap.cs
- **Entity**: `StoredRelocationPipe`
- **Table**: `StoredRelocationPipe` (explicit)
- **Key**: `Id(x => x.StoredRelocationPipeID)` — column `StoredRelocationPipeID`, generated Identity
- **Properties**: `ProviderID` (Not.Nullable), `Name` (Not.Nullable), `Configuration`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Simple configuration table.

### 43. VersionsMap.cs
- **Entity**: `Versions`
- **Table**: `Versions` (explicit)
- **Key**: `Id(x => x.VersionsID)` — column `VersionsID`, generated Identity
- **Properties**: `VersionType` (Not.Nullable), `VersionValue` (Not.Nullable), `VersionRevision`, `VersionCommand`, `VersionProgram`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Database version tracking table. One row per `VersionType`.

### 44. VideoLocalMap.cs
- **Entity**: `VideoLocal`
- **Table**: `VideoLocal` (explicit)
- **Key**: `Id(x => x.VideoLocalID)` — column `VideoLocalID`, generated Identity
- **Properties**: `DateTimeUpdated` (Not.Nullable), `DateTimeCreated` (Not.Nullable), `DateTimeImported`, `FileName` (Not.Nullable, deprecated), `FileSize` (Not.Nullable), `Hash` (Not.Nullable), `HashSource` (Not.Nullable), `IsIgnored` (Not.Nullable), `IsVariation` (Not.Nullable), `MediaVersion` (Not.Nullable), `MediaInfo` (column: `MediaBlob`, nullable, CustomType `MessagePackConverter<MediaContainer>`), `MyListID` (Not.Nullable), `LastAVDumped`, `LastAVDumpVersion`
- **Custom Types**: `MessagePackConverter<MediaContainer>`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `MediaInfo` maps to column `MediaBlob` with MessagePack serialization. This is the critical file record — ED2K hash + file size identify uniqueness.

### 45. VideoLocal_HashDigestMap.cs
- **Entity**: `VideoLocal_HashDigest`
- **Table**: `VideoLocal_HashDigest` (explicit)
- **Key**: `Id(x => x.VideoLocal_HashDigestID)` — column `VideoLocal_HashDigestID`, generated Identity
- **Properties**: `VideoLocalID` (Not.Nullable), `Type` (Not.Nullable), `Value` (Not.Nullable), `Metadata`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Hash types table (ED2K, CRC32, MD5, SHA1) stored as `Type + Value` rows. `VideoLocalID` is FK.

### 46. VideoLocal_PlaceMap.cs
- **Entity**: `VideoLocal_Place`
- **Table**: `VideoLocal_Place` (explicit)
- **Key**: `Id(x => x.ID).Column("VideoLocal_Place_ID")` — column `VideoLocal_Place_ID`, generated Identity
- **Properties**: `VideoID` (column: `VideoLocalID`, Not.Nullable), `ManagedFolderID` (column: `ImportFolderID`, Not.Nullable), `RelativePath` (column: `FilePath`, Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Key property is `ID` mapping to column `VideoLocal_Place_ID`. Three custom column names: `VideoLocalID`, `ImportFolderID`, `FilePath`. `VideoID` is FK to `VideoLocal`, `ManagedFolderID` is FK to `ShokoManagedFolder`.

### 47. VideoLocal_UserMap.cs
- **Entity**: `VideoLocal_User`
- **Table**: `VideoLocal_User` (explicit)
- **Key**: `Id(x => x.VideoLocal_UserID)` — column `VideoLocal_UserID`, generated Identity
- **Properties**: `JMMUserID` (Not.Nullable), `VideoLocalID` (Not.Nullable), `WatchedDate`, `WatchedCount` (Not.Nullable), `ResumePosition` (Not.Nullable), `LastUpdated` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Per-user file watch data. `JMMUserID` and `VideoLocalID` are FK columns.

---

## CrossReference Subdirectory (3 files)

### 48. CrossRef_AniDB_TMDB_ShowMap.cs
- **Entity**: `CrossRef_AniDB_TMDB_Show`
- **Table**: `CrossRef_AniDB_TMDB_Show` (explicit)
- **Key**: `Id(x => x.CrossRef_AniDB_TMDB_ShowID)` — column `CrossRef_AniDB_TMDB_ShowID`, generated Identity
- **Properties**: `AnidbAnimeID` (Not.Nullable), `TmdbShowID` (Not.Nullable), `MatchRating` (Not.Nullable, CustomType `MatchRating`)
- **Custom Types**: `MatchRating`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: AniDB ↔ TMDB show mapping. `MatchRating` enum needs ValueConverter.

### 49. CrossRef_AniDB_TMDB_MovieMap.cs
- **Entity**: `CrossRef_AniDB_TMDB_Movie`
- **Table**: `CrossRef_AniDB_TMDB_Movie` (explicit)
- **Key**: `Id(x => x.CrossRef_AniDB_TMDB_MovieID)` — column `CrossRef_AniDB_TMDB_MovieID`, generated Identity
- **Properties**: `AnidbAnimeID` (Not.Nullable), `AnidbEpisodeID` (Not.Nullable), `TmdbMovieID` (Not.Nullable), `MatchRating` (Not.Nullable, CustomType `MatchRating`)
- **Custom Types**: `MatchRating`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: AniDB ↔ TMDB movie mapping. Links at episode level (OVAs/movies). `AnidbEpisodeID` can be null for series-level matching.

### 50. CrossRef_AniDB_TMDB_EpisodeMap.cs
- **Entity**: `CrossRef_AniDB_TMDB_Episode`
- **Table**: `CrossRef_AniDB_TMDB_Episode` (explicit)
- **Key**: `Id(x => x.CrossRef_AniDB_TMDB_EpisodeID)` — column `CrossRef_AniDB_TMDB_EpisodeID`, generated Identity
- **Properties**: `AnidbAnimeID` (Not.Nullable), `AnidbEpisodeID` (Not.Nullable), `TmdbShowID` (Not.Nullable), `TmdbEpisodeID` (Not.Nullable), `Ordering` (Not.Nullable), `MatchRating` (Not.Nullable, CustomType `MatchRating`)
- **Custom Types**: `MatchRating`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: AniDB ↔ TMDB episode mapping. `MatchRating` enum needs ValueConverter.

---

## TMDB Subdirectory (15 files)

### 51. TMDB_CompanyMap.cs
- **Entity**: `TMDB_Company`
- **Table**: `TMDB_Company` (explicit)
- **Key**: `Id(x => x.TMDB_CompanyID)` — column `TMDB_CompanyID`, generated Identity
- **Properties**: `TmdbCompanyID` (Not.Nullable), `Name` (Not.Nullable), `CountryOfOrigin` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: TMDB production company cache.

### 52. TMDB_Company_EntityMap.cs
- **Entity**: `TMDB_Company_Entity`
- **Table**: `TMDB_Company_Entity` (explicit)
- **Key**: `Id(x => x.TMDB_Company_EntityID)` — column `TMDB_Company_EntityID`, generated Identity
- **Properties**: `TmdbCompanyID` (Not.Nullable), `TmdbEntityType` (Not.Nullable, CustomType `ForeignEntityType`), `TmdbEntityID` (Not.Nullable), `Ordering` (Not.Nullable), `ReleasedAt` (CustomType `DateOnlyConverter`)
- **Custom Types**: `ForeignEntityType`, `DateOnlyConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Join table for production companies. `TmdbEntityType` enum needs ValueConverter. `ReleasedAt` uses `DateOnlyConverter`.

### 53. TMDB_EpisodeMap.cs
- **Entity**: `TMDB_Episode`
- **Table**: `TMDB_Episode` (explicit)
- **Key**: `Id(x => x.TMDB_EpisodeID)` — column `TMDB_EpisodeID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbSeasonID` (Not.Nullable), `TmdbEpisodeID` (Not.Nullable), `TvdbEpisodeID`, `ThumbnailPath`, `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `IsHidden` (Not.Nullable), `SeasonNumber` (Not.Nullable), `EpisodeNumber` (Not.Nullable), `RuntimeMinutes` (column: `Runtime`), `UserRating` (Not.Nullable), `UserVotes` (Not.Nullable), `AiredAt` (CustomType `DateOnlyConverter`), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: `DateOnlyConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `RuntimeMinutes` maps to column `Runtime` (custom column name). `AiredAt` uses `DateOnlyConverter`.

### 54. TMDB_Episode_CastMap.cs
- **Entity**: `TMDB_Episode_Cast`
- **Table**: `TMDB_Episode_Cast` (explicit)
- **Key**: `Id(x => x.TMDB_Episode_CastID)` — column `TMDB_Episode_CastID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbSeasonID` (Not.Nullable), `TmdbEpisodeID` (Not.Nullable), `TmdbPersonID` (Not.Nullable), `TmdbCreditID` (Not.Nullable), `CharacterName` (Not.Nullable), `IsGuestRole` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Episode cast with show/season/episode triple FK.

### 55. TMDB_Episode_CrewMap.cs
- **Entity**: `TMDB_Episode_Crew`
- **Table**: `TMDB_Episode_Crew` (explicit)
- **Key**: `Id(x => x.TMDB_Episode_CrewID)` — column `TMDB_Episode_CrewID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbSeasonID` (Not.Nullable), `TmdbEpisodeID` (Not.Nullable), `TmdbPersonID` (Not.Nullable), `TmdbCreditID` (Not.Nullable), `Job` (Not.Nullable), `Department` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Episode crew with show/season/episode triple FK.

### 56. TMDB_ImageMap.cs
- **Entity**: `TMDB_Image`
- **Table**: `TMDB_Image` (explicit)
- **Key**: `Id(x => x.TMDB_ImageID)` — column `TMDB_ImageID`, generated Identity
- **Properties**: `IsEnabled`, `Width` (Not.Nullable), `Height` (Not.Nullable), `Language` (Not.Nullable, CustomType `TitleLanguageConverter`), `RemoteFileName` (Not.Nullable), `UserRating` (Not.Nullable), `UserVotes` (Not.Nullable)
- **Custom Types**: `TitleLanguageConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Language` enum needs ValueConverter.

### 57. TMDB_Image_EntityMap.cs
- **Entity**: `TMDB_Image_Entity`
- **Table**: `TMDB_Image_Entity` (explicit)
- **Key**: `Id(x => x.TMDB_Image_EntityID)` — column `TMDB_Image_EntityID`, generated Identity
- **Properties**: `RemoteFileName` (Not.Nullable), `ImageType` (Not.Nullable, CustomType `ImageEntityType`), `TmdbEntityType` (Not.Nullable, CustomType `ForeignEntityType`), `TmdbEntityID` (Not.Nullable), `Ordering` (Not.Nullable), `ReleasedAt` (CustomType `DateOnlyConverter`)
- **Custom Types**: `ImageEntityType`, `ForeignEntityType`, `DateOnlyConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Image-to-entity join table. Two enum ValueConverters needed. `ReleasedAt` uses `DateOnlyConverter`.

### 58. TMDB_MovieMap.cs
- **Entity**: `TMDB_Movie`
- **Table**: `TMDB_Movie` (explicit)
- **Key**: `Id(x => x.TMDB_MovieID)` — column `TMDB_MovieID`, generated Identity
- **Properties**: `TmdbMovieID` (Not.Nullable), `TmdbCollectionID`, `ImdbMovieID`, `PosterPath`, `BackdropPath`, `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `OriginalTitle` (Not.Nullable), `OriginalLanguageCode` (Not.Nullable), `IsRestricted` (Not.Nullable), `IsVideo` (Not.Nullable), `Genres` (Not.Nullable, CustomType `StringListConverter`), `Keywords` (Not.Nullable, CustomType `StringListConverter`), `ContentRatings` (Not.Nullable, CustomType `TmdbContentRatingConverter`), `ProductionCountries` (Not.Nullable, CustomType `TmdbProductionCountryConverter`), `RuntimeMinutes` (column: `Runtime`), `UserRating` (Not.Nullable), `UserVotes` (Not.Nullable), `ReleasedAt` (CustomType `DateOnlyConverter`), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: `StringListConverter` (2), `TmdbContentRatingConverter`, `TmdbProductionCountryConverter`, `DateOnlyConverter`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `RuntimeMinutes` maps to column `Runtime`. `Genres` and `Keywords` use `StringListConverter` (JSON). `ContentRatings` and `ProductionCountries` use dedicated converters. `ReleasedAt` uses `DateOnlyConverter`.

### 59. TMDB_Movie_CastMap.cs
- **Entity**: `TMDB_Movie_Cast`
- **Table**: `TMDB_Movie_Cast` (explicit)
- **Key**: `Id(x => x.TMDB_Movie_CastID)` — column `TMDB_Movie_CastID`, generated Identity
- **Properties**: `TmdbMovieID` (Not.Nullable), `TmdbPersonID` (Not.Nullable), `TmdbCreditID` (Not.Nullable), `CharacterName` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Movie cast join table.

### 60. TMDB_Movie_CrewMap.cs
- **Entity**: `TMDB_Movie_Crew`
- **Table**: `TMDB_Movie_Crew` (explicit)
- **Key**: `Id(x => x.TMDB_Movie_CrewID)` — column `TMDB_Movie_CrewID`, generated Identity
- **Properties**: `TmdbMovieID` (Not.Nullable), `TmdbPersonID` (Not.Nullable), `TmdbCreditID` (Not.Nullable), `Job` (Not.Nullable), `Department` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Movie crew join table.

### 61. TMDB_PersonMap.cs
- **Entity**: `TMDB_Person`
- **Table**: `TMDB_Person` (explicit)
- **Key**: `Id(x => x.TMDB_PersonID)` — column `TMDB_PersonID`, generated Identity
- **Properties**: `TmdbPersonID` (Not.Nullable), `EnglishName` (Not.Nullable), `EnglishBiography` (Not.Nullable), `Aliases` (Not.Nullable, CustomType `StringListConverter`), `Gender` (Not.Nullable, CustomType `PersonGender`), `IsRestricted` (Not.Nullable), `BirthDay` (CustomType `DateOnlyConverter`), `DeathDay` (CustomType `DateOnlyConverter`), `PlaceOfBirth`, `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable), `LastOrphanedAt`
- **Custom Types**: `StringListConverter`, `PersonGender`, `DateOnlyConverter` (2)
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Aliases` uses `StringListConverter` (JSON). `Gender` enum needs ValueConverter. `BirthDay` and `DeathDay` use `DateOnlyConverter`.

### 62. TMDB_SeasonMap.cs
- **Entity**: `TMDB_Season`
- **Table**: `TMDB_Season` (explicit)
- **Key**: `Id(x => x.TMDB_SeasonID)` — column `TMDB_SeasonID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbSeasonID` (Not.Nullable), `PosterPath`, `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `EpisodeCount` (Not.Nullable), `HiddenEpisodeCount` (Not.Nullable), `SeasonNumber` (Not.Nullable), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: TMDB season cache. `TmdbShowID` is FK.

### 63. TMDB_ShowMap.cs
- **Entity**: `TMDB_Show`
- **Table**: `TMDB_Show` (explicit)
- **Key**: `Id(x => x.TMDB_ShowID)` — column `TMDB_ShowID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TvdbShowID`, `PosterPath`, `BackdropPath`, `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `OriginalTitle` (Not.Nullable), `OriginalLanguageCode` (Not.Nullable), `IsRestricted` (Not.Nullable), `Genres` (Not.Nullable, CustomType `StringListConverter`), `Keywords` (Not.Nullable, CustomType `StringListConverter`), `ContentRatings` (Not.Nullable, CustomType `TmdbContentRatingConverter`), `ProductionCountries` (Not.Nullable, CustomType `TmdbProductionCountryConverter`), `EpisodeCount` (Not.Nullable), `HiddenEpisodeCount` (Not.Nullable), `SeasonCount` (Not.Nullable), `AlternateOrderingCount` (Not.Nullable), `UserRating` (Not.Nullable), `UserVotes` (Not.Nullable), `FirstAiredAt` (CustomType `DateOnlyConverter`), `LastAiredAt` (CustomType `DateOnlyConverter`), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable), `PreferredAlternateOrderingID`
- **Custom Types**: `StringListConverter` (2), `TmdbContentRatingConverter`, `TmdbProductionCountryConverter`, `DateOnlyConverter` (2)
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Genres` and `Keywords` use `StringListConverter` (JSON). `ContentRatings` and `ProductionCountries` use dedicated converters. `FirstAiredAt` and `LastAiredAt` use `DateOnlyConverter`.

### 64. Text/TMDB_TitleMap.cs
- **Entity**: `TMDB_Title`
- **Table**: `TMDB_Title` (explicit)
- **Key**: `Id(x => x.TMDB_TitleID)` — column `TMDB_TitleID`, generated Identity
- **Properties**: `ParentID` (Not.Nullable), `ParentType` (Not.Nullable, CustomType `ForeignEntityType`), `LanguageCode` (Not.Nullable), `CountryCode` (Not.Nullable), `Value` (Not.Nullable)
- **Custom Types**: `ForeignEntityType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Multi-language title storage. `ParentType` enum needs ValueConverter. Generic parent reference via `ParentID` + `ParentType`.

### 66. Text/TMDB_OverviewMap.cs
- **Entity**: `TMDB_Overview`
- **Table**: `TMDB_Overview` (explicit)
- **Key**: `Id(x => x.TMDB_OverviewID)` — column `TMDB_OverviewID`, generated Identity
- **Properties**: `ParentID` (Not.Nullable), `ParentType` (Not.Nullable, CustomType `ForeignEntityType`), `LanguageCode` (Not.Nullable), `CountryCode` (Not.Nullable), `Value` (Not.Nullable)
- **Custom Types**: `ForeignEntityType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Multi-language overview storage. Same pattern as `TMDB_Title`. `ParentType` enum needs ValueConverter.

---

## TMDB/Optional Subdirectory (7 files)

### 66. TMDB_CollectionMap.cs
- **Entity**: `TMDB_Collection`
- **Table**: `TMDB_Collection` (explicit)
- **Key**: `Id(x => x.TMDB_CollectionID)` — column `TMDB_CollectionID`, generated Identity
- **Properties**: `TmdbCollectionID` (Not.Nullable), `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `MovieCount` (Not.Nullable), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: TMDB movie collection cache.

### 67. TMDB_Collection_MovieMap.cs
- **Entity**: `TMDB_Collection_Movie`
- **Table**: `TMDB_Collection_Movie` (explicit)
- **Key**: `Id(x => x.TMDB_Collection_MovieID)` — column `TMDB_Collection_MovieID`, generated Identity
- **Properties**: `TmdbCollectionID` (Not.Nullable), `TmdbMovieID` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Join table for TMDB movie collections.

### 68. TMDB_NetworkMap.cs
- **Entity**: `TMDB_Network`
- **Table**: `TMDB_Network` (explicit)
- **Key**: `Id(x => x.TMDB_NetworkID)` — column `TMDB_NetworkID`, generated Identity
- **Properties**: `TmdbNetworkID` (Not.Nullable), `Name` (Not.Nullable), `CountryOfOrigin` (Not.Nullable), `LastOrphanedAt`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: TMDB production network cache.

### 69. TMDB_Show_NetworkMap.cs
- **Entity**: `TMDB_Show_Network`
- **Table**: `TMDB_Show_Network` (explicit)
- **Key**: `Id(x => x.TMDB_Show_NetworkID)` — column `TMDB_Show_NetworkID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbNetworkID` (Not.Nullable), `Ordering` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Show-to-network join table.

### 70. TMDB_AlternateOrderingMap.cs
- **Entity**: `TMDB_AlternateOrdering`
- **Table**: `TMDB_AlternateOrdering` (explicit)
- **Key**: `Id(x => x.TMDB_AlternateOrderingID)` — column `TMDB_AlternateOrderingID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbNetworkID`, `TmdbEpisodeGroupCollectionID` (Not.Nullable), `EnglishTitle` (Not.Nullable), `EnglishOverview` (Not.Nullable), `EpisodeCount` (Not.Nullable), `HiddenEpisodeCount` (Not.Nullable), `SeasonCount` (Not.Nullable), `Type` (Not.Nullable, CustomType `AlternateOrderingType`), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: `AlternateOrderingType`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: `Type` enum needs ValueConverter. `TmdbShowID` and `TmdbNetworkID` are FK columns.

### 71. TMDB_AlternateOrdering_SeasonMap.cs
- **Entity**: `TMDB_AlternateOrdering_Season`
- **Table**: `TMDB_AlternateOrdering_Season` (explicit)
- **Key**: `Id(x => x.TMDB_AlternateOrdering_SeasonID)` — column `TMDB_AlternateOrdering_SeasonID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbEpisodeGroupCollectionID` (Not.Nullable), `TmdbEpisodeGroupID` (Not.Nullable), `EnglishTitle` (Not.Nullable), `EpisodeCount` (Not.Nullable), `HiddenEpisodeCount` (Not.Nullable), `SeasonNumber` (Not.Nullable), `IsLocked` (Not.Nullable), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Season within alternate ordering. `TmdbShowID` and `TmdbEpisodeGroupCollectionID` are FK columns.

### 72. TMDB_AlternateOrdering_EpisodeMap.cs
- **Entity**: `TMDB_AlternateOrdering_Episode`
- **Table**: `TMDB_AlternateOrdering_Episode` (explicit)
- **Key**: `Id(x => x.TMDB_AlternateOrdering_EpisodeID)` — column `TMDB_AlternateOrdering_EpisodeID`, generated Identity
- **Properties**: `TmdbShowID` (Not.Nullable), `TmdbEpisodeGroupCollectionID` (Not.Nullable), `TmdbEpisodeGroupID` (Not.Nullable), `TmdbEpisodeID` (Not.Nullable), `SeasonNumber` (Not.Nullable), `EpisodeNumber` (Not.Nullable), `CreatedAt` (Not.Nullable), `LastUpdatedAt` (Not.Nullable)
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()`
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Episode within alternate ordering. `TmdbShowID`, `TmdbEpisodeGroupCollectionID`, and `TmdbEpisodeGroupID` are FK columns.

---

## Root Trakt Mappings (3 files)

### 73. Trakt_ShowMap.cs
- **Entity**: `Trakt_Show`
- **Table**: `Trakt_Show` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.Trakt_ShowID)` — column `Trakt_ShowID`, generated Identity
- **Properties**: `TraktID`, `TmdbShowID`, `Title`, `Year`, `URL`, `Overview`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `Trakt_Show`).

### 74. Trakt_EpisodeMap.cs
- **Entity**: `Trakt_Episode`
- **Table**: `Trakt_Episode` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.Trakt_EpisodeID)` — column `Trakt_EpisodeID`, generated Identity
- **Properties**: `Trakt_ShowID` (Not.Nullable), `EpisodeNumber`, `Overview` (nullable, CustomType `StringClob`), `Season` (Not.Nullable), `Title`, `URL`, `TraktID`
- **Custom Types**: `StringClob`
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `Trakt_Episode`). `Overview` uses `StringClob`. `Trakt_ShowID` is FK.

### 75. Trakt_SeasonMap.cs
- **Entity**: `Trakt_Season`
- **Table**: `Trakt_Season` (implicit — no `Table()` call, class name used)
- **Key**: `Id(x => x.Trakt_SeasonID)` — column `Trakt_SeasonID`, generated Identity
- **Properties**: `Season` (Not.Nullable), `Trakt_ShowID` (Not.Nullable), `URL`
- **Custom Types**: None
- **Indexes/Unique Constraints**: None defined in mapping
- **Relationships**: None
- **LazyLoad**: `Not.LazyLoad()` (no `Table()` call but `Not.LazyLoad()` present)
- **ReadOnly**: None
- **Cascade**: None
- **EF Core Notes**: Table name is implicit (class name `Trakt_Season`). `Trakt_ShowID` is FK.

---

## T009 partial — Core Shoko relationships

**Task**: T009 — Relationship inventory for core Shoko domain entities  
**Generated**: 2026-05-07  
**Entities**: 11 (AnimeSeries, AnimeGroup, AnimeEpisode, VideoLocal, VideoLocal_Place, ShokoManagedFolder, JMMUser, AnimeEpisode_User, AnimeSeries_User, AnimeGroup_User, VideoLocal_User)

### 1. AnimeSeries

- **FK Columns**: `AniDB_ID` → `AniDB_Anime.AniDB_AnimeID` (Not.Nullable), `AnimeGroupID` → `AnimeGroup.AnimeGroupID` (Not.Nullable)
- **Navigation Properties (model code)**:
  - `AniDB_Anime` — lazy-loaded via `RepoFactory.AniDB_Anime.GetByAnimeID(AniDB_ID)` (line 555)
  - `AnimeGroup` — `RepoFactory.AnimeGroup.GetByID(AnimeGroupID)` (line 659)
  - `TopLevelAnimeGroup` — recursive traversal via `AnimeGroup.AnimeGroupParentID` (line 664)
  - `AllGroupsAbove` — list of all parent groups (line 682)
  - `AnimeEpisodes` / `AllAnimeEpisodes` — `RepoFactory.AnimeEpisode.GetBySeriesID(AnimeSeriesID)` filtered/sorted (lines 476, 484)
  - `VideoLocals` — `RepoFactory.VideoLocal.GetByAniDBAnimeID(AniDB_ID)` (line 470)
  - `FileCrossReferences` — `RepoFactory.CrossRef_File_Episode.GetByAnimeID(AniDB_ID)` (line 468)
  - `TmdbShows` / `TmdbShowCrossReferences` — via `CrossRef_AniDB_TMDB_Show` (lines 569, 571)
  - `TmdbMovies` / `TmdbMovieCrossReferences` — via `CrossRef_AniDB_TMDB_Movie` (lines 561, 563)
  - `TmdbSeasons` / `TmdbSeasonCrossReferences` — via `CrossRef_AniDB_TMDB_Season` (lines 582, 589)
  - `TmdbEpisodeCrossReferences` — via `CrossRef_AniDB_TMDB_Episode` (line 576)
  - `MalCrossReferences` — via `CrossRef_AniDB_MAL` (line 606)
- **EF Core Configuration**: `HasOne<AniDB_Anime>().WithOne().HasForeignKey<AnimeSeries>(x => x.AniDB_ID).IsRequired()` + `HasOne<AnimeGroup>().WithMany().HasForeignKey(x => x.AnimeGroupID).IsRequired()`. All other navigations are repository-constructed.
- **NHibernate Mappings**: `AnimeSeriesMap.cs` — no relationships defined; FK columns mapped as simple properties.
- **Parity Risks**: `AniDB_ID` is a logical 1:1 with `AniDB_Anime` but stored as FK column on `AnimeSeries`. EF Core should configure as `HasOne/WithOne` with required FK. `AnimeGroupID` is a many-to-one (many series can belong to one group).

### 2. AnimeGroup

- **FK Columns**: `AnimeGroupParentID` → `AnimeGroup.AnimeGroupID` (nullable, self-referential), `DefaultAnimeSeriesID` → `AnimeSeries.AnimeSeriesID` (nullable), `MainAniDBAnimeID` → `AniDB_Anime.AniDB_AnimeID` (nullable)
- **Navigation Properties (model code)**:
  - `Parent` — `RepoFactory.AnimeGroup.GetByID(AnimeGroupParentID.Value)` (line 68)
  - `Children` — `RepoFactory.AnimeGroup.GetByParentID(AnimeGroupID)` (line 127)
  - `AllChildren` — recursive traversal of children (line 129)
  - `AllGroupsAbove` — list of all parent groups (line 70)
  - `Series` — `RepoFactory.AnimeSeries.GetByGroupID(AnimeGroupID)` (line 171)
  - `AllSeries` — recursive traversal of all series in group hierarchy (line 191)
  - `MainSeries` — via `DefaultAnimeSeriesID` or `MainAniDBAnimeID` (line 148)
  - `TopLevelAnimeGroup` — recursive traversal to root (line 270)
  - `Anime` — series list mapped to AniDB_Anime (line 94)
  - `Tags` / `CustomTags` — aggregated from all series (lines 236, 244)
- **EF Core Configuration**: `HasMany<AnimeGroup>().WithOne().HasForeignKey(x => x.AnimeGroupParentID).OnDelete(DeleteBehavior.Restrict)` (self-referential). `HasOne<AnimeSeries>().WithMany().HasForeignKey(x => x.DefaultAnimeSeriesID).IsRequired(false).OnDelete(DeleteBehavior.Restrict)`.
- **NHibernate Mappings**: `AnimeGroupMap.cs` — no relationships defined.
- **Parity Risks**: Self-referential hierarchy via `AnimeGroupParentID`. `DefaultAnimeSeriesID` is nullable FK to `AnimeSeries`. `MainAniDBAnimeID` is nullable FK to `AniDB_Anime` (not `AnimeSeries`).

### 3. AnimeEpisode

- **FK Columns**: `AniDB_EpisodeID` → `AniDB_Episode.AniDB_EpisodeID` (Not.Nullable), `AnimeSeriesID` → `AnimeSeries.AnimeSeriesID` (Not.Nullable)
- **Navigation Properties (model code)**:
  - `AnimeSeries` — `RepoFactory.AnimeSeries.GetByID(AnimeSeriesID)` (line 308)
  - `AniDB_Episode` — `RepoFactory.AniDB_Episode.GetByEpisodeID(AniDB_EpisodeID)` (line 321)
  - `AniDB_Anime` — via `AniDB_Episode.AniDB_Anime` (line 323)
  - `VideoLocals` — `RepoFactory.VideoLocal.GetByAniDBEpisodeID(AniDB_EpisodeID)` (line 311)
  - `FileCrossReferences` — `RepoFactory.CrossRef_File_Episode.GetByEpisodeID(AniDB_EpisodeID)` (line 314)
  - `TmdbEpisodes` / `TmdbEpisodeCrossReferences` — via `CrossRef_AniDB_TMDB_Episode` (lines 338, 341)
  - `TmdbMovies` / `TmdbMovieCrossReferences` — via `CrossRef_AniDB_TMDB_Movie` (lines 329, 332)
  - `GetUserRecord(int)` — `RepoFactory.AnimeEpisode_User.GetByUserAndEpisodeID()` (line 302)
- **EF Core Configuration**: `HasOne<AnimeSeries>().WithMany().HasForeignKey(x => x.AnimeSeriesID).IsRequired()`. `HasOne<AniDB_Episode>().WithOne().HasForeignKey<AnimeEpisode>(x => x.AniDB_EpisodeID).IsRequired()`.
- **NHibernate Mappings**: `AnimeEpisodeMap.cs` — no relationships defined.
- **Parity Risks**: `AniDB_EpisodeID` is logical 1:1 with `AniDB_Episode`. `AnimeSeriesID` is many-to-one.

### 4. VideoLocal

- **FK Columns**: None directly. Identified by `Hash` (ED2K) + `FileSize`.
- **Navigation Properties (model code)**:
  - `Places` — `RepoFactory.VideoLocalPlace.GetByVideoLocal(VideoLocalID)` (line 111)
  - `Hashes` — `RepoFactory.VideoLocalHashDigest.GetByVideoLocalID(VideoLocalID)` (line 108)
  - `ReleaseInfo` — `RepoFactory.StoredReleaseInfo.GetByEd2kAndFileSize(Hash, FileSize)` (line 114)
  - `AnimeEpisodes` — `RepoFactory.AnimeEpisode.GetByHash(Hash)` (line 120)
  - `EpisodeCrossReferences` — `RepoFactory.CrossRef_File_Episode.GetByEd2k(Hash)` (line 123)
  - `FirstValidPlace` / `FirstResolvedPlace` — filtered from `Places` (lines 126, 129)
- **EF Core Configuration**: No direct FK relationships. `VideoLocal_Place` is the linking entity via `VideoID` FK. `VideoLocal_HashDigest` links via `VideoLocalID` FK.
- **NHibernate Mappings**: `VideoLocalMap.cs` — no relationships defined.
- **Parity Risks**: `VideoLocal` is the canonical file record. Its relationships are all resolved through join/bridge tables (`VideoLocal_Place`, `VideoLocal_HashDigest`, `CrossRef_File_Episode`).

### 5. VideoLocal_Place

- **FK Columns**: `VideoID` → `VideoLocal.VideoLocalID` (Not.Nullable, column: `VideoLocalID`), `ManagedFolderID` → `ShokoManagedFolder.ID` (Not.Nullable, column: `ImportFolderID`)
- **Navigation Properties (model code)**:
  - `VideoLocal` — `RepoFactory.VideoLocal.GetByID(VideoID)` (line 77)
  - `ManagedFolder` — `RepoFactory.ShokoManagedFolder.GetByID(ManagedFolderID)` (line 83)
  - `Path` — computed from `ManagedFolder.Path + RelativePath` (line 48)
  - `IsAvailable` — `File.Exists(Path)` (line 65)
  - `FileInfo` — `new FileInfo(Path)` if available (line 89)
- **EF Core Configuration**: `HasOne<VideoLocal>().WithMany(x => x.Places).HasForeignKey(x => x.VideoID).IsRequired().OnDelete(DeleteBehavior.Cascade)`. `HasOne<ShokoManagedFolder>().WithMany(x => x.Places).HasForeignKey(x => x.ManagedFolderID).IsRequired().OnDelete(DeleteBehavior.Restrict)`.
- **NHibernate Mappings**: `VideoLocal_PlaceMap.cs` — no relationships defined. Column names: `VideoLocalID`, `ImportFolderID`, `FilePath`.
- **Parity Risks**: Bridge table connecting `VideoLocal` to `ShokoManagedFolder` via relative path. One `VideoLocal` can have multiple places (same file duplicated across folders). Key property is `ID` → `VideoLocal_Place_ID` (custom column).

### 6. ShokoManagedFolder

- **FK Columns**: None.
- **Navigation Properties (model code)**:
  - `Places` — `RepoFactory.VideoLocalPlace.GetByManagedFolderID(ID)` (line 92)
- **EF Core Configuration**: `HasMany<VideoLocal_Place>().WithOne(x => x.ManagedFolder).HasForeignKey(x => x.ManagedFolderID).OnDelete(DeleteBehavior.Restrict)`.
- **NHibernate Mappings**: `ShokoManagedFolderMap.cs` — no relationships defined. Table name: `ImportFolder`. Key: `ID` → `ImportFolderID`. Custom columns: `ImportFolderLocation` (Path), `ImportFolderName` (Name).
- **Parity Risks**: Table name differs from class name (`ImportFolder` vs `ShokoManagedFolder`). EF Core must use `ToTable("ImportFolder")`.

### 7. JMMUser

- **FK Columns**: None.
- **Navigation Properties (model code)**: None directly. User tracking via junction tables (`AnimeEpisode_User`, `AnimeSeries_User`, `AnimeGroup_User`, `VideoLocal_User`).
- **EF Core Configuration**: No outgoing FK relationships. Referenced by junction tables.
- **NHibernate Mappings**: `JMMUserMap.cs` — no relationships defined.
- **Parity Risks**: User entity is a leaf in the FK graph. All user relationships are many-to-many via junction tables.

### 8. AnimeEpisode_User

- **FK Columns**: `JMMUserID` → `JMMUser.JMMUserID` (Not.Nullable), `AnimeEpisodeID` → `AnimeEpisode.AnimeEpisodeID` (Not.Nullable), `AnimeSeriesID` → `AnimeSeries.AnimeSeriesID` (Not.Nullable)
- **Navigation Properties (model code)**:
  - `AnimeSeries` — `RepoFactory.AnimeSeries.GetByID(AnimeSeriesID)` (line 116)
  - `AnimeEpisode` — `RepoFactory.AnimeEpisode.GetByID(AnimeEpisodeID)` (line 118)
  - `User` — `RepoFactory.JMMUser.GetByID(JMMUserID)` (line 126)
- **EF Core Configuration**: `HasOne<JMMUser>().WithMany().HasForeignKey(x => x.JMMUserID).IsRequired()`. `HasOne<AnimeEpisode>().WithMany().HasForeignKey(x => x.AnimeEpisodeID).IsRequired()`. `HasOne<AnimeSeries>().WithMany().HasForeignKey(x => x.AnimeSeriesID).IsRequired()`. Unique index on `(JMMUserID, AnimeEpisodeID)` for one-row-per-user-per-episode semantics.
- **NHibernate Mappings**: `AnimeEpisode_UserMap.cs` — no relationships defined. `UserTags` uses `StringListConverter` (delimiter-separated string).
- **Parity Risks**: Junction table with 3 FK columns. `AnimeSeriesID` is redundant (derivable from `AnimeEpisode.AnimeSeriesID`) but stored for query performance. `UserTags` needs `ValueConverter<List<string>, string>` with `"|||"` delimiter.

### 9. AnimeSeries_User

- **FK Columns**: `JMMUserID` → `JMMUser.JMMUserID` (Not.Nullable), `AnimeSeriesID` → `AnimeSeries.AnimeSeriesID` (Not.Nullable)
- **Navigation Properties (model code)**:
  - `AnimeSeries` — `RepoFactory.AnimeSeries.GetByID(AnimeSeriesID)` (line 156)
  - `User` — `RepoFactory.JMMUser.GetByID(JMMUserID)` (line 164)
  - `AnidbAnimeID` — lazy via `RepoFactory.AnimeSeries.GetByID(AnimeSeriesID)?.AniDB_ID` (line 34)
- **EF Core Configuration**: `HasOne<JMMUser>().WithMany().HasForeignKey(x => x.JMMUserID).IsRequired()`. `HasOne<AnimeSeries>().WithMany().HasForeignKey(x => x.AnimeSeriesID).IsRequired()`. Unique index on `(JMMUserID, AnimeSeriesID)`.
- **NHibernate Mappings**: `AnimeSeries_UserMap.cs` — no relationships defined. `UserRatingVoteType` uses `SeriesVoteType` CustomType. `UserTags` uses `StringListConverter`.
- **Parity Risks**: Junction table with 2 FK columns. `UserRatingVoteType` enum needs ValueConverter. `UserTags` needs delimiter-separated ValueConverter.

### 10. AnimeGroup_User

- **FK Columns**: `JMMUserID` → `JMMUser.JMMUserID` (nullable), `AnimeGroupID` → `AnimeGroup.AnimeGroupID` (nullable)
- **Navigation Properties (model code)**: None directly.
- **EF Core Configuration**: `HasOne<JMMUser>().WithMany().HasForeignKey(x => x.JMMUserID).OnDelete(DeleteBehavior.Restrict).IsRequired(false)`. `HasOne<AnimeGroup>().WithMany().HasForeignKey(x => x.AnimeGroupID).OnDelete(DeleteBehavior.Restrict).IsRequired(false)`.
- **NHibernate Mappings**: `AnimeGroup_UserMap.cs` — no relationships defined.
- **Parity Risks**: Both FK columns are nullable (unlike other user junction tables). This allows orphaned user-group tracking records.

### 11. VideoLocal_User

- **FK Columns**: `JMMUserID` → `JMMUser.JMMUserID` (Not.Nullable), `VideoLocalID` → `VideoLocal.VideoLocalID` (Not.Nullable)
- **Navigation Properties (model code)**:
  - `User` — `RepoFactory.JMMUser.GetByID(JMMUserID)` (line 35)
  - `VideoLocal` — `RepoFactory.VideoLocal.GetByID(VideoLocalID)` (line 41)
- **EF Core Configuration**: `HasOne<JMMUser>().WithMany().HasForeignKey(x => x.JMMUserID).IsRequired()`. `HasOne<VideoLocal>().WithMany().HasForeignKey(x => x.VideoLocalID).IsRequired()`. Unique index on `(JMMUserID, VideoLocalID)`.
- **NHibernate Mappings**: `VideoLocal_UserMap.cs` — no relationships defined.
- **Parity Risks**: Junction table with 2 FK columns. One-row-per-user-per-video semantics.

---

## Relationship Summary — Core Shoko Domain

| Relationship | Source Entity | FK Column | Target Entity | Cardinality | Nullable |
|---|---|---|---|---|---|
| Series → Group | `AnimeSeries` | `AnimeGroupID` | `AnimeGroup` | Many-to-One | No |
| Series → AniDB | `AnimeSeries` | `AniDB_ID` | `AniDB_Anime` | One-to-One | No |
| Group → Parent Group | `AnimeGroup` | `AnimeGroupParentID` | `AnimeGroup` | Self-referential | Yes |
| Group → Default Series | `AnimeGroup` | `DefaultAnimeSeriesID` | `AnimeSeries` | One-to-One | Yes |
| Group → Main AniDB | `AnimeGroup` | `MainAniDBAnimeID` | `AniDB_Anime` | One-to-One | Yes |
| Episode → Series | `AnimeEpisode` | `AnimeSeriesID` | `AnimeSeries` | Many-to-One | No |
| Episode → AniDB | `AnimeEpisode` | `AniDB_EpisodeID` | `AniDB_Episode` | One-to-One | No |
| Place → Video | `VideoLocal_Place` | `VideoID` | `VideoLocal` | Many-to-One | No |
| Place → Folder | `VideoLocal_Place` | `ManagedFolderID` | `ShokoManagedFolder` | Many-to-One | No |
| Episode_User → User | `AnimeEpisode_User` | `JMMUserID` | `JMMUser` | Many-to-One | No |
| Episode_User → Episode | `AnimeEpisode_User` | `AnimeEpisodeID` | `AnimeEpisode` | Many-to-One | No |
| Episode_User → Series | `AnimeEpisode_User` | `AnimeSeriesID` | `AnimeSeries` | Many-to-One | No |
| Series_User → User | `AnimeSeries_User` | `JMMUserID` | `JMMUser` | Many-to-One | No |
| Series_User → Series | `AnimeSeries_User` | `AnimeSeriesID` | `AnimeSeries` | Many-to-One | No |
| Group_User → User | `AnimeGroup_User` | `JMMUserID` | `JMMUser` | Many-to-One | Yes |
| Group_User → Group | `AnimeGroup_User` | `AnimeGroupID` | `AnimeGroup` | Many-to-One | Yes |
| Video_User → User | `VideoLocal_User` | `JMMUserID` | `JMMUser` | Many-to-One | No |
| Video_User → Video | `VideoLocal_User` | `VideoLocalID` | `VideoLocal` | Many-to-One | No |

**Total FK relationships**: 18 (16 non-nullable, 2 nullable in `AnimeGroup_User`)  
**Self-referential**: 1 (`AnimeGroup.AnimeGroupParentID`)  
**One-to-One logical**: 2 (`AnimeSeries.AniDB_ID`, `AnimeEpisode.AniDB_EpisodeID`)  
**Bridge tables**: 2 (`VideoLocal_Place`, `VideoLocal_HashDigest` — now documented in separate section)  
**User tracking junction tables**: 4 (`AnimeEpisode_User`, `AnimeSeries_User`, `AnimeGroup_User`, `VideoLocal_User`)

---

## T009 partial — CrossReference relationships

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Generated**: 2026-05-07  
**Entities**: 4 (CrossRef_AniDB_MAL, CrossRef_AniDB_TraktV2, CrossRef_File_Episode, CrossRef_CustomTag)

### 1. CrossRef_AniDB_MAL

- **FK Columns**: `AnimeID` (maps to `AniDB_Anime.AniDB_AnimeID`), `MALID` (maps to `MAL_Anime.MALID`)
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.CrossRef_AniDB_MAL.GetByAnimeID()` and `GetByMALID()`
- **EF Core Configuration**: `HasOne<AniDB_Anime>().WithMany().HasForeignKey(x => x.AnimeID).IsRequired()` + `HasOne<MAL_Anime>().WithMany().HasForeignKey(x => x.MALID).IsRequired()`. Unique index on `(AnimeID, MALID)`.
- **NHibernate Mappings**: `CrossRef_AniDB_MALMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Simple bridge table. `AnimeID` and `MALID` are both non-nullable integers. EF Core must configure both FK relationships explicitly.

### 2. CrossRef_AniDB_TraktV2

- **FK Columns**: `AnimeID` (maps to `AniDB_Anime.AniDB_AnimeID`)
- **Navigation Properties (model code)**: None directly — accessed via repository
- **EF Core Configuration**: `HasOne<AniDB_Anime>().WithMany().HasForeignKey(x => x.AnimeID).IsRequired()`. Note: `TraktID` is a string (not integer FK) — references `Trakt_Show`/`Trakt_Episode` by external ID.
- **NHibernate Mappings**: `CrossRef_AniDB_TraktV2Map.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Complex bridge with season/episode alignment data (`AniDBStartEpisodeType`, `AniDBStartEpisodeNumber`, `TraktStartEpisodeNumber`, `TraktSeasonNumber`). `TraktID` is string (not FK column). `CrossRefSource` and `AniDBStartEpisodeType` are integer enums.

### 3. CrossRef_File_Episode

- **FK Columns**: `EpisodeID` (maps to `AniDB_Episode.EpisodeID`), `AnimeID` (maps to `AniDB_Anime.AniDB_AnimeID`)
- **Navigation Properties (model code)**:
  - `VideoLocal` — `RepoFactory.VideoLocal.GetByEd2k(Hash)` (line 50)
  - `AniDBEpisode` — `RepoFactory.AniDB_Episode.GetByEpisodeID(EpisodeID)` (line 52)
  - `AnimeEpisode` — `RepoFactory.AnimeEpisode.GetByAniDBEpisodeID(EpisodeID)` (line 54)
  - `AniDBAnime` — `RepoFactory.AniDB_Anime.GetByAnimeID(AnimeID)` (line 56)
  - `AnimeSeries` — `RepoFactory.AnimeSeries.GetByAnimeID(AnimeID)` (line 58)
  - `ReleaseInfo` — `RepoFactory.StoredReleaseInfo.GetByEd2kAndFileSize(Hash, FileSize)` (line 60)
- **EF Core Configuration**: `HasOne<AniDB_Episode>().WithMany().HasForeignKey(x => x.EpisodeID).IsRequired()`. `HasOne<AniDB_Anime>().WithMany().HasForeignKey(x => x.AnimeID).IsRequired()`. Unique index on `(Hash, FileSize)` for file identification.
- **NHibernate Mappings**: `CrossRef_File_EpisodeMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Heavy model with computed properties (`PercentageRange`, `IsManuallyLinked`). `Hash` + `FileSize` form composite key for file identification. `AnimeID` can be 0 (meaning "unknown anime"). Complex percentage calculation logic for multi-part files (lines 66-126).

### 4. CrossRef_CustomTag

- **FK Columns**: `CustomTagID` (maps to `CustomTag.CustomTagID`), `CrossRefID` (maps to target entity's ID — polymorphic)
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.CrossRef_CustomTag.GetByCustomTagID()` and `GetByAnimeID()`
- **EF Core Configuration**: `HasOne<CustomTag>().WithMany().HasForeignKey(x => x.CustomTagID).IsRequired()`. `CrossRefID` + `CrossRefType` form polymorphic reference — EF Core cannot model this as a traditional FK; requires owned type or separate tables per target entity.
- **NHibernate Mappings**: `CrossRef_CustomTagMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: `CrossRefType` enum (value 1 = AnimeSeries) determines which entity `CrossRefID` references. EF Core cannot model polymorphic FKs natively — requires separate junction tables per target type or owned entity pattern.

---

## T009 partial — Miscellaneous relationships

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Generated**: 2026-05-07  
**Entities**: 11 (CustomTag, FileNameHash, FilterPreset, Playlist, Scan, ScanFile, ScheduledUpdate, StoredReleaseInfo, StoredReleaseInfo_MatchAttempt, AuthTokens, Versions)

### 1. CustomTag

- **FK Columns**: None.
- **Navigation Properties (model code)**:
  - `AllShokoSeries` — `RepoFactory.CrossRef_CustomTag.GetByCustomTagID(CustomTagID).Select(xref => RepoFactory.AnimeSeries.GetByAnimeID(xref.CrossRefID))` (lines 38-41)
- **EF Core Configuration**: No outgoing FK relationships. Referenced by `CrossRef_CustomTag.CustomTagID`.
- **NHibernate Mappings**: `CustomTagMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Leaf entity. All relationships are polymorphic via `CrossRef_CustomTag`.

### 2. FileNameHash

- **FK Columns**: None.
- **Navigation Properties (model code)**: None — used as intermediate cache for hash lookups
- **EF Core Configuration**: No relationships. Table name is explicit (`FileNameHash`).
- **NHibernate Mappings**: `FileNameHashMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Cache table — `FileName + FileSize → Hash` mapping. No relationships to other entities.

### 3. FilterPreset

- **FK Columns**: `ParentFilterPresetID` → `FilterPreset.FilterPresetID` (nullable)
- **Navigation Properties (model code)**:
  - `Parent` — self-referential nullable FK (line 37)
  - `Children` — self-referential collection (line 39)
- **EF Core Configuration**: `HasOne<FilterPreset>().WithMany(x => x.Children).HasForeignKey(x => x.ParentFilterPresetID).OnDelete(DeleteBehavior.Restrict).IsRequired(false)`. Self-referential relationship.
- **NHibernate Mappings**: `FilterPresetMap.cs` — defines `References(a => a.Parent).Column("ParentFilterPresetID").ReadOnly()` and `HasMany(x => x.Children).Fetch.Join().KeyColumn("ParentFilterPresetID").ReadOnly()`.
- **Parity Risks**: Self-referential hierarchy. `ParentFilterPresetID` is nullable (top-level presets have null or 0). Two hardcoded system presets (`-1` = Seasons, `-2` = Tags, `-3` = Years). `Expression` and `SortingExpression` use custom `FilterExpressionConverter` for JSON serialization.

### 4. Playlist

- **FK Columns**: None.
- **Navigation Properties (model code)**: None — legacy entity, `PlaylistItems` is comma-separated string of item IDs
- **EF Core Configuration**: No relationships. Table name is implicit (class name `Playlist`).
- **NHibernate Mappings**: `PlaylistMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Legacy entity. `PlaylistItems` is a comma-separated string (not a proper junction table). `PlaylistName`, `DefaultPlayOrder`, `PlayWatched`, `PlayUnwatched` are simple scalar columns.

### 5. Scan

- **FK Columns**: None (directly). `ImportFolders` is comma-separated string of `ImportFolderID` values.
- **Navigation Properties (model code)**:
  - `ImportFolders` — parsed from comma-separated string via `RepoFactory.ShokoManagedFolder.GetByID()` (lines 23-31 in model)
- **EF Core Configuration**: No outgoing FK relationships. `ImportFolders` needs `ValueConverter<string, List<int>>` with `","` delimiter.
- **NHibernate Mappings**: `ScanMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: `ImportFolders` is a delimiter-separated string, not a proper junction table. `Status` uses `ScanStatus` CustomType. Table name is explicit (`Scan`).

### 6. ScanFile

- **FK Columns**: `ScanID` (maps to `Scan.ScanID`), `ImportFolderID` (maps to `ShokoManagedFolder.ID`), `VideoLocal_Place_ID` (maps to `VideoLocal_Place.VideoLocal_PlaceID`)
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.ScanFile.GetByScanID()` and `GetWaiting()`
- **EF Core Configuration**: `HasOne<Scan>().WithMany().HasForeignKey(x => x.ScanID).IsRequired()`. `HasOne<ShokoManagedFolder>().WithMany().HasForeignKey(x => x.ImportFolderID).IsRequired()`. `HasOne<VideoLocal_Place>().WithMany().HasForeignKey(x => x.VideoLocal_Place_ID).IsRequired()`.
- **NHibernate Mappings**: `ScanFileMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Junction-like table for scan operations. `Hash` and `HashResult` are string columns. `CheckDate` is nullable. `Status` uses `ScanFileStatus` CustomType. Table name is explicit (`ScanFile`).

### 7. ScheduledUpdate

- **FK Columns**: None.
- **Navigation Properties (model code)**: None — singleton-style tracking per `UpdateType`
- **EF Core Configuration**: No relationships. One row per `UpdateType` enum value.
- **NHibernate Mappings**: `ScheduledUpdateMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Simple tracking table. `UpdateDetails` is optional string. `UpdateType` is integer enum.

### 8. StoredReleaseInfo

- **FK Columns**: None. `ED2K` + `FileSize` form composite identifier for video file matching.
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.StoredReleaseInfo.GetByEd2kAndFileSize()`, `GetByReleaseURI()`, `GetByAnidbEpisodeID()`, `GetByAnidbAnimeID()`
- **EF Core Configuration**: No outgoing FK relationships. `ED2K` + `FileSize` form composite key. `EmbeddedCrossReferences`, `EmbeddedHashes`, `EmbeddedAudioLanguages`, `EmbeddedSubtitleLanguages` use JSON serialization via property getters/setters.
- **NHibernate Mappings**: `StoredReleaseInfoMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Complex model with embedded JSON data (`CrossReferences`, `Hashes`). `Source` uses `ReleaseSource` CustomType. `ReleasedAt` uses `DateOnlyConverter`. `EmbeddedCrossReferences` is always initialized to `"[]"`. Table name is explicit (`StoredReleaseInfo`).

### 9. StoredReleaseInfo_MatchAttempt

- **FK Columns**: None. `ED2K` + `FileSize` identify the video file.
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.StoredReleaseInfo_MatchAttempt.GetByEd2kAndFileSize()`
- **EF Core Configuration**: No outgoing FK relationships. `ED2K` + `FileSize` form composite identifier. `EmbeddedAttemptProviderNames` uses comma-separated delimiter.
- **NHibernate Mappings**: `StoredReleaseInfo_MatchAttemptMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Audit trail for release matching attempts. `ProviderName` and `ProviderID` are null on failed attempts. `AttemptStartedAt` and `AttemptEndedAt` track duration. Table name is explicit (`StoredReleaseInfo_MatchAttempt`).

### 10. AuthTokens

- **FK Columns**: `UserID` (maps to `JMMUser.JMMUserID`)
- **Navigation Properties (model code)**: None directly — accessed via `RepoFactory.AuthTokens.GetByUserID()` and `GetByToken()`
- **EF Core Configuration**: `HasOne<JMMUser>().WithMany().HasForeignKey(x => x.UserID).IsRequired()`. Unique index on `Token`.
- **NHibernate Mappings**: `AuthTokensMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Simple token storage. `Token` is lowercase GUID string. `DeviceName` is lowercase trimmed string. `DeleteAllWithUserID()` in repository cascades deletes.

### 11. Versions

- **FK Columns**: None.
- **Navigation Properties (model code)**: None — database schema version tracking
- **EF Core Configuration**: No relationships. Table name is explicit (`Versions`).
- **NHibernate Mappings**: `VersionsMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Schema migration tracking. `VersionType` categorizes migrations (e.g., "Database", "NHibernate"). `VersionCommand` stores migration SQL. `VersionProgram` stores server version string.

---

## Relationship Summary — CrossReference & Miscellaneous

| Relationship | Source Entity | FK Column | Target Entity | Cardinality | Nullable |
|---|---|---|---|---|---|
| MAL CrossRef → AniDB | `CrossRef_AniDB_MAL` | `AnimeID` | `AniDB_Anime` | Many-to-One | No |
| MAL CrossRef → MAL | `CrossRef_AniDB_MAL` | `MALID` | `MAL_Anime` | Many-to-One | No |
| Trakt CrossRef → AniDB | `CrossRef_AniDB_TraktV2` | `AnimeID` | `AniDB_Anime` | Many-to-One | No |
| CustomTag CrossRef → Tag | `CrossRef_CustomTag` | `CustomTagID` | `CustomTag` | Many-to-One | No |
| CustomTag CrossRef → Target | `CrossRef_CustomTag` | `CrossRefID` | Polymorphic | Many-to-One | No |
| FilterPreset → Parent | `FilterPreset` | `ParentFilterPresetID` | `FilterPreset` | Self-referential | Yes |
| ScanFile → Scan | `ScanFile` | `ScanID` | `Scan` | Many-to-One | No |
| ScanFile → Folder | `ScanFile` | `ImportFolderID` | `ShokoManagedFolder` | Many-to-One | No |
| ScanFile → Place | `ScanFile` | `VideoLocal_Place_ID` | `VideoLocal_Place` | Many-to-One | No |
| Token → User | `AuthTokens` | `UserID` | `JMMUser` | Many-to-One | No |

**Total FK relationships**: 10 (8 non-nullable, 2 nullable/self-referential)  
**Self-referential**: 1 (`FilterPreset.ParentFilterPresetID`)  
**Polymorphic references**: 1 (`CrossRef_CustomTag.CrossRefID` + `CrossRefType`)  
**Delimiter-separated columns**: 3 (`Scan.ImportFolders`, `Playlist.PlaylistItems`, `StoredReleaseInfo_MatchAttempt.EmbeddedAttemptProviderNames`)  
**JSON-embedded columns**: 5 (`StoredReleaseInfo.EmbeddedCrossReferences`, `StoredReleaseInfo.EmbeddedHashes`, `FilterPreset.Expression`, `FilterPreset.SortingExpression`, `CustomTag` unused)

---

## T009 partial — VideoLocal_HashDigest (bridge table)

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Generated**: 2026-05-07  
**Entities**: 1 (VideoLocal_HashDigest)

### 1. VideoLocal_HashDigest

- **FK Columns**: `VideoLocalID` → `VideoLocal.VideoLocalID` (Not.Nullable)
- **Navigation Properties (model code)**: None directly — implements `IHashDigest` interface, accessed via `RepoFactory.VideoLocal_HashDigest.GetByVideoLocalID()`
- **EF Core Configuration**: `HasOne<VideoLocal>().WithMany().HasForeignKey(x => x.VideoLocalID).IsRequired().OnDelete(DeleteBehavior.Cascade)`. Unique index on `(VideoLocalID, Type)` to enforce one-hash-per-type-per-video semantics.
- **NHibernate Mappings**: `VideoLocal_HashDigestMap.cs` — no relationships defined. `Not.LazyLoad()`.
- **Parity Risks**: Bridge table storing multiple hash types (ED2K, MD5, SHA1, CRC32) per video as `Type + Value` rows. `Type` is a string identifier (not FK). `Metadata` is optional string. `VideoLocalID` is the only FK.

---

## Summary

| Category | Count |
|----------|-------|
| **Total mapping files** | **75** |
| Root-level mappings | 47 |
| CrossReference/ subdirectory | 3 |
| TMDB/ subdirectory (main) | 13 |
| TMDB/Text/ subdirectory | 2 |
| TMDB/Optional/ subdirectory | 7 |
| Trakt/ root-level | 3 |
| **Subtotal check** | **47 + 3 + 13 + 2 + 7 + 3 = 75** |

### Key Strategy

All 75 mappings use **Identity** key generation (`Id(x => x.EntityID)` or `Id(x => x.ID).Column("ColumnName")`). No Assigned, Natural, Sequence, or Composite keys. EF Core default `ValueGenerationType.Identity` is compatible with all.

### Custom Types Summary (30 distinct types)

| Custom Type | Count | Mapped To |
|-------------|-------|-----------|
| `StringClob` | 6 | `AniDB_Anime.Description`, `AniDB_Character.Description`, `AniDB_Episode.Description`, `AniDB_Tag.TagDescription`, `AnimeGroup.Description`, `Trakt_Episode.Overview` — all `nvarchar(max)` text |
| `DateOnlyConverter` | 9 | `DateOnly` → `int` (Unix epoch days): `TMDB_Episode.AiredAt`, `TMDB_Movie.ReleasedAt`, `TMDB_Person.BirthDay/DeathDay`, `TMDB_Show.FirstAiredAt/LastAiredAt`, `TMDB_Image_Entity.ReleasedAt`, `TMDB_Company_Entity.ReleasedAt`, `StoredReleaseInfo.ReleasedAt` |
| `StringListConverter` | 7 | JSON-serialized `List<string>`: `AnimeEpisode_User.UserTags`, `AnimeSeries_User.UserTags`, `TMDB_Movie.Genres/Keywords`, `TMDB_Show.Genres/Keywords`, `TMDB_Person.Aliases` |
| `TitleLanguageConverter` | 3 | Title language enums: `AniDB_Anime_Title.Language`, `AniDB_Episode_Title.Language`, `TMDB_Image.Language` |
| `TitleTypeConverter` | 1 | `AniDB_Anime_Title.TitleType` |
| `TmdbContentRatingConverter` | 2 | TMDB content ratings: `TMDB_Movie.ContentRatings`, `TMDB_Show.ContentRatings` |
| `TmdbProductionCountryConverter` | 2 | TMDB production countries: `TMDB_Movie.ProductionCountries`, `TMDB_Show.ProductionCountries` |
| `MatchRating` | 3 | Match rating enums: `CrossRef_AniDB_TMDB_Show.MatchRating`, `CrossRef_AniDB_TMDB_Movie.MatchRating`, `CrossRef_AniDB_TMDB_Episode.MatchRating` |
| `AnimeType` | 1 | `AniDB_Anime.AnimeType` |
| `EpisodeType` | 1 | `AniDB_Episode.EpisodeType` |
| `CharacterType` | 1 | `AniDB_Character.Type` |
| `PersonGender` | 2 | Gender enums: `AniDB_Character.Gender`, `TMDB_Person.Gender` |
| `CharacterAppearanceType` | 1 | `AniDB_Anime_Character.AppearanceType` |
| `CreatorType` | 1 | `AniDB_Creator.Type` |
| `CreatorRoleType` | 1 | `AniDB_Anime_Staff.RoleType` |
| `FilterPresetType` | 1 | `FilterPreset.FilterType` |
| `FilterExpressionConverter` | 2 | JSON filter expressions: `FilterPreset.Expression`, `FilterPreset.SortingExpression` |
| `MessagePackConverter<MediaContainer>` | 1 | Binary `MediaInfo` blob: `VideoLocal.MediaInfo` (column: `MediaBlob`) |
| `DisabledAutoMatchFlag` | 1 | Bitmask flags: `AnimeSeries.DisableAutoMatchFlags` |
| `DataSource` | 2 | Data source enums: `AniDB_Anime_PreferredImage.ImageSource`, `AniDB_Episode_PreferredImage.ImageSource` |
| `ImageEntityType` | 3 | Image entity type: `AniDB_Anime_PreferredImage.ImageType`, `AniDB_Episode_PreferredImage.ImageType`, `TMDB_Image_Entity.ImageType` |
| `ForeignEntityType` | 4 | Foreign entity type: `TMDB_Image_Entity.TmdbEntityType`, `TMDB_Company_Entity.TmdbEntityType`, `TMDB_Title.ParentType`, `TMDB_Overview.ParentType` |
| `AlternateOrderingType` | 1 | `TMDB_AlternateOrdering.Type` |
| `ReleaseSource` | 1 | `StoredReleaseInfo.Source` |
| `ScanStatus` | 1 | `Scan.Status` |
| `ScanFileStatus` | 1 | `ScanFile.Status` |
| `AniDBMessageType` | 1 | `AniDB_Message.Type` |
| `AniDBMessageFlags` | 1 | `AniDB_Message.Flags` |
| `AniDBNotifyType` | 1 | `AniDB_NotifyQueue.Type` |
| `SeriesVoteType` | 1 | `AnimeSeries_User.UserRatingVoteType` |

### Relationship Summary

| Relationship Type | Mapping File | Details |
|-------------------|--------------|---------|
| `References` | FilterPresetMap.cs | `Parent` → `ParentFilterPresetID` (ReadOnly, no cascade) |
| `HasMany` | FilterPresetMap.cs | `Children` → `ParentFilterPresetID` (Fetch.Join, ReadOnly, no cascade) |

**Note**: All relationships use `Not.LazyLoad()` and `ReadOnly()`. No `HasMany` with cascade, no `BelongsTo`, no `ManyToMany`. Relationships are explicitly loaded via repository queries. EF Core should configure these as `HasOne/WithMany` with `OnDelete(DeleteBehavior.Restrict)` or `ClientSetNull`.

### Implicit Table Names (10 mappings)

These mappings do NOT call `Table()` and rely on NHibernate's class-name-to-table-name convention:

| Mapping File | Entity | Implicit Table Name |
|--------------|--------|---------------------|
| `AuthTokensMap.cs` | `AuthTokens` | `AuthTokens` |
| `AniDB_CreatorMap.cs` | `AniDB_Creator` | `AniDB_Creator` |
| `CrossRef_AniDB_MALMap.cs` | `CrossRef_AniDB_MAL` | `CrossRef_AniDB_MAL` |
| `CrossRef_CustomTagMap.cs` | `CrossRef_CustomTag` | `CrossRef_CustomTag` |
| `CustomTagMap.cs` | `CustomTag` | `CustomTag` |
| `PlaylistMap.cs` | `Playlist` | `Playlist` |
| `ScheduledUpdateMap.cs` | `ScheduledUpdate` | `ScheduledUpdate` |
| `Trakt_ShowMap.cs` | `Trakt_Show` | `Trakt_Show` |
| `Trakt_EpisodeMap.cs` | `Trakt_Episode` | `Trakt_Episode` |
| `Trakt_SeasonMap.cs` | `Trakt_Season` | `Trakt_Season` |

**EF Core Note**: All of these should have explicit `ToTable()` calls in EF Core configurations to avoid any ambiguity.

### Custom Column Names (9 mappings)

These mappings use `.Column("CustomName")` to map properties to non-standard column names:

| Mapping File | Property | Column Name |
|--------------|----------|-------------|
| `ShokoManagedFolderMap.cs` | `ID` | `ImportFolderID` |
| `ShokoManagedFolderMap.cs` | `Path` | `ImportFolderLocation` |
| `ShokoManagedFolderMap.cs` | `Name` | `ImportFolderName` |
| `VideoLocal_PlaceMap.cs` | `ID` | `VideoLocal_Place_ID` |
| `VideoLocal_PlaceMap.cs` | `VideoID` | `VideoLocalID` |
| `VideoLocal_PlaceMap.cs` | `ManagedFolderID` | `ImportFolderID` |
| `VideoLocal_PlaceMap.cs` | `RelativePath` | `FilePath` |
| `VideoLocalMap.cs` | `MediaInfo` | `MediaBlob` |
| `JMMUserMap.cs` | `RawAvatarImageMetadata` | `AvatarImageMetadata` |
| `AniDB_TagMap.cs` | `TagNameSource` | `TagName` |
| `StoredReleaseInfoMap.cs` | `EmbeddedHashes` | `Hashes` |
| `StoredReleaseInfoMap.cs` | `EmbeddedAudioLanguages` | `AudioLanguages` |
| `StoredReleaseInfoMap.cs` | `EmbeddedSubtitleLanguages` | `SubtitleLanguages` |
| `StoredReleaseInfoMap.cs` | `EmbeddedCrossReferences` | `CrossReferences` |
| `TMDB_EpisodeMap.cs` | `RuntimeMinutes` | `Runtime` |
| `TMDB_MovieMap.cs` | `RuntimeMinutes` | `Runtime` |
| `StoredReleaseInfo_MatchAttemptMap.cs` | `EmbeddedAttemptProviderNames` | `AttemptProviderNames` |

### No-Table Mappings with Not.LazyLoad (10 mappings)

These mappings omit `Table()` but include `Not.LazyLoad()`:

| Mapping File | Entity |
|--------------|--------|
| `AuthTokensMap.cs` | `AuthTokens` |
| `AniDB_CreatorMap.cs` | `AniDB_Creator` |
| `CrossRef_AniDB_MALMap.cs` | `CrossRef_AniDB_MAL` |
| `CrossRef_CustomTagMap.cs` | `CrossRef_CustomTag` |
| `CustomTagMap.cs` | `CustomTag` |
| `PlaylistMap.cs` | `Playlist` |
| `ScheduledUpdateMap.cs` | `ScheduledUpdate` |
| `Trakt_ShowMap.cs` | `Trakt_Show` |
| `Trakt_EpisodeMap.cs` | `Trakt_Episode` |
| `Trakt_SeasonMap.cs` | `Trakt_Season` |

### EF Core Parity Risks

1. **No indexes defined in any mapping**: None of the 75 mappings define indexes or unique constraints. EF Core configurations MUST add indexes where semantically required (e.g., unique on `AniDB_AnimeUpdate.AnimeID`, unique on `FileNameHash.FileName + FileSize`, unique on `CrossRef_AniDB_TMDB_Show` join keys, etc.).

2. **No relationships defined in most mappings**: Only `FilterPresetMap.cs` defines relationships. All FK relationships (e.g., `AnimeSeries.AniDB_ID` → `AniDB_Anime`, `VideoLocal_Place.VideoID` → `VideoLocal`) must be configured in EF Core.

3. **Identity keys only**: All 75 mappings use Identity keys. EF Core `ValueGeneratedOnAdd()` is the correct default. No Assigned/Natural/HiLo sequences.

4. **Not.LazyLoad everywhere**: All 75 mappings use `Not.LazyLoad()`. EF Core should use explicit loading (no proxies, no lazy loading).

5. **ReadOnly relationships**: The only relationships in `FilterPresetMap.cs` use `ReadOnly()`. EF Core should use `OnDelete(DeleteBehavior.Restrict)` or `ClientSetNull`.

6. **StringClob (nvarchar(max))**: 6 mappings use `StringClob`. EF Core needs `HasColumnType("nvarchar(max)")` for SQL Server compatibility; SQLite handles `TEXT` natively.

7. **MessagePack serialization**: `VideoLocalMap.cs` uses `MessagePackConverter<MediaContainer>` for `MediaBlob`. EF Core needs a custom `ValueConverter<byte[], MediaContainer>` preserving the exact MessagePack binary format.

8. **DateOnlyConverter**: 12 properties use `DateOnlyConverter` mapping `DateOnly` ↔ `int` (Unix epoch days). EF Core needs `ValueConverter<DateOnly, int>` with the same epoch-day arithmetic.

9. **Implicit table names**: 10 mappings rely on NHibernate's convention-based table names. EF Core should use explicit `ToTable()` for clarity and to avoid future naming conflicts.

10. **Custom column names**: 18 property-to-column mappings use non-standard column names. All must be preserved in EF Core `HasColumnName()` calls.

---

## DatabaseFixes / Schema Version Inventory

**Source**: `Shoko.Server/Databases/DatabaseFixes.cs` (1626 lines)  
**Supporting files**: `DatabaseCommand.cs` (86 lines), `BaseDatabase.cs` (392 lines), `SQLite.cs` (~1200 lines), `SQLServer.cs` (~1100 lines), `MySQL.cs` (~1100 lines)

### Schema Version Constants

The schema version is tracked via `DatabaseCommand(Version, Revision, ...)` entries across three provider-specific files. The current maximum version per provider:

| Provider | Max Version | Max Revision | Source File |
|----------|-------------|--------------|-------------|
| **SQLite** | 143 | 6 | `SQLite.cs` |
| **SQL Server** | 155 | 17 | `SQLServer.cs` |
| **MySQL** | 161 | 6 | `MySQL.cs` |

**Current baseline**: The highest version across all providers is **MySQL 161.6** — this represents the most up-to-date schema. SQLite and SQL Server lag behind (143.6 and 155.17 respectively).

**Version key**: `Constants.DatabaseTypeKey = "Database"` — all version entries are stored with `VersionType = "Database"`.

### Versions Table

**Entity**: `Versions` (`Shoko.Server/Models/Internal/Versions.cs`)  
**Mapping**: `VersionsMap.cs`  
**Repository**: `VersionsRepository` (Direct, no cache)

| Column | Type | Nullable | Notes |
|--------|------|----------|-------|
| `VersionsID` | int | No | Identity PK |
| `VersionType` | string | No | Always `"Database"` |
| `VersionValue` | string | No | Major version number (e.g., `"161"`) |
| `VersionRevision` | string | Yes | Minor revision number (e.g., `"6"`) |
| `VersionCommand` | string | Yes | Human-readable command name |
| `VersionProgram` | string | Yes | Application version that applied the migration (semantic versioning string) |

**Repository method**: `GetAllByType(string vertype)` — returns `Dictionary<(string Version, string Revision), Versions>`, grouped by `(VersionValue, VersionRevision)`.

**Version application flow** (`BaseDatabase.cs`):
1. `Init()` — loads existing versions from DB via `RepoFactory.Versions.GetAllByType("Database")`
2. `PreFillVersions()` — migrates legacy single-entry version format to per-command format
3. `_createVersionTable` / `_updateVersionTable` — DDL commands to create/alter the Versions table per provider
4. `ExecuteDatabaseFixes()` — runs each `DatabaseCommand` in order, logging the version to the Versions table after success
5. `ExecuteCommand()` — skips already-applied commands (checked via `AllVersions.ContainsKey((version, revision))`)

### Version Table DDL per Provider

#### SQLite (5 commands)

| Version | Revision | SQL |
|---------|----------|-----|
| 0 | 1 | `CREATE TABLE Versions ( VersionsID INTEGER PRIMARY KEY AUTOINCREMENT, VersionType TEXT NOT NULL, VersionValue TEXT NOT NULL)` |
| 0 | 2 | `ALTER TABLE Versions ADD VersionRevision TEXT NULL` |
| 0 | 3 | `ALTER TABLE Versions ADD VersionCommand TEXT NULL` |
| 0 | 4 | `ALTER TABLE Versions ADD VersionProgram TEXT NULL` |
| 0 | 5 | `CREATE INDEX IX_Versions_VersionType ON Versions(VersionType,VersionValue,VersionRevision)` |

#### SQL Server (7 commands)

| Version | Revision | SQL |
|---------|----------|-----|
| 0 | 1 | `CREATE TABLE [Versions]( [VersionsID] [int] IDENTITY(1,1) NOT NULL, [VersionType] [varchar](100) NOT NULL, [VersionValue] [varchar](100) NOT NULL, CONSTRAINT [PK_Versions] PRIMARY KEY CLUSTERED ( [VersionsID] ASC )... )` |
| 0 | 2 | `CREATE UNIQUE INDEX UIX_Versions_VersionType ON Versions(VersionType)` |
| 0 | 3 | `ALTER TABLE Versions ADD VersionRevision varchar(100) NULL` |
| 0 | 4 | `ALTER TABLE Versions ADD VersionCommand nvarchar(max) NULL` |
| 0 | 5 | `ALTER TABLE Versions ADD VersionProgram varchar(100) NULL` |
| 0 | 6 | `DROP INDEX UIX_Versions_VersionType ON Versions` |
| 0 | 7 | `CREATE INDEX IX_Versions_VersionType ON Versions(VersionType,VersionValue,VersionRevision)` |

#### MySQL

Same structure as SQL Server but with backtick quoting. MySQL shares the same `_updateVersionTable` commands (v0.3–v0.5) for column additions.

### DatabaseCommand Types

**Source**: `DatabaseCommand.cs` — three constructors for three command types:

| Type | Constructor | Execution | Version Tracking |
|------|-------------|-----------|------------------|
| `NormalCommand` | `DatabaseCommand(int version, int revision, string command)` | Raw SQL string executed via provider-specific `ExecuteCommand()` | Yes — logged to Versions table |
| `CodedCommand` | `DatabaseCommand(int version, int revision, Func<object, Tuple<bool, string>> updateCommand)` | C# method (e.g., `DatabaseFixes.MigrateRenamers`) | Yes — logged to Versions table |
| `PostDatabaseFix` | `DatabaseCommand(int version, int revision, Action databaseFix)` | Deferred execution via `ExecuteDatabaseFixes()` after schema commands | Yes — logged to Versions table |

**Command naming**: `CommandName` property returns `[MethodName]` for coded/fix commands, or the raw SQL string for normal commands.

### Schema-Changing Commands (DDL)

These commands create/alter/drop tables and columns. They are the highest-risk for EF Core migration parity:

#### Table Creation Commands

| Version | Revision | Provider | SQL (truncated) |
|---------|----------|----------|-----------------|
| 0 | 1 | All | `CREATE TABLE Versions` |
| 149 | 6 | SQL Server | `CREATE TABLE StoredReleaseInfo (...)` — comprehensive release info table |
| 139 | 4 | SQLite | `CREATE TABLE StoredReleaseInfo (...)` — SQLite variant |
| 107 | 1 | SQL Server | `CREATE TABLE StoredReleaseInfo (...)` — SQL Server variant |

#### Table Drop Commands

| Version | Revision | Provider | SQL |
|---------|----------|----------|-----|
| 150 | 4 | SQL Server | `DROP TABLE IF EXISTS RenameScript; DROP TABLE IF EXISTS RenamerInstance` |
| 139 | 4 | SQLite | `DROP TABLE RenameScript; DROP TABLE RenamerInstance` |
| 158 | 4 | MySQL | `DROP TABLE RenameScript; DROP TABLE RenamerInstance` |
| 112 | 1–3 | SQLite | `DROP COLUMN ContractVersion/Blob/Size` from `AniDB_Anime`, `AnimeSeries`, `AnimeGroup` |
| 110 | 1 | SQLite | `DROP COLUMN PlexContractVersion/Blob/Size` from `AnimeEpisode`, `AnimeGroup_User`, `AnimeSeries_User` |
| 161 | 1–4 | MySQL | `DROP COLUMN MALTitle/StartEpisodeType/StartEpisodeNumber/CrossRefSource` from `CrossRef_AniDB_MAL` |

#### Column Alter Commands (Add)

| Version | Revision | Provider | Table | Column | Type |
|---------|----------|----------|-------|--------|------|
| 20 | 1 | All | `AniDB_File` | `FileVersion` | int/INTEGER |
| 22 | 3 | All | `AniDB_File` | `InternalVersion` | int/INTEGER |
| 42 | 1–7 | SQL Server | Various | `ContractVersion`, `ContractString`, `GroupsIdsVersion`, etc. | nvarchar/int |
| 44 | Various | SQLite | Various | `ContractVersion`, `PlexContractVersion`, `GroupsIdsVersion`, etc. | INTEGER |
| 63 | 1 | SQL Server | `VideoLocal` | `LastAVDumpVersion` | nvarchar(128) |
| 103 | 2 | SQLite | `VideoLocal` | `LastAVDumpVersion` | TEXT |
| 107 | 2 | SQL Server | `VideoLocal` | `LastAVDumpVersion` | nvarchar(128) |
| 156 | 1–4 | SQL Server | `StoredRelocationPipe` | `Configuration` (renamed from temp) | VARBINARY(MAX) |

#### Column Alter Commands (Drop)

| Version | Revision | Provider | Table | Column |
|---------|----------|----------|-------|--------|
| 100 | Various | All | `AnimeEpisode_User` | `ContractVersion` |
| 110 | 1 | SQLite | `AnimeEpisode`, `AnimeGroup_User`, `AnimeSeries_User` | `PlexContractVersion/Blob/Size` |
| 112 | 1–3 | SQLite | `AniDB_Anime`, `AnimeSeries`, `AnimeGroup` | `ContractVersion/Blob/Size` |
| 142 | 1–3 | SQL Server | `GroupFilter` | `GroupsIdsVersion/GroupsIdsString/SeriesIdsString` |
| 142 | 4–6 | SQL Server | `GroupFilter` | `GroupConditionsVersion/GroupConditions` |
| 159 | 1–4 | MySQL | `CrossRef_AniDB_MAL` | `MALTitle/StartEpisodeType/StartEpisodeNumber/CrossRefSource` |
| 161 | 5 | MySQL | `AnimeGroup_User` | `IsFave` |

#### Column Alter Commands (Modify)

| Version | Revision | Provider | Table | Column | Change |
|---------|----------|----------|-------|--------|--------|
| 20 | 3 | SQL Server | `AniDB_File` | `FileVersion` | `ALTER COLUMN int NOT NULL` (was NULL) |
| 22 | 9 | SQL Server | `AniDB_File` | `InternalVersion` | `ALTER COLUMN int NOT NULL` (was NULL) |
| 156 | 3–4 | SQL Server | `StoredRelocationPipe` | `Configuration` | Drop old, rename `temp` → `Configuration` |

#### Index Commands

| Version | Revision | Provider | Index | Table | Columns |
|---------|----------|----------|-------|-------|---------|
| 0 | 2 | SQL Server | `UIX_Versions_VersionType` (unique) | `Versions` | `VersionType` |
| 0 | 5 | SQLite | `IX_Versions_VersionType` | `Versions` | `VersionType, VersionValue, VersionRevision` |
| 0 | 7 | SQL Server | `IX_Versions_VersionType` | `Versions` | `VersionType, VersionValue, VersionRevision` |

### Data-Fix Commands (DatabaseFixes methods)

These are C# methods that perform data migrations, cleanup, or restructuring. They run as `PostDatabaseFix` commands after schema commands:

| Version | Revision | Method | Description |
|---------|----------|--------|-------------|
| 35/37/39 | 1 | `PopulateTagWeight` | Sets all `AniDB_Anime_Tag.Weight = 0` |
| 45/49/53 | 1 | `DeleteSeriesUsersWithoutSeries` | Removes orphaned `AnimeSeries_User` records |
| 63/67/73 | 1 | `RefreshAniDBInfoFromXML` | Repopulates missing AniDB episode descriptions from XML cache |
| 64/68/74 | 2 | `UpdateAllStats` | Refreshes all anime series statistics |
| 66/71/76 | 3 | `MigrateAniDB_AnimeUpdates` | Migrates `AniDB_Anime.DateTimeUpdated` → `AniDB_AnimeUpdate` table |
| 81/86/93 | 2 | `RefreshAniDBInfoFromXML` | Second pass of AniDB info refresh |
| 84/89/96 | 2 | `FixWatchDates` | Corrects episode user watch dates from file user records |
| 93/99/107 | 10 | `FixTagParentIDsAndNameOverrides` | Recalculates AniDB tag parent IDs and name overrides via AniDB HTTP |
| 99/106/113 | 1 | `FixEpisodeDateTimeUpdated` | Resets episode timestamps for orphaned/missing episodes |
| 100/104/118 | 1 | `FixAnimeSourceLinks` | Deduplicates pipe-separated site URLs |
| 100/104/118 | 2 | `FixOrphanedShokoEpisodes` | Fixes/removes Shoko episodes with broken AniDB/Series links |
| 105/112/119 | 4 | `MigrateGroupFilterToFilterPreset` | Converts legacy `GroupFilter` → `FilterPreset` data |
| 105/112/119 | 5 | `DropGroupFilter` | Drops `GroupFilter` and `GroupFilterCondition` tables |
| 111 | 1 | `FixAnimeSourceLinks` | (SQL Server only — duplicate at different version) |
| 111 | 2 | `FixOrphanedShokoEpisodes` | (SQL Server only — duplicate at different version) |
| 115/122/129 | 33 | `CleanupAfterAddingTMDB` | Removes old "MovieDB" image dir, schedules TMDB movie updates |
| 123/131/139 | 11 | `CleanupAfterRemovingTvDB` | Removes old "TvDB" image dir |
| 123/131/139 | 12 | `ClearQuartzQueue` | Clears the Quartz job scheduler queue |
| 124/132/140 | 1 | `RepairMissingTMDBPersons` | Fetches missing TMDB person records referenced by cast/crew |
| 131/141/148 | 3 | `RecreateAnimeCharactersAndCreators` | Recreates character/creator relations from AniDB XML cache |
| 132/142/150 | 12 | `ScheduleTmdbImageUpdates` | Schedules image downloads for all TMDB movies/shows |
| 134/144/152 | 2 | `MoveTmdbImagesOnDisc` | Reorganizes TMDB image files by MD5 hash |
| 138/149/157 | 6 | `ClearQuartzQueue` | Second pass — clears Quartz queue |
| 139/150/158 | 4 | `MoveAnidbFileDataToReleaseInfoFormat` | **Major migration**: Converts `AniDB_File`/`AniDB_ReleaseGroup`/`CrossRef_Languages_AniDB_File`/`CrossRef_Subtitles_AniDB_File` → `StoredReleaseInfo` + `StoredReleaseInfo_MatchAttempt` + `VideoLocal_HashDigest`. Drops 5 old tables and 3 old columns. |
| 140/151/159 | 4 | `MigrateRenamers` | Converts `RenameScript`/`RenamerInstance` → `StoredRelocationPipe`. Drops old tables. |
| 140/151/159 | 21 | `RefreshAnimeSeriesUserStats` | Refreshes all anime series user statistics |
| 143/154/161 | 6 | `EnsureNoOrphanedGroupsOrSeries` | Deletes empty groups, reassigns orphaned series |
| 151 | 20 | `MigrateAnidbVotes` | Converts `AniDB_Vote` → `AnimeSeries_User.AbsoluteUserRating` + `AnimeEpisode_User.AbsoluteUserRating`. Drops `AniDB_Vote` table. |

### Provider-Specific SQL Sections

Each provider file (`SQLite.cs`, `SQLServer.cs`, `MySQL.cs`) contains:

1. **`_createVersionTable`** — DDL to create the Versions table (3–7 commands per provider)
2. **`_updateVersionTable`** — DDL to add missing columns to existing Versions table (3–5 commands per provider)
3. **`_createTables`** — Full table DDL for all entities (117–136 commands per provider)
4. **`_patchCommands`** — Schema versioning commands (DDL + coded commands + data fixes)

**Command count per provider** (approximate):
- SQLite: ~100 `_patchCommands` entries (versions 1–143)
- SQL Server: ~100 `_patchCommands` entries (versions 1–155)
- MySQL: ~100 `_patchCommands` entries (versions 1–161)

**Key divergence**: The three providers diverge in version numbering. SQLite is the oldest (143), SQL Server is middle (155), MySQL is newest (161). This means:
- SQLite databases may need additional migration steps to reach parity with MySQL
- SQL Server databases may need additional migration steps to reach parity with MySQL
- MySQL has the most up-to-date schema

### EF Core Baseline Migration Implications

1. **Starting point**: EF Core's initial migration must reproduce the schema at the **highest common denominator** across all providers. Since MySQL is at v161 and SQLite at v143, the EF Core baseline should target **MySQL 161** schema (the most complete).

2. **Versions table**: EF Core must include the `Versions` table as part of its initial model. The `VersionType = "Database"` key must be preserved. The `VersionProgram` column (application version string) is unique to Shoko's migration system and has no EF Core equivalent.

3. **Dropped tables**: EF Core must NOT include these tables (they were dropped by migrations):
   - `GroupFilter`, `GroupFilterCondition` (dropped v105/112/119)
   - `RenameScript`, `RenamerInstance` (dropped v140/151/158)
   - `AniDB_File`, `AniDB_FileUpdate`, `AniDB_ReleaseGroup` (dropped v139/150/158)
   - `CrossRef_Languages_AniDB_File`, `CrossRef_Subtitles_AniDB_File` (dropped v139/150/158)
   - `AniDB_Vote` (dropped v151.20)

4. **Dropped columns**: EF Core must NOT include these columns (they were dropped by migrations):
   - `VideoLocal.MD5`, `VideoLocal.SHA1`, `VideoLocal.CRC32` (dropped v139/150/158 — replaced by `VideoLocal_HashDigest`)
   - `CrossRef_File_Episode.CrossRefSource` (dropped v139/150/158)
   - `AniDB_Anime.ContractVersion/ContractBlob/ContractSize` (dropped v112 SQLite)
   - `AnimeSeries.ContractVersion/ContractBlob/ContractSize` (dropped v112 SQLite)
   - `AnimeGroup.ContractVersion/ContractBlob/ContractSize` (dropped v112 SQLite)
   - `AnimeEpisode.PlexContractVersion/Blob/Size` (dropped v110 SQLite)
   - `AnimeGroup_User.PlexContractVersion/Blob/Size` (dropped v110 SQLite)
   - `AnimeSeries_User.PlexContractVersion/Blob/Size` (dropped v110 SQLite)
   - `AnimeEpisode_User.ContractVersion` (dropped v100)
   - `GroupFilter.GroupsIdsVersion/GroupsIdsString/SeriesIdsString` (dropped v142 SQL Server)
   - `GroupFilter.GroupConditionsVersion/GroupConditions` (dropped v142 SQL Server)
   - `CrossRef_AniDB_MAL.MALTitle/StartEpisodeType/StartEpisodeNumber/CrossRefSource` (dropped v161 MySQL)
   - `AnimeGroup_User.IsFave` (dropped v161 MySQL)

5. **Renamed columns**: EF Core must use the final column names (post-migration):
   - `StoredRelocationPipe.Configuration` (renamed from `temp` in v156 SQL Server)

6. **New tables created by migrations**: EF Core must include:
   - `AniDB_AnimeUpdate` (created v66/71/76 — data migrated from `AniDB_Anime.DateTimeUpdated`)
   - `StoredReleaseInfo` (created v107/139/149)
   - `StoredReleaseInfo_MatchAttempt` (created v107/139/149)
   - `VideoLocal_HashDigest` (created v139/150/158 — migrated from `VideoLocal.MD5/SHA1/CRC32`)

7. **HashDigest migration**: The `MoveAnidbFileDataToReleaseInfoFormat` migration (v139/150/158) migrates `VideoLocal.MD5`, `VideoLocal.SHA1`, `VideoLocal.CRC32` columns into `VideoLocal_HashDigest` rows (Type + Value). EF Core baseline must include `VideoLocal_HashDigest` table but NOT the old columns on `VideoLocal`.

8. **StoredReleaseInfo migration**: The same migration migrates `AniDB_File`, `AniDB_ReleaseGroup`, `CrossRef_Languages_AniDB_File`, `CrossRef_Subtitles_AniDB_File` data into `StoredReleaseInfo` and `StoredReleaseInfo_MatchAttempt`. The source tables are dropped. EF Core must NOT include them.

9. **Renamer migration**: `RenameScript` and `RenamerInstance` are migrated to `StoredRelocationPipe` and dropped. EF Core must NOT include the old tables.

10. **Votes migration**: `AniDB_Vote` data is migrated to `AnimeSeries_User.AbsoluteUserRating` and `AnimeEpisode_User.AbsoluteUserRating`, then the table is dropped. EF Core must NOT include `AniDB_Vote`.

11. **GroupFilter migration**: `GroupFilter` and `GroupFilterCondition` are migrated to `FilterPreset`, then dropped. EF Core must NOT include them.

12. **Migration tracking**: EF Core's built-in migration history table (`__EFMigrationsHistory`) serves a similar purpose to the `Versions` table but is incompatible with Shoko's existing version tracking. A decision must be made: keep both systems (Versions + __EFMigrationsHistory) or replace Versions with EF Core migrations entirely.

13. **Provider-specific differences**: The EF Core baseline migration must produce compatible schemas across SQLite, SQL Server, and MySQL. Key differences to reconcile:
    - `nvarchar(max)` (SQL Server) vs `TEXT` (SQLite) vs `LONGTEXT` (MySQL) for Clob fields
    - `VARBINARY(MAX)` (SQL Server) vs `BLOB` (SQLite) vs `LONGBLOB` (MySQL) for binary data
    - `DATE` type compatibility for `DateOnlyConverter` properties
    - Identity column syntax differences
    - Index syntax differences

---

## NHibernate Converter Inventory (T006)

**Task**: T006 — Catalog all NHibernate `IUserType` converters and utility types  
**Location**: `Shoko.Server/Databases/NHIbernate/`  
**Total**: 13 files (10 `IUserType` converters, 2 utility types, 1 interceptor + 1 DI injector)

### 1. DateOnlyConverter.cs — `DateOnlyConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `System.DateOnly`
- **DB/Provider Type**: `DATE` (SQLite/SQL Server/MySQL)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)`, then converts string to `DateOnly` via `DateTime.Parse(i).ToDateOnly()`
- **NullSafeSet**: Converts `DateOnly` → `DateTime` via `i.ToDateTime(TimeOnly.MinValue)`, writes as `DBNull.Value` when null
- **Serialization/Storage Format**: Stored as `DateTime` ticks (via `ConvertTo` → `typeof(DateTime)`), DB column type `DATE`
- **Null Handling**: Returns null from DB → `ConvertFrom` returns null; null CLR value → `DBNull.Value`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.Date.SqlType`
- **EF Core ValueConverter Migration**: `ValueConverter<DateOnly, DateTime>` with `ConvertToProvider: d => d.ToDateTime(TimeOnly.MinValue)` and `ConvertFromProvider: dt => DateOnly.FromDateTime(dt)`. Alternatively use `ValueConverter<DateOnly, DateOnly>` with EF Core 8+ built-in `DateOnly` support. **Risk**: `ReturnedType` is `typeof(DateTime)` (not `DateOnly`) — EF Core `ValueConverter` must return the actual CLR type.

### 2. MessagePackConverter&lt;T&gt; — `MessagePackConverter&lt;T&gt;` (generic)

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `T` where `T : class` (any reference type, including `object`)
- **DB/Provider Type**: `BLOB` / `VARBINARY(MAX)` / `LONGVARBINARY` (binary blob)
- **NullSafeGet**: Reads via `NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0], impl)` → returns `byte[]`; deserializes via `MessagePackSerializer.Deserialize<T>(s)` (or `MessagePackSerializer.Typeless.Deserialize(s)` when `T == typeof(object)`)
- **NullSafeSet**: Serializes via `MessagePackSerializer.Serialize(value)` → `byte[]`; writes `DBNull.Value` when null
- **Serialization/Storage Format**: MessagePack binary (compact binary JSON-like format)
- **Null Handling**: DB null → `null`; CLR null → `DBNull.Value`; deserialization exception → logs error via `ILogger`, returns `null`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.BinaryBlob.SqlType`; MessagePack is provider-agnostic
- **EF Core ValueConverter Migration**: `ValueConverter<T, byte[]>` with `ConvertToProvider: v => MessagePackSerializer.Serialize(v)` and `ConvertFromProvider: b => MessagePackSerializer.Deserialize<T>(b)`. **Risk**: Converter depends on `Utils.ServiceContainer.GetRequiredService<ILogger<MessagePackConverter<T>>>()` — EF Core value converters should be stateless. Must inject logger externally or remove logging dependency. Also depends on `MessagePack` NuGet package.

### 3. TypelessMessagePackConverter — `TypelessMessagePackConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `object` (any type, serialized typelessly)
- **DB/Provider Type**: `BLOB` / `VARBINARY(MAX)` / `LONGVARBINARY` (binary blob)
- **NullSafeGet**: Reads via `NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0], impl)` → deserializes via `MessagePackSerializer.Typeless.Deserialize(s)` → returns `object`
- **NullSafeSet**: Serializes via `MessagePackSerializer.Typeless.Serialize(value)` → `byte[]`; writes `DBNull.Value` when null
- **Serialization/Storage Format**: MessagePack Typeless (includes type metadata in the binary payload)
- **Null Handling**: DB null → `null`; CLR null → `DBNull.Value`; deserialization exception → logs error, returns `null`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.BinaryBlob.SqlType`
- **EF Core ValueConverter Migration**: `ValueConverter<object, byte[]>` with `ConvertToProvider: v => MessagePackSerializer.Typeless.Serialize(v)` and `ConvertFromProvider: b => MessagePackSerializer.Typeless.Deserialize(b)`. **Risk**: Same logging dependency issue as `MessagePackConverter<T>`. Typeless deserialization returns `Dictionary<string, object>` / `List<object>` for complex types — runtime type information is in the payload but lost in EF Core queries.

### 4. StringListConverter — `StringListConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `List<string>`
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → splits on `"|||"` delimiter → returns `List<string>`
- **NullSafeSet**: Joins list with `"|||"` delimiter; writes `DBNull.Value` when null
- **Serialization/Storage Format**: Delimiter-separated string using `"|||"` as separator
- **Null Handling**: DB null → empty list `[]`; CLR null → `DBNull.Value`; empty list → `string.Empty`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; delimiter `"|||"` is provider-agnostic
- **EF Core ValueConverter Migration**: `ValueConverter<List<string>, string>` with `ConvertToProvider: l => l?.Join("|||") ?? string.Empty` and `ConvertFromProvider: s => s?.Split("|||", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>()`. **Risk**: List elements containing `"|||"` will corrupt data. The `Join`/`Split` extension methods from `Shoko.Abstractions.Extensions` must be replicated.

### 5. TitleLanguageConverter — `TitleLanguageConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `Shoko.Abstractions.Metadata.Enums.TitleLanguage`
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → converts string to `TitleLanguage` via `s.GetTitleLanguage()` extension; null → `TitleLanguage.Unknown`
- **NullSafeSet**: Converts `TitleLanguage` to string via `t.GetString()` extension; writes `DBNull.Value` when null
- **Serialization/Storage Format**: String representation of enum (via `GetString()` extension method)
- **Null Handling**: DB null → `TitleLanguage.Unknown`; CLR null → `DBNull.Value` (throws `ArgumentNullException` in `ConvertTo` if value is null — but `NullSafeSet` guards with `value == null ? DBNull.Value`)
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; depends on `Shoko.Abstractions.Extensions.GetTitleLanguage()` and `.GetString()` extension methods
- **EF Core ValueConverter Migration**: `ValueConverter<TitleLanguage, string>` with `ConvertToProvider: t => t.GetString()` and `ConvertFromProvider: s => s.GetTitleLanguage()`. **Risk**: Must replicate `GetTitleLanguage()` and `GetString()` extension methods from `Shoko.Abstractions.Extensions`. Enum is defined in `Shoko.Abstractions.Metadata.Enums`.

### 6. TitleTypeConverter — `TitleTypeConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `Shoko.Abstractions.Metadata.Enums.TitleType`
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → converts string to `TitleType` via `s.GetTitleType()` extension; null → `TitleType.None`
- **NullSafeSet**: Converts `TitleType` to string via `t.GetString()` extension; writes `DBNull.Value` when null
- **Serialization/Storage Format**: String representation of enum (via `GetString()` extension method)
- **Null Handling**: DB null → `TitleType.None`; CLR null → `DBNull.Value` (throws `ArgumentNullException` in `ConvertTo` — guarded by `NullSafeSet`)
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; depends on `Shoko.Abstractions.Extensions.GetTitleType()` and `.GetString()` extension methods
- **EF Core ValueConverter Migration**: `ValueConverter<TitleType, string>` with `ConvertToProvider: t => t.GetString()` and `ConvertFromProvider: s => s.GetTitleType()`. **Risk**: Same as `TitleLanguageConverter` — must replicate extension methods.

### 7. TmdbContentRatingConverter — `TmdbContentRatingConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `List<TMDB_ContentRating>` (from `Shoko.Server.Models.TMDB`)
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → splits on `'|'` with `StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries` → maps each via `TMDB_ContentRating.FromString()` → returns `List<TMDB_ContentRating>`
- **NullSafeSet**: Joins list with `'|'` separator via `l.Select(r => r.ToString()).Join('|')`; writes `DBNull.Value` when null
- **Serialization/Storage Format**: Pipe-delimited string using `'|'` as separator
- **Null Handling**: DB null → empty list `[]`; CLR null → `DBNull.Value`; empty list → `string.Empty`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; depends on `TMDB_ContentRating.FromString()` and `ToString()` methods
- **EF Core ValueConverter Migration**: `ValueConverter<List<TMDB_ContentRating>, string>` with `ConvertToProvider: l => l?.Select(r => r.ToString()).Join('|') ?? string.Empty` and `ConvertFromProvider: s => s?.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(TMDB_ContentRating.FromString).ToList() ?? new List<TMDB_ContentRating>()`. **Risk**: List elements containing `'|'` will corrupt data. The `Join` extension method from `Shoko.Abstractions.Extensions` must be replicated.

### 8. TmdbProductionCountryConverter — `TmdbProductionCountryConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `List<TMDB_ProductionCountry>` (from `Shoko.Server.Models.TMDB`)
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → splits on `'|'` with `StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries` → maps each via `TMDB_ProductionCountry.FromString()` → returns `List<TMDB_ProductionCountry>`
- **NullSafeSet**: Joins list with `'|'` separator via `l.Select(r => r.ToString()).Join('|')`; writes `DBNull.Value` when null
- **Serialization/Storage Format**: Pipe-delimited string using `'|'` as separator
- **Null Handling**: DB null → empty list `[]`; CLR null → `DBNull.Value`; empty list → `string.Empty`
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; depends on `TMDB_ProductionCountry.FromString()` and `ToString()` methods
- **EF Core ValueConverter Migration**: `ValueConverter<List<TMDB_ProductionCountry>, string>` with `ConvertToProvider: l => l?.Select(r => r.ToString()).Join('|') ?? string.Empty` and `ConvertFromProvider: s => s?.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(TMDB_ProductionCountry.FromString).ToList() ?? new List<TMDB_ProductionCountry>()`. **Risk**: Same as `TmdbContentRatingConverter` — delimiter collision risk.

### 9. TypeStringConverter — `TypeStringConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `System.Type`
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → resolves type via `Type.GetType(s)` then falls back to `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(a => a.Name.Equals(s) || Equals(a.FullName, s))`; returns `null` if not found
- **NullSafeSet**: Converts `Type` to string via `value.ToString()` (returns `Type.FullName`); writes `DBNull.Value` when null
- **Serialization/Storage Format**: Assembly-qualified type name string (e.g., `"System.String, mscorlib"`)
- **Null Handling**: DB null → `null`; CLR null → `DBNull.Value`; type not found → returns `null` (fallback search)
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; scans all loaded assemblies for type resolution
- **EF Core ValueConverter Migration**: `ValueConverter<Type, string>` with `ConvertToProvider: t => t?.ToString()` and `ConvertFromProvider: s => Type.GetType(s) ?? AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(a => a.Name.Equals(s) || Equals(a.FullName, s))`. **Risk**: Assembly-qualified names may differ across platforms (e.g., `mscorlib` vs `System.Runtime`). Consider storing simple type name + assembly name separately. Type resolution at runtime requires all assemblies to be loaded.

### 10. FilterExpressionConverter — `FilterExpressionConverter`

- **NHibernate Interface**: `IUserType` + `System.ComponentModel.TypeConverter`
- **CLR Type Handled**: `FilterExpression<bool>` (from `Shoko.Server.Filters`)
- **DB/Provider Type**: `VARCHAR` / `NVARCHAR` / `TEXT` (string)
- **NullSafeGet**: Reads via `NHibernateUtil.String.NullSafeGet(rs, names[0], impl)` → deserializes via `JsonConvert.DeserializeObject(s, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = _binder, Error = ... })` where `_binder` is `SimpleNameSerializationBinder(typeof(FilterExpression))`
- **NullSafeSet**: Serializes via `JsonConvert.SerializeObject(value, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = _binder })`
- **Serialization/Storage Format**: JSON with `TypeNameHandling.Objects` (includes type info for polymorphic deserialization), bound by `SimpleNameSerializationBinder` (strips assembly names, matches by type name only)
- **Null Handling**: DB null → `null`; CLR null → `DBNull.Value`; deserialization error → logs via NLog, sets `args.ErrorContext.Handled = true` (continues with partial result)
- **Provider-Specific Behavior**: None — uses `NHibernateUtil.String.SqlType`; depends on `SimpleNameSerializationBinder` which searches all loaded assemblies for type name matches (warning logged if multiple matches)
- **EF Core ValueConverter Migration**: `ValueConverter<FilterExpression<bool>, string>` with `ConvertToProvider: v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = new SimpleNameSerializationBinder(typeof(FilterExpression)) })` and `ConvertFromProvider: s => JsonConvert.DeserializeObject<FilterExpression<bool>>(s, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore, TypeNameHandling = TypeNameHandling.Objects, SerializationBinder = new SimpleNameSerializationBinder(typeof(FilterExpression)), Error = (_, args) => { LogManager.GetCurrentClassLogger().Error(args.ErrorContext.Error); args.ErrorContext.Handled = true; } })`. **Risk**: Heavy dependency on Newtonsoft.Json (already used in project). `TypeNameHandling.Objects` is a security risk if deserializing untrusted data. `SimpleNameSerializationBinder` may match wrong types if multiple assemblies define same type name.

---

### Utility Types (Not IUserType)

### 11. SimpleNameSerializationBinder.cs — `SimpleNameSerializationBinder`

- **Base Type**: `Newtonsoft.Json.Serialization.DefaultSerializationBinder`
- **Purpose**: Custom JSON serialization binder that strips assembly names and matches types by simple name only. Used by `FilterExpressionConverter`.
- **BindToName**: Sets `assemblyName = null`, `typeName = serializedType.Name` (simple name only)
- **BindToType**: Splits type name on `.`, searches all loaded assemblies for types matching the simple name, filters by optional `_baseType` constraint. Logs warning if multiple matches found.
- **EF Core Notes**: Not a value converter — used as a `SerializationBinder` parameter in Newtonsoft.Json settings. Must be replicated for EF Core if JSON serialization is used in value converters.

### 12. NHibernateDependencyInjector.cs — `NHibernateDependencyInjector`

- **Base Type**: `NHibernate.EmptyInterceptor`
- **Purpose**: Interceptor that injects DI container into NHibernate entity instantiation and post-load initialization.
- **Key Methods**:
  - `Instantiate(string clazz, object id)`: Uses `ActivatorUtilities.CreateInstance(_provider, type)` to create entities with constructor injection. Caches type metadata in static dictionaries.
  - `OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)`: Fires registered post-initialization callbacks via `s_postInitializationCallbacks` dictionary.
- **Static API**: `RegisterPostInitializationCallback<T>(Func<T, (string name, object value)[], bool>)` for registering callbacks.
- **EF Core Notes**: Not a value converter. EF Core handles constructor injection automatically via `DbContext` service resolution. No direct EF Core equivalent needed.

### 13. NLogInterceptor.cs — `NLogInterceptor`

- **Base Type**: `NHibernate.EmptyInterceptor`
- **Purpose**: Logging interceptor that logs every SQL statement before execution.
- **Key Method**: `OnPrepareStatement(SqlString sql)` → logs via NLog `Trace` level
- **EF Core Notes**: Not a value converter. EF Core logging handles this via `ILogger` integration. No direct EF Core equivalent needed.

---

### Summary Table

| # | File | Class | NHibernate Interface | CLR Type | DB Type | Serialization |
|---|------|-------|---------------------|----------|---------|---------------|
| 1 | `DateOnlyConverter.cs` | `DateOnlyConverter` | `IUserType` + `TypeConverter` | `DateOnly` | `DATE` | `DateTime` ticks |
| 2 | `MessagePackConverter.cs` | `MessagePackConverter<T>` | `IUserType` + `TypeConverter` | `T` (class) | `BLOB` | MessagePack binary |
| 3 | `TypelessMessagePackConverter.cs` | `TypelessMessagePackConverter` | `IUserType` + `TypeConverter` | `object` | `BLOB` | MessagePack Typeless |
| 4 | `StringListConverter.cs` | `StringListConverter` | `IUserType` + `TypeConverter` | `List<string>` | `VARCHAR/TEXT` | `"|||"` delimiter |
| 5 | `TitleLanguageConverter.cs` | `TitleLanguageConverter` | `IUserType` + `TypeConverter` | `TitleLanguage` | `VARCHAR/TEXT` | String (extension) |
| 6 | `TitleTypeConverter.cs` | `TitleTypeConverter` | `IUserType` + `TypeConverter` | `TitleType` | `VARCHAR/TEXT` | String (extension) |
| 7 | `TmdbContentRatingConverter.cs` | `TmdbContentRatingConverter` | `IUserType` + `TypeConverter` | `List<TMDB_ContentRating>` | `VARCHAR/TEXT` | `'|'` delimiter |
| 8 | `TmdbProductionCountryConverter.cs` | `TmdbProductionCountryConverter` | `IUserType` + `TypeConverter` | `List<TMDB_ProductionCountry>` | `VARCHAR/TEXT` | `'|'` delimiter |
| 9 | `TypeStringConverter.cs` | `TypeStringConverter` | `IUserType` + `TypeConverter` | `Type` | `VARCHAR/TEXT` | Assembly-qualified name |
| 10 | `FilterExpressionConverter.cs` | `FilterExpressionConverter` | `IUserType` + `TypeConverter` | `FilterExpression<bool>` | `VARCHAR/TEXT` | JSON (TypeNameHandling.Objects) |
| 11 | `SimpleNameSerializationBinder.cs` | `SimpleNameSerializationBinder` | `DefaultSerializationBinder` (utility) | N/A | N/A | N/A |
| 12 | `NHibernateDependencyInjector.cs` | `NHibernateDependencyInjector` | `EmptyInterceptor` (utility) | N/A | N/A | N/A |
| 13 | `NLogInterceptor.cs` | `NLogInterceptor` | `EmptyInterceptor` (utility) | N/A | N/A | N/A |

### EF Core Migration Priority

1. **High**: `MessagePackConverter<T>` and `TypelessMessagePackConverter` — used for `VideoLocal.MediaInfo` (MessagePack) and other binary payloads. Critical for data portability.
2. **High**: `StringListConverter` — used for `AnimeEpisode_User.UserTags`, `AnimeSeries_User.UserTags`, and other list<string> properties.
3. **Medium**: `TitleLanguageConverter`, `TitleTypeConverter` — enum converters used in title tables.
4. **Medium**: `TmdbContentRatingConverter`, `TmdbProductionCountryConverter` — enum list converters for TMDB models.
5. **Medium**: `DateOnlyConverter` — used for date-only columns (e.g., `AniDB_Anime.AirDate`, `AniDB_Anime.EndDate`).
6. **Medium**: `FilterExpressionConverter` — used for `FilterPreset.Expression` and `FilterPreset.SortingExpression`.
7. **Low**: `TypeStringConverter` — used sparingly for type references.
8. **Low**: `SimpleNameSerializationBinder` — utility for JSON deserialization, not a value converter.
9. **N/A**: `NHibernateDependencyInjector`, `NLogInterceptor` — NHibernate-specific interceptors, no EF Core equivalent needed.

### Cross-Cutting Risks

1. **Extension method dependencies**: `TitleLanguageConverter`, `TitleTypeConverter`, `StringListConverter`, `TmdbContentRatingConverter`, `TmdbProductionCountryConverter` all depend on extension methods from `Shoko.Abstractions.Extensions` (`GetTitleLanguage()`, `GetString()`, `GetTitleType()`, `Join()`). These must be replicated or moved to a shared location.
2. **Logging dependency**: `MessagePackConverter<T>` and `TypelessMessagePackConverter` call `Utils.ServiceContainer.GetRequiredService<ILogger<...>>()` inside `ConvertFrom`/`ConvertTo`. EF Core value converters must be stateless — inject logging externally or use a different approach.
3. **Delimiter collision**: `StringListConverter` uses `"|||"` and TMDB converters use `'|'` — both risk data corruption if list elements contain the delimiter. Consider JSON array serialization for EF Core.
4. **TypeNameHandling security**: `FilterExpressionConverter` uses `TypeNameHandling.Objects` — a known security risk. EF Core value converters should avoid this if deserializing untrusted data.
5. **Assembly-qualified names**: `TypeStringConverter` stores assembly-qualified type names which may differ across platforms. Consider storing `typeName + assemblyName` as separate columns or using a type registry.

14. **No initial data**: The `PopulateInitialData()` method in `BaseDatabase.cs` creates default users, group filters, rename scripts, and custom tags. EF Core seed data (`OnModelCreating` or `HasData()`) should replicate this initial data population.

---

## Repository Inventory (T007)

**Total**: 85 repository files (verified via `find Shoko.Server/Repositories/ -name '*Repository.cs' | wc -l`)

### T007 partial — Base, Interfaces, Session Infrastructure

This partial pass covers only base classes, interfaces, session infrastructure, factory, startup, and helper files directly under `Shoko.Server/Repositories/`. Cached/ and Direct/ subdirectories are pending.

### 1. BaseRepository.cs

- **File Path**: `Shoko.Server/Repositories/BaseRepository.cs`
- **Class**: `BaseRepository`
- **Category**: Base class (static utility)
- **NHibernate Usage**: None — pure static locking utility
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: No migration needed. Contains only static `Lock()` overloads that wrap actions in `lock()` blocks when `DatabaseSettings.UseDatabaseLock` is enabled. This is a thread-safety wrapper independent of the ORM. EF Core equivalent: keep as-is; `lock()` semantics are identical.
- **Risk Level**: Low

### 2. BaseCachedRepository.cs

- **File Path**: `Shoko.Server/Repositories/BaseCachedRepository.cs`
- **Class**: `BaseCachedRepository<T, S>`
- **Category**: Base class (cached repositories)
- **NHibernate Usage**: `ISessionFactory.OpenSession()`, `ISession.CreateCriteria()`, `ISession.SaveOrUpdate()`, `ISession.Delete()`, `ISession.BeginTransaction()`, `ISession.DeleteAsync()`, `ISession.SaveAsync()`, `ISessionWrapper` extension methods (`Wrap()`, `Insert()`, `Update()`), `QueryOver` (not directly, but `ISessionWrapper` exposes it)
- **Transaction Usage**: Heavy — `Save()` creates new transaction per call; `SaveWithOpenTransaction()` and `DeleteWithOpenTransaction()` expect caller-managed transactions; bulk `Save(IReadOnlyCollection<T>)` and `Delete(IReadOnlyCollection<T>)` use single transaction; async variants exist (`SaveWithOpenTransactionAsync`, `DeleteWithOpenTransactionAsync`)
- **Cache Usage**: Core — wraps all cache operations in `ReaderWriterLockSlim`; `PocoCache<S, T>` for in-memory caching; `Populate()` loads all entities via `CreateCriteria` into `PocoCache`; `UpdateCacheUnsafe()` / `DeleteFromCacheUnsafe()` for cache maintenance; `PopulateIndexes()` / `RegenerateDb()` / `PostProcess()` abstract hooks for typed indexes
- **EF Core Migration Notes**: Major rewrite required. `ISessionFactory.OpenSession()` → `ShokoDbContext` via DI (scoped). `CreateCriteria<T>().List<T>()` → `context.Set<T>().AsNoTracking().ToListAsync()`. `session.SaveOrUpdate()` → `context.Entry(obj).State = EntityState.Modified; await context.SaveChangesAsync()`. `session.Delete()` → `context.Remove(obj)`. `session.BeginTransaction()` → `context.Database.BeginTransaction()`. `ReaderWriterLockSlim` → consider `AsyncReaderWriterLock` or `SemaphoreSlim` for async safety. `PocoCache` structure preserved but population changes. `ISessionWrapper` interface must be replaced with EF Core equivalent. `Utils.ServiceContainer.GetRequiredService<SystemService>()` — prefer DI constructor injection.
- **Risk Level**: High

### 3. BaseDirectRepository.cs

- **File Path**: `Shoko.Server/Repositories/BaseDirectRepository.cs`
- **Class**: `BaseDirectRepository<T, S>`
- **Category**: Base class (direct repositories)
- **NHibernate Usage**: `ISessionFactory.OpenSession()`, `ISession.Get<T>()`, `ISession.CreateCriteria()`, `ISession.SaveOrUpdate()`, `ISession.Delete()`, `ISession.BeginTransaction()`, `ISessionWrapper` extension methods (`Wrap()`, `Insert()`, `Update()`)
- **Transaction Usage**: Moderate — `Save()` creates new transaction; `SaveWithOpenTransaction()` / `DeleteWithOpenTransaction()` expect caller-managed transactions; bulk operations use single transaction
- **Cache Usage**: None (direct access, no caching)
- **EF Core Migration Notes**: Moderate rewrite. `ISessionFactory.OpenSession()` → `ShokoDbContext` via DI. `session.Get<T>(id)` → `context.Set<T>().FindAsync(id)`. `CreateCriteria<T>().List<T>()` → `context.Set<T>().ToListAsync()`. `session.SaveOrUpdate()` → `context.Update()` or `context.Add()` + `SaveChangesAsync()`. `session.Delete()` → `context.Remove()`. `session.BeginTransaction()` → `context.Database.BeginTransaction()`. Same async lock concerns as BaseCachedRepository. `ISessionWrapper` → EF Core adapter.
- **Risk Level**: High

### 4. IRepository.cs

- **File Path**: `Shoko.Server/Repositories/IRepository.cs`
- **Class/Interface**: `IRepository<T, in S>`
- **Category**: Interface
- **NHibernate Usage**: Exposes `ISession` and `ISessionWrapper` overloads for all CRUD methods (`GetByID(ISession, S)`, `GetByID(ISessionWrapper, S)`, etc.)
- **Transaction Usage**: Exposes callback types with `ISession` and `ISessionWrapper` (`DeleteWithOpenTransactionCallback`, `SaveWithOpenTransactionCallback`)
- **Cache Usage**: None (interface only)
- **EF Core Migration Notes**: Interface must be updated to use EF Core session wrapper instead of `ISession`. All `ISession` parameter overloads → `ShokoDbContext` or `ISessionWrapper` (EF Core adapter). Callback types change from `Action<ISession, T>` to `Action<ShokoDbContext, T>` or `Action<ISessionWrapper, T>` (EF Core). This is a breaking change for all 85 repository implementations. Consider creating `IEfCoreRepository<T, S>` as a parallel interface.
- **Risk Level**: High

### 5. ICachedRepository.cs

- **File Path**: `Shoko.Server/Repositories/ICachedRepository.cs`
- **Class/Interface**: `ICachedRepository`
- **Category**: Interface
- **NHibernate Usage**: `Populate(ISessionWrapper session, ...)` — takes `ISessionWrapper` parameter
- **Transaction Usage**: None directly
- **Cache Usage**: None (interface only)
- **EF Core Migration Notes**: `Populate(ISessionWrapper, ...)` → `Populate(ShokoDbContext, ...)` or remove the session parameter entirely since EF Core DbContext is resolved via DI. `PopulateIndexes()`, `RegenerateDb()`, `PostProcess()` are abstract hooks with no NHibernate dependencies — no changes needed.
- **Risk Level**: Medium

### 6. IDirectRepository.cs

- **File Path**: `Shoko.Server/Repositories/IDirectRepository.cs`
- **Class/Interface**: `IDirectRepository` (marker interface)
- **Category**: Interface
- **NHibernate Usage**: None — empty marker interface
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: No changes needed. Marker interface used only for DI registration (`services.AddSingleton<IDirectRepository, Repo>()`). Can be preserved as-is.
- **Risk Level**: Low

### 7. ISessionWrapper.cs

- **File Path**: `Shoko.Server/Repositories/NHibernate/ISessionWrapper.cs`
- **Class/Interface**: `ISessionWrapper` (interface)
- **Category**: Session wrapper interface
- **NHibernate Usage**: Exposes `ICriteria`, `IQuery`, `ISQLQuery`, `IQueryOver<T, T>`, `ITransaction`, `IDbConnection` — all NHibernate types
- **Transaction Usage**: `BeginTransaction()` returns `ITransaction`
- **Cache Usage**: None
- **EF Core Migration Notes**: Complete redesign needed. Replace NHibernate-specific types with EF Core equivalents:
  - `ICriteria` → removed (EF Core uses LINQ)
  - `IQuery` → removed (EF Core uses LINQ)
  - `ISQLQuery` → `DbCommand` or raw SQL via `context.Database.ExecuteSqlRaw()`
  - `IQueryOver<T, T>` → removed (EF Core uses LINQ)
  - `IQueryable<T> Query<T>()` → `context.Set<T>().AsQueryable()`
  - `ITransaction BeginTransaction()` → `DbTransaction` via `context.Database.BeginTransaction()`
  - `Insert/Update/Delete` → map to `context.Add/Update/Remove`
  - `IDbConnection Connection` → `context.Database.GetDbConnection()`
  - `GetAsync<T>` → `context.Set<T>().FindAsync()`
  This interface is the main abstraction point for gradual migration — all repository code goes through `ISessionWrapper`.
- **Risk Level**: High

### 8. SessionWrapper.cs

- **File Path**: `Shoko.Server/Repositories/NHibernate/SessionWrapper.cs`
- **Class**: `SessionWrapper`
- **Category**: Session wrapper (NHibernate ISession)
- **NHibernate Usage**: Wraps `ISession` — delegates all calls to underlying session: `CreateCriteria`, `CreateQuery`, `CreateSQLQuery`, `QueryOver`, `Query`, `Get`, `GetAsync`, `BeginTransaction`, `Insert(Save)`, `Update`, `Delete`, async variants, `Connection`
- **Transaction Usage**: `BeginTransaction()` delegates to `_session.BeginTransaction()`
- **Cache Usage**: None
- **EF Core Migration Notes**: Replace with `EfCoreSessionWrapper` wrapping `DbContext`. All NHibernate-specific methods (Criteria, QueryOver, Query) need EF Core equivalents or must be removed. CRUD delegation maps cleanly: `Insert` → `context.Add()`, `Update` → `context.Update()`, `Delete` → `context.Remove()`. `Connection` → `context.Database.GetDbConnection()`. `GetAsync` → `FindAsync()`.
- **Risk Level**: High

### 9. StatelessSessionWrapper.cs

- **File Path**: `Shoko.Server/Repositories/NHibernate/StatelessSessionWrapper.cs`
- **Class**: `StatelessSessionWrapper`
- **Category**: Session wrapper (NHibernate IStatelessSession)
- **NHibernate Usage**: Wraps `IStatelessSession` — same interface as `SessionWrapper` but delegates to `IStatelessSession`. Key difference: `Insert` uses `_session.Insert()` (stateless, bypasses change tracking), `Update`/`Delete` same as stateful
- **Transaction Usage**: `BeginTransaction()` delegates to `_session.BeginTransaction()`
- **Cache Usage**: None
- **EF Core Migration Notes**: EF Core has no direct stateless session equivalent. `IStatelessSession` is fire-and-forget (no change tracking, no events). EF Core equivalent: `context.ChangeTracker.Clear()` before bulk operations, or use `ExecuteUpdate`/`ExecuteDelete` for bulk operations. `Insert` → `context.Add()` (but call `ChangeTracker.Clear()` after). This wrapper exists primarily for bulk operations. Consider removing and using raw ADO or EF Core bulk extensions for stateless inserts.
- **Risk Level**: Medium

### 10. SessionExtensions.cs

- **File Path**: `Shoko.Server/Repositories/NHibernate/SessionExtensions.cs`
- **Class**: `SessionExtensions` (static extension methods)
- **NHibernate Usage**: `Wrap(this ISession)` → `new SessionWrapper(session)`
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: Replace with `DbContextExtensions.Wrap(this DbContext)` → `new EfCoreSessionWrapper(context)`. Simple one-line extension method.
- **Risk Level**: Low

### 11. StatelessSessionExtensions.cs

- **File Path**: `Shoko.Server/Repositories/NHibernate/StatelessSessionExtensions.cs`
- **Class**: `StatelessSessionExtensions` (static extension methods)
- **NHibernate Usage**: `Wrap(this IStatelessSession)` → `new StatelessSessionWrapper(session)`
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: May not be needed if stateless session pattern is replaced. If bulk operations require it, create `DbContextExtensions` with a `CreateStateless()` method that returns a wrapper with `ChangeTracker.Clear()`.
- **Risk Level**: Low

### 12. RepoFactory.cs

- **File Path**: `Shoko.Server/Repositories/RepoFactory.cs`
- **Class**: `RepoFactory`
- **Category**: Static factory / service locator
- **NHibernate Usage**: Indirect — caches `ICachedRepository[]` and calls `repo.Populate(cancellationToken)` which internally uses NHibernate. Constructor injects all 57+ repositories via DI. `Init()` calls `Populate()` on all cached repos. `PostInit()` calls `RegenerateDb()` and `PostProcess()` on all cached repos.
- **Transaction Usage**: None directly
- **Cache Usage**: Holds `ICachedRepository[]` and drives cache population via `Init()` and `PostInit()`
- **EF Core Migration Notes**: No structural changes needed. `Init()` and `PostInit()` call `ICachedRepository` interface methods which remain the same contract. `Populate(cancellationToken)` signature unchanged (base class handles migration internally). `PostInit()` calls `RegenerateDb()` and `PostProcess()` — unchanged. The static field pattern (`public static AniDB_Anime AniDB_Anime;`) is preserved for backward compatibility. DI constructor injection already in place — no changes needed.
- **Risk Level**: Low

### 13. RepositoryStartup.cs

- **File Path**: `Shoko.Server/Repositories/RepositoryStartup.cs`
- **Class**: `RepositoryStartup` (static extension method)
- **Category**: DI registration
- **NHibernate Usage**: None directly — registers repositories as singletons. `AddDirectRepository<T>()` registers `IDirectRepository` + typed singleton. `AddCachedRepository<T>()` registers `ICachedRepository` + typed singleton.
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: No changes needed for initial migration. `AddShokoDbContext()` extension should be added to register `ShokoDbContext` as scoped (EF Core DbContext pattern). Repository registrations remain as-is since base classes handle the migration internally. Consider adding `services.AddScoped<ShokoDbContext>()` before `AddRepositories()`.
- **Risk Level**: Low

### 14. ChangeTracker.cs

- **File Path**: `Shoko.Server/Repositories/ChangeTracker.cs`
- **Class**: `ChangeTracker<T>` + `Changes<T>`
- **Category**: Helper / utility class
- **NHibernate Usage**: None — generic change tracking utility
- **Transaction Usage**: None
- **Cache Usage**: None
- **EF Core Migration Notes**: No migration needed. Generic thread-safe change tracker using `ReaderWriterLockSlim` and `Dictionary<T, DateTime>`. Used for tracking entity changes across repositories. EF Core has its own `ChangeTracker` but this is a different concern (cross-entity change notification, not EF Core change tracking). Preserved as-is.
- **Risk Level**: Low

### Summary Table — Base/Interface/Session Files

| # | File | Class | Category | NHibernate Types | EF Core Equivalent | Risk |
|---|------|-------|----------|-----------------|-------------------|------|
| 1 | `BaseRepository.cs` | `BaseRepository` | Static utility | None | No change | Low |
| 2 | `BaseCachedRepository.cs` | `BaseCachedRepository<T,S>` | Cached base class | ISession, ISessionFactory, ITransaction | ShokoDbContext, DbTransaction | High |
| 3 | `BaseDirectRepository.cs` | `BaseDirectRepository<T,S>` | Direct base class | ISession, ISessionFactory, ITransaction | ShokoDbContext, DbTransaction | High |
| 4 | `IRepository.cs` | `IRepository<T,S>` | Interface | ISession, ISessionWrapper | ShokoDbContext, ISessionWrapper (EF) | High |
| 5 | `ICachedRepository.cs` | `ICachedRepository` | Interface | ISessionWrapper | ShokoDbContext (DI) | Medium |
| 6 | `IDirectRepository.cs` | `IDirectRepository` | Marker interface | None | No change | Low |
| 7 | `ISessionWrapper.cs` | `ISessionWrapper` | Session interface | ISession, ICriteria, IQuery, IQueryOver, ITransaction | DbContext, DbTransaction | High |
| 8 | `SessionWrapper.cs` | `SessionWrapper` | Session impl | ISession | EfCoreSessionWrapper (DbContext) | High |
| 9 | `StatelessSessionWrapper.cs` | `StatelessSessionWrapper` | Stateless impl | IStatelessSession | No direct equivalent | Medium |
| 10 | `SessionExtensions.cs` | `SessionExtensions` | Extension | ISession.Wrap() | DbContext.Wrap() | Low |
| 11 | `StatelessSessionExtensions.cs` | `StatelessSessionExtensions` | Extension | IStatelessSession.Wrap() | Remove or adapt | Low |
| 12 | `RepoFactory.cs` | `RepoFactory` | Static factory | None (indirect via ICachedRepository) | No change | Low |
| 13 | `RepositoryStartup.cs` | `RepositoryStartup` | DI registration | None | Add ShokoDbContext registration | Low |
| 14 | `ChangeTracker.cs` | `ChangeTracker<T>` | Utility | None | No change | Low |

### Cross-Cutting Risks — Base/Interface/Session

1. **ISessionWrapper abstraction is the migration bottleneck**: All repository code goes through `ISessionWrapper`. This interface must be redesigned to support both NHibernate (during dual-ORM period) and EF Core. Consider a bridge pattern: `ISessionWrapper` → `NHibernateSessionWrapper` / `EfCoreSessionWrapper`.
2. **Async lock safety**: `ReaderWriterLockSlim` in `BaseCachedRepository` is NOT async-safe. EF Core async operations (`SaveChangesAsync`, `FindAsync`) require `AsyncReaderWriterLock` or `SemaphoreSlim` to avoid deadlocks.
3. **Transaction boundary changes**: NHibernate `ISession` transactions are auto-flushed before commit. EF Core `DbContext` has different flush behavior — `SaveChangesAsync()` is the explicit flush point. Must verify all `SaveWithOpenTransaction` call sites.
4. **ISession parameter leakage**: `IRepository<T,S>` and `IUserType` callbacks expose `ISession` directly. All call sites must be audited during migration.
5. **StatelessSession pattern**: `IStatelessSession` is used for bulk operations that bypass change tracking. EF Core has no equivalent — must be replaced with `ExecuteUpdate`/`ExecuteDelete` or raw ADO.NET.

---

### T007 partial — Cached Repositories

This partial pass covers all 42 cached repositories in `Shoko.Server/Repositories/Cached/` (root + `AniDB/` + `TMDB/` subdirectories).

### 15. VideoLocal_UserRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/VideoLocal_UserRepository.cs`
- **Class**: `VideoLocal_UserRepository`
- **Base Class**: `BaseCachedRepository<VideoLocal_User, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based lookups via `PocoIndex` — `GetByVideoLocalID`, `GetByUserID`, `GetByUserAndVideoLocalID`
- **Transaction Usage**: None directly — inherits from base
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_videoLocalIDs`, `_userIDs`, `_userVideoLocalIDs`); all queries wrapped in `ReadLock()`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 3 indexes on `VideoLocalID`, `JMMUserID`, `(JMMUserID, VideoLocalID)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Index-based lookups → EF Core `Where()` + `FirstOrDefault()`. `ReadLock()` → scoped DbContext with `AsNoTracking()`. No NHibernate-specific code. Low risk.
- **Risk Level**: Low

### 16. VideoLocal_PlaceRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/VideoLocal_PlaceRepository.cs`
- **Class**: `VideoLocal_PlaceRepository`
- **Base Class**: `BaseCachedRepository<VideoLocal_Place, int>`
- **NHibernate Usage**: `BeginSaveCallback` validation; `RegenerateDb()` uses `SessionFactory.OpenSession()` and `session.BeginTransaction()` + `DeleteWithOpenTransaction(session, entry)`
- **Query Patterns**: Index-based — `GetByRelativePath`, `GetByManagedFolderID`, `GetByRelativePathAndManagedFolderID`, `GetByVideoLocal`
- **Transaction Usage**: `RegenerateDb()` creates transactions per batch of 50 orphan entries; `BeginSaveCallback` validates before save
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_videoLocalIDs`, `_managedFolderIDs`, `_paths`); `ReadLock()` on all queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `VideoID`, `ManagedFolderID`, `RelativePath`; `RegenerateDb()` deletes orphaned entries (VideoID=0, ManagedFolderID=0, null/empty path)
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None — LINQ on `Cache.Values`
- **EF Core Migration Notes**: `RegenerateDb()` → `ShokoDbContext` via DI; `session.BeginTransaction()` → `context.Database.BeginTransaction()`. `BeginSaveCallback` validation → EF Core `DbContext.SaveChanges()` intercept or service-layer validation. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 17. VideoLocalRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/VideoLocalRepository.cs`
- **Class**: `VideoLocalRepository`
- **Base Class**: `BaseCachedRepository<VideoLocal, int>`
- **NHibernate Usage**: `DeleteWithOpenTransactionCallback` deletes child entities via RepoFactory; `RegenerateDb()` uses `SessionFactory.OpenSession()`, `session.BeginTransaction()`, `DeleteWithOpenTransaction`; `UpdateMediaContracts` calls `VideoService.RefreshMediaInfo`
- **Query Patterns**: Index-based (`_ed2k`, `_ignored`); cross-reference joins via `CrossRef_File_Episode`; cache scans (`Cache.Values`)
- **Transaction Usage**: `RegenerateDb()` creates multiple transactions for orphan cleanup, duplicate merging, and batch deletes; `DeleteWithOpenTransactionCallback` cascades deletes
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_ed2k` for ED2K hash, `_ignored` for IsIgnored); `ReadLock()` on all queries; `Cache.Values` scans for filtering
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `Hash` and `IsIgnored`; null-hash fixup in populate; `RegenerateDb()` queues MediaInfoJob for stale records, cleans empty records, merges duplicates by ED2K hash
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None directly — LINQ on `Cache.Values`
- **EF Core Migration Notes**: High complexity. `RegenerateDb()` is the most complex — 3-phase cleanup (MediaInfoJob queue, empty record deletion, duplicate merge). Requires careful transaction management in EF Core. `DeleteWithOpenTransactionCallback` cascade → EF Core cascade delete or explicit child deletion. `Save(obj, updateEpisodes)` overrides base Save with episode cascade — needs EF Core `SaveChangesAsync()` after each phase. `Utils.ServiceContainer.GetRequiredService<ISchedulerFactory>()` → DI injection. `ReadLock()` → `AsNoTracking()`.
- **Risk Level**: High

### 18. VideoLocal_HashDigestRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/VideoLocal_HashDigestRepository.cs`
- **Class**: `VideoLocal_HashDigestRepository`
- **Base Class**: `BaseCachedRepository<VideoLocal_HashDigest, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByVideoLocalID`, `GetByHashType`, `GetByVideoIDAndHashType`, `GetByHashTypeAndValue`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 4 indexes (`_videoIDs`, `_videoIDAndHashTypes`, `_hashTypes`, `_hashTypeAndValues`); all queries wrapped in `ReadLock()`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `VideoLocalID`, `(VideoLocalID, Type)`, `Type`, `(Type, Value)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Index-based lookups → EF Core `Where()` + `FirstOrDefault()`. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 19. StoredReleaseInfoRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/StoredReleaseInfoRepository.cs`
- **Class**: `StoredReleaseInfoRepository`
- **Base Class**: `BaseCachedRepository<StoredReleaseInfo, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based (`_ed2k`, `_groupIDs`, `_releaseURIs`, `_anidbEpisodeIDs`, `_anidbAnimeIDs`); `GetReleaseGroups()` / `GetUsedReleaseGroups()` / `GetUnusedReleaseGroups()` scan `GetAll()` with LINQ
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 5 indexes; `ReadLock()` on index-based queries; `GetAll()` used for bulk scans
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `ED2K`, `(GroupID, GroupSource)`, `ReleaseURI`, `CrossReferences[].AnidbEpisodeID`, `CrossReferences[].AnidbAnimeID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `GetAll()` for group aggregation
- **EF Core Migration Notes**: Simple. Index-based lookups → EF Core `Where()`. `GetAll()` scans → `context.Set<T>().AsNoTracking()`. `CrossReferences` navigation properties → EF Core `Include()` / `SelectMany()`. Low risk.
- **Risk Level**: Low

### 20. StoredRelocationPipeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/StoredRelocationPipeRepository.cs`
- **Class**: `StoredRelocationPipeRepository`
- **Base Class**: `BaseCachedRepository<StoredRelocationPipe, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByName`, `GetByPipeID`, `GetByProviderID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_pipeIDs`, `_providerIDs`, `_names`); `Lock()` (write lock) on all queries — unusual for read-only operations
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `ID`, `ProviderID`, `Name`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Note: uses `Lock()` (write lock) instead of `ReadLock()` for read queries — likely a design choice for consistency. `Lock()` → scoped DbContext. Low risk.
- **Risk Level**: Low

### 21. StoredReleaseInfo_MatchAttemptRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/StoredReleaseInfo_MatchAttemptRepository.cs`
- **Class**: `StoredReleaseInfo_MatchAttemptRepository`
- **Base Class**: `BaseCachedRepository<StoredReleaseInfo_MatchAttempt, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByEd2k`, `GetByEd2kAndFileSize`, `GetBySourceProviderNames`, `GetByResultProviderNames`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_ed2k`, `_sourceProviderNames`, `_resultProviderNames`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `ED2K`, `AttemptedProviderNames`, `ProviderName`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 22. ShokoManagedFolderRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/ShokoManagedFolderRepository.cs`
- **Class**: `ShokoManagedFolderRepository`
- **Base Class**: `BaseCachedRepository<ShokoManagedFolder, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: `GetByImportLocation` scans `Cache.Values` with path normalization; `GetFromAbsolutePath` scans `GetAll()` with prefix matching
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: No indexes defined; all queries use `ReadLock()` on `Cache.Values` or `GetAll()` scans
- **Populate / PopulateIndexes**: No `PopulateIndexes()` overrides — no indexes
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `Cache.Values` / `GetAll()`
- **EF Core Migration Notes**: Path normalization queries → EF Core `Where()` with string comparison. No indexes means full table scan — consider adding database indexes for `Path` and `Path LIKE @prefix` patterns. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 23. CrossRef_AniDB_TMDB_ShowRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_AniDB_TMDB_ShowRepository.cs`
- **Class**: `CrossRef_AniDB_TMDB_ShowRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_AniDB_TMDB_Show, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnidbAnimeID`, `GetByTmdbShowID`, `GetByAnidbAnimeAndTmdbShowIDs`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_anidbAnimeIDs`, `_tmdbShowIDs`, `_pairedIDs`); `ReadLock()` on all queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `TmdbShowID`, `AnidbAnimeID`, `(AnidbAnimeID, TmdbShowID)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 24. CrossRef_AniDB_TMDB_MovieRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_AniDB_TMDB_MovieRepository.cs`
- **Class**: `CrossRef_AniDB_TMDB_MovieRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_AniDB_TMDB_Movie, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnidbAnimeID`, `GetByAnidbEpisodeID`, `GetByAnidbEpisodeAndTmdbMovieIDs`, `GetByTmdbMovieID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_anidbAnimeIDs`, `_anidbEpisodeIDs`, `_tmdbMovieIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `TmdbMovieID`, `AnidbAnimeID`, `AnidbEpisodeID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 25. CrossRef_AniDB_TMDB_EpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_AniDB_TMDB_EpisodeRepository.cs`
- **Class**: `CrossRef_AniDB_TMDB_EpisodeRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_AniDB_TMDB_Episode, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnidbAnimeID`, `GetByAnidbEpisodeID`, `GetByAnidbEpisodeAndTmdbEpisodeIDs`, `GetByTmdbShowID`, `GetByTmdbEpisodeID`, `GetAllByAnidbAnimeAndTmdbShowIDs`, `GetOnlyByAnidbAnimeAndTmdbShowIDs`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 5 indexes (`_anidbAnimeIDs`, `_anidbEpisodeIDs`, `_tmdbShowIDs`, `_tmdbEpisodeIDs`, `_pairedIDs`); `ReadLock()` on index queries; `GetByAnidbEpisodeID` / `GetByTmdbEpisodeID` sort by `Ordering`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 5 indexes; sorting applied at query time via LINQ `OrderBy(a => a.Ordering)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` / `FirstOrDefault` / `Concat`
- **EF Core Migration Notes**: Simple. `Concat` in `GetAllByAnidbAnimeAndTmdbShowIDs` → EF Core `Union` or separate queries. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 26. JMMUserRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/JMMUserRepository.cs`
- **Class**: `JMMUserRepository`
- **Base Class**: `BaseCachedRepository<JMMUser, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: `GetByUsername` scans `Cache.Values` with case-insensitive comparison; `GetAniDBUser` / `GetTraktUsers` filter by flags
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: No indexes; all queries use `ReadLock()` on `Cache.Values`
- **Populate / PopulateIndexes**: No `PopulateIndexes()` override — no indexes
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `Cache.Values`
- **EF Core Migration Notes**: Case-insensitive username lookup → EF Core `EF.Functions.Collate()` or `.ToLower()` normalization. Password hashing via `Digest.Hash()` — preserve for migration. No indexes means full table scan — consider adding database indexes. Low risk.
- **Risk Level**: Low

### 27. CustomTagRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CustomTagRepository.cs`
- **Class**: `CustomTagRepository`
- **Base Class**: `BaseCachedRepository<CustomTag, int>`
- **NHibernate Usage**: `DeleteWithOpenTransactionCallback` deletes child `CrossRef_CustomTag` entries via RepoFactory
- **Query Patterns**: Index-based (`_names`); `GetByAnimeID` joins via `CrossRef_CustomTag`
- **Transaction Usage**: `DeleteWithOpenTransactionCallback` cascades deletes
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_names`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `TagName`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `GetAll()` for cross-reference join
- **EF Core Migration Notes**: Simple. `DeleteWithOpenTransactionCallback` → EF Core cascade delete or explicit child deletion. Low risk.
- **Risk Level**: Low

### 28. FilterPresetRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/FilterPresetRepository.cs`
- **Class**: `FilterPresetRepository`
- **Base Class**: `BaseCachedRepository<FilterPreset, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based (`_parentIDs`); `GetLockedGroupFilters` / `GetTimeDependentFilters` scan `Cache.Values`; static factory methods `GetAllYearFilters`, `GetAllSeasonFilters`, `GetAllTagFilters` create dynamic presets
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_parentIDs`); `ReadLock()` on index queries; `WriteLock()` on `Save()`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `ParentFilterPresetID`; `PostProcess()` cleans duplicates; `CreateOrVerifyLockedFilters()` creates default filters; `CreateInitialFilters()` creates 7 default group filters with expression trees
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `Cache.Values` / `GetAll()`
- **EF Core Migration Notes**: Complex. `PostProcess()` deletes duplicates — needs EF Core `RemoveRange()`. `CreateOrVerifyLockedFilters()` and `CreateInitialFilters()` create default data — consider EF Core seed data. Expression tree objects (`AndExpression`, `HasWatchedEpisodesExpression`, etc.) are serialized to `Expression` column via custom NHibernate type — preserve serialization logic. `WriteLock()` on `Save()` → scoped DbContext. Medium risk.
- **Risk Level**: Medium

### 29. CrossRef_CustomTagRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_CustomTagRepository.cs`
- **Class**: `CrossRef_CustomTagRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_CustomTag, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByCustomTagID`, `GetByAnimeID`, `GetByUniqueID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_customTagIDs`, `_entityIDandType`); `ReadLock()` on all queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `(CustomTagID, CrossRefType)`, `(CrossRefID, CrossRefType)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 30. CrossRef_File_EpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_File_EpisodeRepository.cs`
- **Class**: `CrossRef_File_EpisodeRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_File_Episode, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByEd2k`, `GetByAnimeID`, `GetByFileNameAndSize`, `GetByEpisodeID`
- **Transaction Usage**: `EndSaveCallback` triggers `RefreshAnimeStatsJob`; `EndDeleteCallback` triggers `RefreshAnimeStatsJob`
- **PocoCache / ReaderWriterLockSlim**: 4 indexes (`_ed2k`, `_anidbAnimeIDs`, `_anidbEpisodeIDs`, `_fileNames`); `ReadLock()` on all queries; `GetByEd2k` sorts by `EpisodeOrder`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 4 indexes; callbacks fire async job processing
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` in `GetByEd2k`
- **EF Core Migration Notes**: Callbacks fire `RefreshAnimeStatsJob` synchronously via `.GetAwaiter().GetResult()` — potential deadlock risk with EF Core async. Consider making callbacks async-compatible. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 31. AnimeSeries_UserRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeSeries_UserRepository.cs`
- **Class**: `AnimeSeries_UserRepository`
- **Base Class**: `BaseCachedRepository<AnimeSeries_User, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByUserAndSeriesID`, `GetByUserID`, `GetBySeriesID`; `GetMostRecentlyWatched` filters and sorts
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_userIDs`, `_seriesIDs`, `_userSeriesIDs`); `ReadLock()` on all queries; `Dictionary<int, ChangeTracker<int>>` tracks per-user changes
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 3 indexes; `EndDeleteCallback` removes from `ChangeTracker`; `Save()` adds to `ChangeTracker`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderByDescending` / `Where` in `GetMostRecentlyWatched`
- **EF Core Migration Notes**: `ChangeTracker<int>` is a custom class (not EF Core) — preserved as-is. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 32. AnimeSeriesRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeSeriesRepository.cs`
- **Class**: `AnimeSeriesRepository`
- **Base Class**: `BaseCachedRepository<AnimeSeries, int>`
- **NHibernate Usage**: `Lock()` method uses `SessionFactory.OpenSession()`, `session.Get<AnimeSeries>()`, `session.CreateSQLQuery()` with raw SQL; `UpdateBatch(ISessionWrapper session, ...)` uses `session.UpdateAsync()`
- **Query Patterns**: Index-based (`AniDBIds`, `Groups`); raw SQL for `GetWithMultipleReleases`, `GetWithDuplicateFiles`, `GetWithMissingEpisodes`; cache scans for `GetWithMissingEpisodes`, `GetMostRecentlyAdded`
- **Transaction Usage**: None directly (but `Lock()` opens sessions internally)
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`AniDBIds` on `AniDB_ID`, `Groups` on `AnimeGroupID`); `ReadLock()` on index queries; `ChangeTracker<int>` for change notification
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 2 indexes + `Changes.AddOrUpdateRange(Cache.Keys)`; `RegenerateDb()` resets preferred titles, ensures groups exist via `AnimeGroupCreator`; `Save()` has complex multi-phase logic with explicit session management
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: **4 raw SQL queries**: `MultipleReleasesIgnoreVariationsQuery`, `MultipleReleasesCountVariationsQuery`, `DuplicateFilesQuery`, `MissingEpisodesCollectingQuery`, `MissingEpisodesQuery` — all use `CreateSQLQuery().AddScalar()`; `UpdateBatch` uses `ISessionWrapper.UpdateAsync()`
- **EF Core Migration Notes**: **High risk.** 4 raw SQL queries need EF Core equivalents (likely via `FromSqlRaw()` or LINQ). `Lock()` method opens explicit sessions — replace with scoped DbContext. `UpdateBatch(ISessionWrapper session, ...)` → `UpdateBatch(ShokoDbContext context, ...)`. `RegenerateDb()` uses `Utils.ServiceContainer.GetRequiredService<AnimeGroupCreator>()` → DI injection. `Save()` has complex multi-phase logic with explicit session management — needs careful EF Core translation. `Changes` tracker preserved as custom class.
- **Risk Level**: High

### 33. AnimeGroup_UserRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeGroup_UserRepository.cs`
- **Class**: `AnimeGroup_UserRepository`
- **Base Class**: `BaseCachedRepository<AnimeGroup_User, int>`
- **NHibernate Usage**: `InsertBatch`, `UpdateBatch`, `DeleteAll` use `ISessionWrapper` — `session.BeginTransaction()`, `session.InsertAsync()`, `session.UpdateAsync()`, `session.CreateSQLQuery().ExecuteUpdateAsync()`
- **Query Patterns**: Index-based — `GetByUserAndGroupID`, `GetByUserID`, `GetByGroupID`
- **Transaction Usage**: `InsertBatch` and `UpdateBatch` create per-batch transactions; `DeleteAll` executes raw SQL `DELETE FROM AnimeGroup_User WHERE AnimeGroup_UserID > 0`
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_groupIDs`, `_userIDs`, `_userGroupIDs`); `ReadLock()` on index queries; `Dictionary<int, ChangeTracker<int>>` for per-user changes
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 3 indexes + initializes `ChangeTracker` entries; `EndDeleteCallback` removes from `ChangeTracker`; `Save()` adds to `ChangeTracker`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: Raw SQL in `DeleteAll`: `DELETE FROM AnimeGroup_User WHERE AnimeGroup_UserID > 0`
- **EF Core Migration Notes**: `InsertBatch`/`UpdateBatch` → EF Core bulk operations or loop with `context.Add()`/`context.Update()` + `SaveChangesAsync()`. `DeleteAll` raw SQL → `context.Database.ExecuteSqlRawAsync("DELETE FROM AnimeGroup_User WHERE AnimeGroup_UserID > 0")` or `context.Set<T>().ExecuteDelete()`. `ISessionWrapper` parameter → `ShokoDbContext`. `ReadLock()` → `AsNoTracking()`. Medium risk.
- **Risk Level**: Medium

### 34. AuthTokensRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AuthTokensRepository.cs`
- **Class**: `AuthTokensRepository`
- **Base Class**: `BaseCachedRepository<AuthTokens, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByToken`, `DeleteAllWithUserID`, `DeleteWithToken`, `GetByUserID`; `CreateNewApiKey` does token cleanup and creation
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_tokens`, `_userIDs`); `ReadLock()` on all queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `Token`, `UserID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `GetAll().ExceptBy()` and `Where()`
- **EF Core Migration Notes**: Simple. `GetByToken` deduplication logic (removes duplicate tokens) → EF Core `Where()` + `RemoveRange()`. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 35. CrossRef_AniDB_MALRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/CrossRef_AniDB_MALRepository.cs`
- **Class**: `CrossRef_AniDB_MALRepository`
- **Base Class**: `BaseCachedRepository<CrossRef_AniDB_MAL, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnimeID`, `GetByMALID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_animeIDs`, `_malIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `MALID`, `AnimeID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 36. AnimeGroupRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeGroupRepository.cs`
- **Class**: `AnimeGroupRepository`
- **Base Class**: `BaseCachedRepository<AnimeGroup, int>`
- **NHibernate Usage**: `Save()` uses `SessionFactory.OpenSession()`, `session.SaveOrUpdate()`, `session.BeginTransaction()`, `SaveWithOpenTransaction(session, group)`; `InsertBatch`/`UpdateBatch`/`DeleteAll` use `ISessionWrapper`
- **Query Patterns**: Index-based (`_parentIDs`); `GetAllTopLevelGroups` via `GetByParentID(0)`
- **Transaction Usage**: `Save()` creates explicit transactions per phase; `InsertBatch`/`UpdateBatch` create per-batch transactions; `DeleteAll` executes raw SQL
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_parentIDs`); `ReadLock()` on index queries; `ChangeTracker<int>` for change notification
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `AnimeGroupParentID` + `Changes.AddOrUpdateRange(Cache.Keys)`; `BeginDeleteCallback` deletes child `AnimeGroup_User`; `EndDeleteCallback` updates parent group stats
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: Raw SQL in `DeleteAll`: `DELETE FROM AnimeGroup WHERE AnimeGroupID <> :excludeId` / `DELETE FROM AnimeGroup WHERE AnimeGroupID > 0`
- **EF Core Migration Notes**: `Save()` has explicit session management with 2-phase save (create ID first, then update contracts) — complex EF Core translation needed. `InsertBatch`/`UpdateBatch` → EF Core bulk operations. `DeleteAll` raw SQL → `ExecuteDelete()`. `SaveWithOpenTransaction(session, group)` → EF Core `context.Update()`. Medium risk.
- **Risk Level**: Medium

### 37. AnimeEpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeEpisodeRepository.cs`
- **Class**: `AnimeEpisodeRepository`
- **Base Class**: `BaseCachedRepository<AnimeEpisode, int>`
- **NHibernate Usage**: `Lock()` uses `SessionFactory.OpenSession()`, `session.CreateSQLQuery()`, `session.BeginTransaction()`; `GetWithMultipleReleases` and `GetWithDuplicateFiles` use raw SQL
- **Query Patterns**: Index-based (`_seriesIDs`, `_anidbEpisodeIDs`); raw SQL for duplicate/multiple release detection; cache scans for `GetMissing`
- **Transaction Usage**: `Lock()` opens sessions internally; raw SQL queries open sessions
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_seriesIDs`, `_anidbEpisodeIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 2 indexes; `BeginDeleteCallback` deletes child `AnimeEpisode_User`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: **5 raw SQL queries**: `MultipleReleasesIgnoreVariationsWithAnimeQuery`, `MultipleReleasesCountVariationsWithAnimeQuery`, `MultipleReleasesIgnoreVariationsQuery`, `MultipleReleasesCountVariationsQuery`, `DuplicateFilesWithAnimeQuery`, `DuplicateFilesQuery` — all use `CreateSQLQuery().AddScalar().SetParameter()`
- **EF Core Migration Notes**: **High risk.** 6 raw SQL queries need EF Core equivalents. `Lock()` method opens explicit sessions — replace with scoped DbContext. `GetMissing()` is complex — iterates series, checks anime group statuses, release groups, and episode availability — needs careful EF Core translation with `Include()` for navigation properties. `ReadLock()` → `AsNoTracking()`.
- **Risk Level**: High

### 38. AnimeEpisode_UserRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AnimeEpisode_UserRepository.cs`
- **Class**: `AnimeEpisode_UserRepository`
- **Base Class**: `BaseCachedRepository<AnimeEpisode_User, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByUserAndEpisodeID`, `GetByUserID`, `GetMostRecentlyWatched`, `GetLastWatchedEpisodeForSeries`, `GetByEpisodeID`, `GetByUserIDAndSeriesID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 4 indexes (`_userIDs`, `_episodeIDs`, `_userEpisodeIDs`, `_userSeriesIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 4 indexes; `RegenerateDb()` saves records with `AnimeEpisode_UserID == 0` (stale data cleanup)
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderByDescending` / `Where` / `Take` / `FirstOrDefault`
- **EF Core Migration Notes**: `RegenerateDb()` saves orphan records — needs EF Core `AddRange()` + `SaveChangesAsync()`. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 39. AniDB_Episode_PreferredImageRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Episode_PreferredImageRepository.cs`
- **Class**: `AniDB_Episode_PreferredImageRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Episode_PreferredImage, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnidbEpisodeIDAndType`, `GetByAnidbEpisodeIDAndTypeAndSource`, `GetByEpisodeID`, `GetByImageSourceAndTypeAndID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_episodeIDs`, `_imageTypes`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `AnidbEpisodeID`, `(ImageSource, ImageType, ImageID)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `FirstOrDefault` in `GetByAnidbEpisodeIDAndType`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 40. AniDB_TagRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_TagRepository.cs`
- **Class**: `AniDB_TagRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Tag, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByTagID`, `GetByParentTagID`, `GetByName`, `GetBySourceName`; `GetAllForLocalSeries` joins via `AnimeSeries` + `AniDB_Anime_Tag`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 4 indexes (`_tagIDs`, `_parentTagIDs`, `_names`, `_sourceNames`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 4 indexes; `RegenerateDb()` fixes backtick characters in `TagDescription`, `TagNameOverride`, `TagNameSource`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `GetAll()` for `GetAllForLocalSeries`
- **EF Core Migration Notes**: `RegenerateDb()` updates tags with backtick replacement — needs EF Core `UpdateRange()` + `SaveChangesAsync()`. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 41. AniDB_Episode_TitleRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Episode_TitleRepository.cs`
- **Class**: `AniDB_Episode_TitleRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Episode_Title, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByEpisodeIDAndLanguage`, `GetByEpisodeID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_episodeIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `AniDB_EpisodeID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` in `GetByEpisodeIDAndLanguage`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 42. AniDB_EpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_EpisodeRepository.cs`
- **Class**: `AniDB_EpisodeRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Episode, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByEpisodeID`, `GetByAnimeID`; cache scans for `GetForDate`; LINQ joins for `GetByAnimeIDAndEpisodeNumber` / `GetByAnimeIDAndEpisodeTypeNumber`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_episodesIDs`, `_animeIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `EpisodeID`, `AnimeID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `Cache.Values` for date range query
- **EF Core Migration Notes**: Simple. `GetForDate` scans `Cache.Values` — consider database index on `AirDate`. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 43. AniDB_Anime_Character_CreatorRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Anime_Character_CreatorRepository.cs`
- **Class**: `AniDB_Anime_Character_CreatorRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime_Character_Creator, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnimeID`, `GetByCharacterID`, `GetByCharacterIDAndAnimeID`, `GetByCreatorID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_animeIDs`, `_characterIDs`, `_creatorIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 3 indexes
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `FirstOrDefault` in `GetByCharacterIDAndAnimeID`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 44. AniDB_Anime_TitleRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Anime_TitleRepository.cs`
- **Class**: `AniDB_Anime_TitleRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime_Title, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnimeID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_animeIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `AnimeID`; `RegenerateDb()` fixes backtick characters in `Title`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` in `RegenerateDb()`
- **EF Core Migration Notes**: `RegenerateDb()` updates titles with backtick replacement — needs EF Core `UpdateRange()` + `SaveChangesAsync()`. Low risk.
- **Risk Level**: Low

### 45. AniDB_CharacterRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_CharacterRepository.cs`
- **Class**: `AniDB_CharacterRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Character, int>`
- **NHibernate Usage**: `GetByName` uses `SessionFactory.OpenSession()`, `session.Query<AniDB_Character>()` (LINQ provider), `.Where()`, `.Take(1)`, `.SingleOrDefault()`
- **Query Patterns**: Index-based (`_characterIDs`); `GetCharactersForAnime` joins via `AniDB_Anime_Character`; `GetByName` uses NHibernate LINQ provider
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_characterIDs`); `ReadLock()` on index queries; `Lock()` on `GetByName`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `CharacterID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: NHibernate LINQ in `GetByName`: `session.Query<AniDB_Character>().Where(a => a.Name == creatorName).Take(1).SingleOrDefault()`
- **EF Core Migration Notes**: `session.Query<T>()` → `context.Set<T>().AsQueryable()`. `Lock()` opens explicit session — replace with scoped DbContext. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 46. AniDB_AnimeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_AnimeRepository.cs`
- **Class**: `AniDB_AnimeRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based (`_animeIDs`); cache scans for `GetForDate`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_animeIDs`, static field); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `AnimeID`; `RegenerateDb()` resets preferred titles for all anime
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `Cache.Values` for date range
- **EF Core Migration Notes**: `RegenerateDb()` resets preferred titles — needs EF Core `UpdateRange()` + `SaveChangesAsync()`. Note: `_animeIDs` is a **static** field — shared across all instances. EF Core migration must preserve this static behavior. Low risk.
- **Risk Level**: Low

### 47. AniDB_Anime_TagRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Anime_TagRepository.cs`
- **Class**: `AniDB_Anime_TagRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime_Tag, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnimeIDAndTagID`, `GetByAnimeID`, `GetByTagID`; `GetAllForLocalSeries` joins via `AnimeSeries`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_animeIDs`, `_tagIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 2 indexes
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `GetAll()` for `GetAllForLocalSeries`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 48. AniDB_Anime_PreferredImageRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Anime_PreferredImageRepository.cs`
- **Class**: `AniDB_Anime_PreferredImageRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime_PreferredImage, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnidbAnimeIDAndType`, `GetByAnidbAnimeIDAndTypeAndSource`, `GetByAnimeID`, `GetByImageSourceAndTypeAndID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_animeIDs`, `_imageTypes`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `AnidbAnimeID`, `(ImageSource, ImageType, ImageID)`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `FirstOrDefault` in `GetByAnidbAnimeIDAndType`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 49. AniDB_CreatorRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_CreatorRepository.cs`
- **Class**: `AniDB_CreatorRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Creator, int>`
- **NHibernate Usage**: `GetByName` uses `SessionFactory.OpenSession()`, `session.Query<AniDB_Creator>()` (LINQ provider), `.Where()`, `.Take(1)`, `.SingleOrDefault()`
- **Query Patterns**: Index-based (`_creatorIDs`); `GetByName` uses NHibernate LINQ provider
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_creatorIDs`); `ReadLock()` on index queries; `Lock()` on `GetByName`
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `CreatorID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: NHibernate LINQ in `GetByName`: `session.Query<AniDB_Creator>().Where(a => a.Name == creatorName).Take(1).SingleOrDefault()`
- **EF Core Migration Notes**: `session.Query<T>()` → `context.Set<T>().AsQueryable()`. `Lock()` opens explicit session — replace with scoped DbContext. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 50. AniDB_Anime_CharacterRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/AniDB/AniDB_Anime_CharacterRepository.cs`
- **Class**: `AniDB_Anime_CharacterRepository`
- **Base Class**: `BaseCachedRepository<AniDB_Anime_Character, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByAnimeID`, `GetByCharacterID`, `GetByAnimeIDAndCharacterID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_animeIDs`, `_characterIDs`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 2 indexes
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `FirstOrDefault` in `GetByAnimeIDAndCharacterID`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 51. TMDB_SeasonRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_SeasonRepository.cs`
- **Class**: `TMDB_SeasonRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Season, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByTmdbShowID`, `GetByTmdbSeasonID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_showIDs`, `_seasonIDs`); no `ReadLock()` — direct index access (unusual)
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `TmdbShowID`, `TmdbSeasonID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` in `GetByTmdbShowID`
- **EF Core Migration Notes**: Note: no `ReadLock()` on index queries — direct `PocoIndex` access. This may be intentional for performance but loses thread-safety. EF Core → `Where()` + `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 52. TMDB_ShowRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_ShowRepository.cs`
- **Class**: `TMDB_ShowRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Show, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByTmdbShowID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_showIDs`); no `ReadLock()` — direct index access
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `TmdbShowID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 53. TMDB_MovieRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_MovieRepository.cs`
- **Class**: `TMDB_MovieRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Movie, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByTmdbMovieID`, `GetByTmdbCollectionID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 2 indexes (`_movieIDs`, `_collectionIDs`); no `ReadLock()` — direct index access
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates indexes on `TmdbMovieID`, `TmdbCollectionID`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` / `ThenBy` in `GetByTmdbCollectionID`
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 54. TMDB_Image_EntityRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_Image_EntityRepository.cs`
- **Class**: `TMDB_Image_EntityRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Image_Entity, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — 6 indexes for foreign entity lookups by type, ID, remote filename; `GetByForeignID`, `GetByForeignIDAndType`, `GetByRemoteFileName`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 6 indexes (`_byImageType`, `_byEntityType`, `_byEntityTypeAndEntityID`, `_byEntityTypeAndImageTypeAndEntityID`, `_byEntityTypeAndImageTypeAndEntityIDAndRemoteFileName`, `_tmdbRemoteFileNames`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 6 indexes covering all foreign entity lookup patterns
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` / `ThenBy` in `GetByForeignID` / `GetByForeignIDAndType`
- **EF Core Migration Notes**: Simple. 6 indexes → EF Core `Where()` with composite conditions. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 55. TMDB_EpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_EpisodeRepository.cs`
- **Class**: `TMDB_EpisodeRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Episode, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based — `GetByTmdbShowID`, `GetByTmdbSeasonID`, `GetByTmdbEpisodeID`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 3 indexes (`_showIDs`, `_seasonIDs`, `_episodeIDs`); no `ReadLock()` — direct index access
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates 3 indexes; sorting applied via LINQ `OrderBy`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` / `ThenBy` in all queries
- **EF Core Migration Notes**: Simple. Low risk.
- **Risk Level**: Low

### 56. TMDB_ImageRepository.cs

- **File Path**: `Shoko.Server/Repositories/Cached/TMDB/TMDB_ImageRepository.cs`
- **Class**: `TMDB_ImageRepository`
- **Base Class**: `BaseCachedRepository<TMDB_Image, int>`
- **NHibernate Usage**: None directly — all access via base class
- **Query Patterns**: Index-based (`_tmdbRemoteFileNames`); `GetByForeignID` / `GetByForeignIDAndType` / `GetByType` delegate to `TMDB_Image_EntityRepository`
- **Transaction Usage**: None directly
- **PocoCache / ReaderWriterLockSlim**: 1 index (`_tmdbRemoteFileNames`); `ReadLock()` on index queries
- **Populate / PopulateIndexes**: `PopulateIndexes()` creates index on `RemoteFileName`
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ on `RepoFactory.TMDB_Image_Entity` for foreign entity lookups
- **EF Core Migration Notes**: Simple. Delegates to `TMDB_Image_EntityRepository` — same migration path. `ReadLock()` → `AsNoTracking()`. Low risk.
- **Risk Level**: Low

### Summary Table — Cached Repositories

| # | File | Class | Base Class | NHibernate Types | EF Core Equivalent | Risk |
|---|------|-------|------------|-----------------|-------------------|------|
| 15 | `VideoLocal_UserRepository.cs` | `VideoLocal_UserRepository` | `BaseCachedRepository<VideoLocal_User, int>` | None | No change | Low |
| 16 | `VideoLocal_PlaceRepository.cs` | `VideoLocal_PlaceRepository` | `BaseCachedRepository<VideoLocal_Place, int>` | OpenSession(), BeginTransaction() in RegenerateDb | ShokoDbContext, BeginTransaction() | Low |
| 17 | `VideoLocalRepository.cs` | `VideoLocalRepository` | `BaseCachedRepository<VideoLocal, int>` | OpenSession(), BeginTransaction() in RegenerateDb, DeleteWithOpenTransactionCallback | ShokoDbContext, cascade delete | **High** |
| 18 | `VideoLocal_HashDigestRepository.cs` | `VideoLocal_HashDigestRepository` | `BaseCachedRepository<VideoLocal_HashDigest, int>` | None | No change | Low |
| 19 | `StoredReleaseInfoRepository.cs` | `StoredReleaseInfoRepository` | `BaseCachedRepository<StoredReleaseInfo, int>` | None | No change | Low |
| 20 | `StoredRelocationPipeRepository.cs` | `StoredRelocationPipeRepository` | `BaseCachedRepository<StoredRelocationPipe, int>` | None | No change | Low |
| 21 | `StoredReleaseInfo_MatchAttemptRepository.cs` | `StoredReleaseInfo_MatchAttemptRepository` | `BaseCachedRepository<StoredReleaseInfo_MatchAttempt, int>` | None | No change | Low |
| 22 | `ShokoManagedFolderRepository.cs` | `ShokoManagedFolderRepository` | `BaseCachedRepository<ShokoManagedFolder, int>` | None | No change | Low |
| 23 | `CrossRef_AniDB_TMDB_ShowRepository.cs` | `CrossRef_AniDB_TMDB_ShowRepository` | `BaseCachedRepository<CrossRef_AniDB_TMDB_Show, int>` | None | No change | Low |
| 24 | `CrossRef_AniDB_TMDB_MovieRepository.cs` | `CrossRef_AniDB_TMDB_MovieRepository` | `BaseCachedRepository<CrossRef_AniDB_TMDB_Movie, int>` | None | No change | Low |
| 25 | `CrossRef_AniDB_TMDB_EpisodeRepository.cs` | `CrossRef_AniDB_TMDB_EpisodeRepository` | `BaseCachedRepository<CrossRef_AniDB_TMDB_Episode, int>` | None | No change | Low |
| 26 | `JMMUserRepository.cs` | `JMMUserRepository` | `BaseCachedRepository<JMMUser, int>` | None | No change | Low |
| 27 | `CustomTagRepository.cs` | `CustomTagRepository` | `BaseCachedRepository<CustomTag, int>` | DeleteWithOpenTransactionCallback | Cascade delete | Low |
| 28 | `FilterPresetRepository.cs` | `FilterPresetRepository` | `BaseCachedRepository<FilterPreset, int>` | None | Seed data, expression serialization | Medium |
| 29 | `CrossRef_CustomTagRepository.cs` | `CrossRef_CustomTagRepository` | `BaseCachedRepository<CrossRef_CustomTag, int>` | None | No change | Low |
| 30 | `CrossRef_File_EpisodeRepository.cs` | `CrossRef_File_EpisodeRepository` | `BaseCachedRepository<CrossRef_File_Episode, int>` | EndSaveCallback, EndDeleteCallback (async job triggers) | DbContext callbacks | Low |
| 31 | `AnimeSeries_UserRepository.cs` | `AnimeSeries_UserRepository` | `BaseCachedRepository<AnimeSeries_User, int>` | None | No change | Low |
| 32 | `AnimeSeriesRepository.cs` | `AnimeSeriesRepository` | `BaseCachedRepository<AnimeSeries, int>` | OpenSession(), CreateSQLQuery(), UpdateAsync() | FromSqlRaw(), DbContext | **High** |
| 33 | `AnimeGroup_UserRepository.cs` | `AnimeGroup_UserRepository` | `BaseCachedRepository<AnimeGroup_User, int>` | InsertAsync(), UpdateAsync(), CreateSQLQuery() | DbContext bulk ops, ExecuteDelete | Medium |
| 34 | `AuthTokensRepository.cs` | `AuthTokensRepository` | `BaseCachedRepository<AuthTokens, int>` | None | No change | Low |
| 35 | `CrossRef_AniDB_MALRepository.cs` | `CrossRef_AniDB_MALRepository` | `BaseCachedRepository<CrossRef_AniDB_MAL, int>` | None | No change | Low |
| 36 | `AnimeGroupRepository.cs` | `AnimeGroupRepository` | `BaseCachedRepository<AnimeGroup, int>` | OpenSession(), SaveOrUpdate(), CreateSQLQuery() | DbContext, ExecuteDelete | Medium |
| 37 | `AnimeEpisodeRepository.cs` | `AnimeEpisodeRepository` | `BaseCachedRepository<AnimeEpisode, int>` | OpenSession(), CreateSQLQuery() | FromSqlRaw(), DbContext | **High** |
| 38 | `AnimeEpisode_UserRepository.cs` | `AnimeEpisode_UserRepository` | `BaseCachedRepository<AnimeEpisode_User, int>` | None | No change | Low |
| 39 | `AniDB_Episode_PreferredImageRepository.cs` | `AniDB_Episode_PreferredImageRepository` | `BaseCachedRepository<AniDB_Episode_PreferredImage, int>` | None | No change | Low |
| 40 | `AniDB_TagRepository.cs` | `AniDB_TagRepository` | `BaseCachedRepository<AniDB_Tag, int>` | None | No change | Low |
| 41 | `AniDB_Episode_TitleRepository.cs` | `AniDB_Episode_TitleRepository` | `BaseCachedRepository<AniDB_Episode_Title, int>` | None | No change | Low |
| 42 | `AniDB_EpisodeRepository.cs` | `AniDB_EpisodeRepository` | `BaseCachedRepository<AniDB_Episode, int>` | None | No change | Low |
| 43 | `AniDB_Anime_Character_CreatorRepository.cs` | `AniDB_Anime_Character_CreatorRepository` | `BaseCachedRepository<AniDB_Anime_Character_Creator, int>` | None | No change | Low |
| 44 | `AniDB_Anime_TitleRepository.cs` | `AniDB_Anime_TitleRepository` | `BaseCachedRepository<AniDB_Anime_Title, int>` | None | No change | Low |
| 45 | `AniDB_CharacterRepository.cs` | `AniDB_CharacterRepository` | `BaseCachedRepository<AniDB_Character, int>` | Query<T>() LINQ | DbContext.Set<T>() | Low |
| 46 | `AniDB_AnimeRepository.cs` | `AniDB_AnimeRepository` | `BaseCachedRepository<AniDB_Anime, int>` | None | No change | Low |
| 47 | `AniDB_Anime_TagRepository.cs` | `AniDB_Anime_TagRepository` | `BaseCachedRepository<AniDB_Anime_Tag, int>` | None | No change | Low |
| 48 | `AniDB_Anime_PreferredImageRepository.cs` | `AniDB_Anime_PreferredImageRepository` | `BaseCachedRepository<AniDB_Anime_PreferredImage, int>` | None | No change | Low |
| 49 | `AniDB_CreatorRepository.cs` | `AniDB_CreatorRepository` | `BaseCachedRepository<AniDB_Creator, int>` | Query<T>() LINQ | DbContext.Set<T>() | Low |
| 50 | `AniDB_Anime_CharacterRepository.cs` | `AniDB_Anime_CharacterRepository` | `BaseCachedRepository<AniDB_Anime_Character, int>` | None | No change | Low |
| 51 | `TMDB_SeasonRepository.cs` | `TMDB_SeasonRepository` | `BaseCachedRepository<TMDB_Season, int>` | None | No change | Low |
| 52 | `TMDB_ShowRepository.cs` | `TMDB_ShowRepository` | `BaseCachedRepository<TMDB_Show, int>` | None | No change | Low |
| 53 | `TMDB_MovieRepository.cs` | `TMDB_MovieRepository` | `BaseCachedRepository<TMDB_Movie, int>` | None | No change | Low |
| 54 | `TMDB_Image_EntityRepository.cs` | `TMDB_Image_EntityRepository` | `BaseCachedRepository<TMDB_Image_Entity, int>` | None | No change | Low |
| 55 | `TMDB_EpisodeRepository.cs` | `TMDB_EpisodeRepository` | `BaseCachedRepository<TMDB_Episode, int>` | None | No change | Low |
| 56 | `TMDB_ImageRepository.cs` | `TMDB_ImageRepository` | `BaseCachedRepository<TMDB_Image, int>` | None | No change | Low |

### Cross-Cutting Risks — Cached Repositories

1. **High-risk repositories (3)**: `VideoLocalRepository`, `AnimeSeriesRepository`, `AnimeEpisodeRepository` — all have complex `RegenerateDb()` with explicit session management and raw SQL queries.
2. **Raw SQL queries (10 total)**: `AnimeSeriesRepository` (4), `AnimeEpisodeRepository` (6) — all use `CreateSQLQuery().AddScalar()`. Need EF Core `FromSqlRaw()` or LINQ equivalents.
3. **ISessionWrapper usage (3 repos)**: `AnimeGroup_UserRepository` (`InsertBatch`, `UpdateBatch`, `DeleteAll`), `AnimeSeriesRepository` (`UpdateBatch`), `AnimeGroupRepository` (`InsertBatch`, `UpdateBatch`, `DeleteAll`) — all accept `ISessionWrapper` parameter.
4. **Explicit session management (6 repos)**: `VideoLocal_PlaceRepository`, `VideoLocalRepository`, `AnimeSeriesRepository`, `AnimeEpisodeRepository`, `AniDB_CharacterRepository`, `AniDB_CreatorRepository` — all open `SessionFactory.OpenSession()` directly.
5. **Callback patterns (4 repos)**: `VideoLocalRepository` (delete cascade), `CustomTagRepository` (delete cascade), `CrossRef_File_EpisodeRepository` (job triggers), `AnimeSeries_UserRepository` (change tracking) — all use `BeginDeleteCallback`, `EndSaveCallback`, `EndDeleteCallback`.
6. **Static PocoIndex fields (1 repo)**: `AniDB_AnimeRepository._animeIDs` is `static` — shared across all instances. Must preserve in EF Core migration.
7. **Missing ReadLock (3 repos)**: `TMDB_SeasonRepository`, `TMDB_ShowRepository`, `TMDB_MovieRepository` — direct `PocoIndex` access without `ReadLock()`. Potential thread-safety issue.
8. **ChangeTracker<int> pattern (4 repos)**: `AnimeSeries_UserRepository`, `AnimeGroup_UserRepository`, `AnimeGroupRepository`, `AnimeSeriesRepository` — all use `ChangeTracker<int>` for per-entity change notification. Custom class, not EF Core.

---

### T007 partial — Direct Repositories

### 1. VersionsRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/VersionsRepository.cs`
- **Class**: `VersionsRepository`
- **Base Class**: `BaseDirectRepository<Versions, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<Versions>()` LINQ provider
- **Query Patterns**: `GetAllByType(string vertype)` — filters by `VersionType`, groups by `(VersionValue, VersionRevision)`, returns `Dictionary<(string, string), Versions>`
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `GroupBy` / `ToDictionary`
- **EF Core Migration Notes**: Simple. Replace `session.Query<Versions>()` with `context.Set<Versions>().AsNoTracking()`. `GroupBy` / `ToDictionary` maps 1:1 to EF Core LINQ. Low risk.
- **Risk Level**: Low

### 2. ScanFileRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/ScanFileRepository.cs`
- **Class**: `ScanFileRepository`
- **Base Class**: `BaseDirectRepository<ScanFile, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<ScanFile>()` LINQ provider
- **Query Patterns**: `GetWaiting(int scanID)` — filters by scanID + status=Waiting, ordered by CheckDate; `GetByScanID(int scanID)` — all files for a scan; `GetWithError(int scanID)` — failed/error files; `GetWaitingCount(int scanID)` — count of waiting files
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `Count`
- **EF Core Migration Notes**: Simple. Replace `session.Query<ScanFile>()` with `context.Set<ScanFile>().AsNoTracking()`. `Count` with predicate maps to `.CountAsync()`. Low risk.
- **Risk Level**: Low

### 3. PlaylistRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/PlaylistRepository.cs`
- **Class**: `PlaylistRepository`
- **Base Class**: `BaseDirectRepository<Playlist, int>`
- **NHibernate Usage**: Overrides `GetAll()` (3 overloads: parameterless, `ISession`, `ISessionWrapper`) — all add `OrderBy(a => a.PlaylistName)` sorting to base class results
- **Query Patterns**: `GetAll()` — returns all playlists sorted by name
- **Transaction Usage**: None directly
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `OrderBy` only
- **EF Core Migration Notes**: Simple. `GetAll()` override uses `base.GetAll()` which calls `context.Set<Playlist>().AsNoTracking()`. Add `.OrderBy(p => p.PlaylistName)` to the base method or override. Low risk.
- **Risk Level**: Low

### 4. ScanRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/ScanRepository.cs`
- **Class**: `ScanRepository`
- **Base Class**: `BaseDirectRepository<Scan, int>`
- **NHibernate Usage**: None directly — uses only inherited methods from `BaseDirectRepository`
- **Query Patterns**: None — no custom queries
- **Transaction Usage**: None
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: None
- **EF Core Migration Notes**: Trivial. No custom code — inherits all CRUD operations from base class. No changes needed.
- **Risk Level**: Low

### 5. ScheduledUpdateRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/ScheduledUpdateRepository.cs`
- **Class**: `ScheduledUpdateRepository`
- **Base Class**: `BaseDirectRepository<ScheduledUpdate, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<ScheduledUpdate>()` LINQ provider
- **Query Patterns**: `GetByUpdateType(int uptype)` — single row lookup by UpdateType
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<ScheduledUpdate>()` with `context.Set<ScheduledUpdate>().AsNoTracking()`. `SingleOrDefault` maps 1:1. Low risk.
- **Risk Level**: Low

### 6. FileNameHashRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/FileNameHashRepository.cs`
- **Class**: `FileNameHashRepository`
- **Base Class**: `BaseDirectRepository<FileNameHash, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<FileNameHash>()` LINQ provider
- **Query Patterns**: `GetByHash(string hash)` — lookup by ED2K hash; `GetByFileNameAndSize(string filename, long filesize)` — lookup by filename + size
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where`
- **EF Core Migration Notes**: Simple. Replace `session.Query<FileNameHash>()` with `context.Set<FileNameHash>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 7. AniDB_AnimeUpdateRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_AnimeUpdateRepository.cs`
- **Class**: `AniDB_AnimeUpdateRepository`
- **Base Class**: `BaseDirectRepository<AniDB_AnimeUpdate, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<AniDB_AnimeUpdate>()` LINQ provider; calls `Delete()` for duplicate rows
- **Query Patterns**: `GetByAnimeID(int id)` — returns most recent `AniDB_AnimeUpdate` by `UpdatedAt`, deletes older duplicates for the same AnimeID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety; calls `Delete()` on older duplicates
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderByDescending` / `FirstOrDefault`
- **EF Core Migration Notes**: Moderate. Replace `session.Query<AniDB_AnimeUpdate>()` with `context.Set<AniDB_AnimeUpdate>().AsNoTracking()`. The deduplication logic (keep newest, delete rest) needs EF Core equivalent: `context.AnimeUpdates.RemoveRange(duplicates)`. Low risk overall.
- **Risk Level**: Low

### 8. AniDB_MessageRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_MessageRepository.cs`
- **Class**: `AniDB_MessageRepository`
- **Base Class**: `BaseDirectRepository<AniDB_Message, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<AniDB_Message>()` LINQ provider
- **Query Patterns**: `GetByMessageId(int id)` — single message lookup; `GetUnhandledFileMoveMessages()` — flags-based filtering (`FileMoved && !FileMoveHandled`)
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault` / `HasFlag`
- **EF Core Migration Notes**: Simple. Replace `session.Query<AniDB_Message>()` with `context.Set<AniDB_Message>().AsNoTracking()`. `HasFlag` maps 1:1. Low risk.
- **Risk Level**: Low

### 9. AniDB_Anime_RelationRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_Anime_RelationRepository.cs`
- **Class**: `AniDB_Anime_RelationRepository`
- **Base Class**: `BaseDirectRepository<AniDB_Anime_Relation, int>`
- **NHibernate Usage**: Opens explicit sessions via `_databaseFactory.SessionFactory.OpenStatelessSession()` (5 methods) and `_databaseFactory.SessionFactory.OpenSession()` (1 method); uses `session.Query<AniDB_Anime_Relation>()` LINQ provider; accepts `ISessionWrapper` parameter in 2 overloads
- **Query Patterns**: `GetByAnimeIDAndRelationID(int animeid, int relatedanimeid)` — composite lookup; `GetByAnimeID(int id)` / `GetByAnimeID(IEnumerable<int> ids)` — single/multi lookup by anime; `GetByRelatedAnimeID(int id)` / `GetByRelatedAnimeID(IEnumerable<int> ids)` — single/multi lookup by related anime; `GetFullLinearRelationTree(int animeID)` — BFS traversal of prequel/sequel chain; `GetAllLinearRelations(ISessionWrapper, int)` — private BFS helper; `GetLinearRelationsUnsafe(ISessionWrapper, int)` — private helper querying both directions
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `FirstOrDefault` / `Contains` / `Select` / `OrderBy`; `ISessionWrapper` parameter overloads use `session.Query<T>()`; BFS traversal uses `Queue<int>` and `HashSet<int>`
- **EF Core Migration Notes**: Moderate complexity. Replace `session.Query<AniDB_Anime_Relation>()` with `context.Set<AniDB_Anime_Relation>().AsNoTracking()`. `OpenStatelessSession()` → no equivalent in EF Core; use `AsNoTracking()` instead. `ISessionWrapper` parameters need to be replaced with `DbContext` or removed (BFS logic is application-level, not DB-level). `Contains` on `int[]` maps to `.Contains()` in EF Core. BFS traversal (`GetAllLinearRelations`, `GetLinearRelationsUnsafe`) is application logic that makes multiple DB calls — consider batching with a single query using CTE or `IN` clause.
- **Risk Level**: Medium

### 10. AniDB_Anime_SimilarRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_Anime_SimilarRepository.cs`
- **Class**: `AniDB_Anime_SimilarRepository`
- **Base Class**: `BaseDirectRepository<AniDB_Anime_Similar, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenStatelessSession()`; uses `session.Query<AniDB_Anime_Similar>()` LINQ provider
- **Query Patterns**: `GetByAnimeIDAndSimilarID(int animeid, int similaranimeid)` — composite lookup; `GetByAnimeID(int id)` — all similar anime, ordered by Approval descending
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault` / `OrderByDescending`
- **EF Core Migration Notes**: Simple. Replace `session.Query<AniDB_Anime_Similar>()` with `context.Set<AniDB_Anime_Similar>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 11. AniDB_GroupStatusRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_GroupStatusRepository.cs`
- **Class**: `AniDB_GroupStatusRepository`
- **Base Class**: `BaseDirectRepository<AniDB_GroupStatus, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenStatelessSession()`; uses `session.Query<AniDB_GroupStatus>()` LINQ provider; uses NHibernate LINQ provider's `Delete()` for bulk delete; depends on `JobFactory` (DI) for `RefreshAnimeStatsJob`
- **Query Patterns**: `GetByAnimeID(int id)` — group status by anime; `DeleteForAnime(int animeid)` — bulk delete + triggers stats refresh job
- **Transaction Usage**: None directly; uses `Lock()` for thread safety; `DeleteForAnime` triggers async job
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / bulk `Delete()` via NHibernate LINQ provider
- **EF Core Migration Notes**: Moderate. Replace `session.Query<AniDB_GroupStatus>()` with `context.Set<AniDB_GroupStatus>().AsNoTracking()`. NHibernate's `Query<T>().Where(...).Delete()` (bulk delete) → EF Core `ExecuteDelete()` (EF Core 7+) or manual `RemoveRange`. The `JobFactory` dependency remains unchanged. Medium risk due to bulk delete pattern.
- **Risk Level**: Medium

### 12. AniDB_NotifyQueueRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_NotifyQueueRepository.cs`
- **Class**: `AniDB_NotifyQueueRepository`
- **Base Class**: `BaseDirectRepository<AniDB_NotifyQueue, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenStatelessSession()`; uses `session.Query<AniDB_NotifyQueue>()` LINQ provider; uses NHibernate LINQ provider's `Delete()` for bulk delete
- **Query Patterns**: `GetByTypeID(AniDBNotifyType type, int id)` — single lookup; `GetByType(AniDBNotifyType type)` — all entries for a type; `DeleteForTypeID(AniDBNotifyType type, int id)` — bulk delete by type + ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault` / bulk `Delete()`
- **EF Core Migration Notes**: Moderate. Replace `session.Query<AniDB_NotifyQueue>()` with `context.Set<AniDB_NotifyQueue>().AsNoTracking()`. NHibernate bulk `Delete()` → EF Core `ExecuteDelete()`. Low-medium risk.
- **Risk Level**: Medium

### 13. AniDB_Anime_StaffRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/AniDB_Anime_StaffRepository.cs`
- **Class**: `AniDB_Anime_StaffRepository`
- **Base Class**: `BaseDirectRepository<AniDB_Anime_Staff, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenStatelessSession()`; uses `session.Query<AniDB_Anime_Staff>()` LINQ provider
- **Query Patterns**: `GetByAnimeID(int animeID)` — staff by anime; `GetByCreatorID(int creatorID)` — staff by creator
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where`
- **EF Core Migration Notes**: Simple. Replace `session.Query<AniDB_Anime_Staff>()` with `context.Set<AniDB_Anime_Staff>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 14. TMDB_PersonRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_PersonRepository.cs`
- **Class**: `TMDB_PersonRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Person, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Person>()` LINQ provider
- **Query Patterns**: `GetByTmdbPersonID(int creditId)` — single lookup by TMDB person ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Person>()` with `context.Set<TMDB_Person>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 15. TMDB_Movie_CastRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_Movie_CastRepository.cs`
- **Class**: `TMDB_Movie_CastRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Movie_Cast, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Movie_Cast>()` LINQ provider
- **Query Patterns**: `GetByTmdbPersonID(int personId)` — cast entries for a person, ordered by movie ID + ordering; `GetByTmdbMovieID(int movieId)` — cast for a movie, ordered by ordering
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Movie_Cast>()` with `context.Set<TMDB_Movie_Cast>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 16. TMDB_Movie_CrewRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_Movie_CrewRepository.cs`
- **Class**: `TMDB_Movie_CrewRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Movie_Crew, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Movie_Crew>()` LINQ provider
- **Query Patterns**: `GetByTmdbPersonID(int personId)` — crew entries for a person, ordered by movie ID + department + job + credit ID; `GetByTmdbMovieID(int movieId)` — crew for a movie, ordered by department + job + credit ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` (multi-level)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Movie_Crew>()` with `context.Set<TMDB_Movie_Crew>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 17. TMDB_CompanyRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_CompanyRepository.cs`
- **Class**: `TMDB_CompanyRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Company, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Company>()` LINQ provider
- **Query Patterns**: `GetByTmdbCompanyID(int companyId)` — single lookup by TMDB company ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Company>()` with `context.Set<TMDB_Company>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 18. TMDB_Company_EntityRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_Company_EntityRepository.cs`
- **Class**: `TMDB_Company_EntityRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Company_Entity, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Company_Entity>()` LINQ provider
- **Query Patterns**: `GetByTmdbCompanyID(int companyId)` — company-entity links ordered by release date; `GetByTmdbEntityTypeAndCompanyID(ForeignEntityType, int)` — filtered by entity type + company; `GetByTmdbEntityTypeAndID(ForeignEntityType, int)` — filtered by entity type + entity ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` / null-coalescing (`ReleasedAt ?? DateOnly.MaxValue`)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Company_Entity>()` with `context.Set<TMDB_Company_Entity>().AsNoTracking()`. Null-coalescing in `OrderBy` maps 1:1 in EF Core. Low risk.
- **Risk Level**: Low

### 19. TMDB_Episode_CrewRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_Episode_CrewRepository.cs`
- **Class**: `TMDB_Episode_CrewRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Episode_Crew, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Episode_Crew>()` LINQ provider
- **Query Patterns**: `GetByTmdbPersonID(int personId)` — ordered by show ID + department + job + credit ID; `GetByTmdbShowID(int showId)` — ordered by department + job + credit ID; `GetByTmdbSeasonID(int seasonId)` — same ordering; `GetByTmdbEpisodeID(int episodeId)` — same ordering
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` (multi-level)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Episode_Crew>()` with `context.Set<TMDB_Episode_Crew>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 20. TMDB_Episode_CastRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/TMDB_Episode_CastRepository.cs`
- **Class**: `TMDB_Episode_CastRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Episode_Cast, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Episode_Cast>()` LINQ provider
- **Query Patterns**: `GetByTmdbPersonID(int personId)` — ordered by show ID + episode ID + ordering; `GetByTmdbShowID(int showId)` — ordered by episode ID + ordering; `GetByTmdbSeasonID(int seasonId)` — same ordering; `GetByTmdbEpisodeID(int episodeId)` — ordered by ordering
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` (multi-level)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Episode_Cast>()` with `context.Set<TMDB_Episode_Cast>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 21. TMDB_TitleRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Text/TMDB_TitleRepository.cs`
- **Class**: `TMDB_TitleRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Title, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Title>()` LINQ provider
- **Query Patterns**: `GetByParentTypeAndID(ForeignEntityType parentType, int parentId)` — titles by parent entity type + ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Title>()` with `context.Set<TMDB_Title>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 22. TMDB_OverviewRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Text/TMDB_OverviewRepository.cs`
- **Class**: `TMDB_OverviewRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Overview, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Overview>()` LINQ provider
- **Query Patterns**: `GetByParentTypeAndID(ForeignEntityType parentType, int parentId)` — overviews by parent entity type + ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Overview>()` with `context.Set<TMDB_Overview>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 23. TMDB_NetworkRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_NetworkRepository.cs`
- **Class**: `TMDB_NetworkRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Network, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Network>()` LINQ provider
- **Query Patterns**: `GetByTmdbNetworkID(int tmdbNetworkId)` — single lookup by TMDB network ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Network>()` with `context.Set<TMDB_Network>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 24. TMDB_Collection_MovieRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_Collection_MovieRepository.cs`
- **Class**: `TMDB_Collection_MovieRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Collection_Movie, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Collection_Movie>()` LINQ provider
- **Query Patterns**: `GetByTmdbCollectionID(int collectionId)` — movies in a collection; `GetByTmdbMovieID(int movieId)` — collection membership for a movie
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Collection_Movie>()` with `context.Set<TMDB_Collection_Movie>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 25. TMDB_Show_NetworkRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_Show_NetworkRepository.cs`
- **Class**: `TMDB_Show_NetworkRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Show_Network, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Show_Network>()` LINQ provider
- **Query Patterns**: `GetByTmdbNetworkID(int networkId)` — shows for a network, ordered by show ID; `GetByTmdbShowID(int showId)` — networks for a show, ordered by ordering
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Show_Network>()` with `context.Set<TMDB_Show_Network>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 26. TMDB_CollectionRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_CollectionRepository.cs`
- **Class**: `TMDB_CollectionRepository`
- **Base Class**: `BaseDirectRepository<TMDB_Collection, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_Collection>()` LINQ provider
- **Query Patterns**: `GetByTmdbCollectionID(int collectionId)` — single lookup by TMDB collection ID
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_Collection>()` with `context.Set<TMDB_Collection>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 27. TMDB_AlternateOrderingRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_AlternateOrderingRepository.cs`
- **Class**: `TMDB_AlternateOrderingRepository`
- **Base Class**: `BaseDirectRepository<TMDB_AlternateOrdering, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_AlternateOrdering>()` LINQ provider
- **Query Patterns**: `GetByTmdbShowID(int showId)` — all alternate orderings for a show; `GetByTmdbEpisodeGroupCollectionID(string episodeGroupCollectionId)` — single lookup by collection ID; `GetByEpisodeGroupCollectionAndShowIDs(string collectionId, int showId)` — composite lookup
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `Take(1)` / `SingleOrDefault`
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_AlternateOrdering>()` with `context.Set<TMDB_AlternateOrdering>().AsNoTracking()`. Low risk.
- **Risk Level**: Low

### 28. TMDB_AlternateOrdering_EpisodeRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_AlternateOrdering_EpisodeRepository.cs`
- **Class**: `TMDB_AlternateOrdering_EpisodeRepository`
- **Base Class**: `BaseDirectRepository<TMDB_AlternateOrdering_Episode, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_AlternateOrdering_Episode>()` LINQ provider
- **Query Patterns**: `GetByTmdbShowID(int showId)` — ordered by collection ID + season number (unspecified first) + episode number; `GetByTmdbEpisodeGroupCollectionID(string collectionId)` — same ordering; `GetByTmdbEpisodeGroupID(string groupId)` — ordered by episode number; `GetByTmdbEpisodeID(int episodeId)` — ordered by group ID; `GetByEpisodeGroupCollectionAndEpisodeIDs(string collectionId, int episodeId)` — composite lookup
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` (multi-level); conditional ordering (`SeasonNumber == 0`)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_AlternateOrdering_Episode>()` with `context.Set<TMDB_AlternateOrdering_Episode>().AsNoTracking()`. Conditional `OrderBy` (`SeasonNumber == 0`) maps 1:1 in EF Core (boolean to int sort). Low risk.
- **Risk Level**: Low

### 29. TMDB_AlternateOrdering_SeasonRepository.cs

- **File Path**: `Shoko.Server/Repositories/Direct/TMDB/Optional/TMDB_AlternateOrdering_SeasonRepository.cs`
- **Class**: `TMDB_AlternateOrdering_SeasonRepository`
- **Base Class**: `BaseDirectRepository<TMDB_AlternateOrdering_Season, int>`
- **NHibernate Usage**: Opens explicit session via `_databaseFactory.SessionFactory.OpenSession()`; uses `session.Query<TMDB_AlternateOrdering_Season>()` LINQ provider
- **Query Patterns**: `GetByTmdbShowID(int showId)` — ordered by collection ID + season number (unspecified first) + season number; `GetByTmdbEpisodeGroupCollectionID(string collectionId)` — same ordering; `GetByTmdbEpisodeGroupID(string groupId)` — single lookup
- **Transaction Usage**: None directly; uses `Lock()` for thread safety
- **Raw SQL/HQL/Criteria/QueryOver/LINQ**: LINQ `Where` / `OrderBy` / `ThenBy` (multi-level); conditional ordering (`SeasonNumber == 0`)
- **EF Core Migration Notes**: Simple. Replace `session.Query<TMDB_AlternateOrdering_Season>()` with `context.Set<TMDB_AlternateOrdering_Season>().AsNoTracking()`. Conditional `OrderBy` maps 1:1. Low risk.
- **Risk Level**: Low

### Summary Table — Direct Repositories

| # | File | Class | Base Class | NHibernate Types | EF Core Equivalent | Risk |
|---|------|-------|------------|-----------------|-------------------|------|
| 1 | `VersionsRepository.cs` | `VersionsRepository` | `BaseDirectRepository<Versions, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 2 | `ScanFileRepository.cs` | `ScanFileRepository` | `BaseDirectRepository<ScanFile, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 3 | `PlaylistRepository.cs` | `PlaylistRepository` | `BaseDirectRepository<Playlist, int>` | GetAll() overrides with ISession, ISessionWrapper | DbContext.Set<T>().OrderBy() | Low |
| 4 | `ScanRepository.cs` | `ScanRepository` | `BaseDirectRepository<Scan, int>` | None | No change | Low |
| 5 | `ScheduledUpdateRepository.cs` | `ScheduledUpdateRepository` | `BaseDirectRepository<ScheduledUpdate, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 6 | `FileNameHashRepository.cs` | `FileNameHashRepository` | `BaseDirectRepository<FileNameHash, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 7 | `AniDB_AnimeUpdateRepository.cs` | `AniDB_AnimeUpdateRepository` | `BaseDirectRepository<AniDB_AnimeUpdate, int>` | OpenSession(), Query<T>() LINQ, Delete() | DbContext.Set<T>(), RemoveRange() | Low |
| 8 | `AniDB_MessageRepository.cs` | `AniDB_MessageRepository` | `BaseDirectRepository<AniDB_Message, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 9 | `AniDB_Anime_RelationRepository.cs` | `AniDB_Anime_RelationRepository` | `BaseDirectRepository<AniDB_Anime_Relation, int>` | OpenStatelessSession(), ISessionWrapper param, Query<T>() LINQ | DbContext.Set<T>(), BFS app-level logic | **Medium** |
| 10 | `AniDB_Anime_SimilarRepository.cs` | `AniDB_Anime_SimilarRepository` | `BaseDirectRepository<AniDB_Anime_Similar, int>` | OpenStatelessSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 11 | `AniDB_GroupStatusRepository.cs` | `AniDB_GroupStatusRepository` | `BaseDirectRepository<AniDB_GroupStatus, int>` | OpenStatelessSession(), Query<T>() LINQ bulk Delete(), JobFactory | DbContext.Set<T>(), ExecuteDelete() | **Medium** |
| 12 | `AniDB_NotifyQueueRepository.cs` | `AniDB_NotifyQueueRepository` | `BaseDirectRepository<AniDB_NotifyQueue, int>` | OpenStatelessSession(), Query<T>() LINQ bulk Delete() | DbContext.Set<T>(), ExecuteDelete() | **Medium** |
| 13 | `AniDB_Anime_StaffRepository.cs` | `AniDB_Anime_StaffRepository` | `BaseDirectRepository<AniDB_Anime_Staff, int>` | OpenStatelessSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 14 | `TMDB_PersonRepository.cs` | `TMDB_PersonRepository` | `BaseDirectRepository<TMDB_Person, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 15 | `TMDB_Movie_CastRepository.cs` | `TMDB_Movie_CastRepository` | `BaseDirectRepository<TMDB_Movie_Cast, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 16 | `TMDB_Movie_CrewRepository.cs` | `TMDB_Movie_CrewRepository` | `BaseDirectRepository<TMDB_Movie_Crew, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 17 | `TMDB_CompanyRepository.cs` | `TMDB_CompanyRepository` | `BaseDirectRepository<TMDB_Company, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 18 | `TMDB_Company_EntityRepository.cs` | `TMDB_Company_EntityRepository` | `BaseDirectRepository<TMDB_Company_Entity, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 19 | `TMDB_Episode_CrewRepository.cs` | `TMDB_Episode_CrewRepository` | `BaseDirectRepository<TMDB_Episode_Crew, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 20 | `TMDB_Episode_CastRepository.cs` | `TMDB_Episode_CastRepository` | `BaseDirectRepository<TMDB_Episode_Cast, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 21 | `TMDB_TitleRepository.cs` | `TMDB_TitleRepository` | `BaseDirectRepository<TMDB_Title, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 22 | `TMDB_OverviewRepository.cs` | `TMDB_OverviewRepository` | `BaseDirectRepository<TMDB_Overview, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 23 | `TMDB_NetworkRepository.cs` | `TMDB_NetworkRepository` | `BaseDirectRepository<TMDB_Network, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 24 | `TMDB_Collection_MovieRepository.cs` | `TMDB_Collection_MovieRepository` | `BaseDirectRepository<TMDB_Collection_Movie, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 25 | `TMDB_Show_NetworkRepository.cs` | `TMDB_Show_NetworkRepository` | `BaseDirectRepository<TMDB_Show_Network, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 26 | `TMDB_CollectionRepository.cs` | `TMDB_CollectionRepository` | `BaseDirectRepository<TMDB_Collection, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 27 | `TMDB_AlternateOrderingRepository.cs` | `TMDB_AlternateOrderingRepository` | `BaseDirectRepository<TMDB_AlternateOrdering, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 28 | `TMDB_AlternateOrdering_EpisodeRepository.cs` | `TMDB_AlternateOrdering_EpisodeRepository` | `BaseDirectRepository<TMDB_AlternateOrdering_Episode, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |
| 29 | `TMDB_AlternateOrdering_SeasonRepository.cs` | `TMDB_AlternateOrdering_SeasonRepository` | `BaseDirectRepository<TMDB_AlternateOrdering_Season, int>` | OpenSession(), Query<T>() LINQ | DbContext.Set<T>() | Low |

### Cross-Cutting Risks — Direct Repositories

1. **Medium-risk repositories (3)**: `AniDB_Anime_RelationRepository` (BFS traversal with ISessionWrapper params and OpenStatelessSession), `AniDB_GroupStatusRepository` (bulk delete via NHibernate LINQ + JobFactory dependency), `AniDB_NotifyQueueRepository` (bulk delete via NHibernate LINQ).
2. **Bulk delete pattern (2 repos)**: `AniDB_GroupStatusRepository`, `AniDB_NotifyQueueRepository` — use NHibernate LINQ provider's `Query<T>().Where(...).Delete()` (server-side bulk delete). EF Core equivalent: `ExecuteDelete()` (EF Core 7+).
3. **OpenStatelessSession usage (5 repos)**: `AniDB_Anime_RelationRepository`, `AniDB_Anime_SimilarRepository`, `AniDB_GroupStatusRepository`, `AniDB_NotifyQueueRepository`, `AniDB_Anime_StaffRepository` — use `OpenStatelessSession()` for read-only queries. EF Core equivalent: `AsNoTracking()`.
4. **ISessionWrapper parameters (1 repo)**: `AniDB_Anime_RelationRepository` — 2 methods accept `ISessionWrapper session` for BFS traversal. These are application-level graph traversal methods that make multiple DB calls; EF Core migration should replace with `DbContext` or remove the abstraction and use direct LINQ.
5. **JobFactory dependency (1 repo)**: `AniDB_GroupStatusRepository` — constructor takes `JobFactory` for triggering `RefreshAnimeStatsJob` after bulk delete. This is a DI dependency that remains unchanged.
6. **No PocoCache / ReaderWriterLockSlim**: None of the 29 Direct repositories use PocoCache or indexes — they are read-through (no cache). All use `Lock()` for thread safety on session creation.
7. **No raw SQL**: Zero raw SQL queries across all 29 Direct repositories — all use LINQ. This is the cleanest migration group.

---

## T008: Raw SQL Query Inventory

**Purpose**: Identify all raw SQL queries across repositories, services, tasks, and database infrastructure for EF Core migration planning.

### Category 1: NHibernate `CreateSQLQuery` in Repository Files

| # | File | Method | SQL Type | Query Text | EF Core Equivalent |
|---|------|--------|----------|------------|-------------------|
| 1 | `Cached/AnimeSeriesRepository.cs:332` | `GetWithMultipleReleases` | SELECT | `SELECT DISTINCT ani.AnimeID FROM VideoLocal AS vl JOIN CrossRef_File_Episode ani ON vl.Hash = ani.Hash WHERE vl.IsVariation = 0 AND vl.Hash != '' GROUP BY ani.AnimeID, ani.EpisodeID HAVING COUNT(ani.EpisodeID) > 1` | LINQ with `Join`, `GroupBy`, `Having`, `Count` |
| 2 | `Cached/AnimeSeriesRepository.cs:381` | `GetWithDuplicateFiles` | SELECT | Complex subquery joining `VideoLocal` → `VideoLocal_Place` → `CrossRef_File_Episode` | LINQ with nested `Where` + `Join` |
| 3 | `Cached/AnimeSeriesRepository.cs:403` | `GetWithMissingEpisodes` | SELECT | `SELECT ser.AniDB_ID FROM AnimeSeries AS ser WHERE ser.MissingEpisodeCount > 0` | Simple LINQ `Where` on `AnimeSeries` |
| 4 | `Cached/AnimeEpisodeRepository.cs:105` | `GetWithMultipleReleases` | SELECT | `SELECT ani.EpisodeID FROM VideoLocal AS vl JOIN CrossRef_File_Episode ani ON vl.Hash = ani.Hash WHERE ani.AnimeID = :animeID GROUP BY ani.EpisodeID HAVING COUNT > 1` | LINQ with parameterized `Where` + `GroupBy` |
| 5 | `Cached/AnimeEpisodeRepository.cs:112` | `GetWithMultipleReleases` | SELECT | Same as #4 but without animeID filter | LINQ with optional parameter |
| 6 | `Cached/AnimeEpisodeRepository.cs:199` | `GetWithDuplicateFiles` | SELECT | Complex subquery with animeID filter | LINQ with nested `Where` + `Join` |
| 7 | `Cached/AnimeEpisodeRepository.cs:205` | `GetWithDuplicateFiles` | SELECT | Same as #6 without animeID filter | LINQ with nested `Where` + `Join` |
| 8 | `Cached/AnimeGroupRepository.cs:146` | `DeleteAll` | DELETE | `DELETE FROM AnimeGroup WHERE AnimeGroupID <> :excludeId` | `context.AnimeGroup.Where(x => x.AnimeGroupID != excludeId).ExecuteDelete()` |
| 9 | `Cached/AnimeGroupRepository.cs:152` | `DeleteAll` | DELETE | `DELETE FROM AnimeGroup WHERE AnimeGroupID > 0` | `context.AnimeGroup.ExecuteDelete()` |
| 10 | `Cached/AnimeGroup_UserRepository.cs:136` | `DeleteAll` | DELETE | `DELETE FROM AnimeGroup_User WHERE AnimeGroup_UserID > 0` | `context.AnimeGroup_User.ExecuteDelete()` |

**Subtotal**: 10 raw SQL queries in 4 repository files.

### Category 2: NHibernate `CreateSQLQuery` in Services/Tasks

| # | File | Method | SQL Type | Query Text | EF Core Equivalent |
|---|------|--------|----------|------------|-------------------|
| 11 | `Tasks/AnimeGroupCreator.cs:110` | `ClearGroupsAndDependencies` | UPDATE | `UPDATE AnimeSeries SET AnimeGroupID = :tempGroupId` | `context.AnimeSeries.ExecuteUpdateAsync(s => s.SetProperty(x => x.AnimeGroupID, tempGroupId))` |
| 12 | `Tasks/AutoAnimeGroupCalculator.cs:104` | `Create` | SELECT | `SELECT fromAnime.AnimeID, toAnime.AnimeID, ... FROM AniDB_Anime_Relation rel INNER JOIN AniDB_Anime fromAnime ... INNER JOIN AniDB_Anime toAnime ...` | LINQ with 2 `InnerJoin` on `AniDB_Anime` |

**Subtotal**: 2 raw SQL queries in 2 task files.

### Category 3: NHibernate `CreateSQLQuery` in `DatabaseFixes.cs` (Migration Scripts)

| # | File | Method | SQL Type | Query Text | Notes |
|---|------|--------|----------|------------|-------|
| 13 | `DatabaseFixes.cs:69` | `MigrateGroupFilterToFilterPreset` | SELECT | `SELECT GroupFilterID, ParentGroupFilterID, ... FROM GroupFilter` | Legacy migration — reads from deprecated table |
| 14 | `DatabaseFixes.cs:141` | `MigrateGroupFilterToFilterPreset` | DROP | `DROP TABLE GroupFilter; DROP TABLE GroupFilterCondition` | Legacy migration cleanup |
| 15 | `DatabaseFixes.cs:918` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT VideoLocalID, Hash, MD5, SHA1, CRC32 FROM VideoLocal` | Data migration — reads from VideoLocal |
| 16 | `DatabaseFixes.cs:925` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT FileID, Hash, GroupID, ... FROM AniDB_File` | Data migration — reads from deprecated table |
| 17 | `DatabaseFixes.cs:941` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT Hash, FileSize, HasResponse, UpdatedAt FROM AniDB_FileUpdate` | Data migration — reads from deprecated table |
| 18 | `DatabaseFixes.cs:947` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT GroupID, GroupName, GroupNameShort FROM AniDB_ReleaseGroup` | Data migration — reads from deprecated table |
| 19 | `DatabaseFixes.cs:952` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT CrossRef_File_EpisodeID, CrossRefSource FROM CrossRef_File_Episode` | Data migration |
| 20 | `DatabaseFixes.cs:956` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT DISTINCT FileID, LanguageName FROM CrossRef_Languages_AniDB_File` | Data migration |
| 21 | `DatabaseFixes.cs:960` | `MoveAnidbFileDataToReleaseInfoFormat` | SELECT | `SELECT DISTINCT FileID, LanguageName FROM CrossRef_Subtitles_AniDB_File` | Data migration |
| 22 | `DatabaseFixes.cs:1240` | `MoveAnidbFileDataToReleaseInfoCleanup` | DROP × 5 | `DROP TABLE AniDB_File; AniDB_FileUpdate; AniDB_ReleaseGroup; CrossRef_Languages_AniDB_File; CrossRef_Subtitles_AniDB_File` | Schema cleanup |
| 23 | `DatabaseFixes.cs:1241` | `MoveAnidbFileDataToReleaseInfoCleanup` | ALTER | `ALTER TABLE CrossRef_File_Episode DROP COLUMN CrossRefSource` | Schema migration |
| 24 | `DatabaseFixes.cs:1242` | `MoveAnidbFileDataToReleaseInfoCleanup` | ALTER | `ALTER TABLE VideoLocal DROP COLUMN MD5` | Schema migration |
| 25 | `DatabaseFixes.cs:1243` | `MoveAnidbFileDataToReleaseInfoCleanup` | ALTER | `ALTER TABLE VideoLocal DROP COLUMN SHA1` | Schema migration |
| 26 | `DatabaseFixes.cs:1244` | `MoveAnidbFileDataToReleaseInfoCleanup` | ALTER | `ALTER TABLE VideoLocal DROP COLUMN CRC32` | Schema migration |
| 27 | `DatabaseFixes.cs:1272` | `MigrateRenamers` | SELECT | `SELECT ScriptName, RenamerType, IsEnabledOnImport, Script FROM RenameScript` | Data migration — reads from deprecated table |
| 28 | `DatabaseFixes.cs:1348` | `MigrateRenamers` | SELECT | `SELECT Name, Type, Settings FROM RenamerInstance` | Data migration — reads from deprecated table |
| 29 | `DatabaseFixes.cs:1436` | `MigrateRenamers` | INSERT | `INSERT INTO StoredRelocationPipe (ProviderID, Name, Configuration) VALUES (:ProviderID, :Name, :Configuration)` | Data migration |
| 30 | `DatabaseFixes.cs:1443` | `MigrateRenamers` | DROP | `DROP TABLE IF EXISTS RenameScript; DROP TABLE IF EXISTS RenamerInstance` | Schema cleanup |
| 31 | `DatabaseFixes.cs:1477` | `MigrateAnidbVotes` | SELECT | `SELECT EntityID, VoteValue, VoteType FROM AniDB_Vote` | Data migration — reads from deprecated table |
| 32 | `DatabaseFixes.cs:1553` | `MigrateAnidbVotes` | DROP | `DROP TABLE IF EXISTS AniDB_Vote` | Schema cleanup |

**Subtotal**: 20 raw SQL queries in `DatabaseFixes.cs` (8 SELECT, 8 DROP, 3 ALTER, 1 INSERT).

### Category 4: NHibernate `CreateSQLQuery` in Database Provider Files

| # | File | Method | SQL Type | Query Text | Notes |
|---|------|--------|----------|------------|-------|
| 33 | `SQLServer.cs:950` | `AlterImdbMovieIDType` | ALTER | `ALTER TABLE TMDB_Movie ADD ImdbMovieID NVARCHAR(12) NULL DEFAULT NULL` | SQL Server-specific schema migration |
| 34 | `SQLServer.cs:1007` | `DropColumnWithDefaultConstraint` | SELECT | `SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID(...) AND PARENT_COLUMN_ID = ...` | SQL Server-specific constraint lookup |
| 35 | `SQLServer.cs:1016` | `DropColumnWithDefaultConstraint` | ALTER | `ALTER TABLE {table} DROP CONSTRAINT {name}` | SQL Server-specific constraint drop |
| 36 | `SQLServer.cs:1020` | `DropColumnWithDefaultConstraint` | ALTER | `ALTER TABLE {table} DROP COLUMN {column}` | SQL Server-specific column drop |
| 37 | `SQLServer.cs:1029` | `DropDefaultConstraint` | SELECT | Same as #34 (constraint lookup) | SQL Server-specific |
| 38 | `SQLServer.cs:1036` | `DropDefaultConstraint` | ALTER | `ALTER TABLE {table} DROP CONSTRAINT {name}` | SQL Server-specific |

**Subtotal**: 6 raw SQL queries in `SQLServer.cs` (1 ALTER ADD, 2 SELECT constraint, 2 DROP CONSTRAINT, 1 DROP COLUMN).

### Category 5: Raw ADO.NET (Non-NHibernate)

These use `SqlCommand`, `MySqlCommand`, `SqliteCommand` directly — NOT NHibernate. They operate at the connection level for schema initialization and Quartz setup.

| # | File | Method | Command Type | Purpose |
|---|------|--------|-------------|---------|
| 39 | `SQLite.cs:70` | `CreateSchema` | `ExecuteNonQuery` | Create `Versions` table |
| 40 | `SQLite.cs:156` | `GetVersion` | `ExecuteScalar` | Count rows in `Versions` |
| 41 | `SQLite.cs:176` | `IsNewDatabase` | `ExecuteScalar` | Check if `Versions` table exists |
| 42 | `SQLite.cs:1488` | `GetSchema` | `ExecuteReader` | `PRAGMA table_info({tableName})` |
| 43 | `MySQL.cs:54` | `CreateSchema` | `ExecuteScalar` | Create `Versions` table |
| 44 | `MySQL.cs:82` | `GetVersion` | `ExecuteNonQuery` | Insert version row |
| 45 | `MySQL.cs:153` | `GetVersion` | `ExecuteScalar` | Select version |
| 46 | `MySQL.cs:196` | `IsNewDatabase` | `ExecuteReader` | Check `Versions` table existence |
| 47 | `SQLServer.cs:52` | `CreateSchema` | `ExecuteNonQuery` | Create `Versions` table |
| 48 | `SQLServer.cs:104` | `GetVersion` | `ExecuteNonQuery` | Insert version row |
| 49 | `SQLServer.cs:165` | `GetVersion` | `ExecuteScalar` | Select version |
| 50 | `SQLServer.cs:181` | `IsNewDatabase` | `ExecuteScalar` | Check `Versions` table existence |
| 51 | `SQLServer.cs:198` | `IsNewDatabase` | `ExecuteScalar` | `SELECT count(*) FROM sysobjects WHERE name = 'Versions'` |
| 52 | `SQLServer.cs:204` | `IsNewDatabase` | `ExecuteScalar` | Check column existence via `INFORMATION_SCHEMA` |
| 53 | `QuartzStartup.cs:179` | `EnsureQuartzDatabaseExists_SQLServer` | `ExecuteScalar` | Check if `QRTZ_TRIGGERS` table exists |
| 54 | `QuartzStartup.cs:578` | `EnsureQuartzDatabaseExists_SQLServer` | `ExecuteNonQuery` | Execute Quartz SQL script |
| 55 | `QuartzStartup.cs:598` | `EnsureQuartzDatabaseExists_MySQL` | `ExecuteScalar` | Check if `QRTZ_TRIGGERS` table exists |
| 56 | `QuartzStartup.cs:778` | `EnsureQuartzDatabaseExists_MySQL` | `ExecuteNonQuery` | Execute Quartz SQL script |
| 57 | `QuartzStartup.cs:789` | `EnsureQuartzDatabaseExists_SQLite` | `ExecuteScalar` | Check if `QRTZ_TRIGGERS` table exists |
| 58 | `QuartzStartup.cs:989` | `EnsureQuartzDatabaseExists_SQLite` | `ExecuteNonQuery` | Execute Quartz SQL script |
| 59 | `SqliteDriverFix.cs:30` | `Fix` | `ExecuteNonQuery` | SQLite driver fix |

**Subtotal**: 21 raw ADO.NET commands across 6 files.

### Summary

| Category | Files | Queries | SELECT | INSERT | UPDATE | DELETE/DROP/ALTER |
|----------|-------|---------|--------|--------|--------|-------------------|
| Repository files | 4 | 10 | 7 | 0 | 0 | 3 (DELETE) |
| Services/Tasks | 2 | 2 | 1 | 0 | 1 | 0 |
| DatabaseFixes.cs | 1 | 20 | 8 | 1 | 0 | 11 (8 DROP, 3 ALTER) |
| SQLServer.cs | 1 | 6 | 2 | 0 | 0 | 4 (1 ALTER ADD, 2 DROP CONSTRAINT, 1 DROP COLUMN) |
| Raw ADO.NET | 6 | 21 | 5 | 0 | 0 | 16 (schema init scripts) |
| **Total** | **14** | **59** | **23** | **1** | **1** | **34** |

### EF Core Migration Notes

**Repository queries (10 total)**:
- 7 SELECT queries → Convert to LINQ with `Join`, `GroupBy`, `Having`, `Where`. These are the most complex to translate due to `HAVING COUNT > 1` patterns.
- 3 DELETE queries → `context.Set<T>().ExecuteDelete()` (EF Core 7+). Straightforward.

**Service queries (2 total)**:
- 1 UPDATE → `context.Set<T>().ExecuteUpdateAsync()` (EF Core 7+). Straightforward.
- 1 SELECT → LINQ with 2 `InnerJoin`. Straightforward.

**DatabaseFixes queries (20 total)**:
- These are **one-time migration scripts** that run during server startup. They read from deprecated tables (`AniDB_File`, `RenameScript`, `RenamerInstance`, `AniDB_Vote`, `GroupFilter`) and migrate data to new tables.
- SELECT queries → LINQ via `ShokoDbContext` (once EF Core is in place).
- DROP/ALTER queries → `context.Database.ExecuteSqlRaw()` (EF Core) — these are provider-specific schema changes that EF Core Migrations handles differently. Consider keeping as raw SQL for migration scripts.

**SQLServer.cs queries (6 total)**:
- These are **SQL Server-specific schema helpers** for dropping columns with default constraints. SQL Server requires dropping the constraint before the column.
- Keep as `context.Database.ExecuteSqlRaw()` during migration scripts — EF Core Migrations handles this automatically but one-off scripts benefit from raw SQL.

**Raw ADO.NET (21 total)**:
- These operate at the **connection level** for schema initialization (creating `Versions` table, checking table existence, PRAGMA queries, Quartz setup).
- Replace with EF Core `context.Database.GetDbConnection()` + `DbCommand` for consistency, OR keep as raw ADO.NET since they're provider-specific initialization code.
- Quartz setup scripts (6 commands) — keep as raw ADO.NET; EF Core has no equivalent for Quartz schema initialization.

### Risk Assessment

| Risk | Count | Files | Notes |
|------|-------|-------|-------|
| **High** | 3 | `AnimeSeriesRepository.cs`, `AnimeEpisodeRepository.cs`, `DatabaseFixes.cs` | Complex HAVING/GROUP BY queries; 20 migration queries with deprecated tables |
| **Medium** | 2 | `AnimeGroupRepository.cs`, `AnimeGroup_UserRepository.cs` | Bulk DELETE via raw SQL → `ExecuteDelete()` |
| **Low** | 9 | `AutoAnimeGroupCalculator.cs`, `AnimeGroupCreator.cs`, `SQLServer.cs`, `SQLite.cs`, `MySQL.cs`, `QuartzStartup.cs`, `SqliteDriverFix.cs` | Simple SELECT/UPDATE/DROP operations |

### Recommended Approach

1. **Repository SELECT queries**: Convert to LINQ. The `HAVING COUNT > 1` patterns need careful LINQ translation using `GroupBy` + `Where(g => g.Count() > 1)`.
2. **Repository DELETE queries**: Replace with EF Core `ExecuteDelete()` (EF Core 7+).
3. **Service UPDATE query**: Replace with EF Core `ExecuteUpdateAsync()` (EF Core 7+).
4. **DatabaseFixes SELECT queries**: Convert to LINQ via `ShokoDbContext` — these run during migration when EF Core is already in place.
5. **DatabaseFixes DROP/ALTER queries**: Keep as `context.Database.ExecuteSqlRaw()` — EF Core Migrations handles DDL differently, and these are one-time migration scripts.
6. **SQLServer constraint helpers**: Keep as `context.Database.ExecuteSqlRaw()` — SQL Server-specific DDL that EF Core doesn't abstract well.
7. **Raw ADO.NET for schema init**: Keep as raw ADO.NET for `Versions` table operations and Quartz setup — these are provider-specific initialization that happens before EF Core is fully configured.

---

## T009 partial — AniDB relationships

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Scope**: 19 AniDB entities (`Shoko.Server/Mappings/AniDB_*Map.cs`)  
**Source**: 19 mapping files + 19 model files (`Shoko.Server/Models/AniDB/AniDB_*.cs`)  
**Generated**: 2026-05-07

**Key observation**: None of the 19 AniDB mapping files define `References`, `HasMany`, `HasManyToMany`, or `ManyToMany` relationships. All relationships are resolved via the repository pattern (LINQ queries on cached repositories). However, 17 of the 19 model files contain extensive navigation properties that are NOT mapped by NHibernate — these are populated by repository methods. EF Core must explicitly configure these relationships.

### 1. AniDB_Anime (AniDB_AnimeMap.cs)

- **Table**: `AniDB_Anime`
- **Primary Key**: `AniDB_AnimeID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AnimeSeries Series` — 1:1 logical relationship via `AnimeSeries.AniDB_ID` FK (not a direct FK on `AniDB_Anime`)
  - `ICollection<AniDB_Episode> Episodes` — 1:N via `AniDB_Episode.AnimeID` FK
  - `ICollection<AniDB_Anime_Tag> AnimeTags` — 1:N join table
  - `ICollection<AniDB_Anime_Character> Characters` — 1:N join table
  - `ICollection<AniDB_Anime_Staff> Staff` — 1:N join table
  - `ICollection<AniDB_Anime_Title> Titles` — 1:N multi-language titles
  - `ICollection<AniDB_Anime_Relation> Relations` — N:N self-referential (outgoing)
  - `ICollection<AniDB_Anime_Similar> Similar` — N:N self-referential (outgoing)
  - `ICollection<CrossRef_AniDB_TMDB_Show> CrossRefTMDBShows` — 1:N cross-reference
  - `ICollection<CrossRef_AniDB_TMDB_Movie> CrossRefTMDbMovies` — 1:N cross-reference
  - `ICollection<CrossRef_AniDB_TMDB_Episode> CrossRefTMDbEpisodes` — 1:N cross-reference
  - `ICollection<CrossRef_AniDB_MAL> CrossRefMAL` — 1:N cross-reference
  - `ICollection<CrossRef_AniDB_TraktV2> CrossRefTrakt` — 1:N cross-reference
  - `ICollection<AniDB_Anime_PreferredImage> PreferredImages` — 1:N preferred images
  - `AniDB_AnimeUpdate AnimeUpdate` — 1:1 tracking table (logical, PK on `AniDB_AnimeUpdate`)
  - `AniDB_GroupStatus GroupStatus` — 1:1 cache table (logical, PK on `AniDB_GroupStatus`)
- **EF Core Relationships to Configure**:
  - `HasOne<AnimeSeries>().WithOne().HasForeignKey<AnimeSeries>(a => a.AniDB_ID).IsRequired()` — logical 1:1, FK on dependent
  - `HasMany(e => e.Episodes).WithOne().HasForeignKey(e => e.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.AnimeTags).WithOne().HasForeignKey(t => t.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.Characters).WithOne().HasForeignKey(c => c.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.Staff).WithOne().HasForeignKey(s => s.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.Titles).WithOne().HasForeignKey(t => t.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.Relations).WithOne().HasForeignKey(r => r.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N (self-referential via RelatedAnimeID)
  - `HasMany(e => e.Similar).WithOne().HasForeignKey(s => s.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N (self-referential via SimilarAnimeID)
  - `HasOne(e => e.AnimeUpdate).WithOne().HasForeignKey<AniDB_AnimeUpdate>(u => u.AnimeID).OnDelete(DeleteBehavior.Cascade)` — logical 1:1, unique index on `AnimeID`
  - `HasOne(e => e.GroupStatus).WithOne().HasForeignKey<AniDB_GroupStatus>(g => g.AnimeID).OnDelete(DeleteBehavior.Cascade)` — logical 1:1, unique index on `AnimeID`
  - `HasMany(e => e.CrossRefTMDBShows).WithOne().HasForeignKey(c => c.AnidbAnimeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(e => e.CrossRefTMDbMovies).WithOne().HasForeignKey(c => c.AnidbAnimeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(e => e.CrossRefTMDbEpisodes).WithOne().HasForeignKey(c => c.AnidbAnimeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(e => e.CrossRefMAL).WithOne().HasForeignKey(c => c.AnimeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(e => e.CrossRefTrakt).WithOne().HasForeignKey(c => c.AnimeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(e => e.PreferredImages).WithOne().HasForeignKey(i => i.AnidbAnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
- **Self-References**:
  - `AniDB_Anime_Relation.RelatedAnimeID` → `AniDB_Anime.AniDB_AnimeID` — self-referential N:N relation table
  - `AniDB_Anime_Similar.SimilarAnimeID` → `AniDB_Anime.AniDB_AnimeID` — self-referential similarity table
- **EF Core Notes**:
  - `AniDB_AnimeUpdate` and `AniDB_GroupStatus` use Identity PKs but should have unique indexes on `AnimeID` to enforce one-row-per-anime semantics
  - Self-referential relations (Relation/Similar) need `HasMany().WithMany()` with join table configuration
  - Cross-source references (TMDB/MAL/Trakt) use `DeleteBehavior.Restrict` to prevent cascade deletes across providers
  - All navigation properties are populated via repository methods in `AniDB_AnimeRepository` — no NHibernate eager loading

### 2. AniDB_Episode (AniDB_EpisodeMap.cs)

- **Table**: `AniDB_Episode`
- **Primary Key**: `AniDB_EpisodeID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Episode.AnimeID` FK
  - `AnimeEpisode ShokoEpisode` — logical 1:1 via `AnimeEpisode.AniDB_EpisodeID` FK
  - `ICollection<AniDB_Episode_Title> Titles` — 1:N multi-language titles
  - `AniDB_Episode_PreferredImage PreferredImage` — 1:1 preferred image
  - `ICollection<CrossRef_AniDB_TMDB_Episode> CrossRefTMDbEpisodes` — 1:N cross-reference
  - `ICollection<CrossRef_AniDB_TMDB_Movie> CrossRefTMDbMovies` — 1:N cross-reference (for OVAs/movies)
  - `ICollection<CrossRef_File_Episode> FileEpisodes` — 1:N file-to-episode mapping
- **EF Core Relationships to Configure**:
  - `HasOne(e => e.Anime).WithMany(a => a.Episodes).HasForeignKey(e => e.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(e => e.ShokoEpisode).WithOne(s => s.AniDEpisode).HasForeignKey<AnimeEpisode>(s => s.AniDB_EpisodeID).OnDelete(DeleteBehavior.Cascade)` — logical 1:1
  - `HasMany(e => e.Titles).WithOne().HasForeignKey(t => t.AnidbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasOne(e => e.PreferredImage).WithOne().HasForeignKey<AniDB_Episode_PreferredImage>(i => i.AnidbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — logical 1:1
  - `HasMany(e => e.CrossRefTMDbEpisodes).WithOne().HasForeignKey(c => c.AnidbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(e => e.CrossRefTMDbMovies).WithOne().HasForeignKey(c => c.AnidbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(e => e.FileEpisodes).WithOne().HasForeignKey(f => f.EpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N
- **EF Core Notes**:
  - `PreferredImage` uses composite FK (`AnidbAnimeID`, `AnidbEpisodeID`) — configure both as FKs
  - `ShokoEpisode` logical 1:1 has FK on the dependent side (`AnimeEpisode`)
  - Cross-source references use `DeleteBehavior.Restrict`

### 3. AniDB_Tag (AniDB_TagMap.cs)

- **Table**: `AniDB_Tag`
- **Primary Key**: `TagID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Tag ParentTag` — N:1 self-referential via `AniDB_Tag.ParentTagID` FK
  - `ICollection<AniDB_Tag> ChildTags` — 1:N self-referential
  - `ICollection<AniDB_Anime_Tag> AnimeTags` — 1:N join table
- **EF Core Relationships to Configure**:
  - `HasOne(t => t.ParentTag).WithMany(t => t.ChildTags).HasForeignKey(t => t.ParentTagID).OnDelete(DeleteBehavior.Restrict)` — self-referential N:1
  - `HasMany(t => t.AnimeTags).WithOne(at => at.Tag).HasForeignKey(at => at.TagID).OnDelete(DeleteBehavior.Restrict)` — 1:N
- **EF Core Notes**:
  - Self-referential parent/child hierarchy via `ParentTagID` FK
  - `DeleteBehavior.Restrict` on parent reference prevents cascade delete of parent when child anime-tags are deleted
  - `TagNameSource` maps to column `TagName` (custom column name)

### 4. AniDB_Creator (AniDB_CreatorMap.cs)

- **Table**: `AniDB_Creator`
- **Primary Key**: `AniDB_CreatorID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `ICollection<AniDB_Anime_Staff> Staff` — 1:N via `AniDB_Anime_Staff.CreatorID` FK
  - `ICollection<AniDB_Anime_Character_Creator> CharacterCreators` — 1:N via `AniDB_Anime_Character_Creator.CreatorID` FK
- **EF Core Relationships to Configure**:
  - `HasMany(c => c.Staff).WithOne(s => s.Creator).HasForeignKey(s => s.CreatorID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(c => c.CharacterCreators).WithOne(cc => cc.Creator).HasForeignKey(cc => cc.CreatorID).OnDelete(DeleteBehavior.Restrict)` — 1:N
- **EF Core Notes**:
  - No self-references
  - Both relationships are 1:N from creator to join tables

### 5. AniDB_Character (AniDB_CharacterMap.cs)

- **Table**: `AniDB_Character`
- **Primary Key**: `AniDB_CharacterID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `ICollection<AniDB_Anime_Character> AnimeCharacters` — 1:N via `AniDB_Anime_Character.CharacterID` FK
  - `ICollection<AniDB_Anime_Character_Creator> CharacterCreators` — 1:N via `AniDB_Anime_Character_Creator.CharacterID` FK
- **EF Core Relationships to Configure**:
  - `HasMany(c => c.AnimeCharacters).WithOne(ac => ac.Character).HasForeignKey(ac => ac.CharacterID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(c => c.CharacterCreators).WithOne(cc => cc.Character).HasForeignKey(cc => cc.CharacterID).OnDelete(DeleteBehavior.Restrict)` — 1:N
- **EF Core Notes**:
  - No self-references
  - Both relationships are 1:N from character to join tables

### 6. AniDB_Anime_Tag (AniDB_Anime_TagMap.cs)

- **Table**: `AniDB_Anime_Tag`
- **Primary Key**: `AniDB_Anime_TagID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Tag.AnimeID` FK
  - `AniDB_Tag Tag` — N:1 via `AniDB_Anime_Tag.TagID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(at => at.Anime).WithMany(a => a.AnimeTags).HasForeignKey(at => at.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(at => at.Tag).WithMany(t => t.AnimeTags).HasForeignKey(at => at.TagID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Join table entity with two FKs to other entities
  - Cascade on Anime side (deleting anime deletes all its tags)
  - Restrict on Tag side (tag may be shared across multiple anime)

### 7. AniDB_Anime_Character (AniDB_Anime_CharacterMap.cs)

- **Table**: `AniDB_Anime_Character`
- **Primary Key**: `AniDB_Anime_CharacterID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Character.AnimeID` FK
  - `AniDB_Character Character` — N:1 via `AniDB_Anime_Character.CharacterID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(ac => ac.Anime).WithMany(a => a.Characters).HasForeignKey(ac => ac.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(ac => ac.Character).WithMany(c => c.AnimeCharacters).HasForeignKey(ac => ac.CharacterID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Join table entity with two FKs
  - Cascade on Anime side, Restrict on Character side

### 8. AniDB_Anime_Character_Creator (AniDB_Anime_Character_CreatorMap.cs)

- **Table**: `AniDB_Anime_Character_Creator`
- **Primary Key**: `AniDB_Anime_Character_CreatorID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Character_Creator.AnimeID` FK
  - `AniDB_Character Character` — N:1 via `AniDB_Anime_Character_Creator.CharacterID` FK
  - `AniDB_Creator Creator` — N:1 via `AniDB_Anime_Character_Creator.CreatorID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(ccc => ccc.Anime).WithMany(a => a.CharacterCreators).HasForeignKey(ccc => ccc.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(ccc => ccc.Character).WithMany(c => c.CharacterCreators).HasForeignKey(ccc => ccc.CharacterID).OnDelete(DeleteBehavior.Restrict)` — N:1
  - `HasOne(ccc => ccc.Creator).WithMany(cr => cr.CharacterCreators).HasForeignKey(ccc => ccc.CreatorID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Ternary join table with three FKs
  - Cascade on Anime side, Restrict on Character and Creator sides

### 9. AniDB_Anime_Staff (AniDB_Anime_StaffMap.cs)

- **Table**: `AniDB_Anime_Staff`
- **Primary Key**: `AniDB_Anime_StaffID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Staff.AnimeID` FK
  - `AniDB_Creator Creator` — N:1 via `AniDB_Anime_Staff.CreatorID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(s => s.Anime).WithMany(a => a.Staff).HasForeignKey(s => s.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(s => s.Creator).WithMany(c => c.Staff).HasForeignKey(s => s.CreatorID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Join table entity with two FKs
  - Cascade on Anime side, Restrict on Creator side

### 10. AniDB_Anime_Title (AniDB_Anime_TitleMap.cs)

- **Table**: `AniDB_Anime_Title`
- **Primary Key**: `AniDB_Anime_TitleID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Title.AnimeID` FK
- **EF Core Relationships to Configure**:
  - `HasMany(t => t.Titles).WithOne(at => at.Anime).HasForeignKey(at => at.AnimeID).OnDelete(DeleteBehavior.Cascade)` — 1:N (configured on AniDB_Anime side)
- **EF Core Notes**:
  - Multi-language title table; no inverse navigation property in model
  - `Language` and `TitleType` enums need ValueConverters

### 11. AniDB_Episode_Title (AniDB_Episode_TitleMap.cs)

- **Table**: `AniDB_Episode_Title`
- **Primary Key**: `AniDB_Episode_TitleID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Episode Episode` — N:1 via `AniDB_Episode_Title.AnidbEpisodeID` FK
- **EF Core Relationships to Configure**:
  - `HasMany(e => e.Titles).WithOne(et => et.Episode).HasForeignKey(et => et.AnidbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — 1:N (configured on AniDB_Episode side)
- **EF Core Notes**:
  - Multi-language episode title table; no inverse navigation property in model
  - `Language` enum needs ValueConverter

### 12. AniDB_Anime_PreferredImage (AniDB_Anime_PreferredImageMap.cs)

- **Table**: `AniDB_Anime_PreferredImage`
- **Primary Key**: `AniDB_Anime_PreferredImageID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_PreferredImage.AnidbAnimeID` FK
  - `Image Image` — N:1 via `AniDB_Anime_PreferredImage.ImageID` + `AniDB_Anime_PreferredImage.ImageSource` (cross-source FK)
- **EF Core Relationships to Configure**:
  - `HasOne(pi => pi.Anime).WithMany(a => a.PreferredImages).HasForeignKey(pi => pi.AnidbAnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(pi => pi.Image).WithMany().HasForeignKey(pi => pi.ImageID).OnDelete(DeleteBehavior.Restrict)` — N:1, composite FK with `ImageSource`
- **EF Core Notes**:
  - Cross-source reference: `ImageID` + `ImageSource` (enum: `DataSource.AniDB` or `DataSource.TMDB`) reference either `AniDB_Anime_PreferredImage` or `TMDB_Image`
  - `ImageSource` and `ImageType` enums need ValueConverters
  - EF Core requires configuring the dependent entity's FKs as a composite key for the relationship

### 13. AniDB_Episode_PreferredImage (AniDB_Episode_PreferredImageMap.cs)

- **Table**: `AniDB_Episode_PreferredImage`
- **Primary Key**: `AniDB_Episode_PreferredImageID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Episode_PreferredImage.AnidbAnimeID` FK
  - `AniDB_Episode Episode` — N:1 via `AniDB_Episode_PreferredImage.AnidbEpisodeID` FK
  - `Image Image` — N:1 via `AniDB_Episode_PreferredImage.ImageID` + `AniDB_Episode_PreferredImage.ImageSource` (cross-source FK)
- **EF Core Relationships to Configure**:
  - `HasOne(pi => pi.Anime).WithMany().HasForeignKey(pi => pi.AnidbAnimeID).OnDelete(DeleteBehavior.Restrict)` — N:1
  - `HasOne(pi => pi.Episode).WithOne(e => e.PreferredImage).HasForeignKey<AniDB_Episode_PreferredImage>(pi => pi.AnidbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(pi => pi.Image).WithMany().HasForeignKey(pi => pi.ImageID).OnDelete(DeleteBehavior.Restrict)` — N:1, composite FK with `ImageSource`
- **EF Core Notes**:
  - Composite FK on Episode side: `(AnidbAnimeID, AnidbEpisodeID)` — both are FKs to AniDB entities
  - Cross-source reference: same pattern as `AniDB_Anime_PreferredImage`
  - `ImageSource` and `ImageType` enums need ValueConverters

### 14. AniDB_Anime_Relation (AniDB_Anime_RelationMap.cs)

- **Table**: `AniDB_Anime_Relation`
- **Primary Key**: `AniDB_Anime_RelationID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Relation.AnimeID` FK
  - `AniDB_Anime RelatedAnime` — N:1 via `AniDB_Anime_Relation.RelatedAnimeID` FK (self-referential)
- **EF Core Relationships to Configure**:
  - `HasOne(r => r.Anime).WithMany(a => a.Relations).HasForeignKey(r => r.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(r => r.RelatedAnime).WithMany(a => a.Relations).HasForeignKey(r => r.RelatedAnimeID).OnDelete(DeleteBehavior.Restrict)` — N:1, self-referential
- **EF Core Notes**:
  - Self-referential N:N relation table (anime ↔ anime relations)
  - `RelationType` is an int/enum — needs ValueConverter
  - Cascade on source anime side, Restrict on related anime side (to prevent circular cascade deletes)

### 15. AniDB_Anime_Similar (AniDB_Anime_SimilarMap.cs)

- **Table**: `AniDB_Anime_Similar`
- **Primary Key**: `AniDB_Anime_SimilarID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_Anime_Similar.AnimeID` FK
  - `AniDB_Anime SimilarAnime` — N:1 via `AniDB_Anime_Similar.SimilarAnimeID` FK (self-referential)
- **EF Core Relationships to Configure**:
  - `HasOne(s => s.Anime).WithMany(a => a.Similar).HasForeignKey(s => s.AnimeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(s => s.SimilarAnime).WithMany(a => a.Similar).HasForeignKey(s => s.SimilarAnimeID).OnDelete(DeleteBehavior.Restrict)` — N:1, self-referential
- **EF Core Notes**:
  - Self-referential N:N similarity table (anime ↔ anime similarity)
  - `Approval` and `Total` are numeric properties (approval score, total votes)
  - Cascade on source anime side, Restrict on similar anime side

### 16. AniDB_AnimeUpdate (AniDB_AnimeUpdateMap.cs)

- **Table**: `AniDB_AnimeUpdate`
- **Primary Key**: `AniDB_AnimeUpdateID` (Identity) — NOT `AnimeID`
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_AnimeUpdate.AnimeID` FK (logical 1:1)
- **EF Core Relationships to Configure**:
  - `HasOne(u => u.Anime).WithOne(a => a.AnimeUpdate).HasForeignKey<AniDB_AnimeUpdate>(u => u.AnimeID).OnDelete(DeleteBehavior.Cascade)` — logical 1:1
  - **Unique index on `AnimeID`** — required to enforce one-row-per-anime semantics
- **EF Core Notes**:
  - Identity PK (`AniDB_AnimeUpdateID`) but logically one row per anime
  - `AnimeID` is a regular column (not PK) — must add unique index
  - Used for tracking last AniDB fetch timestamp (`UpdatedAt`)

### 17. AniDB_GroupStatus (AniDB_GroupStatusMap.cs)

- **Table**: `AniDB_GroupStatus`
- **Primary Key**: `AniDB_GroupStatusID` (Identity) — NOT `AnimeID`
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `AniDB_Anime Anime` — N:1 via `AniDB_GroupStatus.AnimeID` FK (logical 1:1)
- **EF Core Relationships to Configure**:
  - `HasOne(g => g.Anime).WithOne(a => a.GroupStatus).HasForeignKey<AniDB_GroupStatus>(g => g.AnimeID).OnDelete(DeleteBehavior.Cascade)` — logical 1:1
  - **Unique index on `AnimeID`** — required to enforce one-row-per-anime semantics
- **EF Core Notes**:
  - Identity PK (`AniDB_GroupStatusID`) but logically one row per anime
  - `AnimeID` is a regular column (not PK) — must add unique index
  - Cache of AniDB GROUPSTATUS UDP response (group name, completion state, episode range)

### 18. AniDB_Message (AniDB_MessageMap.cs)

- **Table**: `AniDB_Message`
- **Primary Key**: `AniDB_MessageID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate): None
- **EF Core Relationships to Configure**: None
- **EF Core Notes**:
  - Standalone entity with no relationships
  - `Type` and `Flags` enums need ValueConverters
  - Used for AniDB notification/message storage

### 19. AniDB_NotifyQueue (AniDB_NotifyQueueMap.cs)

- **Table**: `AniDB_NotifyQueue`
- **Primary Key**: `AniDB_NotifyQueueID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate): None
- **EF Core Relationships to Configure**: None
- **EF Core Notes**:
  - Standalone entity with no relationships
  - `Type` enum needs ValueConverter
  - Staging table for raw AniDB notification IDs (type + ID)

---

### Relationship Summary — AniDB Entities

| Source Entity | Target Entity | Relationship Type | FK Column(s) | Cascade | Notes |
|---------------|---------------|-------------------|--------------|---------|-------|
| `AniDB_Anime` | `AnimeSeries` | 1:1 (logical) | `AnimeSeries.AniDB_ID` | Cascade | FK on dependent side |
| `AniDB_Anime` | `AniDB_Episode` | 1:N | `AniDB_Episode.AnimeID` | Cascade | |
| `AniDB_Anime` | `AniDB_Anime_Tag` | 1:N | `AniDB_Anime_Tag.AnimeID` | Cascade | Join table |
| `AniDB_Anime` | `AniDB_Anime_Character` | 1:N | `AniDB_Anime_Character.AnimeID` | Cascade | Join table |
| `AniDB_Anime` | `AniDB_Anime_Staff` | 1:N | `AniDB_Anime_Staff.AnimeID` | Cascade | Join table |
| `AniDB_Anime` | `AniDB_Anime_Title` | 1:N | `AniDB_Anime_Title.AnimeID` | Cascade | Multi-language |
| `AniDB_Anime` | `AniDB_Anime_Relation` | 1:N | `AniDB_Anime_Relation.AnimeID` | Cascade | Self-ref (outgoing) |
| `AniDB_Anime` | `AniDB_Anime_Similar` | 1:N | `AniDB_Anime_Similar.AnimeID` | Cascade | Self-ref (outgoing) |
| `AniDB_Anime` | `AniDB_AnimeUpdate` | 1:1 (logical) | `AniDB_AnimeUpdate.AnimeID` | Cascade | Unique index on AnimeID |
| `AniDB_Anime` | `AniDB_GroupStatus` | 1:1 (logical) | `AniDB_GroupStatus.AnimeID` | Cascade | Unique index on AnimeID |
| `AniDB_Anime` | `AniDB_Anime_PreferredImage` | 1:N | `AniDB_Anime_PreferredImage.AnidbAnimeID` | Cascade | Cross-source image |
| `AniDB_Anime` | `CrossRef_AniDB_TMDB_Show` | 1:N | `CrossRef_AniDB_TMDB_Show.AnidbAnimeID` | Restrict | Cross-provider |
| `AniDB_Anime` | `CrossRef_AniDB_TMDB_Movie` | 1:N | `CrossRef_AniDB_TMDB_Movie.AnidbAnimeID` | Restrict | Cross-provider |
| `AniDB_Anime` | `CrossRef_AniDB_TMDB_Episode` | 1:N | `CrossRef_AniDB_TMDB_Episode.AnidbAnimeID` | Restrict | Cross-provider |
| `AniDB_Anime` | `CrossRef_AniDB_MAL` | 1:N | `CrossRef_AniDB_MAL.AnimeID` | Restrict | Cross-provider |
| `AniDB_Anime` | `CrossRef_AniDB_TraktV2` | 1:N | `CrossRef_AniDB_TraktV2.AnimeID` | Restrict | Cross-provider |
| `AniDB_Episode` | `AniDB_Anime` | N:1 | `AniDB_Episode.AnimeID` | Cascade | Inverse of 1:N above |
| `AniDB_Episode` | `AnimeEpisode` | 1:1 (logical) | `AnimeEpisode.AniDB_EpisodeID` | Cascade | FK on dependent side |
| `AniDB_Episode` | `AniDB_Episode_Title` | 1:N | `AniDB_Episode_Title.AnidbEpisodeID` | Cascade | Multi-language |
| `AniDB_Episode` | `AniDB_Episode_PreferredImage` | 1:1 (logical) | Composite `(AnidbAnimeID, AnidbEpisodeID)` | Cascade | Cross-source image |
| `AniDB_Episode` | `CrossRef_AniDB_TMDB_Episode` | 1:N | `CrossRef_AniDB_TMDB_Episode.AnidbEpisodeID` | Restrict | Cross-provider |
| `AniDB_Episode` | `CrossRef_AniDB_TMDB_Movie` | 1:N | `CrossRef_AniDB_TMDB_Movie.AnidbEpisodeID` | Restrict | Cross-provider |
| `AniDB_Episode` | `CrossRef_File_Episode` | 1:N | `CrossRef_File_Episode.EpisodeID` | Restrict | File mapping |
| `AniDB_Tag` | `AniDB_Tag` | N:1 (self) | `AniDB_Tag.ParentTagID` | Restrict | Self-referential hierarchy |
| `AniDB_Tag` | `AniDB_Anime_Tag` | 1:N | `AniDB_Anime_Tag.TagID` | Restrict | Join table |
| `AniDB_Creator` | `AniDB_Anime_Staff` | 1:N | `AniDB_Anime_Staff.CreatorID` | Restrict | Join table |
| `AniDB_Creator` | `AniDB_Anime_Character_Creator` | 1:N | `AniDB_Anime_Character_Creator.CreatorID` | Restrict | Join table |
| `AniDB_Character` | `AniDB_Anime_Character` | 1:N | `AniDB_Anime_Character.CharacterID` | Restrict | Join table |
| `AniDB_Character` | `AniDB_Anime_Character_Creator` | 1:N | `AniDB_Anime_Character_Creator.CharacterID` | Restrict | Join table |
| `AniDB_Anime_Relation` | `AniDB_Anime` (Related) | N:1 (self) | `AniDB_Anime_Relation.RelatedAnimeID` | Restrict | Self-referential |
| `AniDB_Anime_Similar` | `AniDB_Anime` (Similar) | N:1 (self) | `AniDB_Anime_Similar.SimilarAnimeID` | Restrict | Self-referential |
| `AniDB_Anime_PreferredImage` | `Image` | N:1 | Composite `(ImageID, ImageSource)` | Restrict | Cross-source (AniDB/TMDB) |
| `AniDB_Episode_PreferredImage` | `Image` | N:1 | Composite `(ImageID, ImageSource)` | Restrict | Cross-source (AniDB/TMDB) |

### EF Core Parity Risks

1. **No relationships in any AniDB mapping**: All 19 AniDB mappings have zero `References`, `HasMany`, or `ManyToMany` declarations. Every single relationship must be explicitly configured in EF Core entity configurations. This is a significant migration effort.

2. **Logical 1:1 relationships with Identity PKs**: `AniDB_AnimeUpdate` and `AniDB_GroupStatus` use Identity PKs but are logically 1:1 with `AniDB_Anime`. EF Core must use `HasOne/WithOne` with a foreign key on the dependent side plus a unique index on the FK column. NHibernate does not enforce this at the mapping level.

3. **Self-referential relations**: `AniDB_Anime_Relation` and `AniDB_Anime_Similar` are self-referential join tables. EF Core must configure both FKs pointing to `AniDB_Anime` with different navigation property names on the same entity.

4. **Cross-source image references**: `AniDB_Anime_PreferredImage` and `AniDB_Episode_PreferredImage` reference images from either AniDB or TMDB via a composite FK (`ImageID` + `ImageSource` enum). EF Core must handle discriminated unions or separate relationships — this is a known EF Core limitation with single FK column referencing multiple entity types.

5. **Composite FK on `AniDB_Episode_PreferredImage`**: Uses both `AnidbAnimeID` and `AnidbEpisodeID` as FKs. EF Core must configure both as relationship FKs.

6. **Cascade vs Restrict patterns**: Anime-side relationships use `Cascade` (deleting anime cascades to episodes, tags, characters, etc.). Cross-provider references use `Restrict` (deleting an anime does NOT cascade to TMDB/MAL/Trakt cross-references). This pattern must be preserved.

7. **Navigation properties in models but not in mappings**: 17 of 19 AniDB models have navigation properties that are NOT mapped by NHibernate. These are populated by repository methods. EF Core must explicitly configure all relationships to enable `Include()`/`ThenInclude()` loading.

8. **Repository pattern as relationship resolver**: All AniDB relationships are currently resolved via repository LINQ queries (e.g., `AniDB_AnimeRepository.GetAnimeWithEpisodes(animeID)`). EF Core configurations should enable similar queries via `Include()`/`ThenInclude()` or explicit LINQ projections.

---

## T009 partial — TMDB relationships

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Scope**: 22 TMDB entities (`Shoko.Server/Mappings/TMDB/*.cs` + `Shoko.Server/Mappings/Text/*.cs` + `Shoko.Server/Mappings/TMDB/Optional/*.cs`)  
**Source**: 22 mapping files + 22 model files (`Shoko.Server/Models/TMDB/*.cs`)  
**Generated**: 2026-05-07

**Key observation**: None of the 22 TMDB mapping files define `References`, `HasMany`, `HasManyToMany`, or `ManyToMany` relationships. All relationships are resolved via the repository pattern (LINQ queries on cached repositories). However, most model files contain navigation properties that are NOT mapped by NHibernate — these are populated by repository methods. EF Core must explicitly configure these relationships.

### 1. TMDB_Show (TMDB_ShowMap.cs)

- **Table**: `TMDB_Show`
- **Primary Key**: `TMDB_ShowID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbSeasons` → `TMDB_Season` — 1:N via `TMDB_Season.TmdbShowID` FK
  - `TmdbEpisodes` → `TMDB_Episode` — 1:N via `TMDB_Episode.TmdbShowID` FK
  - `TmdbCompanyCrossReferences` → `TMDB_Company_Entity` — 1:N via `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Show` (polymorphic)
  - `TmdbCompanies` → `TMDB_Company` — derived from Company_Entity
  - `TmdbNetworkCrossReferences` → `TMDB_Show_Network` — 1:N via `TMDB_Show_Network.TmdbShowID` FK
  - `TmdbNetworks` → `TMDB_Network` — derived from Show_Network
  - `PreferredAlternateOrdering` → `TMDB_AlternateOrdering` — N:1 via `TMDB_AlternateOrdering.TmdbShowID` FK
  - `TmdbAlternateOrdering` → `TMDB_AlternateOrdering` — 1:N via `TMDB_AlternateOrdering.TmdbShowID` FK
  - `CrossReferences` → `CrossRef_AniDB_TMDB_Show` — 1:N cross-reference
  - `EpisodeCrossReferences` → `CrossRef_AniDB_TMDB_Episode` — 1:N cross-reference (aggregated from episodes)
  - `Cast` → `TMDB_Episode_Cast` — 1:N aggregated from episode cast by show
  - `Crew` → `TMDB_Episode_Crew` — 1:N aggregated from episode crew by show
  - `DefaultPoster` → `TMDB_Image` — N:1 via `RemoteFileName` lookup (not FK)
  - `DefaultBackdrop` → `TMDB_Image` — N:1 via `RemoteFileName` lookup (not FK)
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Show` (polymorphic)
  - `SeasonCrossReferences` → `CrossRef_AniDB_TMDB_Season` — derived from episodes
- **EF Core Relationships to Configure**:
  - `HasMany(s => s.TmdbSeasons).WithOne().HasForeignKey(s => s.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(s => s.TmdbEpisodes).WithOne().HasForeignKey(e => e.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(s => s.TmdbCompanyCrossReferences).WithOne().HasForeignKey(c => c.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via ParentType discriminator
  - `HasMany(s => s.TmdbNetworkCrossReferences).WithOne().HasForeignKey(sn => sn.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(s => s.TmdbAlternateOrdering).WithOne(ao => ao.TmdbShow).HasForeignKey(ao => ao.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasOne(s => s.CrossReferences).WithOne().HasForeignKey<CrossRef_AniDB_TMDB_Show>(c => c.TmdbShowID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - `HasMany(s => s.Cast).WithOne(ec => ec.TmdbEpisode).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N aggregated
  - `HasMany(s => s.Crew).WithOne(ec => ec.TmdbEpisode).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N aggregated
  - Polymorphic image: `HasMany(s => s.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - `PreferredAlternateOrderingID` is a string property (not FK column) — resolved via repository lookup, not a direct FK
  - `Cast` and `Crew` are aggregated from episode-level cast/crew — no direct FK from Show to Episode_Cast/Episode_Crew
  - `DefaultPoster` and `DefaultBackdrop` use string `RemoteFileName` for image lookup — not a FK relationship
  - All polymorphic references (Company_Entity, Image_Entity) use `ForeignEntityType` enum discriminator
  - `TMDB_Base<int>` base class provides `Id = TmdbShowID`

### 2. TMDB_Season (TMDB_SeasonMap.cs)

- **Table**: `TMDB_Season`
- **Primary Key**: `TMDB_SeasonID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbShow` → `TMDB_Show` — N:1 via `TMDB_Season.TmdbShowID` FK
  - `TmdbEpisodes` → `TMDB_Episode` — 1:N via `TMDB_Episode.TmdbSeasonID` FK
  - `Cast` → `TMDB_Episode_Cast` — 1:N aggregated from episode cast by season
  - `Crew` → `TMDB_Episode_Crew` — 1:N aggregated from episode crew by season
  - `DefaultPoster` → `TMDB_Image` — N:1 via `RemoteFileName` lookup
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Season` (polymorphic)
- **EF Core Relationships to Configure**:
  - `HasOne(s => s.TmdbShow).WithMany(sh => sh.TmdbSeasons).HasForeignKey(s => s.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasMany(s => s.TmdbEpisodes).WithOne(e => e.TmdbSeason).HasForeignKey(e => e.TmdbSeasonID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(s => s.Cast).WithOne(ec => ec.TmdbEpisode).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N aggregated
  - `HasMany(s => s.Crew).WithOne(ec => ec.TmdbEpisode).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N aggregated
  - Polymorphic image: `HasMany(s => s.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - `Cast` and `Crew` are aggregated from episode-level cast/crew — no direct FK from Season to Episode_Cast/Episode_Crew
  - `TMDB_Base<int>` base class provides `Id = TmdbSeasonID`

### 3. TMDB_Episode (TMDB_EpisodeMap.cs)

- **Table**: `TMDB_Episode`
- **Primary Key**: `TMDB_EpisodeID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbShow` → `TMDB_Show` — N:1 via `TMDB_Episode.TmdbShowID` FK
  - `TmdbSeason` → `TMDB_Season` — N:1 via `TMDB_Episode.TmdbSeasonID` FK
  - `Cast` → `TMDB_Episode_Cast` — 1:N via `TMDB_Episode_Cast.TmdbEpisodeID` FK
  - `Crew` → `TMDB_Episode_Crew` — 1:N via `TMDB_Episode_Crew.TmdbEpisodeID` FK
  - `TmdbAlternateOrderingEpisodes` → `TMDB_AlternateOrdering_Episode` — 1:N via `TMDB_AlternateOrdering_Episode.TmdbEpisodeID` FK
  - `DefaultThumbnail` → `TMDB_Image` — N:1 via `RemoteFileName` lookup
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Episode` (polymorphic)
  - `CrossReferences` → `CrossRef_AniDB_TMDB_Episode` — 1:N cross-reference
  - `FileCrossReferences` → `CrossRef_File_Episode` — derived
- **EF Core Relationships to Configure**:
  - `HasOne(e => e.TmdbShow).WithMany(s => s.TmdbEpisodes).HasForeignKey(e => e.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(e => e.TmdbSeason).WithMany(s => s.TmdbEpisodes).HasForeignKey(e => e.TmdbSeasonID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasMany(e => e.Cast).WithOne(c => c.TmdbEpisode).HasForeignKey(c => c.TmdbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.Crew).WithOne(cr => cr.TmdbEpisode).HasForeignKey(cr => cr.TmdbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(e => e.TmdbAlternateOrderingEpisodes).WithOne(aoe => aoe.TmdbEpisode).HasForeignKey(aoe => aoe.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasOne(e => e.CrossReferences).WithOne().HasForeignKey<CrossRef_AniDB_TMDB_Episode>(c => c.AnidbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - Polymorphic image: `HasMany(e => e.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - `RuntimeMinutes` maps to column `Runtime` (custom column name)
  - `AiredAt` uses `DateOnlyConverter`
  - `TMDB_Base<int>` base class provides `Id = TmdbEpisodeID`

### 4. TMDB_Movie (TMDB_MovieMap.cs)

- **Table**: `TMDB_Movie`
- **Primary Key**: `TMDB_MovieID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbCollection` → `TMDB_Collection` — N:1 via `TMDB_Movie.TmdbCollectionID` FK (nullable)
  - `Cast` → `TMDB_Movie_Cast` — 1:N via `TMDB_Movie_Cast.TmdbMovieID` FK
  - `Crew` → `TMDB_Movie_Crew` — 1:N via `TMDB_Movie_Crew.TmdbMovieID` FK
  - `TmdbCompanyCrossReferences` → `TMDB_Company_Entity` — 1:N via `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Movie` (polymorphic)
  - `TmdbCompanies` → `TMDB_Company` — derived from Company_Entity
  - `DefaultPoster` → `TMDB_Image` — N:1 via `RemoteFileName` lookup
  - `DefaultBackdrop` → `TMDB_Image` — N:1 via `RemoteFileName` lookup
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Movie` (polymorphic)
  - `CrossReferences` → `CrossRef_AniDB_TMDB_Movie` — 1:N cross-reference
- **EF Core Relationships to Configure**:
  - `HasOne(m => m.TmdbCollection).WithMany(c => c.TmdbMovies).HasForeignKey(m => m.TmdbCollectionID).OnDelete(DeleteBehavior.Restrict)` — N:1, optional
  - `HasMany(m => m.Cast).WithOne(c => c.TmdbMovie).HasForeignKey(c => c.TmdbMovieID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(m => m.Crew).WithOne(cr => cr.TmdbMovie).HasForeignKey(cr => cr.TmdbMovieID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(m => m.TmdbCompanyCrossReferences).WithOne().HasForeignKey(c => c.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via ParentType discriminator
  - `HasOne(m => m.CrossReferences).WithOne().HasForeignKey<CrossRef_AniDB_TMDB_Movie>(c => c.TmdbMovieID).OnDelete(DeleteBehavior.Restrict)` — 1:N cross-reference
  - Polymorphic image: `HasMany(m => m.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - `RuntimeMinutes` maps to column `Runtime` (custom column name)
  - `ReleasedAt` uses `DateOnlyConverter`
  - `Genres` and `Keywords` use `StringListConverter` (JSON)
  - `ContentRatings` uses `TmdbContentRatingConverter`
  - `ProductionCountries` uses `TmdbProductionCountryConverter`
  - `TMDB_Base<int>` base class provides `Id = TmdbMovieID`

### 5. TMDB_Person (TMDB_PersonMap.cs)

- **Table**: `TMDB_Person`
- **Primary Key**: `TMDB_PersonID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `GetAllBiographies()` → `TMDB_Overview` — 1:N via `TMDB_Overview.ParentID` + `ParentType=Person` (polymorphic)
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Person` (polymorphic)
  - `EpisodeCastRoles` → `TMDB_Episode_Cast` — 1:N via `TMDB_Episode_Cast.TmdbPersonID` FK
  - `MovieCastRoles` → `TMDB_Movie_Cast` — 1:N via `TMDB_Movie_Cast.TmdbPersonID` FK
  - `SeriesCastRoles` → `TMDB_Show_Cast` — 1:N aggregated from episode cast
  - `EpisodeCrewRoles` → `TMDB_Episode_Crew` — 1:N via `TMDB_Episode_Crew.TmdbPersonID` FK
  - `MovieCrewRoles` → `TMDB_Movie_Crew` — 1:N via `TMDB_Movie_Crew.TmdbPersonID` FK
  - `SeriesCrewRoles` → `TMDB_Show_Crew` — 1:N aggregated from episode crew
- **EF Core Relationships to Configure**:
  - Polymorphic biographies: `HasMany(p => p.Biographies).WithOne().HasForeignKey(o => o.ParentID).OnDelete(DeleteBehavior.Cascade)` — polymorphic via ParentType discriminator
  - Polymorphic images: `HasMany(p => p.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
  - `HasMany(p => p.EpisodeCastRoles).WithOne(ec => ec.TmdbPerson).HasForeignKey(ec => ec.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(p => p.MovieCastRoles).WithOne(mc => mc.TmdbPerson).HasForeignKey(mc => mc.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(p => p.EpisodeCrewRoles).WithOne(ec => ec.TmdbPerson).HasForeignKey(ec => ec.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - `HasMany(p => p.MovieCrewRoles).WithOne(mc => mc.TmdbPerson).HasForeignKey(mc => mc.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — 1:N
- **EF Core Notes**:
  - `Gender` uses `PersonGender` enum — needs ValueConverter
  - `BirthDay` and `DeathDay` use `DateOnlyConverter`
  - `Aliases` uses `StringListConverter` (JSON)
  - `TMDB_Base<int>` base class provides `Id = TmdbPersonID`

### 6. TMDB_Image (TMDB_ImageMap.cs)

- **Table**: `TMDB_Image`
- **Primary Key**: `TMDB_ImageID` (Identity)
- **Base Class**: `Image_Base` (provides `ID`, `ContentType`, `Source`, `ImageType`, `IsPreferred`, `IsEnabled`, `IsLocked`, `IsLocalAvailable`, `IsRemoteAvailable`, `AspectRatio`, `Width`, `Height`, `LanguageCode`, `Language`, `RemoteURL`, `LocalPath`)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate): None (referenced by other entities via polymorphic `TMDB_Image_Entity`)
- **EF Core Relationships to Configure**:
  - Referenced by `TMDB_Image_Entity` polymorphically — no direct navigation property on Image
  - `HasMany(ie => ie.Image).WithOne().HasForeignKey(ie => ie.RemoteFileName).OnDelete(DeleteBehavior.Restrict)` — polymorphic inverse
- **EF Core Notes**:
  - `Image_Base` has unmapped properties (`Source`, `ImageType`, `IsPreferred`, `RemoteURL`, `LocalPath`) — these are not persisted to DB
  - `Language` uses `TitleLanguageConverter` enum — needs ValueConverter
  - Only 7 properties mapped: `IsEnabled`, `Width`, `Height`, `Language`, `RemoteFileName`, `UserRating`, `UserVotes`
  - `RemoteFileName` is the lookup key for polymorphic image references

### 7. TMDB_Image_Entity (TMDB_Image_EntityMap.cs)

- **Table**: `TMDB_Image_Entity`
- **Primary Key**: `TMDB_Image_EntityID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (polymorphic via ParentID + ParentType):
  - `GetTmdbImage()` → `TMDB_Image` — N:1 via `RemoteFileName` lookup
  - `GetTmdbEntity()` → `IEntityMetadata` — polymorphic: Movie, Episode, Season, Show, Collection, Person, Network, Company
- **EF Core Relationships to Configure**:
  - `HasOne(ie => ie.Image).WithMany().HasForeignKey(ie => ie.RemoteFileName).OnDelete(DeleteBehavior.Restrict)` — N:1
  - Polymorphic entity: `HasOne(ie => ie.TmdbEntity).WithMany().HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - **Polymorphic relationship**: `TmdbEntityID` + `TmdbEntityType` (enum discriminator) references 8 different entity types
  - `ImageType` uses `ImageEntityType` enum — needs ValueConverter
  - `TmdbEntityType` uses `ForeignEntityType` enum — needs ValueConverter
  - `ReleasedAt` uses `DateOnlyConverter`
  - **EF Core Risk**: Discriminated union pattern — EF Core cannot directly map a single FK column to multiple entity types. Must use TPH hierarchy with discriminator or separate relationships per entity type.

### 8. TMDB_Company (TMDB_CompanyMap.cs)

- **Table**: `TMDB_Company`
- **Primary Key**: `TMDB_CompanyID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Company` (polymorphic)
  - `GetTmdbCompanyCrossReferences()` → `TMDB_Company_Entity` — 1:N via `TMDB_Company_Entity.TmdbCompanyID` FK
  - `GetTmdbEntities()` → `IEntityMetadata` — derived from Company_Entity
  - `GetTmdbShows()` → `TMDB_Show` — derived (only Show type in Company_Entity)
  - `GetTmdbMovies()` → `TMDB_Movie` — derived (only Movie type in Company_Entity)
- **EF Core Relationships to Configure**:
  - `HasMany(c => c.TmdbCompanyCrossReferences).WithOne(ce => ce.TmdbCompany).HasForeignKey(ce => ce.TmdbCompanyID).OnDelete(DeleteBehavior.Restrict)` — 1:N
  - Polymorphic images: `HasMany(c => c.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - `TMDB_Base<int>` base class provides `Id = TmdbCompanyID`

### 9. TMDB_Company_Entity (TMDB_Company_EntityMap.cs)

- **Table**: `TMDB_Company_Entity`
- **Primary Key**: `TMDB_Company_EntityID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (polymorphic):
  - `GetTmdbCompany()` → `TMDB_Company` — N:1 via `TMDB_Company_Entity.TmdbCompanyID` FK
  - `GetTmdbEntity()` → `IEntityMetadata` — polymorphic: Show, Movie
- **EF Core Relationships to Configure**:
  - `HasOne(ce => ce.TmdbCompany).WithMany(c => c.TmdbCompanyCrossReferences).HasForeignKey(ce => ce.TmdbCompanyID).OnDelete(DeleteBehavior.Restrict)` — N:1
  - Polymorphic entity: `HasOne(ce => ce.TmdbEntity).WithMany().HasForeignKey(ce => ce.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator (2 target types: Show, Movie)
- **EF Core Notes**:
  - `TmdbEntityType` uses `ForeignEntityType` enum — needs ValueConverter
  - `ReleasedAt` uses `DateOnlyConverter`
  - **EF Core Risk**: Polymorphic relationship with 2 target types (Show, Movie) — must use discriminator

### 10. TMDB_Collection (Optional) (TMDB_CollectionMap.cs)

- **Table**: `TMDB_Collection`
- **Primary Key**: `TMDB_CollectionID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `GetAllTitles()` → `TMDB_Title` — 1:N via `TMDB_Title.ParentID` + `ParentType=Collection` (polymorphic text table)
  - `GetAllOverviews()` → `TMDB_Overview` — 1:N via `TMDB_Overview.ParentID` + `ParentType=Collection` (polymorphic text table)
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Collection` (polymorphic)
  - `GetTmdbMovies()` → `TMDB_Movie` — 1:N via `TMDB_Movie.TmdbCollectionID` FK (inverse of N:1 on Movie)
- **EF Core Relationships to Configure**:
  - `HasMany(c => c.Titles).WithOne().HasForeignKey(t => t.ParentID).OnDelete(DeleteBehavior.Cascade)` — polymorphic via ParentType discriminator
  - `HasMany(c => c.Overviews).WithOne().HasForeignKey(o => o.ParentID).OnDelete(DeleteBehavior.Cascade)` — polymorphic via ParentType discriminator
  - `HasMany(c => c.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
  - `HasMany(c => c.TmdbMovies).WithOne(m => m.TmdbCollection).HasForeignKey(m => m.TmdbCollectionID).OnDelete(DeleteBehavior.Restrict)` — 1:N (optional FK on Movie side)
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - `TMDB_Base<int>` base class provides `Id = TmdbCollectionID`

### 11. TMDB_Collection_Movie (Optional) (TMDB_Collection_MovieMap.cs)

- **Table**: `TMDB_Collection_Movie`
- **Primary Key**: `TMDB_Collection_MovieID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**: None (pure join entity)
- **EF Core Relationships to Configure**:
  - `HasOne(cm => cm.TmdbCollection).WithMany(c => c.TmdbMovies).HasForeignKey(cm => cm.TmdbCollectionID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(cm => cm.TmdbMovie).WithMany(m => m.CollectionMembers).HasForeignKey(cm => cm.TmdbMovieID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - Pure join table with `Ordering` property for sort order
  - **EF Core Risk**: Consider composite unique index on `(TmdbCollectionID, TmdbMovieID)` to prevent duplicates

### 12. TMDB_Network (Optional) (TMDB_NetworkMap.cs)

- **Table**: `TMDB_Network`
- **Primary Key**: `TMDB_NetworkID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `GetImages()` → `TMDB_Image` — 1:N via `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Network` (polymorphic)
  - `GetTmdbNetworkCrossReferences()` → `TMDB_Show_Network` — 1:N via `TMDB_Show_Network.TmdbNetworkID` FK
  - `Shows` → `TMDB_Show` — derived from Show_Network cross-refs
- **EF Core Relationships to Configure**:
  - `HasMany(n => n.TmdbShowNetworks).WithOne(sn => sn.TmdbNetwork).HasForeignKey(sn => sn.TmdbNetworkID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - Polymorphic images: `HasMany(n => n.Images).WithOne(ie => ie.TmdbEntity).HasForeignKey(ie => ie.TmdbEntityID).OnDelete(DeleteBehavior.Restrict)` — polymorphic via TmdbEntityType discriminator
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - `LastOrphanedAt` is nullable DateTime

### 13. TMDB_Show_Network (Optional) (TMDB_Show_NetworkMap.cs)

- **Table**: `TMDB_Show_Network`
- **Primary Key**: `TMDB_Show_NetworkID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**:
  - `GetTmdbNetwork()` → `TMDB_Network` — N:1 via `TMDB_Show_Network.TmdbNetworkID` FK
  - `GetTmdbShow()` → `TMDB_Show` — N:1 via `TMDB_Show_Network.TmdbShowID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(sn => sn.TmdbShow).WithMany(s => s.TmdbShowNetworks).HasForeignKey(sn => sn.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(sn => sn.TmdbNetwork).WithMany(n => n.TmdbShowNetworks).HasForeignKey(sn => sn.TmdbNetworkID).OnDelete(DeleteBehavior.Cascade)` — N:1
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - Join table between TMDB_Show and TMDB_Network with `Ordering` property
  - **EF Core Risk**: Consider composite unique index on `(TmdbShowID, TmdbNetworkID)` to prevent duplicates

### 14. TMDB_AlternateOrdering (Optional) (TMDB_AlternateOrderingMap.cs)

- **Table**: `TMDB_AlternateOrdering`
- **Primary Key**: `TMDB_AlternateOrderingID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbShow` → `TMDB_Show` — N:1 via `TMDB_AlternateOrdering.TmdbShowID` FK
  - `TmdbNetwork` → `TMDB_Network` — N:1 via `TMDB_AlternateOrdering.TmdbNetworkID` FK (nullable)
  - `TmdbAlternateOrderingSeasons` → `TMDB_AlternateOrdering_Season` — 1:N via `TMDB_AlternateOrdering_Season.TmdbEpisodeGroupCollectionID` FK
  - `TmdbAlternateOrderingEpisodes` → `TMDB_AlternateOrdering_Episode` — 1:N via `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupCollectionID` FK
  - `Cast` → `TMDB_Show_Cast` — aggregated from alternate ordering episodes
  - `Crew` → `TMDB_Show_Crew` — aggregated from alternate ordering episodes
- **EF Core Relationships to Configure**:
  - `HasOne(ao => ao.TmdbShow).WithMany(s => s.TmdbAlternateOrdering).HasForeignKey(ao => ao.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(ao => ao.TmdbNetwork).WithMany().HasForeignKey(ao => ao.TmdbNetworkID).OnDelete(DeleteBehavior.Restrict)` — N:1, optional
  - `HasMany(ao => ao.TmdbAlternateOrderingSeasons).WithOne(aos => aos.TmdbAlternateOrdering).HasForeignKey(aos => aos.TmdbEpisodeGroupCollectionID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(ao => ao.TmdbAlternateOrderingEpisodes).WithOne(aoe => aoe.TmdbAlternateOrdering).HasForeignKey(aoe => aoe.TmdbEpisodeGroupCollectionID).OnDelete(DeleteBehavior.Cascade)` — 1:N
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - `TMDB_Base<string>` base class — uses `TmdbEpisodeGroupCollectionID` (string) as logical identity, but `TMDB_AlternateOrderingID` (int) is DB PK
  - **EF Core Risk**: Must map both `TMDB_AlternateOrderingID` as PK and `TmdbEpisodeGroupCollectionID` as unique index
  - `Type` uses `AlternateOrderingType` enum — needs ValueConverter

### 15. TMDB_AlternateOrdering_Season (Optional) (TMDB_AlternateOrdering_SeasonMap.cs)

- **Table**: `TMDB_AlternateOrdering_Season`
- **Primary Key**: `TMDB_AlternateOrdering_SeasonID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbShow` → `TMDB_Show` — N:1 via `TMDB_AlternateOrdering_Season.TmdbShowID` FK
  - `TmdbAlternateOrdering` → `TMDB_AlternateOrdering` — N:1 via `TMDB_AlternateOrdering_Season.TmdbEpisodeGroupCollectionID` FK
  - `TmdbAlternateOrderingEpisodes` → `TMDB_AlternateOrdering_Episode` — 1:N via `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupID` FK
  - `Cast` → `TMDB_Season_Cast` — aggregated from alternate ordering episodes
  - `Crew` → `TMDB_Season_Crew` — aggregated from alternate ordering episodes
- **EF Core Relationships to Configure**:
  - `HasOne(aos => aos.TmdbShow).WithMany().HasForeignKey(aos => aos.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(aos => aos.TmdbAlternateOrdering).WithMany(ao => ao.TmdbAlternateOrderingSeasons).HasForeignKey(aos => aos.TmdbEpisodeGroupCollectionID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasMany(aos => aos.TmdbAlternateOrderingEpisodes).WithOne(aoe => aoe.TmdbAlternateOrderingSeason).HasForeignKey(aoe => aoe.TmdbEpisodeGroupID).OnDelete(DeleteBehavior.Cascade)` — 1:N
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - `TMDB_Base<string>` base class — uses `TmdbEpisodeGroupID` (string) as logical identity, but `TMDB_AlternateOrdering_SeasonID` (int) is DB PK
  - **EF Core Risk**: Composite key of `(TmdbEpisodeGroupCollectionID, TmdbEpisodeGroupID)` for uniqueness
  - `IsLocked` is a boolean flag

### 16. TMDB_AlternateOrdering_Episode (Optional) (TMDB_AlternateOrdering_EpisodeMap.cs)

- **Table**: `TMDB_AlternateOrdering_Episode`
- **Primary Key**: `TMDB_AlternateOrdering_EpisodeID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `TmdbShow` → `TMDB_Show` — N:1 via `TMDB_AlternateOrdering_Episode.TmdbShowID` FK
  - `TmdbAlternateOrdering` → `TMDB_AlternateOrdering` — N:1 via `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupCollectionID` FK
  - `TmdbAlternateOrderingSeason` → `TMDB_AlternateOrdering_Season` — N:1 via `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupID` FK
  - `TmdbEpisode` → `TMDB_Episode` — N:1 via `TMDB_AlternateOrdering_Episode.TmdbEpisodeID` FK
- **EF Core Relationships to Configure**:
  - `HasOne(aoe => aoe.TmdbShow).WithMany().HasForeignKey(aoe => aoe.TmdbShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(aoe => aoe.TmdbAlternateOrdering).WithMany(ao => ao.TmdbAlternateOrderingEpisodes).HasForeignKey(aoe => aoe.TmdbEpisodeGroupCollectionID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(aoe => aoe.TmdbAlternateOrderingSeason).WithMany(aos => aos.TmdbAlternateOrderingEpisodes).HasForeignKey(aoe => aoe.TmdbEpisodeGroupID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(aoe => aoe.TmdbEpisode).WithMany(e => e.TmdbAlternateOrderingEpisodes).HasForeignKey(aoe => aoe.TmdbEpisodeID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Located in `TMDB/Optional/` subdirectory — optional TMDB folder mapping
  - `TMDB_Base<string>` base class — uses `Id = $"{TmdbEpisodeGroupID}:{TmdbEpisodeID}"` as logical identity, but `TMDB_AlternateOrdering_EpisodeID` (int) is DB PK
  - **EF Core Risk**: Composite key of `(TmdbEpisodeGroupCollectionID, TmdbEpisodeGroupID, TmdbEpisodeID)` for uniqueness
  - Hierarchy: `TMDB_Show` (1) ──< `TMDB_AlternateOrdering` (1) ──< `TMDB_AlternateOrdering_Season` (N) ──< `TMDB_AlternateOrdering_Episode` (N)

### 17. TMDB_Movie_Cast (TMDB_Movie_CastMap.cs)

- **Table**: `TMDB_Movie_Cast`
- **Primary Key**: `TMDB_Movie_CastID` (Identity)
- **Base Class**: `TMDB_Cast` (abstract base with `TmdbPersonID`, `TmdbCreditID`, `CharacterName`, `Ordering`)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**:
  - `GetTmdbMovie()` → `TMDB_Movie` — N:1 via `TMDB_Movie_Cast.TmdbMovieID` FK
  - `GetTmdbPerson()` → `TMDB_Person` — N:1 via `TMDB_Movie_Cast.TmdbPersonID` FK (inherited from TMDB_Cast)
  - `ParentOfType` → `IMovie` (ICast<IMovie> interface)
- **EF Core Relationships to Configure**:
  - `HasOne(mc => mc.TmdbMovie).WithMany(m => m.Cast).HasForeignKey(mc => mc.TmdbMovieID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(mc => mc.TmdbPerson).WithMany(p => p.MovieCastRoles).HasForeignKey(mc => mc.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Inherits from abstract `TMDB_Cast` base class — EF Core must use TPH (Table-Per-Hierarchy) or TPT (Table-Per-Type) inheritance mapping
  - `TMDB_Cast` is abstract — cannot be instantiated directly
  - Consider TPH with `TMDB_Episode_Cast` (both share `TmdbPersonID`, `TmdbCreditID`, `CharacterName`, `Ordering`)

### 18. TMDB_Movie_Crew (TMDB_Movie_CrewMap.cs)

- **Table**: `TMDB_Movie_Crew`
- **Primary Key**: `TMDB_Movie_CrewID` (Identity)
- **Base Class**: `TMDB_Crew` (abstract base with `TmdbPersonID`, `TmdbCreditID`, `Job`, `Department`)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**:
  - `GetTmdbMovie()` → `TMDB_Movie` — N:1 via `TMDB_Movie_Crew.TmdbMovieID` FK
  - `GetTmdbPerson()` → `TMDB_Person` — N:1 via `TMDB_Movie_Crew.TmdbPersonID` FK (inherited from TMDB_Crew)
  - `ParentOfType` → `IMovie` (ICrew<IMovie> interface)
- **EF Core Relationships to Configure**:
  - `HasOne(mc => mc.TmdbMovie).WithMany(m => m.Crew).HasForeignKey(mc => mc.TmdbMovieID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(mc => mc.TmdbPerson).WithMany(p => p.MovieCrewRoles).HasForeignKey(mc => mc.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Inherits from abstract `TMDB_Crew` base class — EF Core must use TPH or TPT inheritance mapping
  - `TMDB_Crew` is abstract — cannot be instantiated directly
  - Consider TPH with `TMDB_Episode_Crew` (both share `TmdbPersonID`, `TmdbCreditID`, `Job`, `Department`)

### 19. TMDB_Episode_Cast (TMDB_Episode_CastMap.cs)

- **Table**: `TMDB_Episode_Cast`
- **Primary Key**: `TMDB_Episode_CastID` (Identity)
- **Base Class**: `TMDB_Cast` (abstract base with `TmdbPersonID`, `TmdbCreditID`, `CharacterName`, `Ordering`)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**:
  - `GetTmdbEpisode()` → `TMDB_Episode` — N:1 via `TMDB_Episode_Cast.TmdbEpisodeID` FK
  - `GetTmdbPerson()` → `TMDB_Person` — N:1 via `TMDB_Episode_Cast.TmdbPersonID` FK (inherited from TMDB_Cast)
  - `ParentOfType` → `IEpisode` (ICast<IEpisode> interface)
- **EF Core Relationships to Configure**:
  - `HasOne(ec => ec.TmdbEpisode).WithMany(e => e.Cast).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(ec => ec.TmdbPerson).WithMany(p => p.EpisodeCastRoles).HasForeignKey(ec => ec.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Inherits from abstract `TMDB_Cast` base class — TPH with `TMDB_Movie_Cast`
  - Additional FK columns: `TmdbShowID`, `TmdbSeasonID` (triple FK: show + season + episode)
  - `IsGuestRole` is a boolean flag (not in base class)

### 20. TMDB_Episode_Crew (TMDB_Episode_CrewMap.cs)

- **Table**: `TMDB_Episode_Crew`
- **Primary Key**: `TMDB_Episode_CrewID` (Identity)
- **Base Class**: `TMDB_Crew` (abstract base with `TmdbPersonID`, `TmdbCreditID`, `Job`, `Department`)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**:
  - `GetTmdbEpisode()` → `TMDB_Episode` — N:1 via `TMDB_Episode_Crew.TmdbEpisodeID` FK
  - `GetTmdbPerson()` → `TMDB_Person` — N:1 via `TMDB_Episode_Crew.TmdbPersonID` FK (inherited from TMDB_Crew)
  - `ParentOfType` → `IEpisode` (ICrew<IEpisode> interface)
- **EF Core Relationships to Configure**:
  - `HasOne(ec => ec.TmdbEpisode).WithMany(e => e.Crew).HasForeignKey(ec => ec.TmdbEpisodeID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(ec => ec.TmdbPerson).WithMany(p => p.EpisodeCrewRoles).HasForeignKey(ec => ec.TmdbPersonID).OnDelete(DeleteBehavior.Restrict)` — N:1
- **EF Core Notes**:
  - Inherits from abstract `TMDB_Crew` base class — TPH with `TMDB_Movie_Crew`
  - Additional FK columns: `TmdbShowID`, `TmdbSeasonID` (triple FK: show + season + episode)

### 21. TMDB_Title (Text/TMDB_TitleMap.cs)

- **Table**: `TMDB_Title`
- **Primary Key**: `TMDB_TitleID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**: None (pure data class, no navigation methods)
- **EF Core Relationships to Configure**:
  - **Polymorphic parent reference**: `ParentID` + `ParentType` (ForeignEntityType enum) references Show, Season, Episode, Movie, Collection
  - `HasMany<Tmdb_Title>(parent).WithOne().HasForeignKey(t => t.ParentID).OnDelete(DeleteBehavior.Cascade)` — polymorphic via ParentType discriminator
- **EF Core Notes**:
  - **Polymorphic text table**: `ParentID` + `ParentType` is a generic parent reference pattern
  - `ParentType` uses `ForeignEntityType` enum — needs ValueConverter
  - Same pattern as `TMDB_Overview`
  - **EF Core Risk**: Cannot use a single polymorphic navigation. Must use TPH hierarchy with discriminator on `ParentType` or separate owned entities per parent type.

### 22. TMDB_Overview (Text/TMDB_OverviewMap.cs)

- **Table**: `TMDB_Overview`
- **Primary Key**: `TMDB_OverviewID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties**: None (pure data class, no navigation methods)
- **EF Core Relationships to Configure**:
  - **Polymorphic parent reference**: `ParentID` + `ParentType` (ForeignEntityType enum) references Show, Season, Episode, Movie, Collection, Person
  - `HasMany<Tmdb_Overview>(parent).WithOne().HasForeignKey(o => o.ParentID).OnDelete(DeleteBehavior.Cascade)` — polymorphic via ParentType discriminator
- **EF Core Notes**:
  - **Polymorphic text table**: `ParentID` + `ParentType` is a generic parent reference pattern
  - `ParentType` uses `ForeignEntityType` enum — needs ValueConverter
  - Same pattern as `TMDB_Title`
  - **EF Core Risk**: Cannot use a single polymorphic navigation. Must use TPH hierarchy with discriminator on `ParentType` or separate owned entities per parent type.

---

### Relationship Summary — TMDB Entities

| Source Entity | Target Entity | Relationship Type | FK Column(s) | Required/Optional | Cascade | Notes |
|---------------|---------------|-------------------|--------------|-------------------|---------|-------|
| **TMDB_Show** | **TMDB_Season** | 1:N | `TMDB_Season.TmdbShowID` | Required | Cascade | |
| **TMDB_Show** | **TMDB_Episode** | 1:N | `TMDB_Episode.TmdbShowID` | Required | Cascade | |
| **TMDB_Show** | **TMDB_Company_Entity** | 1:N (polymorphic) | `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Show` | Optional | Restrict | Polymorphic via ForeignEntityType |
| **TMDB_Show** | **TMDB_Show_Network** | 1:N | `TMDB_Show_Network.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_Show** | **TMDB_AlternateOrdering** | 1:N | `TMDB_AlternateOrdering.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_Show** | **TMDB_Title** | 1:N (polymorphic) | `TMDB_Title.ParentID` + `ParentType=Show` | Optional | Cascade | Polymorphic text table |
| **TMDB_Show** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Show` | Optional | Cascade | Polymorphic text table |
| **TMDB_Show** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Show` | Optional | Restrict | Polymorphic image table |
| **TMDB_Show** | **CrossRef_AniDB_TMDB_Show** | 1:N | `CrossRef_AniDB_TMDB_Show.TmdbShowID` | Optional | Restrict | Cross-reference to AniDB |
| **TMDB_Show** | **TMDB_Episode_Cast** | 1:N (aggregated) | `TMDB_Episode_Cast.TmdbEpisodeID` | Optional | Restrict | Aggregated from episodes |
| **TMDB_Show** | **TMDB_Episode_Crew** | 1:N (aggregated) | `TMDB_Episode_Crew.TmdbEpisodeID` | Optional | Restrict | Aggregated from episodes |
| **TMDB_Season** | **TMDB_Episode** | 1:N | `TMDB_Episode.TmdbSeasonID` | Required | Cascade | |
| **TMDB_Season** | **TMDB_Title** | 1:N (polymorphic) | `TMDB_Title.ParentID` + `ParentType=Season` | Optional | Cascade | Polymorphic text table |
| **TMDB_Season** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Season` | Optional | Cascade | Polymorphic text table |
| **TMDB_Season** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Season` | Optional | Restrict | Polymorphic image table |
| **TMDB_Episode** | **TMDB_Title** | 1:N (polymorphic) | `TMDB_Title.ParentID` + `ParentType=Episode` | Optional | Cascade | Polymorphic text table |
| **TMDB_Episode** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Episode` | Optional | Cascade | Polymorphic text table |
| **TMDB_Episode** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Episode` | Optional | Restrict | Polymorphic image table |
| **TMDB_Episode** | **TMDB_Episode_Cast** | 1:N | `TMDB_Episode_Cast.TmdbEpisodeID` | Required | Cascade | |
| **TMDB_Episode** | **TMDB_Episode_Crew** | 1:N | `TMDB_Episode_Crew.TmdbEpisodeID` | Required | Cascade | |
| **TMDB_Episode** | **TMDB_AlternateOrdering_Episode** | 1:N | `TMDB_AlternateOrdering_Episode.TmdbEpisodeID` | Optional | Restrict | Optional folder mapping |
| **TMDB_Episode** | **CrossRef_AniDB_TMDB_Episode** | 1:N | `CrossRef_AniDB_TMDB_Episode.AnidbEpisodeID` | Optional | Restrict | Cross-reference to AniDB |
| **TMDB_Movie** | **TMDB_Collection** | N:1 | `TMDB_Movie.TmdbCollectionID` | Optional | Restrict | Nullable FK |
| **TMDB_Movie** | **TMDB_Movie_Cast** | 1:N | `TMDB_Movie_Cast.TmdbMovieID` | Required | Cascade | |
| **TMDB_Movie** | **TMDB_Movie_Crew** | 1:N | `TMDB_Movie_Crew.TmdbMovieID` | Required | Cascade | |
| **TMDB_Movie** | **TMDB_Company_Entity** | 1:N (polymorphic) | `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Movie` | Optional | Restrict | Polymorphic via ForeignEntityType |
| **TMDB_Movie** | **TMDB_Title** | 1:N (polymorphic) | `TMDB_Title.ParentID` + `ParentType=Movie` | Optional | Cascade | Polymorphic text table |
| **TMDB_Movie** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Movie` | Optional | Cascade | Polymorphic text table |
| **TMDB_Movie** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Movie` | Optional | Restrict | Polymorphic image table |
| **TMDB_Movie** | **CrossRef_AniDB_TMDB_Movie** | 1:N | `CrossRef_AniDB_TMDB_Movie.TmdbMovieID` | Optional | Restrict | Cross-reference to AniDB |
| **TMDB_Movie** | **TMDB_Collection_Movie** | 1:N | `TMDB_Collection_Movie.TmdbMovieID` | Required | Restrict | Join table (optional folder) |
| **TMDB_Collection** | **TMDB_Collection_Movie** | 1:N | `TMDB_Collection_Movie.TmdbCollectionID` | Required | Cascade | Join table (optional folder) |
| **TMDB_Collection** | **TMDB_Title** | 1:N (polymorphic) | `TMDB_Title.ParentID` + `ParentType=Collection` | Optional | Cascade | Polymorphic text table |
| **TMDB_Collection** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Collection` | Optional | Cascade | Polymorphic text table |
| **TMDB_Collection** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Collection` | Optional | Restrict | Polymorphic image table |
| **TMDB_Person** | **TMDB_Overview** | 1:N (polymorphic) | `TMDB_Overview.ParentID` + `ParentType=Person` | Optional | Cascade | Polymorphic text table (biographies) |
| **TMDB_Person** | **TMDB_Movie_Cast** | 1:N | `TMDB_Movie_Cast.TmdbPersonID` | Required | Restrict | |
| **TMDB_Person** | **TMDB_Episode_Cast** | 1:N | `TMDB_Episode_Cast.TmdbPersonID` | Required | Restrict | |
| **TMDB_Person** | **TMDB_Movie_Crew** | 1:N | `TMDB_Movie_Crew.TmdbPersonID` | Required | Restrict | |
| **TMDB_Person** | **TMDB_Episode_Crew** | 1:N | `TMDB_Episode_Crew.TmdbPersonID` | Required | Restrict | |
| **TMDB_Company** | **TMDB_Company_Entity** | 1:N | `TMDB_Company_Entity.TmdbCompanyID` | Required | Restrict | |
| **TMDB_Company** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Company` | Optional | Restrict | Polymorphic image table |
| **TMDB_Company_Entity** | **TMDB_Company** | N:1 | `TMDB_Company_Entity.TmdbCompanyID` | Required | Restrict | |
| **TMDB_Company_Entity** | **TMDB_Show** | N:1 (polymorphic) | `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Show` | Optional | Restrict | Polymorphic |
| **TMDB_Company_Entity** | **TMDB_Movie** | N:1 (polymorphic) | `TMDB_Company_Entity.TmdbEntityID` + `ParentType=Movie` | Optional | Restrict | Polymorphic |
| **TMDB_Network** | **TMDB_Show_Network** | 1:N | `TMDB_Show_Network.TmdbNetworkID` | Required | Cascade | Optional folder mapping |
| **TMDB_Network** | **TMDB_Image_Entity** | 1:N (polymorphic) | `TMDB_Image_Entity.TmdbEntityID` + `TmdbEntityType=Network` | Optional | Restrict | Polymorphic image table |
| **TMDB_Show_Network** | **TMDB_Show** | N:1 | `TMDB_Show_Network.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_Show_Network** | **TMDB_Network** | N:1 | `TMDB_Show_Network.TmdbNetworkID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering** | **TMDB_Show** | N:1 | `TMDB_AlternateOrdering.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering** | **TMDB_Network** | N:1 | `TMDB_AlternateOrdering.TmdbNetworkID` | Optional | Restrict | Optional folder mapping |
| **TMDB_AlternateOrdering** | **TMDB_AlternateOrdering_Season** | 1:N | `TMDB_AlternateOrdering_Season.TmdbEpisodeGroupCollectionID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering** | **TMDB_AlternateOrdering_Episode** | 1:N | `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupCollectionID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Season** | **TMDB_Show** | N:1 | `TMDB_AlternateOrdering_Season.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Season** | **TMDB_AlternateOrdering** | N:1 | `TMDB_AlternateOrdering_Season.TmdbEpisodeGroupCollectionID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Season** | **TMDB_AlternateOrdering_Episode** | 1:N | `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Episode** | **TMDB_Show** | N:1 | `TMDB_AlternateOrdering_Episode.TmdbShowID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Episode** | **TMDB_AlternateOrdering** | N:1 | `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupCollectionID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Episode** | **TMDB_AlternateOrdering_Season** | N:1 | `TMDB_AlternateOrdering_Episode.TmdbEpisodeGroupID` | Required | Cascade | Optional folder mapping |
| **TMDB_AlternateOrdering_Episode** | **TMDB_Episode** | N:1 | `TMDB_AlternateOrdering_Episode.TmdbEpisodeID` | Required | Restrict | Optional folder mapping |
| **TMDB_Movie_Cast** | **TMDB_Movie** | N:1 | `TMDB_Movie_Cast.TmdbMovieID` | Required | Cascade | Cast/crew join entity |
| **TMDB_Movie_Cast** | **TMDB_Person** | N:1 | `TMDB_Movie_Cast.TmdbPersonID` | Required | Restrict | Cast/crew join entity |
| **TMDB_Movie_Crew** | **TMDB_Movie** | N:1 | `TMDB_Movie_Crew.TmdbMovieID` | Required | Cascade | Cast/crew join entity |
| **TMDB_Movie_Crew** | **TMDB_Person** | N:1 | `TMDB_Movie_Crew.TmdbPersonID` | Required | Restrict | Cast/crew join entity |
| **TMDB_Episode_Cast** | **TMDB_Episode** | N:1 | `TMDB_Episode_Cast.TmdbEpisodeID` | Required | Cascade | Cast/crew join entity |
| **TMDB_Episode_Cast** | **TMDB_Person** | N:1 | `TMDB_Episode_Cast.TmdbPersonID` | Required | Restrict | Cast/crew join entity |
| **TMDB_Episode_Crew** | **TMDB_Episode** | N:1 | `TMDB_Episode_Crew.TmdbEpisodeID` | Required | Cascade | Cast/crew join entity |
| **TMDB_Episode_Crew** | **TMDB_Person** | N:1 | `TMDB_Episode_Crew.TmdbPersonID` | Required | Restrict | Cast/crew join entity |

### EF Core Parity Risks

1. **No relationships in any TMDB mapping**: All 22 TMDB mappings have zero `References`, `HasMany`, or `ManyToMany` declarations. Every single relationship must be explicitly configured in EF Core entity configurations. This is a significant migration effort.

2. **Polymorphic parent references (HIGH RISK)**: `TMDB_Title`, `TMDB_Overview`, `TMDB_Image_Entity`, and `TMDB_Company_Entity` all use a polymorphic parent reference pattern: `ParentID` (int) + `ParentType` (ForeignEntityType enum). EF Core cannot directly map a single FK column to multiple entity types. Recommended approaches:
   - **TPH (Table-Per-Hierarchy)**: Single table with discriminator column on `ParentType` — but this requires separate tables per parent type since they reference different entities.
   - **Separate owned entities**: Create owned types per parent entity type (e.g., `ShowTitles`, `EpisodeTitles`, etc.) — verbose but type-safe.
   - **Repository pattern**: Keep using repository LINQ queries for polymorphic lookups (same as NHibernate) — simplest migration path.

3. **Polymorphic image references (HIGH RISK)**: `TMDB_Image_Entity` references 8 different entity types (Movie, Episode, Season, Show, Collection, Network, Company, Person) via `TmdbEntityID` + `TmdbEntityType` discriminator. Same polymorphic challenge as #2 but with more target types. `TMDB_Image` is referenced via `RemoteFileName` string lookup, not FK.

4. **Abstract base classes for cast/crew (MEDIUM RISK)**: `TMDB_Movie_Cast` and `TMDB_Episode_Cast` inherit from abstract `TMDB_Cast`. `TMDB_Movie_Crew` and `TMDB_Episode_Crew` inherit from abstract `TMDB_Crew`. EF Core must use TPH (Table-Per-Hierarchy) or TPT (Table-Per-Type) inheritance mapping. TPH is recommended since both cast types share the same columns (`TmdbPersonID`, `TmdbCreditID`, `CharacterName`, `Ordering`) and both crew types share the same columns (`TmdbPersonID`, `TmdbCreditID`, `Job`, `Department`).

5. **Optional TMDB folder mappings (MEDIUM RISK)**: Entities in `TMDB/Optional/` subdirectory (`TMDB_Collection`, `TMDB_Collection_Movie`, `TMDB_Network`, `TMDB_Show_Network`, `TMDB_AlternateOrdering`, `TMDB_AlternateOrdering_Season`, `TMDB_AlternateOrdering_Episode`) are optional and may not exist in all databases. EF Core must handle nullable FKs gracefully (`TMDB_Movie.TmdbCollectionID` is nullable).

6. **Alternate ordering join semantics (HIGH RISK)**: `TMDB_AlternateOrdering`, `TMDB_AlternateOrdering_Season`, and `TMDB_AlternateOrdering_Episode` use `TMDB_Base<string>` base class where the `Id` property is a string (not the int DB PK). EF Core must:
   - Map `TMDB_AlternateOrderingID` (int) as the PK
   - Add unique index on `TmdbEpisodeGroupCollectionID` (string)
   - Map composite key `(TmdbEpisodeGroupCollectionID, TmdbEpisodeGroupID)` for `TMDB_AlternateOrdering_Season`
   - Map composite key `(TmdbEpisodeGroupCollectionID, TmdbEpisodeGroupID, TmdbEpisodeID)` for `TMDB_AlternateOrdering_Episode`

7. **Navigation properties in models but not in mappings**: 19 of 22 TMDB models have navigation properties that are NOT mapped by NHibernate. These are populated by repository methods. EF Core must explicitly configure all relationships to enable `Include()`/`ThenInclude()` loading.

8. **Repository pattern as relationship resolver**: All TMDB relationships are currently resolved via repository LINQ queries (e.g., `TMDB_ShowRepository.GetByTmdbShowID(showID)`). EF Core configurations should enable similar queries via `Include()`/`ThenInclude()` or explicit LINQ projections.

9. **Custom types used in TMDB (9 distinct types)**:
   - `StringListConverter` — `Genres`, `Keywords`, `Aliases` (JSON-serialized `List<string>`)
   - `TmdbContentRatingConverter` — `ContentRatings` (pipe-delimited `List<TMDB_ContentRating>`)
   - `TmdbProductionCountryConverter` — `ProductionCountries` (pipe-delimited `List<TMDB_ProductionCountry>`)
   - `DateOnlyConverter` — `FirstAiredAt`, `LastAiredAt`, `ReleasedAt`, `BirthDay`, `DeathDay`, `AiredAt`
   - `TitleLanguageConverter` — `TMDB_Image.Language`
   - `ForeignEntityType` — `TMDB_Title.ParentType`, `TMDB_Overview.ParentType`, `TMDB_Image_Entity.TmdbEntityType`, `TMDB_Company_Entity.TmdbEntityType`
   - `ImageEntityType` — `TMDB_Image_Entity.ImageType`
   - `PersonGender` — `TMDB_Person.Gender`
   - `AlternateOrderingType` — `TMDB_AlternateOrdering.Type`

10. **Image_Base unmapped properties**: `TMDB_Image` inherits `Image_Base` which has properties (`Source`, `ImageType`, `IsPreferred`, `RemoteURL`, `LocalPath`) that are NOT mapped to DB. Only 7 properties from `Image_Base` are actually persisted: `IsEnabled`, `Width`, `Height`, `Language`, `RemoteFileName`, `UserRating`, `UserVotes`.

11. **Explicit table names**: All 22 TMDB entities explicitly call `Table("...")` in their mapping files — no implicit table names to worry about.

12. **Custom column names**: `RuntimeMinutes` maps to column `Runtime` in both `TMDB_Episode` and `TMDB_Movie`.

---

## T009 partial — Trakt relationships

**Task**: T009 — Document relationship mapping from FluentNHibernate mappings  
**Scope**: 3 Trakt entities (`Shoko.Server/Mappings/Trakt_*.cs`)  
**Source**: 3 mapping files + 3 model files (`Shoko.Server/Models/Trakt/*.cs`)  
**Generated**: 2026-05-07

**Key observation**: None of the 3 Trakt mapping files define `References`, `HasMany`, `HasManyToMany`, or `ManyToMany` relationships. All relationships are resolved via the repository pattern.

### 1. Trakt_Show (Trakt_ShowMap.cs)

- **Table**: `Trakt_Show` (implicit — no `Table()` call, class name used)
- **Primary Key**: `Trakt_ShowID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `Trakt_Seasons` → `Trakt_Season` — 1:N via `Trakt_Season.Trakt_ShowID` FK
  - `Trakt_Episodes` → `Trakt_Episode` — 1:N via `Trakt_Episode.Trakt_ShowID` FK
- **EF Core Relationships to Configure**:
  - `HasMany(s => s.Trakt_Seasons).WithOne(se => se.Trakt_Show).HasForeignKey(se => se.Trakt_ShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
  - `HasMany(s => s.Trakt_Episodes).WithOne(e => e.Trakt_Show).HasForeignKey(e => e.Trakt_ShowID).OnDelete(DeleteBehavior.Cascade)` — 1:N
- **EF Core Notes**:
  - Implicit table name — must use explicit `ToTable("Trakt_Show")`
  - `Overview` uses `StringClob` (nvarchar(max))

### 2. Trakt_Season (Trakt_SeasonMap.cs)

- **Table**: `Trakt_Season` (implicit — no `Table()` call, class name used)
- **Primary Key**: `Trakt_SeasonID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `Trakt_Show` → `Trakt_Show` — N:1 via `Trakt_Season.Trakt_ShowID` FK
  - `Trakt_Episodes` → `Trakt_Episode` — 1:N via `Trakt_Episode.Season` FK (not `Trakt_SeasonID`)
- **EF Core Relationships to Configure**:
  - `HasOne(se => se.Trakt_Show).WithMany(s => s.Trakt_Seasons).HasForeignKey(se => se.Trakt_ShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasMany(se => se.Trakt_Episodes).WithOne(e => e.Trakt_Season).HasForeignKey(e => e.Season).OnDelete(DeleteBehavior.Cascade)` — 1:N (note: FK is `Season` int, not `Trakt_SeasonID`)
- **EF Core Notes**:
  - Implicit table name — must use explicit `ToTable("Trakt_Season")`
  - Episode FK uses `Season` (int) not `Trakt_SeasonID` — unusual mapping

### 3. Trakt_Episode (Trakt_EpisodeMap.cs)

- **Table**: `Trakt_Episode` (implicit — no `Table()` call, class name used)
- **Primary Key**: `Trakt_EpisodeID` (Identity)
- **NHibernate Relationships**: None defined in mapping
- **Model Navigation Properties** (not mapped by NHibernate):
  - `Trakt_Show` → `Trakt_Show` — N:1 via `Trakt_Episode.Trakt_ShowID` FK
  - `Trakt_Season` → `Trakt_Season` — N:1 via `Trakt_Episode.Season` FK (not `Trakt_SeasonID`)
- **EF Core Relationships to Configure**:
  - `HasOne(e => e.Trakt_Show).WithMany(s => s.Trakt_Episodes).HasForeignKey(e => e.Trakt_ShowID).OnDelete(DeleteBehavior.Cascade)` — N:1
  - `HasOne(e => e.Trakt_Season).WithMany(se => se.Trakt_Episodes).HasForeignKey(e => e.Season).OnDelete(DeleteBehavior.Restrict)` — N:1 (note: FK is `Season` int, not `Trakt_SeasonID`)
- **EF Core Notes**:
  - Implicit table name — must use explicit `ToTable("Trakt_Episode")`
  - `Overview` uses `StringClob` (nvarchar(max))
  - Episode FK to Season uses `Season` (int) not `Trakt_SeasonID` — unusual mapping

---

### Relationship Summary — Trakt Entities

| Source Entity | Target Entity | Relationship Type | FK Column(s) | Required/Optional | Cascade | Notes |
|---------------|---------------|-------------------|--------------|-------------------|---------|-------|
| **Trakt_Show** | **Trakt_Season** | 1:N | `Trakt_Season.Trakt_ShowID` | Required | Cascade | |
| **Trakt_Show** | **Trakt_Episode** | 1:N | `Trakt_Episode.Trakt_ShowID` | Required | Cascade | |
| **Trakt_Season** | **Trakt_Show** | N:1 | `Trakt_Season.Trakt_ShowID` | Required | Cascade | Inverse of 1:N above |
| **Trakt_Season** | **Trakt_Episode** | 1:N | `Trakt_Episode.Season` | Required | Cascade | FK is `Season` int, not `Trakt_SeasonID` |
| **Trakt_Episode** | **Trakt_Show** | N:1 | `Trakt_Episode.Trakt_ShowID` | Required | Cascade | Inverse of 1:N above |
| **Trakt_Episode** | **Trakt_Season** | N:1 | `Trakt_Episode.Season` | Required | Restrict | FK is `Season` int, not `Trakt_SeasonID` |

### EF Core Parity Risks — Trakt

1. **No relationships in any Trakt mapping**: All 3 Trakt mappings have zero relationship declarations. Every FK relationship must be explicitly configured.
2. **Implicit table names**: All 3 entities use implicit table names (class name = table name). EF Core must use explicit `ToTable()` for clarity.
3. **Unusual FK mapping**: `Trakt_Episode.Season` is an int FK that references `Trakt_Season.Season` (not `Trakt_SeasonID`). This is a non-standard FK pattern that must be preserved.
4. **StringClob**: `Trakt_Episode.Overview` uses `StringClob` (nvarchar(max)).

---

## DatabaseCommand Schema Mutation Inventory (T010)

**Task**: T010 — Catalog schema-changing DatabaseCommand entries in DatabaseFixes.cs and provider files
**Generated**: 2026-05-07
**Location**: `Shoko.Server/Databases/`
**Scope**: SQLite.cs, MySQL.cs, SQLServer.cs

All schema-altering DatabaseCommand entries across SQLite, MySQL, and SQLServer providers.

## Summary
| Provider | File | _createVersionTable | _updateVersionTable | _createTables | _patchCommands | Max Patch Version | CodedCommands | Helper Functions |
|----------|------|---------------------|---------------------|---------------|----------------|-------------------|---------------|------------------|
| SQLite | `SQLite.cs` | 1 | 4 | 110 | 467 | 143 | 27 | 14 |
| MySQL | `MySQL.cs` | 2 | 5 | 108 | 553 | 161 | 27 | 2 |
| SQLServer | `SQLServer.cs` | 2 | 5 | 110 | 525 | 156 | 27 | 5 |

---

## SQLite
**Provider class**: `SQLite`
**File**: `Shoko.Server/Databases/SQLite.cs`

### Version Table Creation (`_createVersionTable`)
- **[0,1]** `NormalCommand`: CREATE TABLE Versions ( VersionsID INTEGER PRIMARY KEY AUTOINCREMENT, VersionType TEXT NOT NULL, VersionValue TEXT NOT N...

### Version Table Schema Updates (`_updateVersionTable`)
- **[0,2]** `NormalCommand`: ALTER TABLE Versions ADD VersionRevision TEXT NULL;
- **[0,3]** `NormalCommand`: ALTER TABLE Versions ADD VersionCommand TEXT NULL;
- **[0,4]** `NormalCommand`: ALTER TABLE Versions ADD VersionProgram TEXT NULL;
- **[0,5]** `NormalCommand`: CREATE INDEX IX_Versions_VersionType ON Versions(VersionType,VersionValue,VersionRevision);

### Initial Table Creation (`_createTables`)
**Total commands**: 110
**Version range**: [1, 110]

**Tables created**: 57
- [1,1] `AniDB_Anime`
- [1,3] `AniDB_Anime_Category`
- [1,6] `AniDB_Anime_Character`
- [1,9] `AniDB_Anime_Relation`
- [1,12] `AniDB_Anime_Review`
- [1,15] `AniDB_Anime_Similar`
- [1,18] `AniDB_Anime_Tag`
- [1,21] `AniDB_Anime_Title`
- [1,23] `AniDB_Category`
- [1,25] `AniDB_Character`
- [1,27] `AniDB_Character_Seiyuu`
- [1,31] `AniDB_Seiyuu`
- [1,33] `AniDB_Episode`
- [1,36] `AniDB_File`
- [1,40] `AniDB_GroupStatus`
- [1,43] `AniDB_ReleaseGroup`
- [1,45] `AniDB_Review`
- [1,47] `AniDB_Tag`
- [1,49] `AnimeEpisode`
- [1,52] `AnimeGroup`
- [1,53] `AnimeSeries`
- [1,55] `CommandRequest`
- [1,56] `CrossRef_AniDB_Other`
- [1,58] `CrossRef_AniDB_TvDB`
- [1,60] `CrossRef_File_Episode`
- [1,62] `CrossRef_Languages_AniDB_File`
- [1,63] `CrossRef_Subtitles_AniDB_File`
- [1,64] `FileNameHash`
- [1,66] `Language`
- [1,68] `ImportFolder`
- [1,69] `ScheduledUpdate`
- [1,71] `VideoInfo`
- [1,73] `VideoLocal`
- [1,75] `DuplicateFile`
- [1,76] `GroupFilter`
- [1,77] `GroupFilterCondition`
- [1,78] `AniDB_Vote`
- [1,79] `TvDB_ImageFanart`
- [1,81] `TvDB_ImageWideBanner`
- [1,83] `TvDB_ImagePoster`
- [1,85] `TvDB_Episode`
- [1,87] `TvDB_Series`
- [1,89] `AniDB_Anime_DefaultImage`
- [1,91] `MovieDB_Movie`
- [1,93] `MovieDB_Poster`
- [1,94] `MovieDB_Fanart`
- [1,95] `JMMUser`
- [1,96] `Trakt_Episode`
- [1,97] `Trakt_ImagePoster`
- [1,98] `Trakt_ImageFanart`
- [1,99] `Trakt_Show`
- [1,100] `Trakt_Season`
- [1,101] `CrossRef_AniDB_Trakt`
- [1,102] `AnimeEpisode_User`
- [1,105] `AnimeSeries_User`
- [1,107] `AnimeGroup_User`
- [1,109] `VideoLocal_User`

**Indexes created**: 14
- [1,4] `IX_AniDB_Anime_Category_AnimeID`
- [1,7] `IX_AniDB_Anime_Character_AnimeID`
- [1,10] `IX_AniDB_Anime_Relation_AnimeID`
- [1,13] `IX_AniDB_Anime_Review_AnimeID`
- [1,16] `IX_AniDB_Anime_Similar_AnimeID`
- [1,19] `IX_AniDB_Anime_Tag_AnimeID`
- [1,22] `IX_AniDB_Anime_Title_AnimeID`
- [1,28] `IX_AniDB_Character_Seiyuu_CharID`
- [1,29] `IX_AniDB_Character_Seiyuu_SeiyuuID`
- [1,34] `IX_AniDB_Episode_AnimeID`
- [1,39] `IX_AniDB_File_File_Source`
- [1,41] `IX_AniDB_GroupStatus_AnimeID`
- [1,51] `IX_AnimeEpisode_AnimeSeriesID`
- [1,104] `IX_AnimeEpisode_User_User_AnimeSeriesID`

### Patch Commands (`_patchCommands`)
**Total commands**: 467
- **NormalCommand**: 394
- **CodedCommand**: 27
- **DataMigration** (excluded from schema catalog): 46

#### Schema Changes by Version

**Version [2,1]** (1 commands):
  **CREATE TABLE:**
  - `IgnoreAnime`

**Version [2,2]** (1 commands):

**Version [3,1]** (1 commands):
  **CREATE TABLE:**
  - `Trakt_Friend`

**Version [3,2]** (1 commands):

**Version [4,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD DefaultAnimeSeriesID INTEGER NULL

**Version [5,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD CanEditServerSettings INTEGER NULL

**Version [6,3]** (1 commands):

**Version [6,4]** (1 commands):

**Version [6,5]** (1 commands):

**Version [6,6]** (1 commands):

**Version [7,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD VideoBitDepth TEXT NULL

**Version [9,1]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder ADD IsWatched INTEGER NOT NULL DEFAULT 1

**Version [10,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [10,2]** (1 commands):

**Version [10,3]** (1 commands):

**Version [11,1]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [11,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [11,3]** (1 commands):

**Version [11,4]** (1 commands):

**Version [12,1]** (1 commands):
  **CREATE TABLE:**
  - `Playlist`

**Version [13,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD SeriesNameOverride text

**Version [14,1]** (1 commands):
  **CREATE TABLE:**
  - `BookmarkedAnime`

**Version [14,2]** (1 commands):

**Version [15,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD DateTimeCreated DATETIME NULL

**Version [16,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [16,2]** (1 commands):

**Version [17,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_MylistStats`

**Version [18,1]** (1 commands):
  **CREATE TABLE:**
  - `FileFfdshowPreset`

**Version [18,2]** (1 commands):

**Version [19,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD DisableExternalLinksFlag INTEGER NULL

**Version [20,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD FileVersion INTEGER NULL

**Version [21,1]** (1 commands):
  **CREATE TABLE:**
  - `RenameScript`

**Version [22,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsCensored INTEGER NULL

**Version [22,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsDeprecated INTEGER NULL

**Version [22,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD InternalVersion INTEGER NULL

**Version [24,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD IsVariation INTEGER NULL

**Version [25,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Recommendation`

**Version [25,2]** (1 commands):

**Version [26,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_File_Episode_Hash`

**Version [26,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_File_Episode_EpisodeID`

**Version [28,1]** (1 commands):
  **CREATE TABLE:**
  - `LogMessage`

**Version [29,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDBV2`

**Version [29,2]** (1 commands):

**Version [30,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD Locked INTEGER NULL

**Version [31,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD FullInfo TEXT NULL

**Version [32,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TraktV2`

**Version [32,2]** (1 commands):

**Version [33,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_Trakt_Episode`

**Version [33,2]** (1 commands):

**Version [35,1]** (1 commands):
  **CREATE TABLE:**
  - `CustomTag`

**Version [35,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_CustomTag`

**Version [36,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD Weight INTEGER NULL

**Version [38,1]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Episode`: ALTER TABLE Trakt_Episode ADD TraktID INTEGER NULL

**Version [40,1]** (1 commands):
  **DROP TABLE:**
  - `LogMessage`

**Version [41,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DefaultFolder TEXT NULL

**Version [42,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexUsers TEXT NULL

**Version [43,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD FilterType INTEGER NOT NULL DEFAULT 1

**Version [44,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractBlob BLOB NULL

**Version [44,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,6]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD LatestEpisodeAirDate DATETIME NULL

**Version [44,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractBlob BLOB NULL

**Version [44,8]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,9]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractBlob BLOB NULL

**Version [44,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD LatestEpisodeAirDate DATETIME NULL

**Version [44,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractBlob BLOB NULL

**Version [44,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractBlob BLOB NULL

**Version [44,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,19]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupsIdsVersion INTEGER NOT NULL DEFAULT 0

**Version [44,20]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupsIdsString TEXT NULL

**Version [44,21]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupConditionsVersion INTEGER NOT NULL DEFAULT 0

**Version [44,22]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupConditions TEXT NULL

**Version [44,23]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD ParentGroupFilterID INTEGER NULL

**Version [44,24]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD InvisibleInClients INTEGER NOT NULL DEFAULT 0

**Version [44,25]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD SeriesIdsVersion INTEGER NOT NULL DEFAULT 0

**Version [44,26]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD SeriesIdsString TEXT NULL

**Version [44,27]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,28]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractBlob BLOB NULL

**Version [44,29]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,30]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractVersion INTEGER NOT NULL DEFAULT 0

**Version [44,31]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractBlob BLOB NULL

**Version [44,32]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractSize INTEGER NOT NULL DEFAULT 0

**Version [44,33]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaVersion INTEGER NOT NULL DEFAULT 0

**Version [44,34]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaBlob BLOB NULL

**Version [44,35]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaSize INTEGER NOT NULL DEFAULT 0

**Version [46,1]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_Place`

**Version [46,2]** (1 commands):

**Version [46,6]** (1 commands):
  **CREATE TABLE:**
  - `CloudAccount`

**Version [46,7]** (1 commands):

**Version [46,8]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder ADD CloudID INTEGER NULL

**Version [46,9]** (1 commands):
  **DROP TABLE:**
  - `VideoInfo`

**Version [47,1]** (1 commands):
  **DROP INDEX:**
  - `UIX2_VideoLocal_Hash`

**Version [47,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_VideoLocal_Hash`

**Version [48,1]** (1 commands):
  **CREATE TABLE:**
  - `AuthTokens`

**Version [49,1]** (1 commands):
  **CREATE TABLE:**
  - `Scan`

**Version [49,2]** (1 commands):
  **CREATE TABLE:**
  - `ScanFile`

**Version [49,3]** (1 commands):
  **CREATE INDEX:**
  - `UIX_ScanFileStatus`

**Version [53,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexToken TEXT NULL

**Version [54,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsChaptered INTEGER NOT NULL DEFAULT -1

**Version [55,1]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD RenamerType TEXT NOT NULL DEFAULT 'Legacy'

**Version [55,2]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD ExtraData TEXT

**Version [56,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Character_CharID`

**Version [57,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_TvDB_Episode_Id`

**Version [58,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD AirsOn TEXT NULL

**Version [59,1]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImageFanart`

**Version [59,2]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImagePoster`

**Version [60,1]** (1 commands):
  **CREATE TABLE:**
  - `AnimeCharacter`

**Version [60,2]** (1 commands):
  **CREATE TABLE:**
  - `AnimeStaff`

**Version [60,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_Anime_Staff`

**Version [61,1]** (1 commands):
  **ALTER TABLE:**
  - `MovieDB_Movie`: ALTER TABLE MovieDB_Movie ADD Rating INTEGER NOT NULL DEFAULT 0

**Version [61,2]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Series`: ALTER TABLE TvDB_Series ADD Rating INTEGER NULL

**Version [62,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode ADD Description TEXT NOT NULL DEFAULT ''

**Version [66,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_AnimeUpdate`

**Version [66,2]** (1 commands):

**Version [70,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_MAL_Anime`

**Version [70,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD Site_JP TEXT NULL

**Version [70,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD Site_EN TEXT NULL

**Version [70,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD Wikipedia_ID TEXT NULL

**Version [70,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD WikipediaJP_ID TEXT NULL

**Version [70,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD SyoboiID INTEGER NULL

**Version [70,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD AnisonID INTEGER NULL

**Version [70,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD CrunchyrollID TEXT NULL

**Version [71,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MyListID INTEGER NOT NULL DEFAULT 0

**Version [72,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_Title`

**Version [73,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_TvDB_Episode_AniDBEpisodeID`

**Version [73,3]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [73,4]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [73,5]** (1 commands):

**Version [73,6]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [73,7]** (1 commands):

**Version [76,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD UpdatedAt DATETIME NOT NULL DEFAULT '2000-01-01 00:00:00'

**Version [79,1]** (2 commands):
  **DROP INDEX:**
  - `IF`
  - `IF`

**Version [80,1]** (1 commands):
  **DROP INDEX:**
  - `IF`

**Version [81,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [83,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD WatchedCount INTEGER NOT NULL DEFAULT 0;

**Version [83,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD LastUpdated DATETIME NOT NULL DEFAULT '2000-01-01 00:00:00';

**Version [84,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD LastEpisodeUpdate DATETIME DEFAULT NULL;

**Version [85,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD MainAniDBAnimeID INTEGER DEFAULT NULL;

**Version [89,1]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Category`

**Version [89,2]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Review`

**Version [89,3]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Category`

**Version [89,4]** (1 commands):
  **DROP TABLE:**
  - `AniDB_MylistStats`

**Version [89,5]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Review`

**Version [89,6]** (1 commands):
  **DROP TABLE:**
  - `CloudAccount`

**Version [89,7]** (1 commands):
  **DROP TABLE:**
  - `FileFfdshowPreset`

**Version [89,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Trakt`

**Version [89,9]** (1 commands):
  **DROP TABLE:**
  - `Trakt_Friend`

**Version [89,10]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime RENAME COLUMN DisableExternalLinksFlag TO DisableExternalLinksFlag_old; ALTE...

**Version [89,11]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder RENAME COLUMN IsWatched TO IsWatched_old; ALTER TABLE ImportFolder ADD IsWa...

**Version [89,12]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal RENAME COLUMN IsVariation TO IsVariation_old; ALTER TABLE VideoLocal ADD IsVa...

**Version [89,13]** (1 commands):
  **CREATE INDEX:**
  - `UIX2_AniDB_Anime_AnimeID`

**Version [89,14]** (1 commands):
  **DROP INDEX:**
  - `IX_AniDB_File_File_Source`

**Version [89,15]** (1 commands):
  **DROP INDEX:**
  - `IX_CrossRef_File_Episode_EpisodeID`

**Version [89,16]** (1 commands):
  **DROP INDEX:**
  - `IX_CrossRef_File_Episode_Hash`

**Version [89,17]** (1 commands):
  **CREATE INDEX:**
  - `UIX2_VideoLocal_Hash`

**Version [89,18]** (1 commands):
  **CREATE INDEX:**
  - `UIX2_VideoLocal_User_User_VideoLocalID`

**Version [89,19]** (1 commands):

**Version [91,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Episode_EpisodeType`

**Version [92,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD DateTimeImported DATETIME DEFAULT NULL;

**Version [93,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD Verified integer NOT NULL DEFAULT 0;

**Version [93,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD ParentTagID integer DEFAULT NULL;

**Version [93,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD TagNameOverride TEXT DEFAULT NULL;

**Version [93,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD LastUpdated DATETIME NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [93,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN Spoiler;

**Version [93,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN LocalSpoiler;

**Version [93,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN TagCount;

**Version [93,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD LocalSpoiler integer NOT NULL DEFAULT 0;

**Version [93,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag DROP COLUMN Approval;

**Version [94,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD IsHidden integer NOT NULL DEFAULT 0;

**Version [94,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD HiddenUnwatchedEpisodeCount integer NOT NULL DEFAULT 0;

**Version [96,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_FileUpdate`

**Version [96,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_FileUpdate`

**Version [97,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN DisableExternalLinksFlag;

**Version [97,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DisableAutoMatchFlags integer NOT NULL DEFAULT 0;

**Version [97,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD VNDBID INTEGER NULL

**Version [97,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD BangumiID INTEGER NULL

**Version [97,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD LianID INTEGER NULL

**Version [97,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD FunimationID TEXT NULL

**Version [97,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD HiDiveID TEXT NULL

**Version [98,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN LianID;

**Version [98,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimePlanetID;

**Version [98,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimeNfo;

**Version [98,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD LainID INTEGER NULL

**Version [100,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCount integer NOT NULL DEFAULT 0;

**Version [100,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCountGroups integer NOT NULL DEFAULT 0;

**Version [102,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageBlob BLOB NULL;

**Version [102,2]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageMetadata VARCHAR(128) NULL;

**Version [103,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumped DATETIME;

**Version [103,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumpVersion TEXT;

**Version [105,1]** (1 commands):
  **CREATE TABLE:**
  - `FilterPreset`

**Version [105,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_FilterPreset_ParentFilterPresetID`

**Version [106,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup DROP COLUMN SortName;

**Version [107,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode DROP COLUMN PlexContractVersion;ALTER TABLE AnimeEpisode DROP COLUMN PlexCo...

**Version [108,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CommandRequest_CommandType`

**Version [109,1]** (1 commands):
  **DROP TABLE:**
  - `CommandRequest`

**Version [110,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD EpisodeNameOverride text

**Version [112,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN ContractVersion;ALTER TABLE AniDB_Anime DROP COLUMN ContractBlob...

**Version [112,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries DROP COLUMN ContractVersion;ALTER TABLE AnimeSeries DROP COLUMN ContractBlob...

**Version [112,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup DROP COLUMN ContractVersion;ALTER TABLE AnimeGroup DROP COLUMN ContractBlob;A...

**Version [113,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN MediaSize;

**Version [114,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_NotifyQueue`

**Version [114,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Message`

**Version [115,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Episode`

**Version [115,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`

**Version [115,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Show`

**Version [115,4]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image`

**Version [115,5]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_PreferredImage`

**Version [115,6]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Title`

**Version [115,7]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Overview`

**Version [115,8]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company`

**Version [115,9]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Network`

**Version [115,10]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Person`

**Version [115,11]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie`

**Version [115,12]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Cast`

**Version [115,13]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company_Entity`

**Version [115,14]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Crew`

**Version [115,15]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Show`

**Version [115,16]** (1 commands):
  **CREATE TABLE:**
  - `Tmdb_Show_Network`

**Version [115,17]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Season`

**Version [115,18]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode`

**Version [115,19]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Cast`

**Version [115,20]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Crew`

**Version [115,21]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering`

**Version [115,22]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Season`

**Version [115,23]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Episode`

**Version [115,24]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection`

**Version [115,25]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection_Movie`

**Version [115,27]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Other`

**Version [115,28]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Fanart`

**Version [115,29]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Movie`

**Version [115,30]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Poster`

**Version [115,31]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_DefaultImage`

**Version [115,32]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_PreferredImage`

**Version [117,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN TvdbShowID INTEGER NULL DEFAULT NULL;

**Version [117,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD COLUMN TvdbEpisodeID INTEGER NULL DEFAULT NULL;

**Version [117,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN ImdbMovieID INTEGER NULL DEFAULT NULL;

**Version [117,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie DROP COLUMN ImdbMovieID;

**Version [117,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN ImdbMovieID TEXT NULL DEFAULT NULL;

**Version [117,8]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Overview`

**Version [117,9]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Title`

**Version [117,10]** (1 commands):

**Version [117,11]** (1 commands):

**Version [118,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie RENAME AnidbEpisodeID TO AniDBEpisodeID_OLD; ALTER TABLE Cross...

**Version [119,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN PosterPath TEXT NULL DEFAULT NULL;

**Version [119,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN BackdropPath TEXT NULL DEFAULT NULL;

**Version [119,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN PosterPath TEXT NULL DEFAULT NULL;

**Version [119,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN BackdropPath TEXT NULL DEFAULT NULL;

**Version [121,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Creator`

**Version [121,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character_Creator`

**Version [121,3]** (1 commands):

**Version [121,4]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CreatorID`

**Version [121,5]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CharacterID`

**Version [121,6]** (1 commands):

**Version [121,9]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Seiyuu`

**Version [121,10]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Character_Seiyuu`

**Version [122,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN PreferredAlternateOrderingID TEXT NULL DEFAULT NULL;

**Version [123,1]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Episode`

**Version [123,2]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Series`

**Version [123,3]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageFanart`

**Version [123,4]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImagePoster`

**Version [123,5]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageWideBanner`

**Version [123,6]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [123,7]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [123,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`

**Version [123,9]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show DROP COLUMN TvDB_ID;

**Version [123,10]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show ADD COLUMN TmdbShowID INTEGER NULL;

**Version [125,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN Keywords TEXT NULL DEFAULT NULL;

**Version [125,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD COLUMN ProductionCountries TEXT NULL DEFAULT NULL;

**Version [125,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN Keywords TEXT NULL DEFAULT NULL;

**Version [125,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN ProductionCountries TEXT NULL DEFAULT NULL;

**Version [126,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Relation_RelatedAnimeID`

**Version [127,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbSeasonID`

**Version [127,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbShowID`

**Version [128,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD COLUMN IsHidden INTEGER NOT NULL DEFAULT 0;

**Version [128,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ADD COLUMN HiddenEpisodeCount INTEGER NOT NULL DEFAULT 0;

**Version [128,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD COLUMN HiddenEpisodeCount INTEGER NOT NULL DEFAULT 0;

**Version [128,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Season`: ALTER TABLE TMDB_AlternateOrdering_Season ADD COLUMN HiddenEpisodeCount INTEGER NOT NULL DEFAULT 0;

**Version [128,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering`: ALTER TABLE TMDB_AlternateOrdering ADD COLUMN HiddenEpisodeCount INTEGER NOT NULL DEFAULT 0;

**Version [129,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID`

**Version [129,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID_TmdbShowID`

**Version [129,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID`

**Version [129,4]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID_TmdbEpisodeID`

**Version [129,5]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbEpisodeID`

**Version [129,6]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbShowID`

**Version [129,7]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbAnimeID`

**Version [129,8]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID`

**Version [129,9]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID_TmdbMovieID`

**Version [129,10]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_TmdbMovieID`

**Version [129,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID`

**Version [129,12]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID_TmdbShowID`

**Version [129,13]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_TmdbShowID`

**Version [129,14]** (1 commands):

**Version [129,15]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbEpisodeGroupCollectionID`

**Version [129,16]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbShowID`

**Version [129,17]** (1 commands):

**Version [129,18]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbEpisodeGroupCollectionID_TmdbShowID`

**Version [129,19]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbShowID`

**Version [129,20]** (1 commands):

**Version [129,21]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbCollectionID`

**Version [129,22]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbMovieID`

**Version [129,23]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbCompanyID`

**Version [129,24]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbEntityType_TmdbEntityID`

**Version [129,25]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_TmdbCompanyID`

**Version [129,26]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbEpisodeID`

**Version [129,27]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbPersonID`

**Version [129,28]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbSeasonID`

**Version [129,29]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbShowID`

**Version [129,30]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbEpisodeID`

**Version [129,31]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbPersonID`

**Version [129,32]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbSeasonID`

**Version [129,33]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbShowID`

**Version [129,34]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbMovieID`

**Version [129,35]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbPersonID`

**Version [129,36]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbMovieID`

**Version [129,37]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbPersonID`

**Version [129,38]** (1 commands):

**Version [129,39]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_TmdbCollectionID`

**Version [129,40]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Person_TmdbPersonID`

**Version [129,41]** (1 commands):

**Version [129,42]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Season_TmdbShowID`

**Version [129,43]** (1 commands):

**Version [130,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [130,7]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character`

**Version [130,8]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [130,9]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character`

**Version [130,10]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character_Creator`

**Version [130,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Staff_CreatorID`

**Version [131,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character ADD COLUMN Type INTEGER NOT NULL DEFAULT 0;

**Version [131,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character ADD COLUMN LastUpdated DATETIME NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [132,1]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image_Entity`

**Version [132,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbMovieID;

**Version [132,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbEpisodeID;

**Version [132,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbSeasonID;

**Version [132,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbShowID;

**Version [132,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbCollectionID;

**Version [132,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbNetworkID;

**Version [132,8]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbCompanyID;

**Version [132,9]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbPersonID;

**Version [132,10]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN ForeignType;

**Version [132,11]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN ImageType;

**Version [133,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ADD COLUMN PosterPath TEXT NULL DEFAULT NULL;

**Version [133,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD COLUMN ThumbnailPath TEXT NULL DEFAULT NULL;

**Version [135,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [135,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [136,1]** (1 commands):
  **ALTER TABLE:**
  - `Tmdb_Show_Network`: ALTER TABLE Tmdb_Show_Network RENAME TO Tmdb_Show_Network_old;

**Version [136,2]** (1 commands):
  **ALTER TABLE:**
  - `Tmdb_Show_Network_old`: ALTER TABLE Tmdb_Show_Network_old RENAME TO TMDB_Show_Network;

**Version [137,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie ADD COLUMN MatchRating INTEGER NOT NULL DEFAULT 1;

**Version [137,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie DROP COLUMN Source;

**Version [137,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE CrossRef_AniDB_TMDB_Show ADD COLUMN MatchRating INTEGER NOT NULL DEFAULT 1;

**Version [137,6]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE CrossRef_AniDB_TMDB_Show DROP COLUMN Source;

**Version [138,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [138,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [138,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [138,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [138,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [139,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo`

**Version [139,2]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo_MatchAttempt`

**Version [139,3]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_HashDigest`

**Version [139,5]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder DROP COLUMN ImportFolderType;

**Version [139,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_Place`: ALTER TABLE VideoLocal_Place DROP COLUMN ImportFolderType;

**Version [140,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredRelocationPipe`

**Version [140,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_ProviderID`

**Version [140,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_Name`

**Version [140,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN AbsoluteUserRating INTEGER;

**Version [140,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN UserRatingVoteType INTEGER;

**Version [140,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN IsFavorite INTEGER NOT NULL DEFAULT 0;

**Version [140,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN LastVideoUpdate DATETIME;

**Version [140,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN LastUpdated DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00';

**Version [140,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD COLUMN UserTags NOT NULL DEFAULT '';

**Version [140,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD COLUMN AbsoluteUserRating INTEGER;

**Version [140,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD COLUMN IsFavorite INTEGER NOT NULL DEFAULT 0;

**Version [140,19]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD COLUMN LastUpdated DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00'...

**Version [140,20]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD COLUMN UserTags NOT NULL DEFAULT '';

**Version [140,23]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Person`: ALTER TABLE TMDB_Person ADD COLUMN LastOrphanedAt DATETIME;

**Version [140,24]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Network`: ALTER TABLE TMDB_Network ADD COLUMN LastOrphanedAt DATETIME;

**Version [143,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN MALTitle;

**Version [143,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN StartEpisodeType;

**Version [143,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN StartEpisodeNumber;

**Version [143,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN CrossRefSource;

**Version [143,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User DROP COLUMN IsFave;

#### CodedCommand Entries in `_patchCommands`

- **[37,1]** `DatabaseFixes.PopulateTagWeight`
- **[45,1]** `DatabaseFixes.DeleteSeriesUsersWithoutSeries`
- **[63,1]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[64,2]** `DatabaseFixes.UpdateAllStats`
- **[66,3]** `DatabaseFixes.MigrateAniDB_AnimeUpdates`
- **[81,2]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[84,2]** `DatabaseFixes.FixWatchDates`
- **[93,10]** `DatabaseFixes.FixTagParentIDsAndNameOverrides`
- **[99,1]** `DatabaseFixes.FixEpisodeDateTimeUpdated`
- **[100,3]** `DatabaseFixes.UpdateSeriesWithHiddenEpisodes`
- **[104,1]** `DatabaseFixes.FixAnimeSourceLinks`
- **[104,2]** `DatabaseFixes.FixOrphanedShokoEpisodes`
- **[105,4]** `DatabaseFixes.MigrateGroupFilterToFilterPreset`
- **[105,5]** `DatabaseFixes.DropGroupFilter`
- **[115,33]** `DatabaseFixes.CleanupAfterAddingTMDB`
- **[123,11]** `DatabaseFixes.CleanupAfterRemovingTvDB`
- **[123,12]** `DatabaseFixes.ClearQuartzQueue`
- **[124,1]** `DatabaseFixes.RepairMissingTMDBPersons`
- **[131,3]** `DatabaseFixes.RecreateAnimeCharactersAndCreators`
- **[132,12]** `DatabaseFixes.ScheduleTmdbImageUpdates`
- **[134,2]** `DatabaseFixes.MoveTmdbImagesOnDisc`
- **[138,6]** `DatabaseFixes.ClearQuartzQueue`
- **[139,4]** `DatabaseFixes.MoveAnidbFileDataToReleaseInfoFormat`
- **[140,4]** `DatabaseFixes.MigrateRenamers`
- **[140,21]** `DatabaseFixes.MigrateAnidbVotes`
- **[140,22]** `DatabaseFixes.RefreshAnimeSeriesUserStats`
- **[143,6]** `DatabaseFixes.EnsureNoOrphanedGroupsOrSeries`

### Helper Functions (PostDatabaseFix)

#### Tuple<bool, string> Functions
- `DropLanguage`
- `DropAniDB_AnimeColumns`
- `DropAniDB_Anime_CharacterColumns`
- `DropAniDB_CharacterColumns`
- `AlterAniDB_GroupStatus`
- `DropAniDB_FileColumns`
- `DropAnimeEpisode_UserColumns`
- `DropVideoLocal_Media`
- `DropAniDB_EpisodeTitles`
- `RenameCrossRef_AniDB_TvDB_Episode`
- `DropAniDB_AnimeAllCategories`
- `DropVideoLocalColumns`
- `DropTvDB_EpisodeFirstAiredColumn`
- `AlterVideoLocalUser`

---

## MySQL
**Provider class**: `MySQL`
**File**: `Shoko.Server/Databases/MySQL.cs`

### Version Table Creation (`_createVersionTable`)
- **[0,1]** `NormalCommand`: CREATE TABLE `Versions` ( `VersionsID` INT NOT NULL AUTO_INCREMENT , `VersionType` VARCHAR(100) NOT NULL , `VersionValue...
- **[0,2]** `NormalCommand`: ALTER TABLE `Versions` ADD UNIQUE INDEX `UIX_Versions_VersionType` (`VersionType` ASC) ;

### Version Table Schema Updates (`_updateVersionTable`)
- **[0,3]** `NormalCommand`: ALTER TABLE `Versions` ADD `VersionRevision` varchar(100) NULL;
- **[0,4]** `NormalCommand`: ALTER TABLE `Versions` ADD `VersionCommand` text NULL;
- **[0,5]** `NormalCommand`: ALTER TABLE `Versions` ADD `VersionProgram` varchar(100) NULL;
- **[0,6]** `NormalCommand`: ALTER TABLE `Versions` DROP INDEX `UIX_Versions_VersionType` ;
- **[0,7]** `NormalCommand`: ALTER TABLE `Versions` ADD INDEX `IX_Versions_VersionType` (`VersionType`,`VersionValue`,`VersionRevision`);

### Initial Table Creation (`_createTables`)
**Total commands**: 108
**Version range**: [1, 110]

**Tables created**: 57
- [1,3] `AniDB_Anime`
- [1,5] `AniDB_Anime_Category`
- [1,8] `AniDB_Anime_Character`
- [1,11] `AniDB_Anime_Relation`
- [1,14] `AniDB_Anime_Review`
- [1,17] `AniDB_Anime_Similar`
- [1,20] `AniDB_Anime_Tag`
- [1,23] `AniDB_Anime_Title`
- [1,25] `AniDB_Category`
- [1,27] `AniDB_Character`
- [1,29] `AniDB_Character_Seiyuu`
- [1,33] `AniDB_Seiyuu`
- [1,35] `AniDB_Episode`
- [1,38] `AniDB_File`
- [1,41] `AniDB_GroupStatus`
- [1,44] `AniDB_ReleaseGroup`
- [1,46] `AniDB_Review`
- [1,48] `AniDB_Tag`
- [1,50] `AnimeEpisode`
- [1,53] `AnimeEpisode_User`
- [1,56] `AnimeGroup`
- [1,57] `AnimeSeries`
- [1,59] `AnimeSeries_User`
- [1,61] `AnimeGroup_User`
- [1,63] `VideoLocal`
- [1,65] `VideoLocal_User`
- [1,67] `CommandRequest`
- [1,68] `CrossRef_AniDB_Other`
- [1,70] `CrossRef_AniDB_TvDB`
- [1,72] `CrossRef_File_Episode`
- [1,74] `CrossRef_Languages_AniDB_File`
- [1,75] `CrossRef_Subtitles_AniDB_File`
- [1,76] `FileNameHash`
- [1,77] `Language`
- [1,79] `ImportFolder`
- [1,80] `ScheduledUpdate`
- [1,82] `VideoInfo`
- [1,84] `DuplicateFile`
- [1,85] `GroupFilter`
- [1,86] `GroupFilterCondition`
- [1,87] `AniDB_Vote`
- [1,88] `TvDB_ImageFanart`
- [1,90] `TvDB_ImageWideBanner`
- [1,92] `TvDB_ImagePoster`
- [1,94] `TvDB_Episode`
- [1,96] `TvDB_Series`
- [1,98] `AniDB_Anime_DefaultImage`
- [1,100] `MovieDB_Movie`
- [1,102] `MovieDB_Poster`
- [1,103] `MovieDB_Fanart`
- [1,104] `JMMUser`
- [1,105] `Trakt_Episode`
- [1,106] `Trakt_ImagePoster`
- [1,107] `Trakt_ImageFanart`
- [1,108] `Trakt_Show`
- [1,109] `Trakt_Season`
- [1,110] `CrossRef_AniDB_Trakt`

**Indexes created**: 0

### Patch Commands (`_patchCommands`)
**Total commands**: 553
- **NormalCommand**: 488
- **CodedCommand**: 27
- **DataMigration** (excluded from schema catalog): 38

#### Schema Changes by Version

**Version [2,1]** (1 commands):
  **CREATE TABLE:**
  - `IgnoreAnime`

**Version [2,2]** (1 commands):
  **ALTER TABLE:**
  - `IgnoreAnime`: ALTER TABLE `IgnoreAnime` ADD UNIQUE INDEX `UIX_IgnoreAnime_User_AnimeID` (`JMMUserID` ASC, `AnimeID...

**Version [3,1]** (1 commands):
  **CREATE TABLE:**
  - `Trakt_Friend`

**Version [3,2]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Friend`: ALTER TABLE `Trakt_Friend` ADD UNIQUE INDEX `UIX_Trakt_Friend_Username` (`Username` ASC) ;

**Version [4,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD DefaultAnimeSeriesID int NULL

**Version [5,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD CanEditServerSettings int NULL

**Version [6,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD VideoBitDepth varchar(100) NULL

**Version [7,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB`: ALTER TABLE `CrossRef_AniDB_TvDB` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_TvDB_Season` (`TvDBID` ASC, `...

**Version [7,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_Trakt`: ALTER TABLE `CrossRef_AniDB_Trakt` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_Trakt_Season` (`TraktID` ASC...

**Version [7,5]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_Trakt`: ALTER TABLE `CrossRef_AniDB_Trakt` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_Trakt_Anime` (`AnimeID` ASC)...

**Version [8,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser CHANGE COLUMN Password Password VARCHAR(150) NULL DEFAULT NULL ;

**Version [9,1]** (1 commands):
  **ALTER TABLE:**
  - `CommandRequest`: ALTER TABLE `CommandRequest` CHANGE COLUMN `CommandID` `CommandID` text character set utf8 NOT NULL ...

**Version [9,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_File_Episode`: ALTER TABLE `CrossRef_File_Episode` CHANGE COLUMN `FileName` `FileName` text character set utf8 NOT ...

**Version [9,3]** (1 commands):
  **ALTER TABLE:**
  - `FileNameHash`: ALTER TABLE `FileNameHash` CHANGE COLUMN `FileName` `FileName` text character set utf8 NOT NULL ;

**Version [10,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Category`: ALTER TABLE `AniDB_Category` CHANGE COLUMN `CategoryName` `CategoryName` text character set utf8 NOT...

**Version [10,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Category`: ALTER TABLE `AniDB_Category` CHANGE COLUMN `CategoryDescription` `CategoryDescription` text characte...

**Version [10,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE `AniDB_Episode` CHANGE COLUMN `RomajiName` `RomajiName` text character set utf8 NOT NULL...

**Version [10,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE `AniDB_Episode` CHANGE COLUMN `EnglishName` `EnglishName` text character set utf8 NOT NU...

**Version [10,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Relation`: ALTER TABLE `AniDB_Anime_Relation` CHANGE COLUMN `RelationType` `RelationType` text character set ut...

**Version [10,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE `AniDB_Character` CHANGE COLUMN `CharName` `CharName` text character set utf8 NOT NULL ;

**Version [10,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Seiyuu`: ALTER TABLE `AniDB_Seiyuu` CHANGE COLUMN `SeiyuuName` `SeiyuuName` text character set utf8 NOT NULL ...

**Version [10,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `File_Description` `File_Description` text character set utf8...

**Version [10,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `Anime_GroupName` `Anime_GroupName` text character set utf8 N...

**Version [10,10]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `Anime_GroupNameShort` `Anime_GroupNameShort` text character ...

**Version [10,11]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `FileName` `FileName` text character set utf8 NOT NULL ;

**Version [10,12]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_GroupStatus`: ALTER TABLE `AniDB_GroupStatus` CHANGE COLUMN `GroupName` `GroupName` text character set utf8 NOT NU...

**Version [10,13]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_ReleaseGroup`: ALTER TABLE `AniDB_ReleaseGroup` CHANGE COLUMN `GroupName` `GroupName` text character set utf8 NOT N...

**Version [10,14]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_ReleaseGroup`: ALTER TABLE `AniDB_ReleaseGroup` CHANGE COLUMN `GroupNameShort` `GroupNameShort` text character set ...

**Version [10,15]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_ReleaseGroup`: ALTER TABLE `AniDB_ReleaseGroup` CHANGE COLUMN `URL` `URL` text character set utf8 NOT NULL ;

**Version [10,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` CHANGE COLUMN `GroupName` `GroupName` text character set utf8 NOT NULL ;

**Version [10,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` CHANGE COLUMN `SortName` `SortName` text character set utf8 NOT NULL ;

**Version [10,18]** (1 commands):
  **ALTER TABLE:**
  - `CommandRequest`: ALTER TABLE `CommandRequest` CHANGE COLUMN `CommandID` `CommandID` text character set utf8 NOT NULL ...

**Version [10,19]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_File_Episode`: ALTER TABLE `CrossRef_File_Episode` CHANGE COLUMN `FileName` `FileName` text character set utf8 NOT ...

**Version [10,20]** (1 commands):
  **ALTER TABLE:**
  - `FileNameHash`: ALTER TABLE `FileNameHash` CHANGE COLUMN `FileName` `FileName` text character set utf8 NOT NULL ;

**Version [10,21]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE `ImportFolder` CHANGE COLUMN `ImportFolderLocation` `ImportFolderLocation` text characte...

**Version [10,22]** (1 commands):
  **ALTER TABLE:**
  - `DuplicateFile`: ALTER TABLE `DuplicateFile` CHANGE COLUMN `FilePathFile1` `FilePathFile1` text character set utf8 NO...

**Version [10,23]** (1 commands):
  **ALTER TABLE:**
  - `DuplicateFile`: ALTER TABLE `DuplicateFile` CHANGE COLUMN `FilePathFile2` `FilePathFile2` text character set utf8 NO...

**Version [10,24]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE `TvDB_Episode` CHANGE COLUMN `Filename` `Filename` text character set utf8 NOT NULL ;

**Version [10,25]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE `TvDB_Episode` CHANGE COLUMN `EpisodeName` `EpisodeName` text character set utf8 NOT NUL...

**Version [10,26]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Series`: ALTER TABLE `TvDB_Series` CHANGE COLUMN `SeriesName` `SeriesName` text character set utf8 NOT NULL ;

**Version [10,27]** (1 commands):
  **ALTER TABLE:**
  - `DuplicateFile`: ALTER TABLE `DuplicateFile` CHANGE COLUMN `FilePathFile2` `FilePathFile2` text character set utf8 NO...

**Version [10,28]** (1 commands):
  **ALTER TABLE:**
  - `DuplicateFile`: ALTER TABLE `DuplicateFile` CHANGE COLUMN `FilePathFile2` `FilePathFile2` text character set utf8 NO...

**Version [11,1]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE `ImportFolder` ADD `IsWatched` int NULL ;

**Version [11,3]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE `ImportFolder` CHANGE COLUMN `IsWatched` `IsWatched` int NOT NULL ;

**Version [12,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [12,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_AnimeID` (`AnimeID` ASC) ;

**Version [12,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_MALID` (`MALID` ASC) ;

**Version [13,1]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [13,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [13,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_AnimeID` (`AnimeID` ASC) ;

**Version [13,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_Anime` (`MALID` ASC, `Anim...

**Version [14,1]** (1 commands):
  **CREATE TABLE:**
  - `Playlist`

**Version [15,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `SeriesNameOverride` text NULL ;

**Version [16,1]** (1 commands):
  **CREATE TABLE:**
  - `BookmarkedAnime`

**Version [16,2]** (1 commands):
  **ALTER TABLE:**
  - `BookmarkedAnime`: ALTER TABLE `BookmarkedAnime` ADD UNIQUE INDEX `UIX_BookmarkedAnime_AnimeID` (`AnimeID` ASC) ;

**Version [17,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `DateTimeCreated` datetime NULL ;

**Version [17,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` CHANGE COLUMN `DateTimeCreated` `DateTimeCreated` datetime NOT NULL ;

**Version [18,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [18,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_TvDB_Episode_AniDBEpi...

**Version [19,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_MylistStats`

**Version [20,1]** (1 commands):
  **CREATE TABLE:**
  - `FileFfdshowPreset`

**Version [20,2]** (1 commands):
  **ALTER TABLE:**
  - `FileFfdshowPreset`: ALTER TABLE `FileFfdshowPreset` ADD UNIQUE INDEX `UIX_FileFfdshowPreset_Hash` (`Hash` ASC, `FileSize...

**Version [21,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD `DisableExternalLinksFlag` int NULL ;

**Version [21,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` CHANGE COLUMN `DisableExternalLinksFlag` `DisableExternalLinksFlag` int NO...

**Version [22,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` ADD `FileVersion` int NULL ;

**Version [22,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `FileVersion` `FileVersion` int NOT NULL ;

**Version [23,1]** (1 commands):
  **CREATE TABLE:**
  - `RenameScript`

**Version [24,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` ADD `IsCensored` int NULL ;

**Version [24,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` ADD `IsDeprecated` int NULL ;

**Version [24,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` ADD `InternalVersion` int NULL ;

**Version [24,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `IsCensored` `IsCensored` int NOT NULL ;

**Version [24,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `IsDeprecated` `IsDeprecated` int NOT NULL ;

**Version [24,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `InternalVersion` `InternalVersion` int NOT NULL ;

**Version [25,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `IsVariation` int NULL ;

**Version [25,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` CHANGE COLUMN `IsVariation` `IsVariation` int NOT NULL ;

**Version [26,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Recommendation`

**Version [26,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Recommendation`: ALTER TABLE `AniDB_Recommendation` ADD UNIQUE INDEX `UIX_AniDB_Recommendation` (`AnimeID` ASC, `User...

**Version [28,1]** (1 commands):
  **CREATE TABLE:**
  - `LogMessage`

**Version [29,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDBV2`

**Version [29,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDBV2`: ALTER TABLE `CrossRef_AniDB_TvDBV2` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_TvDBV2` (`AnimeID` ASC, `Tv...

**Version [30,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `Locked` int NULL ;

**Version [31,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD FullInfo varchar(10000) NULL

**Version [32,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TraktV2`

**Version [32,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TraktV2`: ALTER TABLE `CrossRef_AniDB_TraktV2` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_TraktV2` (`AnimeID` ASC, `...

**Version [33,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_Trakt_Episode`

**Version [33,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_Trakt_Episode`: ALTER TABLE `CrossRef_AniDB_Trakt_Episode` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_Trakt_Episode_AniDBE...

**Version [35,1]** (1 commands):
  **CREATE TABLE:**
  - `CustomTag`

**Version [35,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_CustomTag`

**Version [37,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP INDEX `UIX_CrossRef_AniDB_MAL_AnimeID` ;

**Version [37,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP INDEX `UIX_CrossRef_AniDB_MAL_Anime` ;

**Version [37,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_MALID` (`MALID` ASC) ;

**Version [37,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_MAL_Anime` (`AnimeID` ASC, `St...

**Version [38,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD Weight int NULL

**Version [40,1]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Episode`: ALTER TABLE Trakt_Episode ADD TraktID int NULL

**Version [42,1]** (1 commands):
  **DROP TABLE:**
  - `LogMessage`

**Version [43,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DefaultFolder text character set utf8

**Version [44,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexUsers text character set utf8

**Version [45,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `FilterType` int NULL ;

**Version [45,3]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` CHANGE COLUMN `FilterType` `FilterType` int NOT NULL ;

**Version [46,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD `ContractVersion` int NOT NULL DEFAULT 0

**Version [46,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD `ContractString` mediumtext character set utf8 NULL

**Version [46,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` ADD `ContractVersion` int NOT NULL DEFAULT 0

**Version [46,4]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` ADD `ContractString` mediumtext character set utf8 NULL

**Version [46,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `PlexContractVersion` int NOT NULL DEFAULT 0

**Version [46,6]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `PlexContractString` mediumtext character set utf8 NULL

**Version [46,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `KodiContractVersion` int NOT NULL DEFAULT 0

**Version [46,8]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `KodiContractString` mediumtext character set utf8 NULL

**Version [46,9]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `ContractVersion` int NOT NULL DEFAULT 0

**Version [46,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `ContractString` mediumtext character set utf8 NULL

**Version [46,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `PlexContractVersion` int NOT NULL DEFAULT 0

**Version [46,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `PlexContractString` mediumtext character set utf8 NULL

**Version [46,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `KodiContractVersion` int NOT NULL DEFAULT 0

**Version [46,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `KodiContractString` mediumtext character set utf8 NULL

**Version [46,15]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `GroupsIdsVersion` int NOT NULL DEFAULT 0

**Version [46,16]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `GroupsIdsString` mediumtext character set utf8 NULL

**Version [46,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD `ContractVersion` int NOT NULL DEFAULT 0

**Version [46,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD `ContractString` mediumtext character set utf8 NULL

**Version [47,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` ADD `PlexContractVersion` int NOT NULL DEFAULT 0

**Version [47,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` ADD `PlexContractString` mediumtext character set utf8 NULL

**Version [47,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `MediaVersion` int NOT NULL DEFAULT 0

**Version [47,4]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `MediaString` mediumtext character set utf8 NULL

**Version [48,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` DROP COLUMN `KodiContractVersion`

**Version [48,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` DROP COLUMN `KodiContractString`

**Version [48,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` DROP COLUMN `KodiContractVersion`

**Version [48,4]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` DROP COLUMN `KodiContractString`

**Version [49,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD LatestEpisodeAirDate datetime NULL

**Version [49,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD LatestEpisodeAirDate datetime NULL

**Version [50,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `GroupConditionsVersion` int NOT NULL DEFAULT 0

**Version [50,2]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `GroupConditions` mediumtext character set utf8 NULL

**Version [50,3]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `ParentGroupFilterID` int NULL

**Version [50,4]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `InvisibleInClients` int NOT NULL DEFAULT 0

**Version [50,5]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `SeriesIdsVersion` int NOT NULL DEFAULT 0

**Version [50,6]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD `SeriesIdsString` mediumtext character set utf8 NULL

**Version [51,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD `ContractBlob` mediumblob NULL

**Version [51,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD `ContractSize` int NOT NULL DEFAULT 0

**Version [51,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` DROP COLUMN `ContractString`

**Version [51,4]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `MediaBlob` mediumblob NULL

**Version [51,5]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `MediaSize` int NOT NULL DEFAULT 0

**Version [51,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` DROP COLUMN `MediaString`

**Version [51,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` ADD `PlexContractBlob` mediumblob NULL

**Version [51,8]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` ADD `PlexContractSize` int NOT NULL DEFAULT 0

**Version [51,9]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` DROP COLUMN `PlexContractString`

**Version [51,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD `ContractBlob` mediumblob NULL

**Version [51,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD `ContractSize` int NOT NULL DEFAULT 0

**Version [51,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` DROP COLUMN `ContractString`

**Version [51,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `ContractBlob` mediumblob NULL

**Version [51,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `ContractSize` int NOT NULL DEFAULT 0

**Version [51,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` DROP COLUMN `ContractString`

**Version [51,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `PlexContractBlob` mediumblob NULL

**Version [51,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD `PlexContractSize` int NOT NULL DEFAULT 0

**Version [51,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` DROP COLUMN `PlexContractString`

**Version [51,19]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `PlexContractBlob` mediumblob NULL

**Version [51,20]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` ADD `PlexContractSize` int NOT NULL DEFAULT 0

**Version [51,21]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` DROP COLUMN `PlexContractString`

**Version [51,22]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` ADD `ContractBlob` mediumblob NULL

**Version [51,23]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` ADD `ContractSize` int NOT NULL DEFAULT 0

**Version [51,24]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE `AnimeGroup` DROP COLUMN `ContractString`

**Version [52,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` DROP COLUMN `AllCategories`

**Version [54,1]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_Place`

**Version [54,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `FileName` text character set utf8 NOT NULL

**Version [54,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `VideoCodec` varchar(100) NOT NULL DEFAULT ''

**Version [54,4]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `VideoBitrate` varchar(100) NOT NULL DEFAULT ''

**Version [54,5]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `VideoBitDepth` varchar(100) NOT NULL DEFAULT ''

**Version [54,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `VideoFrameRate` varchar(100) NOT NULL DEFAULT ''

**Version [54,7]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `VideoResolution` varchar(100) NOT NULL DEFAULT ''

**Version [54,8]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `AudioCodec` varchar(100) NOT NULL DEFAULT ''

**Version [54,9]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `AudioBitrate` varchar(100) NOT NULL DEFAULT ''

**Version [54,10]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `Duration` bigint NOT NULL DEFAULT 0

**Version [54,12]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` DROP COLUMN `FilePath`

**Version [54,13]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` DROP COLUMN `ImportFolderID`

**Version [54,14]** (1 commands):
  **CREATE TABLE:**
  - `CloudAccount`

**Version [54,15]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE `ImportFolder` ADD `CloudID` int NULL

**Version [54,16]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE `VideoLocal_User` MODIFY COLUMN `WatchedDate` datetime NULL

**Version [54,17]** (2 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE `VideoLocal_User` ADD `ResumePosition` bigint NOT NULL DEFAULT 0
  - `VideoLocal_User`: ALTER TABLE `VideoLocal_User` ADD `ResumePosition` bigint NOT NULL DEFAULT 0

**Version [54,19]** (1 commands):
  **DROP TABLE:**
  - `VideoInfo`

**Version [55,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` DROP INDEX `UIX_VideoLocal_Hash` ;

**Version [55,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD INDEX `IX_VideoLocal_Hash` (`Hash` ASC) ;

**Version [56,1]** (1 commands):
  **CREATE TABLE:**
  - `AuthTokens`

**Version [57,1]** (1 commands):
  **CREATE TABLE:**
  - `Scan`

**Version [57,2]** (1 commands):
  **CREATE TABLE:**
  - `ScanFile`

**Version [57,3]** (1 commands):
  **ALTER TABLE:**
  - `ScanFile`: ALTER TABLE `ScanFile` ADD  INDEX `UIX_ScanFileStatus` (`ScanID` ASC, `Status` ASC, `CheckDate` ASC)...

**Version [59,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE `GroupFilter` ADD INDEX `IX_groupfilter_GroupFilterName` (`GroupFilterName`(250));

**Version [62,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexToken text character set utf8

**Version [63,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsChaptered INT NOT NULL DEFAULT -1

**Version [64,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_File_Episode`: ALTER TABLE `CrossRef_File_Episode` ADD INDEX `IX_Xref_Epid` (`episodeid` ASC) ;

**Version [64,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE `CrossRef_Subtitles_AniDB_File` ADD INDEX `IX_Xref_Sub_AniDBFile` (`fileid` ASC) ;

**Version [64,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE `CrossRef_Languages_AniDB_File` ADD INDEX `IX_Xref_Epid` (`fileid` ASC) ;

**Version [65,1]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD RenamerType varchar(255) character set utf8 NOT NULL DEFAULT 'Legacy'

**Version [65,2]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD ExtraData TEXT character set utf8

**Version [66,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Character`: ALTER TABLE `AniDB_Anime_Character` ADD INDEX `IX_AniDB_Anime_Character_CharID` (`CharID` ASC) ;

**Version [67,1]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE `TvDB_Episode` ADD `Rating` int NULL

**Version [67,2]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE `TvDB_Episode` ADD `AirDate` datetime NULL

**Version [67,3]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE `TvDB_Episode` DROP COLUMN `FirstAired`

**Version [68,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `AirsOn` TEXT character set utf8 NULL

**Version [69,1]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImageFanart`

**Version [69,2]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImagePoster`

**Version [70,1]** (1 commands):
  **CREATE TABLE:**
  - `AnimeCharacter`

**Version [70,2]** (1 commands):
  **CREATE TABLE:**
  - `AnimeStaff`

**Version [70,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_Anime_Staff`

**Version [71,1]** (1 commands):
  **ALTER TABLE:**
  - `MovieDB_Movie`: ALTER TABLE `MovieDB_Movie` ADD `Rating` INT NOT NULL DEFAULT 0

**Version [71,2]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Series`: ALTER TABLE `TvDB_Series` ADD `Rating` INT NULL

**Version [72,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE `AniDB_Episode` ADD `Description` text character set utf8 NOT NULL

**Version [76,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_AnimeUpdate`

**Version [76,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_AnimeUpdate`: ALTER TABLE `AniDB_AnimeUpdate` ADD INDEX `UIX_AniDB_AnimeUpdate` (`AnimeID` ASC) ;

**Version [80,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP INDEX `UIX_CrossRef_AniDB_MAL_Anime` ;

**Version [80,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD ( `Site_JP` text character set utf8 null, `Site_EN` text character set...

**Version [81,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE `VideoLocal` ADD `MyListID` INT NOT NULL DEFAULT 0

**Version [83,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE `AniDB_Episode` DROP COLUMN `EnglishName`

**Version [83,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE `AniDB_Episode` DROP COLUMN `RomajiName`

**Version [83,3]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_Title`

**Version [84,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode` DROP INDEX `UIX_CrossRef_AniDB_TvDB_Episode_AniDBEpisodeID...

**Version [84,2]** (1 commands):

**Version [84,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode_Override` DROP COLUMN `AnimeID`

**Version [84,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode_Override` CHANGE `CrossRef_AniDB_TvDB_EpisodeID` `CrossRef_...

**Version [84,5]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode_Override` ADD UNIQUE INDEX `UIX_AniDB_TvDB_Episode_Override...

**Version [84,6]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [84,7]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [84,8]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB`: ALTER TABLE `CrossRef_AniDB_TvDB` ADD UNIQUE INDEX `UIX_AniDB_TvDB_AniDBID_TvDBID` (`AniDBID` ASC, `...

**Version [84,9]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [84,10]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`: ALTER TABLE `CrossRef_AniDB_TvDB_Episode` ADD UNIQUE INDEX `UIX_CrossRef_AniDB_TvDB_Episode_AniDBID_...

**Version [87,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` CHANGE COLUMN `File_AudioCodec` `File_AudioCodec` VARCHAR(500) NOT NULL;

**Version [88,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE `AnimeSeries` ADD `UpdatedAt` datetime NOT NULL DEFAULT '2000-01-01 00:00:00';

**Version [90,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN VideoCodec, DROP COLUMN VideoBitrate, DROP COLUMN VideoFrameRate,...

**Version [93,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [95,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD WatchedCount INT NOT NULL DEFAULT 0;

**Version [95,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD LastUpdated datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CU...

**Version [96,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD LastEpisodeUpdate datetime DEFAULT NULL;

**Version [97,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD MainAniDBAnimeID INT DEFAULT NULL;

**Version [98,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User DROP COLUMN ContractSize, DROP COLUMN ContractBlob, DROP COLUMN Contra...

**Version [99,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` DROP `File_AudioCodec`, DROP `File_VideoCodec`, DROP `File_VideoResolution`...

**Version [99,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_GroupStatus`: ALTER TABLE `AniDB_GroupStatus` MODIFY `Rating` decimal(6,2) NULL; UPDATE `AniDB_GroupStatus` SET `R...

**Version [99,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE `AniDB_Character` DROP COLUMN CreatorListRaw;

**Version [99,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Character`: ALTER TABLE `AniDB_Anime_Character` DROP COLUMN EpisodeListRaw;

**Version [99,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` DROP COLUMN AwardList;

**Version [99,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE `AniDB_File` DROP COLUMN AnimeID;

**Version [100,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE CrossRef_Languages_AniDB_File ADD LanguageName nvarchar(100) NOT NULL DEFAULT '';

**Version [100,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE CrossRef_Languages_AniDB_File DROP COLUMN LanguageID;

**Version [100,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE CrossRef_Subtitles_AniDB_File ADD LanguageName nvarchar(100) NOT NULL DEFAULT '';

**Version [100,6]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE CrossRef_Subtitles_AniDB_File DROP COLUMN LanguageID;

**Version [100,7]** (1 commands):
  **DROP TABLE:**
  - `Language`

**Version [101,1]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Category`

**Version [101,2]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Review`

**Version [101,3]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Category`

**Version [101,4]** (1 commands):
  **DROP TABLE:**
  - `AniDB_MylistStats`

**Version [101,5]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Review`

**Version [101,6]** (1 commands):
  **DROP TABLE:**
  - `CloudAccount`

**Version [101,7]** (1 commands):
  **DROP TABLE:**
  - `FileFfdshowPreset`

**Version [101,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Trakt`

**Version [101,9]** (1 commands):
  **DROP TABLE:**
  - `Trakt_Friend`

**Version [101,10]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN VideoBitDepth;

**Version [101,11]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD INDEX IX_AniDB_File_FileID (FileID);

**Version [101,12]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_File_Episode`: ALTER TABLE CrossRef_File_Episode DROP INDEX IX_Xref_Epid; ALTER TABLE CrossRef_File_Episode ADD IND...

**Version [101,13]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE CrossRef_Languages_AniDB_File DROP INDEX IX_Xref_Epid;

**Version [101,14]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE CrossRef_Subtitles_AniDB_File DROP INDEX IX_Xref_Sub_AniDBFile;

**Version [101,15]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter DROP INDEX IX_groupfilter_GroupFilterName;

**Version [101,16]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP INDEX IX_VideoLocal_Hash; ALTER TABLE VideoLocal ADD UNIQUE INDEX UIX_Vi...

**Version [103,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_GroupStatus`: ALTER TABLE AniDB_GroupStatus MODIFY GroupName LONGTEXT NULL; ALTER TABLE AniDB_GroupStatus MODIFY E...

**Version [104,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode ADD INDEX IX_AniDB_Episode_EpisodeType (EpisodeType);

**Version [105,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode_Title`: ALTER TABLE AniDB_Episode_Title MODIFY Title TEXT NOT NULL

**Version [106,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD DateTimeImported datetime DEFAULT NULL;

**Version [107,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD Verified integer NOT NULL DEFAULT 0;

**Version [107,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD ParentTagID integer DEFAULT NULL;

**Version [107,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD TagNameOverride varchar(150) DEFAULT NULL;

**Version [107,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD LastUpdated datetime NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [107,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN Spoiler;

**Version [107,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN LocalSpoiler;

**Version [107,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN TagCount;

**Version [107,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD LocalSpoiler integer NOT NULL DEFAULT 0;

**Version [107,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag DROP COLUMN Approval;

**Version [108,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD IsHidden integer NOT NULL DEFAULT 0;

**Version [108,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD HiddenUnwatchedEpisodeCount integer NOT NULL DEFAULT 0;

**Version [110,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_FileUpdate`

**Version [110,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_FileUpdate`: ALTER TABLE `AniDB_FileUpdate` ADD INDEX `IX_AniDB_FileUpdate` (`FileSize` ASC, `Hash` ASC) ;

**Version [111,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN DisableExternalLinksFlag;

**Version [111,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DisableAutoMatchFlags integer NOT NULL DEFAULT 0;

**Version [111,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE `AniDB_Anime` ADD ( `VNDBID` INT NULL, `BangumiID` INT NULL, `LianID` INT NULL, `Funimat...

**Version [112,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN LianID;

**Version [112,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimePlanetID;

**Version [112,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimeNfo;

**Version [112,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD LainID INT NULL

**Version [114,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCount INT NOT NULL DEFAULT 0;

**Version [114,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCountGroups INT NOT NULL DEFAULT 0;

**Version [116,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageBlob BLOB NULL;

**Version [116,2]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageMetadata VARCHAR(128) NULL;

**Version [117,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumped datetime;

**Version [117,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumpVersion nvarchar(128);

**Version [119,1]** (1 commands):
  **CREATE TABLE:**
  - `FilterPreset`

**Version [119,2]** (1 commands):
  **ALTER TABLE:**
  - `FilterPreset`: ALTER TABLE FilterPreset ADD INDEX IX_FilterPreset_ParentFilterPresetID (ParentFilterPresetID); ALTE...

**Version [122,1]** (1 commands):
  **ALTER TABLE:**
  - `CommandRequest`: ALTER TABLE CommandRequest ADD INDEX IX_CommandRequest_CommandType (CommandType); ALTER TABLE Comman...

**Version [123,1]** (1 commands):
  **DROP TABLE:**
  - `CommandRequest`

**Version [124,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE `AnimeEpisode` ADD `EpisodeNameOverride` text NULL;

**Version [126,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN ContractVersion;ALTER TABLE AniDB_Anime DROP COLUMN ContractBlob...

**Version [126,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries DROP COLUMN ContractVersion;ALTER TABLE AnimeSeries DROP COLUMN ContractBlob...

**Version [126,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup DROP COLUMN ContractVersion;ALTER TABLE AnimeGroup DROP COLUMN ContractBlob;A...

**Version [127,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN MediaSize;

**Version [128,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_NotifyQueue`

**Version [128,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Message`

**Version [129,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Episode`

**Version [129,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`

**Version [129,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Show`

**Version [129,4]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image`

**Version [129,5]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_PreferredImage`

**Version [129,6]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Title`

**Version [129,7]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Overview`

**Version [129,8]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company`

**Version [129,9]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Network`

**Version [129,10]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Person`

**Version [129,11]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie`

**Version [129,12]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Cast`

**Version [129,13]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company_Entity`

**Version [129,14]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Crew`

**Version [129,15]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Show`

**Version [129,16]** (1 commands):
  **CREATE TABLE:**
  - `Tmdb_Show_Network`

**Version [129,17]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Season`

**Version [129,18]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode`

**Version [129,19]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Cast`

**Version [129,20]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Crew`

**Version [129,21]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering`

**Version [129,22]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Season`

**Version [129,23]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Episode`

**Version [129,24]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection`

**Version [129,25]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection_Movie`

**Version [129,27]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Other`

**Version [129,28]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Fanart`

**Version [129,29]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Movie`

**Version [129,30]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Poster`

**Version [129,31]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_DefaultImage`

**Version [129,32]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_PreferredImage`

**Version [132,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `TvdbShowID` INT NULL DEFAULT NULL;

**Version [132,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE `TMDB_Episode` ADD COLUMN `TvdbEpisodeID` INT NULL DEFAULT NULL;

**Version [132,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `ImdbMovieID` INT NULL DEFAULT NULL;

**Version [132,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` DROP COLUMN `ImdbMovieID`;

**Version [132,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `ImdbMovieID` VARCHAR(12) NULL DEFAULT NULL;

**Version [132,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Overview`: ALTER TABLE `TMDB_Overview` ADD INDEX `IX_TMDB_Overview` (ParentType, ParentID)

**Version [132,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Title`: ALTER TABLE `TMDB_Title` ADD INDEX `IX_TMDB_Title` (ParentType, ParentID)

**Version [132,8]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE `TMDB_Episode` ADD UNIQUE INDEX `UIX_TMDB_Episode_TmdbEpisodeID` (TmdbEpisodeID)

**Version [132,9]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD UNIQUE INDEX `UIX_TMDB_Show_TmdbShowID` (TmdbShowID)

**Version [133,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie CHANGE COLUMN AnidbEpisodeID AnidbEpisodeID INT NOT NULL DEFAU...

**Version [134,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `PosterPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [134,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `BackdropPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [134,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `PosterPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [134,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `BackdropPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [136,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Creator`

**Version [136,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character_Creator`

**Version [136,3]** (1 commands):

**Version [136,4]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CreatorID`

**Version [136,5]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CharacterID`

**Version [136,6]** (1 commands):

**Version [136,9]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [136,10]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [137,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `PreferredAlternateOrderingID`  VARCHAR(64) CHARACTER SET UTF8 NU...

**Version [138,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` CHANGE COLUMN `ContentRatings` `ContentRatings` VARCHAR(512) CHARACTER SET U...

**Version [138,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` CHANGE COLUMN `ContentRatings` `ContentRatings` VARCHAR(512) CHARACTER SET ...

**Version [139,1]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Episode`

**Version [139,2]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Series`

**Version [139,3]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageFanart`

**Version [139,4]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImagePoster`

**Version [139,5]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageWideBanner`

**Version [139,6]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [139,7]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [139,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`

**Version [139,9]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show DROP COLUMN TvDB_ID;

**Version [139,10]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show ADD COLUMN TmdbShowID INT NULL;

**Version [141,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `Keywords` VARCHAR(512) NULL DEFAULT NULL;

**Version [141,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` ADD COLUMN `ProductionCountries` VARCHAR(32) NULL DEFAULT NULL;

**Version [141,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `Keywords` VARCHAR(512) NULL DEFAULT NULL;

**Version [141,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `ProductionCountries` VARCHAR(32) NULL DEFAULT NULL;

**Version [142,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Relation`: ALTER TABLE `AniDB_Anime_Relation` ADD INDEX `IX_AniDB_Anime_Relation_RelatedAnimeID` (`RelatedAnime...

**Version [143,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` CHANGE COLUMN `ProductionCountries` `ProductionCountries` VARCHAR(255) NULL...

**Version [143,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` CHANGE COLUMN `ProductionCountries` `ProductionCountries` VARCHAR(255) NULL;

**Version [144,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbSeasonID`

**Version [144,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbShowID`

**Version [145,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE `TMDB_Episode` ADD COLUMN `IsHidden` int NOT NULL DEFAULT 0;

**Version [145,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE `TMDB_Season` ADD COLUMN `HiddenEpisodeCount` int NOT NULL DEFAULT 0;

**Version [145,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` ADD COLUMN `HiddenEpisodeCount` int NOT NULL DEFAULT 0;

**Version [145,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Season`: ALTER TABLE `TMDB_AlternateOrdering_Season` ADD COLUMN `HiddenEpisodeCount` int NOT NULL DEFAULT 0;

**Version [145,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering`: ALTER TABLE `TMDB_AlternateOrdering` ADD COLUMN `HiddenEpisodeCount` int NOT NULL DEFAULT 0;

**Version [146,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID`

**Version [146,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID_TmdbShowID`

**Version [146,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID`

**Version [146,4]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID_TmdbEpisodeID`

**Version [146,5]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbEpisodeID`

**Version [146,6]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbShowID`

**Version [146,7]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbAnimeID`

**Version [146,8]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID`

**Version [146,9]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID_TmdbMovieID`

**Version [146,10]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_TmdbMovieID`

**Version [146,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID`

**Version [146,12]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID_TmdbShowID`

**Version [146,13]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_TmdbShowID`

**Version [146,14]** (1 commands):

**Version [146,15]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbEpisodeGroupCollectionID`

**Version [146,16]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbShowID`

**Version [146,17]** (1 commands):

**Version [146,18]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbEpisodeGroupCollection_TmdbShow`

**Version [146,19]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbShowID`

**Version [146,20]** (1 commands):

**Version [146,21]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbCollectionID`

**Version [146,22]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbMovieID`

**Version [146,23]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbCompanyID`

**Version [146,24]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbEntityType_TmdbEntityID`

**Version [146,25]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_TmdbCompanyID`

**Version [146,26]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbEpisodeID`

**Version [146,27]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbPersonID`

**Version [146,28]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbSeasonID`

**Version [146,29]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbShowID`

**Version [146,30]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbEpisodeID`

**Version [146,31]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbPersonID`

**Version [146,32]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbSeasonID`

**Version [146,33]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbShowID`

**Version [146,34]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbMovieID`

**Version [146,35]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbPersonID`

**Version [146,36]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbMovieID`

**Version [146,37]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbPersonID`

**Version [146,38]** (1 commands):

**Version [146,39]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_TmdbCollectionID`

**Version [146,40]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Person_TmdbPersonID`

**Version [146,41]** (1 commands):

**Version [146,42]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Season_TmdbShowID`

**Version [146,43]** (1 commands):

**Version [147,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [147,7]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character`

**Version [147,8]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [147,9]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character`

**Version [147,10]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character_Creator`

**Version [147,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Staff_CreatorID`

**Version [148,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE `AniDB_Character` ADD `Type` int NOT NULL DEFAULT 0;

**Version [148,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE `AniDB_Character` ADD `LastUpdated` datetime NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [150,1]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image_Entity`

**Version [150,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbMovieID`;

**Version [150,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbEpisodeID`;

**Version [150,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbSeasonID`;

**Version [150,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbShowID`;

**Version [150,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbCollectionID`;

**Version [150,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbNetworkID`;

**Version [150,8]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbCompanyID`;

**Version [150,9]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `TmdbPersonID`;

**Version [150,10]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `ForeignType`;

**Version [150,11]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE `TMDB_Image` DROP COLUMN `ImageType`;

**Version [151,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE `TMDB_Season` ADD COLUMN `PosterPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [151,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE `TMDB_Episode` ADD COLUMN `ThumbnailPath` VARCHAR(64) NULL DEFAULT NULL;

**Version [153,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [153,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [154,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE `TMDB_Show` MODIFY COLUMN `Keywords` LONGTEXT NULL;

**Version [154,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE `TMDB_Movie` MODIFY COLUMN `Keywords` LONGTEXT NULL;

**Version [155,1]** (1 commands):

**Version [156,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE `CrossRef_AniDB_TMDB_Movie` ADD COLUMN `MatchRating` INT NOT NULL DEFAULT 1;

**Version [156,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE `CrossRef_AniDB_TMDB_Movie` DROP COLUMN `Source`;

**Version [156,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE `CrossRef_AniDB_TMDB_Show` ADD COLUMN `MatchRating` INT NOT NULL DEFAULT 1;

**Version [156,6]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE `CrossRef_AniDB_TMDB_Show` DROP COLUMN `Source`;

**Version [157,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [157,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [157,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [157,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [157,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [158,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo`

**Version [158,2]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo_MatchAttempt`

**Version [158,3]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_HashDigest`

**Version [158,5]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE `ImportFolder` DROP COLUMN `ImportFolderType`;

**Version [158,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_Place`: ALTER TABLE `VideoLocal_Place` DROP COLUMN `ImportFolderType`;

**Version [159,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredRelocationPipe`

**Version [159,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_ProviderID`

**Version [159,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_Name`

**Version [159,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [159,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [159,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `AbsoluteUserRating` INT;

**Version [159,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `UserRatingVoteType` INT;

**Version [159,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `IsFavorite` INT NOT NULL DEFAULT 0;

**Version [159,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `LastVideoUpdate` datetime;

**Version [159,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `LastUpdated` datetime NOT NULL DEFAULT '0001-01-01 00:00:...

**Version [159,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE `AnimeSeries_User` ADD COLUMN `UserTags` TEXT NOT NULL DEFAULT '';

**Version [159,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD COLUMN `AbsoluteUserRating` INT;

**Version [159,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD COLUMN `IsFavorite` INT NOT NULL DEFAULT 0;

**Version [159,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD COLUMN `LastUpdated` datetime NOT NULL DEFAULT '0001-01-01 00:00...

**Version [159,19]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE `AnimeEpisode_User` ADD COLUMN `UserTags` TEXT NOT NULL DEFAULT '';

**Version [159,22]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Person`: ALTER TABLE `TMDB_Person` ADD COLUMN `LastOrphanedAt` DATETIME;

**Version [159,23]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Network`: ALTER TABLE `TMDB_Network` ADD COLUMN `LastOrphanedAt` DATETIME;

**Version [161,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP COLUMN `MALTitle`;

**Version [161,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP COLUMN `StartEpisodeType`;

**Version [161,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP COLUMN `StartEpisodeNumber`;

**Version [161,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE `CrossRef_AniDB_MAL` DROP COLUMN `CrossRefSource`;

**Version [161,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE `AnimeGroup_User` DROP COLUMN `IsFave`;

#### CodedCommand Entries in `_patchCommands`

- **[39,1]** `DatabaseFixes.PopulateTagWeight`
- **[53,1]** `DatabaseFixes.DeleteSeriesUsersWithoutSeries`
- **[73,1]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[74,2]** `DatabaseFixes.UpdateAllStats`
- **[76,3]** `DatabaseFixes.MigrateAniDB_AnimeUpdates`
- **[93,2]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[96,2]** `DatabaseFixes.FixWatchDates`
- **[107,10]** `DatabaseFixes.FixTagParentIDsAndNameOverrides`
- **[113,1]** `DatabaseFixes.FixEpisodeDateTimeUpdated`
- **[114,3]** `DatabaseFixes.UpdateSeriesWithHiddenEpisodes`
- **[118,1]** `DatabaseFixes.FixAnimeSourceLinks`
- **[118,2]** `DatabaseFixes.FixOrphanedShokoEpisodes`
- **[119,4]** `DatabaseFixes.MigrateGroupFilterToFilterPreset`
- **[119,5]** `DatabaseFixes.DropGroupFilter`
- **[129,33]** `DatabaseFixes.CleanupAfterAddingTMDB`
- **[139,11]** `DatabaseFixes.CleanupAfterRemovingTvDB`
- **[139,12]** `DatabaseFixes.ClearQuartzQueue`
- **[140,1]** `DatabaseFixes.RepairMissingTMDBPersons`
- **[148,3]** `DatabaseFixes.RecreateAnimeCharactersAndCreators`
- **[150,12]** `DatabaseFixes.ScheduleTmdbImageUpdates`
- **[152,2]** `DatabaseFixes.MoveTmdbImagesOnDisc`
- **[157,6]** `DatabaseFixes.ClearQuartzQueue`
- **[158,4]** `DatabaseFixes.MoveAnidbFileDataToReleaseInfoFormat`
- **[159,4]** `DatabaseFixes.MigrateRenamers`
- **[159,20]** `DatabaseFixes.MigrateAnidbVotes`
- **[159,21]** `DatabaseFixes.RefreshAnimeSeriesUserStats`
- **[161,6]** `DatabaseFixes.EnsureNoOrphanedGroupsOrSeries`

### Helper Functions (PostDatabaseFix)

#### Tuple<bool, string> Functions
- `MySQLFixUTF8MB4`
- `SetDefaultCollationToUTF8MB4`

---

## SQLServer
**Provider class**: `SQLServer`
**File**: `Shoko.Server/Databases/SQLServer.cs`

### Version Table Creation (`_createVersionTable`)
- **[0,1]** `NormalCommand`: CREATE TABLE [Versions]( [VersionsID] [int] IDENTITY(1,1) NOT NULL, [VersionType] [varchar](100) NOT NULL, [VersionValue...
- **[0,2]** `NormalCommand`: CREATE UNIQUE INDEX UIX_Versions_VersionType ON Versions(VersionType)

### Version Table Schema Updates (`_updateVersionTable`)
- **[0,3]** `NormalCommand`: ALTER TABLE Versions ADD VersionRevision varchar(100) NULL;
- **[0,4]** `NormalCommand`: ALTER TABLE Versions ADD VersionCommand nvarchar(max) NULL;
- **[0,5]** `NormalCommand`: ALTER TABLE Versions ADD VersionProgram varchar(100) NULL;
- **[0,6]** `NormalCommand`: DROP INDEX UIX_Versions_VersionType ON Versions;
- **[0,7]** `NormalCommand`: CREATE INDEX IX_Versions_VersionType ON Versions(VersionType,VersionValue,VersionRevision);

### Initial Table Creation (`_createTables`)
**Total commands**: 110
**Version range**: [1, 110]

**Tables created**: 57
- [1,1] `AniDB_Anime`
- [1,3] `AniDB_Anime_Category`
- [1,6] `AniDB_Anime_Character`
- [1,9] `AniDB_Anime_Relation`
- [1,12] `AniDB_Anime_Review`
- [1,15] `AniDB_Anime_Similar`
- [1,18] `AniDB_Anime_Tag`
- [1,21] `AniDB_Anime_Title`
- [1,23] `AniDB_Category`
- [1,25] `AniDB_Character`
- [1,27] `AniDB_Character_Seiyuu`
- [1,31] `AniDB_Seiyuu`
- [1,33] `AniDB_Episode`
- [1,36] `AniDB_File`
- [1,39] `AniDB_GroupStatus`
- [1,42] `AniDB_ReleaseGroup`
- [1,44] `AniDB_Review`
- [1,46] `AniDB_Tag`
- [1,48] `AnimeEpisode`
- [1,51] `AnimeGroup`
- [1,52] `AnimeSeries`
- [1,54] `CommandRequest`
- [1,55] `CrossRef_AniDB_Other`
- [1,57] `CrossRef_AniDB_TvDB`
- [1,60] `CrossRef_File_Episode`
- [1,62] `CrossRef_Languages_AniDB_File`
- [1,63] `CrossRef_Subtitles_AniDB_File`
- [1,64] `FileNameHash`
- [1,66] `Language`
- [1,68] `ImportFolder`
- [1,69] `ScheduledUpdate`
- [1,71] `VideoInfo`
- [1,73] `VideoLocal`
- [1,75] `DuplicateFile`
- [1,76] `GroupFilter`
- [1,77] `GroupFilterCondition`
- [1,78] `AniDB_Vote`
- [1,79] `TvDB_ImageFanart`
- [1,81] `TvDB_ImageWideBanner`
- [1,83] `TvDB_ImagePoster`
- [1,85] `TvDB_Episode`
- [1,87] `TvDB_Series`
- [1,89] `AniDB_Anime_DefaultImage`
- [1,91] `MovieDB_Movie`
- [1,93] `MovieDB_Poster`
- [1,94] `MovieDB_Fanart`
- [1,95] `JMMUser`
- [1,96] `Trakt_Episode`
- [1,97] `Trakt_ImagePoster`
- [1,98] `Trakt_ImageFanart`
- [1,99] `Trakt_Show`
- [1,100] `Trakt_Season`
- [1,101] `CrossRef_AniDB_Trakt`
- [1,102] `AnimeEpisode_User`
- [1,105] `AnimeSeries_User`
- [1,107] `AnimeGroup_User`
- [1,109] `VideoLocal_User`

**Indexes created**: 13
- [1,4] `IX_AniDB_Anime_Category_AnimeID`
- [1,7] `IX_AniDB_Anime_Character_AnimeID`
- [1,10] `IX_AniDB_Anime_Relation_AnimeID`
- [1,13] `IX_AniDB_Anime_Review_AnimeID`
- [1,16] `IX_AniDB_Anime_Similar_AnimeID`
- [1,19] `IX_AniDB_Anime_Tag_AnimeID`
- [1,22] `IX_AniDB_Anime_Title_AnimeID`
- [1,28] `IX_AniDB_Character_Seiyuu_CharID`
- [1,29] `IX_AniDB_Character_Seiyuu_SeiyuuID`
- [1,34] `IX_AniDB_Episode_AnimeID`
- [1,40] `IX_AniDB_GroupStatus_AnimeID`
- [1,50] `IX_AnimeEpisode_AnimeSeriesID`
- [1,104] `IX_AnimeEpisode_User_User_AnimeSeriesID`

### Patch Commands (`_patchCommands`)
**Total commands**: 525
- **NormalCommand**: 460
- **CodedCommand**: 27
- **DataMigration** (excluded from schema catalog): 38

#### Schema Changes by Version

**Version [2,1]** (1 commands):
  **CREATE TABLE:**
  - `IgnoreAnime`

**Version [2,2]** (1 commands):

**Version [3,1]** (1 commands):
  **CREATE TABLE:**
  - `Trakt_Friend`

**Version [3,2]** (1 commands):

**Version [4,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD DefaultAnimeSeriesID int NULL

**Version [5,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD CanEditServerSettings int NULL

**Version [6,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD VideoBitDepth varchar(max) NULL

**Version [7,3]** (1 commands):

**Version [7,4]** (1 commands):

**Version [7,5]** (1 commands):

**Version [8,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ALTER COLUMN Password NVARCHAR(150) NULL

**Version [9,1]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder ADD IsWatched int NULL

**Version [9,3]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder ALTER COLUMN IsWatched int NOT NULL

**Version [10,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [10,2]** (1 commands):

**Version [10,3]** (1 commands):

**Version [11,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_MAL_AnimeID`

**Version [11,2]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_MAL_MALID`

**Version [11,3]** (1 commands):
  **DROP TABLE:**
  - `dbo`

**Version [11,4]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_MAL`

**Version [11,5]** (1 commands):

**Version [11,6]** (1 commands):

**Version [12,1]** (1 commands):
  **CREATE TABLE:**
  - `Playlist`

**Version [13,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD SeriesNameOverride nvarchar(500) NULL

**Version [14,1]** (1 commands):
  **CREATE TABLE:**
  - `BookmarkedAnime`

**Version [14,2]** (1 commands):

**Version [15,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD DateTimeCreated datetime NULL

**Version [15,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ALTER COLUMN DateTimeCreated datetime NOT NULL

**Version [16,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [16,2]** (1 commands):

**Version [17,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_MylistStats`

**Version [18,1]** (1 commands):
  **CREATE TABLE:**
  - `FileFfdshowPreset`

**Version [18,2]** (1 commands):

**Version [19,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD DisableExternalLinksFlag int NULL

**Version [19,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ALTER COLUMN DisableExternalLinksFlag int NOT NULL

**Version [20,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD FileVersion int NULL

**Version [20,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ALTER COLUMN FileVersion int NOT NULL

**Version [21,1]** (1 commands):
  **CREATE TABLE:**
  - `RenameScript`

**Version [22,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsCensored int NULL

**Version [22,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsDeprecated int NULL

**Version [22,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD InternalVersion int NULL

**Version [22,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ALTER COLUMN IsCensored int NOT NULL

**Version [22,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ALTER COLUMN IsDeprecated int NOT NULL

**Version [22,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ALTER COLUMN InternalVersion int NOT NULL

**Version [23,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD IsVariation int NULL

**Version [23,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ALTER COLUMN IsVariation int NOT NULL

**Version [24,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Recommendation`

**Version [24,2]** (1 commands):

**Version [26,1]** (1 commands):
  **CREATE TABLE:**
  - `LogMessage`

**Version [27,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDBV2`

**Version [27,2]** (1 commands):

**Version [28,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD Locked int NULL

**Version [29,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoInfo`: ALTER TABLE VideoInfo ADD FullInfo varchar(max) NULL

**Version [30,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TraktV2`

**Version [30,2]** (1 commands):

**Version [31,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_Trakt_Episode`

**Version [31,2]** (1 commands):

**Version [33,1]** (1 commands):
  **CREATE TABLE:**
  - `CustomTag`

**Version [33,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_CustomTag`

**Version [34,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD Weight int NULL

**Version [36,1]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Episode`: ALTER TABLE Trakt_Episode ADD TraktID int NULL

**Version [38,1]** (1 commands):
  **DROP TABLE:**
  - `LogMessage`

**Version [39,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DefaultFolder nvarchar(max) NULL

**Version [40,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexUsers nvarchar(max) NULL

**Version [41,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD FilterType int NULL

**Version [41,3]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ALTER COLUMN FilterType int NOT NULL

**Version [42,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractVersion int NOT NULL DEFAULT(0), ContractString nvarchar(MAX) NU...

**Version [42,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractVersion int NOT NULL DEFAULT(0), ContractString nvarchar(MAX) NUL...

**Version [42,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractVersion int NOT NULL DEFAULT(0), PlexContractString nvar...

**Version [42,4]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractVersion int NOT NULL DEFAULT(0), ContractString nvarchar(MAX) NU...

**Version [42,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractVersion int NOT NULL DEFAULT(0), PlexContractString nva...

**Version [42,6]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupsIdsVersion int NOT NULL DEFAULT(0), GroupsIdsString nvarchar(MAX) ...

**Version [42,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractVersion int NOT NULL DEFAULT(0), ContractString nvarchar(M...

**Version [43,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractVersion int NOT NULL DEFAULT(0), PlexContractString nvarcha...

**Version [43,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaVersion int NOT NULL DEFAULT(0), MediaString nvarchar(MAX) NULL

**Version [44,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User DROP COLUMN KodiContractVersion

**Version [44,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User DROP COLUMN KodiContractString

**Version [44,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User DROP COLUMN KodiContractVersion

**Version [44,6]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User DROP COLUMN KodiContractString

**Version [45,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD LatestEpisodeAirDate [datetime] NULL

**Version [45,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD LatestEpisodeAirDate [datetime] NULL

**Version [46,1]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD GroupConditionsVersion int NOT NULL DEFAULT(0), GroupConditions nvarchar...

**Version [46,2]** (1 commands):
  **ALTER TABLE:**
  - `GroupFilter`: ALTER TABLE GroupFilter ADD SeriesIdsVersion int NOT NULL DEFAULT(0), SeriesIdsString nvarchar(MAX) ...

**Version [47,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractBlob varbinary(MAX) NULL

**Version [47,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD ContractSize int NOT NULL DEFAULT(0)

**Version [47,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN ContractString

**Version [47,4]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaBlob varbinary(MAX) NULL

**Version [47,5]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MediaSize int NOT NULL DEFAULT(0)

**Version [47,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN MediaString

**Version [47,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractBlob varbinary(MAX) NULL

**Version [47,8]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD PlexContractSize int NOT NULL DEFAULT(0)

**Version [47,9]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode DROP COLUMN PlexContractString

**Version [47,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractBlob varbinary(MAX) NULL

**Version [47,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD ContractSize int NOT NULL DEFAULT(0)

**Version [47,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User DROP COLUMN ContractString

**Version [47,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractBlob varbinary(MAX) NULL

**Version [47,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD ContractSize int NOT NULL DEFAULT(0)

**Version [47,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries DROP COLUMN ContractString

**Version [47,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractBlob varbinary(MAX) NULL

**Version [47,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD PlexContractSize int NOT NULL DEFAULT(0)

**Version [47,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User DROP COLUMN PlexContractString

**Version [47,19]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractBlob varbinary(MAX) NULL

**Version [47,20]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ADD PlexContractSize int NOT NULL DEFAULT(0)

**Version [47,21]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User DROP COLUMN PlexContractString

**Version [47,22]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractBlob varbinary(MAX) NULL

**Version [47,23]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD ContractSize int NOT NULL DEFAULT(0)

**Version [47,24]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup DROP COLUMN ContractString

**Version [48,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AllCategories

**Version [50,1]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_Place`

**Version [50,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD FileName nvarchar(max) NOT NULL DEFAULT(''), VideoCodec varchar(max) NOT ...

**Version [50,4]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal DROP COLUMN FilePath, ImportFolderID

**Version [50,6]** (1 commands):
  **CREATE TABLE:**
  - `CloudAccount`

**Version [50,7]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder ADD CloudID int NULL

**Version [50,8]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ALTER COLUMN WatchedDate datetime NULL

**Version [50,9]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD ResumePosition bigint NOT NULL DEFAULT (0)

**Version [50,10]** (1 commands):
  **DROP TABLE:**
  - `VideoInfo`

**Version [51,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_VideoLocal_Hash`

**Version [51,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_VideoLocal_Hash`

**Version [52,1]** (1 commands):
  **CREATE TABLE:**
  - `AuthTokens`

**Version [53,1]** (1 commands):
  **CREATE TABLE:**
  - `Scan`

**Version [53,2]** (1 commands):
  **CREATE TABLE:**
  - `ScanFile`

**Version [53,3]** (1 commands):
  **CREATE INDEX:**
  - `UIX_ScanFileStatus`

**Version [57,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD PlexToken nvarchar(max) NULL

**Version [58,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File ADD IsChaptered INT NOT NULL DEFAULT(-1)

**Version [59,1]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD RenamerType nvarchar(max) NOT NULL DEFAULT('Legacy')

**Version [59,2]** (1 commands):
  **ALTER TABLE:**
  - `RenameScript`: ALTER TABLE RenameScript ADD ExtraData nvarchar(max)

**Version [60,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Character_CharID`

**Version [61,1]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE TvDB_Episode ADD Rating INT NULL

**Version [61,2]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE TvDB_Episode ADD AirDate datetime NULL

**Version [61,3]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Episode`: ALTER TABLE TvDB_Episode DROP COLUMN FirstAired

**Version [62,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD AirsOn varchar(10) NULL

**Version [63,1]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImageFanart`

**Version [63,2]** (1 commands):
  **DROP TABLE:**
  - `Trakt_ImagePoster`

**Version [64,1]** (1 commands):
  **CREATE TABLE:**
  - `AnimeCharacter`

**Version [64,2]** (1 commands):
  **CREATE TABLE:**
  - `AnimeStaff`

**Version [64,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_Anime_Staff`

**Version [65,1]** (1 commands):
  **ALTER TABLE:**
  - `MovieDB_Movie`: ALTER TABLE MovieDB_Movie ADD Rating INT NOT NULL DEFAULT(0)

**Version [65,2]** (1 commands):
  **ALTER TABLE:**
  - `TvDB_Series`: ALTER TABLE TvDB_Series ADD Rating INT NULL

**Version [66,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode ADD Description nvarchar(max) NOT NULL DEFAULT('')

**Version [70,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character ALTER COLUMN CharName nvarchar(max) NOT NULL

**Version [71,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_AnimeUpdate`

**Version [71,2]** (1 commands):

**Version [75,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_MAL_Anime`

**Version [75,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD Site_JP nvarchar(max), Site_EN nvarchar(max), Wikipedia_ID nvarchar(max)...

**Version [76,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD MyListID INT NOT NULL DEFAULT(0)

**Version [77,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode DROP COLUMN EnglishName

**Version [77,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode DROP COLUMN RomajiName

**Version [77,3]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_Title`

**Version [78,1]** (1 commands):
  **DROP INDEX:**
  - `UIX_CrossRef_AniDB_TvDB_Episode_AniDBEpisodeID`

**Version [78,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`: ALTER TABLE CrossRef_AniDB_TvDB_Episode_Override DROP COLUMN AnimeID

**Version [78,5]** (1 commands):

**Version [78,6]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [78,7]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [78,8]** (1 commands):

**Version [78,9]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [78,10]** (1 commands):

**Version [81,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD UpdatedAt datetime NOT NULL DEFAULT '2000-01-01 00:00:00';

**Version [84,1]** (1 commands):
  **DROP INDEX:**
  - `IF`

**Version [85,1]** (1 commands):
  **DROP INDEX:**
  - `IF`

**Version [86,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [88,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD WatchedCount INT NOT NULL DEFAULT 0;

**Version [88,3]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ADD LastUpdated datetime NOT NULL DEFAULT CURRENT_TIMESTAMP;

**Version [89,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD LastEpisodeUpdate datetime DEFAULT NULL;

**Version [90,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ADD MainAniDBAnimeID INT DEFAULT NULL;

**Version [91,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User DROP COLUMN ContractSize;

**Version [91,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User DROP COLUMN ContractBlob;

**Version [91,4]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User DROP COLUMN ContractVersion;

**Version [92,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File DROP COLUMN File_AudioCodec, File_VideoCodec, File_VideoResolution, File_File...

**Version [92,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File Alter COLUMN IsCensored bit NULL; ALTER TABLE AniDB_File ALTER COLUMN IsDepre...

**Version [92,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_GroupStatus`: ALTER TABLE AniDB_GroupStatus Alter COLUMN Rating decimal(6,2) NULL; UPDATE AniDB_GroupStatus SET Ra...

**Version [92,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character DROP COLUMN CreatorListRaw;

**Version [92,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Character`: ALTER TABLE AniDB_Anime_Character DROP COLUMN EpisodeListRaw;

**Version [92,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AwardList;

**Version [92,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_File`: ALTER TABLE AniDB_File DROP COLUMN AnimeID;

**Version [93,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE CrossRef_Languages_AniDB_File ADD LanguageName nvarchar(100) NOT NULL DEFAULT '';

**Version [93,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Languages_AniDB_File`: ALTER TABLE CrossRef_Languages_AniDB_File DROP COLUMN LanguageID;

**Version [93,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE CrossRef_Subtitles_AniDB_File ADD LanguageName nvarchar(100) NOT NULL DEFAULT '';

**Version [93,6]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_Subtitles_AniDB_File`: ALTER TABLE CrossRef_Subtitles_AniDB_File DROP COLUMN LanguageID;

**Version [93,7]** (1 commands):
  **DROP TABLE:**
  - `Language`

**Version [94,1]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Category`

**Version [94,2]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_Review`

**Version [94,3]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Category`

**Version [94,4]** (1 commands):
  **DROP TABLE:**
  - `AniDB_MylistStats`

**Version [94,5]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Review`

**Version [94,6]** (1 commands):
  **DROP TABLE:**
  - `CloudAccount`

**Version [94,7]** (1 commands):
  **DROP TABLE:**
  - `FileFfdshowPreset`

**Version [94,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Trakt`

**Version [94,9]** (1 commands):
  **DROP TABLE:**
  - `Trakt_Friend`

**Version [94,10]** (1 commands):

**Version [96,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_GroupStatus`: ALTER TABLE AniDB_GroupStatus ALTER COLUMN GroupName nvarchar(max); ALTER TABLE AniDB_GroupStatus AL...

**Version [97,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Episode_EpisodeType`

**Version [98,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode_Title`: ALTER TABLE AniDB_Episode_Title ALTER COLUMN Title nvarchar(max) NOT NULL;

**Version [99,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD DateTimeImported datetime DEFAULT NULL;

**Version [100,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD Verified integer NOT NULL DEFAULT 0;

**Version [100,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD ParentTagID integer DEFAULT NULL;

**Version [100,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD TagNameOverride varchar(150) DEFAULT NULL;

**Version [100,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag ADD LastUpdated datetime NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [100,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN Spoiler;

**Version [100,6]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN LocalSpoiler;

**Version [100,7]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Tag`: ALTER TABLE AniDB_Tag DROP COLUMN TagCount;

**Version [100,8]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag ADD LocalSpoiler integer NOT NULL DEFAULT 0;

**Version [100,9]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime_Tag`: ALTER TABLE AniDB_Anime_Tag DROP COLUMN Approval;

**Version [101,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD IsHidden integer NOT NULL DEFAULT 0;

**Version [101,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD HiddenUnwatchedEpisodeCount integer NOT NULL DEFAULT 0;

**Version [103,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_FileUpdate`

**Version [103,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_FileUpdate`

**Version [104,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN DisableExternalLinksFlag;

**Version [104,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD DisableAutoMatchFlags integer NOT NULL DEFAULT 0;

**Version [104,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD VNDBID int, BangumiID int, LianID int, FunimationID nvarchar(max), HiDiv...

**Version [105,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN LianID;

**Version [105,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimePlanetID;

**Version [105,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime DROP COLUMN AnimeNfo;

**Version [105,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ADD LainID INT NULL

**Version [107,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCount int NOT NULL DEFAULT 0;

**Version [107,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ADD HiddenMissingEpisodeCountGroups int NOT NULL DEFAULT 0;

**Version [109,1]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageBlob VARBINARY(MAX) NULL;

**Version [109,2]** (1 commands):
  **ALTER TABLE:**
  - `JMMUser`: ALTER TABLE JMMUser ADD AvatarImageMetadata NVARCHAR(128) NULL;

**Version [110,1]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumped datetime;

**Version [110,2]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ADD LastAVDumpVersion nvarchar(128);

**Version [112,1]** (1 commands):
  **CREATE TABLE:**
  - `FilterPreset`

**Version [112,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_FilterPreset_ParentFilterPresetID`

**Version [113,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup DROP COLUMN SortName;

**Version [114,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode DROP COLUMN PlexContractBlob;ALTER TABLE AnimeGroup_User DROP COLUMN PlexCo...

**Version [115,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CommandRequest_CommandType`

**Version [116,1]** (1 commands):
  **DROP TABLE:**
  - `CommandRequest`

**Version [117,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ADD EpisodeNameOverride nvarchar(500) NULL

**Version [121,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_NotifyQueue`

**Version [121,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Message`

**Version [122,1]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Episode`

**Version [122,2]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`

**Version [122,3]** (1 commands):
  **CREATE TABLE:**
  - `CrossRef_AniDB_TMDB_Show`

**Version [122,4]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image`

**Version [122,5]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_PreferredImage`

**Version [122,6]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Title`

**Version [122,7]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Overview`

**Version [122,8]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company`

**Version [122,9]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Network`

**Version [122,10]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Person`

**Version [122,11]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie`

**Version [122,12]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Cast`

**Version [122,13]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Company_Entity`

**Version [122,14]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Movie_Crew`

**Version [122,15]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Show`

**Version [122,16]** (1 commands):
  **CREATE TABLE:**
  - `Tmdb_Show_Network`

**Version [122,17]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Season`

**Version [122,18]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode`

**Version [122,19]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Cast`

**Version [122,20]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Episode_Crew`

**Version [122,21]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering`

**Version [122,22]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Season`

**Version [122,23]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_AlternateOrdering_Episode`

**Version [122,24]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection`

**Version [122,25]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Collection_Movie`

**Version [122,27]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_Other`

**Version [122,28]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Fanart`

**Version [122,29]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Movie`

**Version [122,30]** (1 commands):
  **DROP TABLE:**
  - `MovieDB_Poster`

**Version [122,31]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Anime_DefaultImage`

**Version [122,32]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Episode_PreferredImage`

**Version [124,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD TvdbShowID INT NULL DEFAULT NULL;

**Version [124,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD TvdbEpisodeID INT NULL DEFAULT NULL;

**Version [124,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD ImdbMovieID INT NULL DEFAULT NULL;

**Version [124,5]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Overview`

**Version [124,6]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Title`

**Version [124,7]** (1 commands):

**Version [124,8]** (1 commands):

**Version [125,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie ALTER COLUMN AnidbEpisodeID INT NOT NULL;

**Version [125,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie ADD CONSTRAINT DF_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID DEF...

**Version [126,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD PosterPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [126,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD BackdropPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [126,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD PosterPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [126,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD BackdropPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [128,1]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Creator`

**Version [128,2]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character_Creator`

**Version [128,3]** (1 commands):

**Version [128,4]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CreatorID`

**Version [128,5]** (1 commands):
  **CREATE INDEX:**
  - `UIX_AniDB_Character_Creator_CharacterID`

**Version [128,6]** (1 commands):

**Version [128,9]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Seiyuu`

**Version [128,10]** (1 commands):
  **DROP TABLE:**
  - `AniDB_Character_Seiyuu`

**Version [129,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD PreferredAlternateOrderingID NVARCHAR(64) NULL DEFAULT NULL;

**Version [130,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ALTER COLUMN ContentRatings NVARCHAR(512) NOT NULL;

**Version [130,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ALTER COLUMN ContentRatings NVARCHAR(512) NOT NULL;

**Version [131,1]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Episode`

**Version [131,2]** (1 commands):
  **DROP TABLE:**
  - `TvDB_Series`

**Version [131,3]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageFanart`

**Version [131,4]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImagePoster`

**Version [131,5]** (1 commands):
  **DROP TABLE:**
  - `TvDB_ImageWideBanner`

**Version [131,6]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB`

**Version [131,7]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode`

**Version [131,8]** (1 commands):
  **DROP TABLE:**
  - `CrossRef_AniDB_TvDB_Episode_Override`

**Version [131,9]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show DROP COLUMN TvDB_ID;

**Version [131,10]** (1 commands):
  **ALTER TABLE:**
  - `Trakt_Show`: ALTER TABLE Trakt_Show ADD TmdbShowID INT NULL;

**Version [133,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD Keywords NVARCHAR(512) NULL DEFAULT NULL;

**Version [133,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ADD ProductionCountries NVARCHAR(32) NULL DEFAULT NULL;

**Version [133,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD Keywords NVARCHAR(512) NULL DEFAULT NULL;

**Version [133,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD ProductionCountries NVARCHAR(32) NULL DEFAULT NULL;

**Version [134,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Relation_RelatedAnimeID`

**Version [135,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ALTER COLUMN ProductionCountries NVARCHAR(255) NULL;

**Version [135,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ALTER COLUMN ProductionCountries NVARCHAR(255) NULL;

**Version [136,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbSeasonID`

**Version [136,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_TmdbShowID`

**Version [137,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD IsHidden int NOT NULL DEFAULT 0;

**Version [137,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ADD HiddenEpisodeCount int NOT NULL DEFAULT 0;

**Version [137,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ADD HiddenEpisodeCount int NOT NULL DEFAULT 0;

**Version [137,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Season`: ALTER TABLE TMDB_AlternateOrdering_Season ADD HiddenEpisodeCount int NOT NULL DEFAULT 0;

**Version [137,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering`: ALTER TABLE TMDB_AlternateOrdering ADD HiddenEpisodeCount int NOT NULL DEFAULT 0;

**Version [138,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Person`: ALTER TABLE TMDB_Person ALTER COLUMN CreatedAt datetime2;

**Version [138,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Person`: ALTER TABLE TMDB_Person ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ALTER COLUMN CreatedAt datetime2;

**Version [138,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ALTER COLUMN CreatedAt datetime2;

**Version [138,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ALTER COLUMN CreatedAt datetime2;

**Version [138,8]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,9]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ALTER COLUMN CreatedAt datetime2;

**Version [138,10]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,11]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering`: ALTER TABLE TMDB_AlternateOrdering ALTER COLUMN CreatedAt datetime2;

**Version [138,12]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering`: ALTER TABLE TMDB_AlternateOrdering ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,13]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Season`: ALTER TABLE TMDB_AlternateOrdering_Season ALTER COLUMN CreatedAt datetime2;

**Version [138,14]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Season`: ALTER TABLE TMDB_AlternateOrdering_Season ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,15]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Episode`: ALTER TABLE TMDB_AlternateOrdering_Episode ALTER COLUMN CreatedAt datetime2;

**Version [138,16]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_AlternateOrdering_Episode`: ALTER TABLE TMDB_AlternateOrdering_Episode ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,17]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Collection`: ALTER TABLE TMDB_Collection ALTER COLUMN CreatedAt datetime2;

**Version [138,18]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Collection`: ALTER TABLE TMDB_Collection ALTER COLUMN LastUpdatedAt datetime2;

**Version [138,20]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Creator`: ALTER TABLE AniDB_Creator ALTER COLUMN LastUpdatedAt datetime2;

**Version [139,1]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID`

**Version [139,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbAnimeID_TmdbShowID`

**Version [139,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID`

**Version [139,4]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_AnidbEpisodeID_TmdbEpisodeID`

**Version [139,5]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbEpisodeID`

**Version [139,6]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Episode_TmdbShowID`

**Version [139,7]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbAnimeID`

**Version [139,8]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID`

**Version [139,9]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_AnidbEpisodeID_TmdbMovieID`

**Version [139,10]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Movie_TmdbMovieID`

**Version [139,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID`

**Version [139,12]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_AnidbAnimeID_TmdbShowID`

**Version [139,13]** (1 commands):
  **CREATE INDEX:**
  - `IX_CrossRef_AniDB_TMDB_Show_TmdbShowID`

**Version [139,14]** (1 commands):

**Version [139,15]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbEpisodeGroupCollectionID`

**Version [139,16]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_Season_TmdbShowID`

**Version [139,17]** (1 commands):

**Version [139,18]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbEpisodeGroupCollectionID_TmdbShowID`

**Version [139,19]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_AlternateOrdering_TmdbShowID`

**Version [139,20]** (1 commands):

**Version [139,21]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbCollectionID`

**Version [139,22]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Collection_Movie_TmdbMovieID`

**Version [139,23]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbCompanyID`

**Version [139,24]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_Entity_TmdbEntityType_TmdbEntityID`

**Version [139,25]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Company_TmdbCompanyID`

**Version [139,26]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbEpisodeID`

**Version [139,27]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbPersonID`

**Version [139,28]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbSeasonID`

**Version [139,29]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Cast_TmdbShowID`

**Version [139,30]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbEpisodeID`

**Version [139,31]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbPersonID`

**Version [139,32]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbSeasonID`

**Version [139,33]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Episode_Crew_TmdbShowID`

**Version [139,34]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbMovieID`

**Version [139,35]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Cast_TmdbPersonID`

**Version [139,36]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbMovieID`

**Version [139,37]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_Crew_TmdbPersonID`

**Version [139,38]** (1 commands):

**Version [139,39]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Movie_TmdbCollectionID`

**Version [139,40]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Person_TmdbPersonID`

**Version [139,41]** (1 commands):

**Version [139,42]** (1 commands):
  **CREATE INDEX:**
  - `IX_TMDB_Season_TmdbShowID`

**Version [139,43]** (1 commands):

**Version [140,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [140,7]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Character`

**Version [140,8]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Staff`

**Version [140,9]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character`

**Version [140,10]** (1 commands):
  **CREATE TABLE:**
  - `AniDB_Anime_Character_Creator`

**Version [140,11]** (1 commands):
  **CREATE INDEX:**
  - `IX_AniDB_Anime_Staff_CreatorID`

**Version [141,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character ADD Type int NOT NULL DEFAULT 0;

**Version [141,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Character`: ALTER TABLE AniDB_Character ADD LastUpdated datetime2 NOT NULL DEFAULT '1970-01-01 00:00:00';

**Version [142,1]** (1 commands):
  **CREATE TABLE:**
  - `TMDB_Image_Entity`

**Version [142,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbMovieID;

**Version [142,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbEpisodeID;

**Version [142,4]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbSeasonID;

**Version [142,5]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbShowID;

**Version [142,6]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbCollectionID;

**Version [142,7]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbNetworkID;

**Version [142,8]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbCompanyID;

**Version [142,9]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN TmdbPersonID;

**Version [142,10]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN ForeignType;

**Version [142,11]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Image`: ALTER TABLE TMDB_Image DROP COLUMN ImageType;

**Version [143,1]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Season`: ALTER TABLE TMDB_Season ADD PosterPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [143,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Episode`: ALTER TABLE TMDB_Episode ADD ThumbnailPath NVARCHAR(64) NULL DEFAULT NULL;

**Version [145,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [145,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [146,2]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Show`: ALTER TABLE TMDB_Show ALTER COLUMN Keywords NVARCHAR(MAX) NULL;

**Version [146,3]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Movie`: ALTER TABLE TMDB_Movie ALTER COLUMN Keywords NVARCHAR(MAX) NULL;

**Version [148,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie ADD MatchRating int NOT NULL DEFAULT 1;

**Version [148,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Movie`: ALTER TABLE CrossRef_AniDB_TMDB_Movie DROP COLUMN Source;

**Version [148,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE CrossRef_AniDB_TMDB_Show ADD MatchRating int NOT NULL DEFAULT 1;

**Version [148,6]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_TMDB_Show`: ALTER TABLE CrossRef_AniDB_TMDB_Show DROP COLUMN Source;

**Version [149,1]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [149,2]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [149,3]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [149,4]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [149,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [150,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo`

**Version [150,2]** (1 commands):
  **CREATE TABLE:**
  - `StoredReleaseInfo_MatchAttempt`

**Version [150,3]** (1 commands):
  **CREATE TABLE:**
  - `VideoLocal_HashDigest`

**Version [150,5]** (1 commands):
  **ALTER TABLE:**
  - `ImportFolder`: ALTER TABLE ImportFolder DROP COLUMN ImportFolderType;

**Version [150,6]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal_Place`: ALTER TABLE VideoLocal_Place DROP COLUMN ImportFolderType;

**Version [151,1]** (1 commands):
  **CREATE TABLE:**
  - `StoredRelocationPipe`

**Version [151,2]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_ProviderID`

**Version [151,3]** (1 commands):
  **CREATE INDEX:**
  - `IX_StoredRelocationPipe_Name`

**Version [151,5]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [151,6]** (1 commands):
  **DROP TABLE:**
  - `IF`

**Version [151,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD AbsoluteUserRating INT;

**Version [151,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD UserRatingVoteType INT;

**Version [151,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD IsFavorite INT NOT NULL DEFAULT 0;

**Version [151,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD LastVideoUpdate datetime2;

**Version [151,14]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD LastUpdated datetime2 NOT NULL DEFAULT '0001-01-01 00:00:00';

**Version [151,15]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD UserTags TEXT NOT NULL DEFAULT '';

**Version [151,16]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD AbsoluteUserRating INT;

**Version [151,17]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD IsFavorite INT NOT NULL DEFAULT 0;

**Version [151,18]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD LastUpdated datetime2 NOT NULL DEFAULT '0001-01-01 00:00:00';

**Version [151,19]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ADD UserTags TEXT NOT NULL DEFAULT '';

**Version [151,22]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Person`: ALTER TABLE TMDB_Person ADD LastOrphanedAt DATETIME2;

**Version [151,23]** (1 commands):
  **ALTER TABLE:**
  - `TMDB_Network`: ALTER TABLE TMDB_Network ADD LastOrphanedAt DATETIME2;

**Version [153,1]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode_User`: ALTER TABLE AnimeEpisode_User ALTER COLUMN WatchedDate datetime2 NULL;

**Version [153,2]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User ALTER COLUMN WatchedDate datetime2 NULL;

**Version [153,3]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ALTER COLUMN WatchedDate datetime2 NULL;

**Version [153,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ALTER COLUMN LastEpisodeUpdate datetime2 NULL;

**Version [153,6]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries_User`: ALTER TABLE AnimeSeries_User ADD DEFAULT NULL FOR LastEpisodeUpdate;

**Version [154,1]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN MALTitle;

**Version [154,2]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN StartEpisodeType;

**Version [154,3]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN StartEpisodeNumber;

**Version [154,4]** (1 commands):
  **ALTER TABLE:**
  - `CrossRef_AniDB_MAL`: ALTER TABLE CrossRef_AniDB_MAL DROP COLUMN CrossRefSource;

**Version [154,5]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup_User`: ALTER TABLE AnimeGroup_User DROP COLUMN IsFave;

**Version [155,1]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ALTER COLUMN AirDate datetime2 NULL;

**Version [155,2]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ALTER COLUMN EndDate datetime2 NULL;

**Version [155,3]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,4]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Anime`: ALTER TABLE AniDB_Anime ALTER COLUMN DateTimeDescUpdated datetime2 NOT NULL;

**Version [155,5]** (1 commands):
  **ALTER TABLE:**
  - `AniDB_Episode`: ALTER TABLE AniDB_Episode ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,6]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,7]** (1 commands):
  **ALTER TABLE:**
  - `AnimeEpisode`: ALTER TABLE AnimeEpisode ALTER COLUMN DateTimeCreated datetime2 NOT NULL;

**Version [155,8]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,9]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ALTER COLUMN DateTimeCreated datetime2 NOT NULL;

**Version [155,10]** (1 commands):
  **ALTER TABLE:**
  - `AnimeSeries`: ALTER TABLE AnimeSeries ALTER COLUMN EpisodeAddedDate datetime2 NULL;

**Version [155,11]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,12]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ALTER COLUMN DateTimeCreated datetime2 NOT NULL;

**Version [155,13]** (1 commands):
  **ALTER TABLE:**
  - `AnimeGroup`: ALTER TABLE AnimeGroup ALTER COLUMN EpisodeAddedDate datetime2 NULL;

**Version [155,14]** (1 commands):
  **ALTER TABLE:**
  - `FileNameHash`: ALTER TABLE FileNameHash ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,15]** (1 commands):
  **ALTER TABLE:**
  - `ScheduledUpdate`: ALTER TABLE ScheduledUpdate ALTER COLUMN LastUpdate datetime2 NOT NULL;

**Version [155,16]** (1 commands):
  **ALTER TABLE:**
  - `VideoLocal`: ALTER TABLE VideoLocal ALTER COLUMN DateTimeUpdated datetime2 NOT NULL;

**Version [155,17]** (2 commands):
  **ALTER TABLE:**
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ALTER COLUMN WatchedDate datetime2 NULL;
  - `VideoLocal_User`: ALTER TABLE VideoLocal_User ALTER COLUMN LastUpdated datetime2 NOT NULL DEFAULT CURRENT_TIMESTAMP;

**Version [156,1]** (1 commands):
  **ALTER TABLE:**
  - `StoredRelocationPipe`: ALTER TABLE StoredRelocationPipe ADD temp [VARBINARY] (MAX) NULL;

**Version [156,3]** (1 commands):
  **ALTER TABLE:**
  - `StoredRelocationPipe`: ALTER TABLE StoredRelocationPipe DROP COLUMN Configuration;

#### CodedCommand Entries in `_patchCommands`

- **[35,1]** `DatabaseFixes.PopulateTagWeight`
- **[49,1]** `DatabaseFixes.DeleteSeriesUsersWithoutSeries`
- **[67,1]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[68,2]** `DatabaseFixes.UpdateAllStats`
- **[71,3]** `DatabaseFixes.MigrateAniDB_AnimeUpdates`
- **[86,2]** `DatabaseFixes.RefreshAniDBInfoFromXML`
- **[89,2]** `DatabaseFixes.FixWatchDates`
- **[100,10]** `DatabaseFixes.FixTagParentIDsAndNameOverrides`
- **[106,1]** `DatabaseFixes.FixEpisodeDateTimeUpdated`
- **[107,3]** `DatabaseFixes.UpdateSeriesWithHiddenEpisodes`
- **[111,1]** `DatabaseFixes.FixAnimeSourceLinks`
- **[111,2]** `DatabaseFixes.FixOrphanedShokoEpisodes`
- **[112,4]** `DatabaseFixes.MigrateGroupFilterToFilterPreset`
- **[112,5]** `DatabaseFixes.DropGroupFilter`
- **[122,33]** `DatabaseFixes.CleanupAfterAddingTMDB`
- **[131,11]** `DatabaseFixes.CleanupAfterRemovingTvDB`
- **[131,12]** `DatabaseFixes.ClearQuartzQueue`
- **[132,1]** `DatabaseFixes.RepairMissingTMDBPersons`
- **[141,3]** `DatabaseFixes.RecreateAnimeCharactersAndCreators`
- **[142,12]** `DatabaseFixes.ScheduleTmdbImageUpdates`
- **[144,2]** `DatabaseFixes.MoveTmdbImagesOnDisc`
- **[149,6]** `DatabaseFixes.ClearQuartzQueue`
- **[150,4]** `DatabaseFixes.MoveAnidbFileDataToReleaseInfoFormat`
- **[151,4]** `DatabaseFixes.MigrateRenamers`
- **[151,20]** `DatabaseFixes.MigrateAnidbVotes`
- **[151,21]** `DatabaseFixes.RefreshAnimeSeriesUserStats`
- **[154,6]** `DatabaseFixes.EnsureNoOrphanedGroupsOrSeries`

### Helper Functions (PostDatabaseFix)

#### Tuple<bool, string> Functions
- `DropDefaultsOnAnimeEpisode_User`
- `DropDefaultOnChaptered`
- `DropDefaultOnCreatorLastUpdatedAt`
- `DropDefaultOnTMDBShowMovieKeywords`
- `DropLastEpisodeUpdateDefaultOnAnimeSeries_User`

---

