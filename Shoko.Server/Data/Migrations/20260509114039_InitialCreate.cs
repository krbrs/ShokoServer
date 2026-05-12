using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shoko.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AniDB_Anime",
                columns: table => new
                {
                    AniDB_AnimeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AirDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    URL = table.Column<string>(type: "TEXT", nullable: true),
                    Picname = table.Column<string>(type: "TEXT", nullable: true),
                    BeginYear = table.Column<int>(type: "INTEGER", nullable: false),
                    EndYear = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeType = table.Column<int>(type: "INTEGER", nullable: false),
                    MainTitle = table.Column<string>(type: "TEXT", nullable: false),
                    AllTitles = table.Column<string>(type: "TEXT", nullable: false),
                    AllTags = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeCountNormal = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeCountSpecial = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    VoteCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TempRating = table.Column<int>(type: "INTEGER", nullable: false),
                    TempVoteCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AvgReviewRating = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeDescUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImageEnabled = table.Column<int>(type: "INTEGER", nullable: false),
                    Restricted = table.Column<int>(type: "INTEGER", nullable: false),
                    ANNID = table.Column<int>(type: "INTEGER", nullable: true),
                    AllCinemaID = table.Column<int>(type: "INTEGER", nullable: true),
                    AnisonID = table.Column<int>(type: "INTEGER", nullable: true),
                    SyoboiID = table.Column<int>(type: "INTEGER", nullable: true),
                    VNDBID = table.Column<int>(type: "INTEGER", nullable: true),
                    BangumiID = table.Column<int>(type: "INTEGER", nullable: true),
                    LainID = table.Column<int>(type: "INTEGER", nullable: true),
                    Site_JP = table.Column<string>(type: "TEXT", nullable: true),
                    Site_EN = table.Column<string>(type: "TEXT", nullable: true),
                    Wikipedia_ID = table.Column<string>(type: "TEXT", nullable: true),
                    WikipediaJP_ID = table.Column<string>(type: "TEXT", nullable: true),
                    CrunchyrollID = table.Column<string>(type: "TEXT", nullable: true),
                    FunimationID = table.Column<string>(type: "TEXT", nullable: true),
                    HiDiveID = table.Column<string>(type: "TEXT", nullable: true),
                    LatestEpisodeNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime", x => x.AniDB_AnimeID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Character",
                columns: table => new
                {
                    AniDB_Anime_CharacterID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    Appearance = table.Column<string>(type: "TEXT", nullable: false),
                    AppearanceType = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Character", x => x.AniDB_Anime_CharacterID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Character_Creator",
                columns: table => new
                {
                    AniDB_Anime_Character_CreatorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatorID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Character_Creator", x => x.AniDB_Anime_Character_CreatorID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_PreferredImage",
                columns: table => new
                {
                    AniDB_Anime_PreferredImageID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnidbAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageType = table.Column<byte>(type: "INTEGER", nullable: false),
                    ImageSource = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_PreferredImage", x => x.AniDB_Anime_PreferredImageID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Relation",
                columns: table => new
                {
                    AniDB_Anime_RelationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    RelationType = table.Column<string>(type: "TEXT", nullable: false),
                    RelatedAnimeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Relation", x => x.AniDB_Anime_RelationID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Similar",
                columns: table => new
                {
                    AniDB_Anime_SimilarID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    SimilarAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Approval = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Similar", x => x.AniDB_Anime_SimilarID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Staff",
                columns: table => new
                {
                    AniDB_Anime_StaffID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatorID = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    RoleType = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Staff", x => x.AniDB_Anime_StaffID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Tag",
                columns: table => new
                {
                    AniDB_Anime_TagID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TagID = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalSpoiler = table.Column<bool>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Tag", x => x.AniDB_Anime_TagID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Anime_Title",
                columns: table => new
                {
                    AniDB_Anime_TitleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TitleType = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Anime_Title", x => x.AniDB_Anime_TitleID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_AnimeUpdate",
                columns: table => new
                {
                    AniDB_AnimeUpdateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_AnimeUpdate", x => x.AniDB_AnimeUpdateID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Character",
                columns: table => new
                {
                    AniDB_CharacterID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Character", x => x.AniDB_CharacterID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Creator",
                columns: table => new
                {
                    AniDB_CreatorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalName = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    EnglishHomepageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    JapaneseHomepageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    EnglishWikiUrl = table.Column<string>(type: "TEXT", nullable: true),
                    JapaneseWikiUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Creator", x => x.AniDB_CreatorID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Episode",
                columns: table => new
                {
                    AniDB_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    LengthSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<string>(type: "TEXT", nullable: false),
                    Votes = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeType = table.Column<byte>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AirDate = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Episode", x => x.AniDB_EpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Episode_PreferredImage",
                columns: table => new
                {
                    AniDB_Episode_PreferredImageID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnidbAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnidbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageType = table.Column<byte>(type: "INTEGER", nullable: false),
                    ImageSource = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Episode_PreferredImage", x => x.AniDB_Episode_PreferredImageID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Episode_Title",
                columns: table => new
                {
                    AniDB_Episode_TitleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AniDB_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Episode_Title", x => x.AniDB_Episode_TitleID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_GroupStatus",
                columns: table => new
                {
                    AniDB_GroupStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupID = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    CompletionState = table.Column<int>(type: "INTEGER", nullable: false),
                    LastEpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    Votes = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeRange = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_GroupStatus", x => x.AniDB_GroupStatusID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Message",
                columns: table => new
                {
                    AniDB_MessageID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageID = table.Column<int>(type: "INTEGER", nullable: false),
                    FromUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    FromUserName = table.Column<string>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    Flags = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Message", x => x.AniDB_MessageID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_NotifyQueue",
                columns: table => new
                {
                    AniDB_NotifyQueueID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ID = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_NotifyQueue", x => x.AniDB_NotifyQueueID);
                });

            migrationBuilder.CreateTable(
                name: "AniDB_Tag",
                columns: table => new
                {
                    AniDB_TagID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentTagID = table.Column<int>(type: "INTEGER", nullable: true),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    TagNameOverride = table.Column<string>(type: "TEXT", nullable: true),
                    GlobalSpoiler = table.Column<bool>(type: "INTEGER", nullable: false),
                    Verified = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagDescription = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniDB_Tag", x => x.AniDB_TagID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeEpisode",
                columns: table => new
                {
                    AnimeEpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeSeriesID = table.Column<int>(type: "INTEGER", nullable: false),
                    AniDB_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    EpisodeNameOverride = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeEpisode", x => x.AnimeEpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeEpisode_User",
                columns: table => new
                {
                    AnimeEpisode_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JMMUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeSeriesID = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlayedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StoppedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    AbsoluteUserRating = table.Column<int>(type: "INTEGER", nullable: true),
                    UserTags = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeEpisode_User", x => x.AnimeEpisode_UserID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeGroup",
                columns: table => new
                {
                    AnimeGroupID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeGroupParentID = table.Column<int>(type: "INTEGER", nullable: true),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsManuallyNamed = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EpisodeAddedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LatestEpisodeAirDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MissingEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MissingEpisodeCountGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    OverrideDescription = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultAnimeSeriesID = table.Column<int>(type: "INTEGER", nullable: true),
                    MainAniDBAnimeID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeGroup", x => x.AnimeGroupID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeGroup_User",
                columns: table => new
                {
                    AnimeGroup_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JMMUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeGroupID = table.Column<int>(type: "INTEGER", nullable: false),
                    UnwatchedEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlayedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StoppedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeGroup_User", x => x.AnimeGroup_UserID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeSeries",
                columns: table => new
                {
                    AnimeSeriesID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeGroupID = table.Column<int>(type: "INTEGER", nullable: false),
                    AniDB_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DefaultAudioLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultSubtitleLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeAddedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LatestEpisodeAirDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AirsOn = table.Column<int>(type: "INTEGER", nullable: true),
                    MissingEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MissingEpisodeCountGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenMissingEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenMissingEpisodeCountGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    LatestLocalEpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SeriesNameOverride = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DisableAutoMatchFlags = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeSeries", x => x.AnimeSeriesID);
                });

            migrationBuilder.CreateTable(
                name: "AnimeSeries_User",
                columns: table => new
                {
                    AnimeSeries_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JMMUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnimeSeriesID = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    UnwatchedEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenUnwatchedEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlayedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StoppedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastEpisodeUpdate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastVideoUpdate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AbsoluteUserRating = table.Column<int>(type: "INTEGER", nullable: true),
                    UserRatingVoteType = table.Column<int>(type: "INTEGER", nullable: true),
                    UserTags = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeSeries_User", x => x.AnimeSeries_UserID);
                });

            migrationBuilder.CreateTable(
                name: "AuthTokens",
                columns: table => new
                {
                    AuthID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthTokens", x => x.AuthID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_AniDB_MAL",
                columns: table => new
                {
                    CrossRef_AniDB_MALID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    MALID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_AniDB_MAL", x => x.CrossRef_AniDB_MALID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_AniDB_TMDB_Episode",
                columns: table => new
                {
                    CrossRef_AniDB_TMDB_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnidbAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnidbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchRating = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_AniDB_TMDB_Episode", x => x.CrossRef_AniDB_TMDB_EpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_AniDB_TMDB_Movie",
                columns: table => new
                {
                    CrossRef_AniDB_TMDB_MovieID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnidbAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnidbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbMovieID = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchRating = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_AniDB_TMDB_Movie", x => x.CrossRef_AniDB_TMDB_MovieID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_AniDB_TMDB_Show",
                columns: table => new
                {
                    CrossRef_AniDB_TMDB_ShowID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnidbAnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchRating = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_AniDB_TMDB_Show", x => x.CrossRef_AniDB_TMDB_ShowID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_CustomTag",
                columns: table => new
                {
                    CrossRef_CustomTagID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomTagID = table.Column<int>(type: "INTEGER", nullable: false),
                    CrossRefID = table.Column<int>(type: "INTEGER", nullable: false),
                    CrossRefType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_CustomTag", x => x.CrossRef_CustomTagID);
                });

            migrationBuilder.CreateTable(
                name: "CrossRef_File_Episode",
                columns: table => new
                {
                    CrossRef_File_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    AnimeID = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    Percentage = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossRef_File_Episode", x => x.CrossRef_File_EpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "CustomTag",
                columns: table => new
                {
                    CustomTagID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    TagDescription = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomTag", x => x.CustomTagID);
                });

            migrationBuilder.CreateTable(
                name: "FileNameHash",
                columns: table => new
                {
                    FileNameHashID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileNameHash", x => x.FileNameHashID);
                });

            migrationBuilder.CreateTable(
                name: "FilterPreset",
                columns: table => new
                {
                    FilterPresetID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentFilterPresetID = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ApplyAtSeriesLevel = table.Column<bool>(type: "INTEGER", nullable: false),
                    Locked = table.Column<bool>(type: "INTEGER", nullable: false),
                    FilterType = table.Column<int>(type: "INTEGER", nullable: false),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Expression = table.Column<string>(type: "TEXT", nullable: true),
                    SortingExpression = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterPreset", x => x.FilterPresetID);
                });

            migrationBuilder.CreateTable(
                name: "ImportFolder",
                columns: table => new
                {
                    ImportFolderID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImportFolderName = table.Column<string>(type: "TEXT", nullable: false),
                    ImportFolderLocation = table.Column<string>(type: "TEXT", nullable: false),
                    IsWatched = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDropSource = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDropDestination = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportFolder", x => x.ImportFolderID);
                });

            migrationBuilder.CreateTable(
                name: "JMMUser",
                columns: table => new
                {
                    JMMUserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAniDBUser = table.Column<int>(type: "INTEGER", nullable: false),
                    IsTraktUser = table.Column<int>(type: "INTEGER", nullable: false),
                    HideCategories = table.Column<string>(type: "TEXT", nullable: true),
                    CanEditServerSettings = table.Column<int>(type: "INTEGER", nullable: true),
                    PlexUsers = table.Column<string>(type: "TEXT", nullable: true),
                    PlexToken = table.Column<string>(type: "TEXT", nullable: true),
                    AvatarImageBlob = table.Column<byte[]>(type: "BLOB", nullable: true),
                    AvatarImageMetadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JMMUser", x => x.JMMUserID);
                });

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    PlaylistID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlaylistName = table.Column<string>(type: "TEXT", nullable: true),
                    PlaylistItems = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultPlayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayWatched = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayUnwatched = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.PlaylistID);
                });

            migrationBuilder.CreateTable(
                name: "Scan",
                columns: table => new
                {
                    ScanID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImportFolders = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scan", x => x.ScanID);
                });

            migrationBuilder.CreateTable(
                name: "ScanFile",
                columns: table => new
                {
                    ScanFileID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScanID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImportFolderID = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoLocal_Place_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    HashResult = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanFile", x => x.ScanFileID);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledUpdate",
                columns: table => new
                {
                    ScheduledUpdateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UpdateType = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDetails = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledUpdate", x => x.ScheduledUpdateID);
                });

            migrationBuilder.CreateTable(
                name: "StoredReleaseInfo",
                columns: table => new
                {
                    StoredReleaseInfoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ED2K = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ID = table.Column<string>(type: "TEXT", nullable: true),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseURI = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    ProvidedFileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalFilename = table.Column<string>(type: "TEXT", nullable: true),
                    IsCensored = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsCreditless = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsChaptered = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsCorrupted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Source = table.Column<byte>(type: "INTEGER", nullable: false),
                    Hashes = table.Column<string>(type: "TEXT", nullable: true),
                    CrossReferences = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    ReleasedAt = table.Column<int>(type: "INTEGER", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GroupID = table.Column<string>(type: "TEXT", nullable: true),
                    GroupSource = table.Column<string>(type: "TEXT", nullable: true),
                    GroupName = table.Column<string>(type: "TEXT", nullable: true),
                    GroupShortName = table.Column<string>(type: "TEXT", nullable: true),
                    AudioLanguages = table.Column<string>(type: "TEXT", nullable: true),
                    SubtitleLanguages = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredReleaseInfo", x => x.StoredReleaseInfoID);
                });

            migrationBuilder.CreateTable(
                name: "StoredReleaseInfo_MatchAttempt",
                columns: table => new
                {
                    StoredReleaseInfo_MatchAttemptID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttemptProviderNames = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    ProviderID = table.Column<Guid>(type: "TEXT", nullable: true),
                    ED2K = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    AttemptStartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AttemptEndedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredReleaseInfo_MatchAttempt", x => x.StoredReleaseInfo_MatchAttemptID);
                });

            migrationBuilder.CreateTable(
                name: "StoredRelocationPipe",
                columns: table => new
                {
                    StoredRelocationPipeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Configuration = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredRelocationPipe", x => x.StoredRelocationPipeID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_AlternateOrdering",
                columns: table => new
                {
                    TMDB_AlternateOrderingID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbNetworkID = table.Column<int>(type: "INTEGER", nullable: true),
                    TmdbEpisodeGroupCollectionID = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_AlternateOrdering", x => x.TMDB_AlternateOrderingID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_AlternateOrdering_Episode",
                columns: table => new
                {
                    TMDB_AlternateOrdering_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeGroupCollectionID = table.Column<string>(type: "TEXT", nullable: false),
                    TmdbEpisodeGroupID = table.Column<string>(type: "TEXT", nullable: false),
                    TmdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_AlternateOrdering_Episode", x => x.TMDB_AlternateOrdering_EpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_AlternateOrdering_Season",
                columns: table => new
                {
                    TMDB_AlternateOrdering_SeasonID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeGroupCollectionID = table.Column<string>(type: "TEXT", nullable: false),
                    TmdbEpisodeGroupID = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SeasonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_AlternateOrdering_Season", x => x.TMDB_AlternateOrdering_SeasonID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Collection",
                columns: table => new
                {
                    TMDB_CollectionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbCollectionID = table.Column<int>(type: "INTEGER", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    MovieCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Collection", x => x.TMDB_CollectionID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Collection_Movie",
                columns: table => new
                {
                    TMDB_Collection_MovieID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbCollectionID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbMovieID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Collection_Movie", x => x.TMDB_Collection_MovieID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Company",
                columns: table => new
                {
                    TMDB_CompanyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbCompanyID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Company", x => x.TMDB_CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Company_Entity",
                columns: table => new
                {
                    TMDB_Company_EntityID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbCompanyID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEntityType = table.Column<byte>(type: "INTEGER", nullable: false),
                    TmdbEntityID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleasedAt = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Company_Entity", x => x.TMDB_Company_EntityID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Episode",
                columns: table => new
                {
                    TMDB_EpisodeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbSeasonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TvdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: true),
                    ThumbnailPath = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeasonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Runtime = table.Column<int>(type: "INTEGER", nullable: true),
                    UserRating = table.Column<double>(type: "REAL", nullable: false),
                    UserVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    AiredAt = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Episode", x => x.TMDB_EpisodeID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Episode_Cast",
                columns: table => new
                {
                    TMDB_Episode_CastID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbSeasonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGuestRole = table.Column<bool>(type: "INTEGER", nullable: false),
                    TmdbPersonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbCreditID = table.Column<string>(type: "TEXT", nullable: false),
                    CharacterName = table.Column<string>(type: "TEXT", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Episode_Cast", x => x.TMDB_Episode_CastID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Episode_Crew",
                columns: table => new
                {
                    TMDB_Episode_CrewID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbSeasonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbEpisodeID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbPersonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbCreditID = table.Column<string>(type: "TEXT", nullable: false),
                    Job = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Episode_Crew", x => x.TMDB_Episode_CrewID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Image",
                columns: table => new
                {
                    TMDB_ImageID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteFileName = table.Column<string>(type: "TEXT", nullable: false),
                    UserRating = table.Column<double>(type: "REAL", nullable: false),
                    UserVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Image", x => x.TMDB_ImageID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Image_Entity",
                columns: table => new
                {
                    TMDB_Image_EntityID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RemoteFileName = table.Column<string>(type: "TEXT", nullable: false),
                    ImageType = table.Column<byte>(type: "INTEGER", nullable: false),
                    TmdbEntityType = table.Column<byte>(type: "INTEGER", nullable: false),
                    TmdbEntityID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleasedAt = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Image_Entity", x => x.TMDB_Image_EntityID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Movie",
                columns: table => new
                {
                    TMDB_MovieID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbMovieID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbCollectionID = table.Column<int>(type: "INTEGER", nullable: true),
                    ImdbMovieID = table.Column<string>(type: "TEXT", nullable: true),
                    PosterPath = table.Column<string>(type: "TEXT", nullable: false),
                    BackdropPath = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalTitle = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalLanguageCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsRestricted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVideo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Genres = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    ContentRatings = table.Column<string>(type: "TEXT", nullable: false),
                    ProductionCountries = table.Column<string>(type: "TEXT", nullable: false),
                    Runtime = table.Column<int>(type: "INTEGER", nullable: true),
                    UserRating = table.Column<double>(type: "REAL", nullable: false),
                    UserVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleasedAt = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Movie", x => x.TMDB_MovieID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Movie_Cast",
                columns: table => new
                {
                    TMDB_Movie_CastID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbMovieID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbPersonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbCreditID = table.Column<string>(type: "TEXT", nullable: false),
                    CharacterName = table.Column<string>(type: "TEXT", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Movie_Cast", x => x.TMDB_Movie_CastID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Movie_Crew",
                columns: table => new
                {
                    TMDB_Movie_CrewID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbMovieID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbPersonID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbCreditID = table.Column<string>(type: "TEXT", nullable: false),
                    Job = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Movie_Crew", x => x.TMDB_Movie_CrewID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Network",
                columns: table => new
                {
                    TMDB_NetworkID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbNetworkID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "TEXT", nullable: false),
                    LastOrphanedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Network", x => x.TMDB_NetworkID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Overview",
                columns: table => new
                {
                    TMDB_OverviewID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentType = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageCode = table.Column<string>(type: "TEXT", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Overview", x => x.TMDB_OverviewID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Person",
                columns: table => new
                {
                    TMDB_PersonID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbPersonID = table.Column<int>(type: "INTEGER", nullable: false),
                    EnglishName = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishBiography = table.Column<string>(type: "TEXT", nullable: false),
                    Aliases = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<byte>(type: "INTEGER", nullable: false),
                    IsRestricted = table.Column<bool>(type: "INTEGER", nullable: false),
                    BirthDay = table.Column<int>(type: "INTEGER", nullable: true),
                    DeathDay = table.Column<int>(type: "INTEGER", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastOrphanedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Person", x => x.TMDB_PersonID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Season",
                columns: table => new
                {
                    TMDB_SeasonID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbSeasonID = table.Column<int>(type: "INTEGER", nullable: false),
                    PosterPath = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Season", x => x.TMDB_SeasonID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Show",
                columns: table => new
                {
                    TMDB_ShowID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TvdbShowID = table.Column<int>(type: "INTEGER", nullable: true),
                    PosterPath = table.Column<string>(type: "TEXT", nullable: false),
                    BackdropPath = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishOverview = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalTitle = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalLanguageCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsRestricted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Genres = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    ContentRatings = table.Column<string>(type: "TEXT", nullable: false),
                    ProductionCountries = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenEpisodeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AlternateOrderingCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UserRating = table.Column<double>(type: "REAL", nullable: false),
                    UserVotes = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstAiredAt = table.Column<int>(type: "INTEGER", nullable: true),
                    LastAiredAt = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PreferredAlternateOrderingID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Show", x => x.TMDB_ShowID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Show_Network",
                columns: table => new
                {
                    TMDB_Show_NetworkID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbShowID = table.Column<int>(type: "INTEGER", nullable: false),
                    TmdbNetworkID = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordering = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Show_Network", x => x.TMDB_Show_NetworkID);
                });

            migrationBuilder.CreateTable(
                name: "TMDB_Title",
                columns: table => new
                {
                    TMDB_TitleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentType = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageCode = table.Column<string>(type: "TEXT", nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TMDB_Title", x => x.TMDB_TitleID);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    VersionsID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VersionType = table.Column<string>(type: "TEXT", nullable: false),
                    VersionValue = table.Column<string>(type: "TEXT", nullable: false),
                    VersionRevision = table.Column<string>(type: "TEXT", nullable: true),
                    VersionCommand = table.Column<string>(type: "TEXT", nullable: true),
                    VersionProgram = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.VersionsID);
                });

            migrationBuilder.CreateTable(
                name: "VideoLocal",
                columns: table => new
                {
                    VideoLocalID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    HashSource = table.Column<int>(type: "INTEGER", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateTimeImported = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    IsIgnored = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVariation = table.Column<bool>(type: "INTEGER", nullable: false),
                    MediaVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    LastAVDumped = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastAVDumpVersion = table.Column<string>(type: "TEXT", nullable: true),
                    MyListID = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaBlob = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLocal", x => x.VideoLocalID);
                });

            migrationBuilder.CreateTable(
                name: "VideoLocal_HashDigest",
                columns: table => new
                {
                    VideoLocal_HashDigestID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoLocalID = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLocal_HashDigest", x => x.VideoLocal_HashDigestID);
                });

            migrationBuilder.CreateTable(
                name: "VideoLocal_Place",
                columns: table => new
                {
                    VideoLocal_Place_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoLocalID = table.Column<int>(type: "INTEGER", nullable: false),
                    ImportFolderID = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLocal_Place", x => x.VideoLocal_Place_ID);
                });

            migrationBuilder.CreateTable(
                name: "VideoLocal_User",
                columns: table => new
                {
                    VideoLocal_UserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JMMUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoLocalID = table.Column<int>(type: "INTEGER", nullable: false),
                    WatchedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResumePosition = table.Column<long>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WatchedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLocal_User", x => x.VideoLocal_UserID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AniDB_Anime");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Character");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Character_Creator");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_PreferredImage");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Relation");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Similar");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Staff");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Tag");

            migrationBuilder.DropTable(
                name: "AniDB_Anime_Title");

            migrationBuilder.DropTable(
                name: "AniDB_AnimeUpdate");

            migrationBuilder.DropTable(
                name: "AniDB_Character");

            migrationBuilder.DropTable(
                name: "AniDB_Creator");

            migrationBuilder.DropTable(
                name: "AniDB_Episode");

            migrationBuilder.DropTable(
                name: "AniDB_Episode_PreferredImage");

            migrationBuilder.DropTable(
                name: "AniDB_Episode_Title");

            migrationBuilder.DropTable(
                name: "AniDB_GroupStatus");

            migrationBuilder.DropTable(
                name: "AniDB_Message");

            migrationBuilder.DropTable(
                name: "AniDB_NotifyQueue");

            migrationBuilder.DropTable(
                name: "AniDB_Tag");

            migrationBuilder.DropTable(
                name: "AnimeEpisode");

            migrationBuilder.DropTable(
                name: "AnimeEpisode_User");

            migrationBuilder.DropTable(
                name: "AnimeGroup");

            migrationBuilder.DropTable(
                name: "AnimeGroup_User");

            migrationBuilder.DropTable(
                name: "AnimeSeries");

            migrationBuilder.DropTable(
                name: "AnimeSeries_User");

            migrationBuilder.DropTable(
                name: "AuthTokens");

            migrationBuilder.DropTable(
                name: "CrossRef_AniDB_MAL");

            migrationBuilder.DropTable(
                name: "CrossRef_AniDB_TMDB_Episode");

            migrationBuilder.DropTable(
                name: "CrossRef_AniDB_TMDB_Movie");

            migrationBuilder.DropTable(
                name: "CrossRef_AniDB_TMDB_Show");

            migrationBuilder.DropTable(
                name: "CrossRef_CustomTag");

            migrationBuilder.DropTable(
                name: "CrossRef_File_Episode");

            migrationBuilder.DropTable(
                name: "CustomTag");

            migrationBuilder.DropTable(
                name: "FileNameHash");

            migrationBuilder.DropTable(
                name: "FilterPreset");

            migrationBuilder.DropTable(
                name: "ImportFolder");

            migrationBuilder.DropTable(
                name: "JMMUser");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "Scan");

            migrationBuilder.DropTable(
                name: "ScanFile");

            migrationBuilder.DropTable(
                name: "ScheduledUpdate");

            migrationBuilder.DropTable(
                name: "StoredReleaseInfo");

            migrationBuilder.DropTable(
                name: "StoredReleaseInfo_MatchAttempt");

            migrationBuilder.DropTable(
                name: "StoredRelocationPipe");

            migrationBuilder.DropTable(
                name: "TMDB_AlternateOrdering");

            migrationBuilder.DropTable(
                name: "TMDB_AlternateOrdering_Episode");

            migrationBuilder.DropTable(
                name: "TMDB_AlternateOrdering_Season");

            migrationBuilder.DropTable(
                name: "TMDB_Collection");

            migrationBuilder.DropTable(
                name: "TMDB_Collection_Movie");

            migrationBuilder.DropTable(
                name: "TMDB_Company");

            migrationBuilder.DropTable(
                name: "TMDB_Company_Entity");

            migrationBuilder.DropTable(
                name: "TMDB_Episode");

            migrationBuilder.DropTable(
                name: "TMDB_Episode_Cast");

            migrationBuilder.DropTable(
                name: "TMDB_Episode_Crew");

            migrationBuilder.DropTable(
                name: "TMDB_Image");

            migrationBuilder.DropTable(
                name: "TMDB_Image_Entity");

            migrationBuilder.DropTable(
                name: "TMDB_Movie");

            migrationBuilder.DropTable(
                name: "TMDB_Movie_Cast");

            migrationBuilder.DropTable(
                name: "TMDB_Movie_Crew");

            migrationBuilder.DropTable(
                name: "TMDB_Network");

            migrationBuilder.DropTable(
                name: "TMDB_Overview");

            migrationBuilder.DropTable(
                name: "TMDB_Person");

            migrationBuilder.DropTable(
                name: "TMDB_Season");

            migrationBuilder.DropTable(
                name: "TMDB_Show");

            migrationBuilder.DropTable(
                name: "TMDB_Show_Network");

            migrationBuilder.DropTable(
                name: "TMDB_Title");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "VideoLocal");

            migrationBuilder.DropTable(
                name: "VideoLocal_HashDigest");

            migrationBuilder.DropTable(
                name: "VideoLocal_Place");

            migrationBuilder.DropTable(
                name: "VideoLocal_User");
        }
    }
}
