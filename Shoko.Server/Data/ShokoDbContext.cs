using Microsoft.EntityFrameworkCore;
using Shoko.Server.API.v3.Models.Shoko;
using Shoko.Server.Data.Converters;
using Shoko.Server.Data.Configurations;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Models.AniDB.Embedded;
using Shoko.Server.Models.Shoko.Embedded;
using Shoko.Server.Models.CrossReference;
using Shoko.Server.Models.Internal;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Models.Release;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Models.Trakt;

namespace Shoko.Server.Data;

/// <summary>
/// EF Core DbContext for the Shoko Server domain model.
///
/// Entity configurations are loaded via ApplyConfigurationsFromAssembly.
///
/// Provider configuration is intentionally absent from OnConfiguring — provider selection
/// is handled externally via DatabaseFactory and passed through DbContextOptions.
/// </summary>
public partial class ShokoDbContext : DbContext
{
    #region Constructors

    public ShokoDbContext(DbContextOptions<ShokoDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region DbSet Properties — Shoko.Server.Models.Shoko (18 entities)

    public DbSet<AnimeSeries> AnimeSeries { get; set; } = null!;
    public DbSet<AnimeEpisode> AnimeEpisode { get; set; } = null!;
    public DbSet<VideoLocal> VideoLocal { get; set; } = null!;
    public DbSet<VideoLocal_HashDigest> VideoLocal_HashDigest { get; set; } = null!;
    public DbSet<VideoLocal_Place> VideoLocal_Place { get; set; } = null!;
    public DbSet<ShokoManagedFolder> ShokoManagedFolder { get; set; } = null!;
    public DbSet<FilterPreset> FilterPreset { get; set; } = null!;
    public DbSet<CustomTag> CustomTag { get; set; } = null!;
    public DbSet<StoredReleaseInfo> StoredReleaseInfo { get; set; } = null!;
    public DbSet<StoredReleaseInfo_MatchAttempt> StoredReleaseInfo_MatchAttempt { get; set; } = null!;
    public DbSet<StoredRelocationPipe> StoredRelocationPipe { get; set; } = null!;
    public DbSet<FileNameHash> FileNameHash { get; set; } = null!;
    public DbSet<AnimeEpisode_User> AnimeEpisode_User { get; set; } = null!;
    public DbSet<AnimeSeries_User> AnimeSeries_User { get; set; } = null!;
    public DbSet<AnimeGroup_User> AnimeGroup_User { get; set; } = null!;
    public DbSet<VideoLocal_User> VideoLocal_User { get; set; } = null!;
    public DbSet<AnimeGroup> AnimeGroup { get; set; } = null!;
    public DbSet<JMMUser> JMMUser { get; set; } = null!;

    #endregion

    #region DbSet Properties — Shoko.Server.Models.AniDB (19 entities)

    public DbSet<AniDB_Anime> AniDB_Anime { get; set; } = null!;
    public DbSet<AniDB_Episode> AniDB_Episode { get; set; } = null!;
    public DbSet<AniDB_Tag> AniDB_Tag { get; set; } = null!;
    public DbSet<AniDB_Creator> AniDB_Creator { get; set; } = null!;
    public DbSet<AniDB_Anime_Tag> AniDB_Anime_Tag { get; set; } = null!;
    public DbSet<AniDB_Anime_Character> AniDB_Anime_Character { get; set; } = null!;
    public DbSet<AniDB_Anime_Character_Creator> AniDB_Anime_Character_Creator { get; set; } = null!;
    public DbSet<AniDB_Anime_Staff> AniDB_Anime_Staff { get; set; } = null!;
    public DbSet<AniDB_Anime_Title> AniDB_Anime_Title { get; set; } = null!;
    public DbSet<AniDB_Anime_PreferredImage> AniDB_Anime_PreferredImage { get; set; } = null!;
    public DbSet<AniDB_AnimeUpdate> AniDB_AnimeUpdate { get; set; } = null!;
    public DbSet<AniDB_Anime_Relation> AniDB_Anime_Relation { get; set; } = null!;
    public DbSet<AniDB_Anime_Similar> AniDB_Anime_Similar { get; set; } = null!;
    public DbSet<AniDB_Episode_Title> AniDB_Episode_Title { get; set; } = null!;
    public DbSet<AniDB_Episode_PreferredImage> AniDB_Episode_PreferredImage { get; set; } = null!;
    public DbSet<AniDB_GroupStatus> AniDB_GroupStatus { get; set; } = null!;
    public DbSet<AniDB_NotifyQueue> AniDB_NotifyQueue { get; set; } = null!;
    public DbSet<AniDB_Message> AniDB_Message { get; set; } = null!;
    public DbSet<AniDB_Character> AniDB_Character { get; set; } = null!;

    #endregion

    #region DbSet Properties — Shoko.Server.Models.TMDB (22 entities)

    public DbSet<TMDB_Show> TMDB_Show { get; set; } = null!;
    public DbSet<TMDB_Movie> TMDB_Movie { get; set; } = null!;
    public DbSet<TMDB_Episode> TMDB_Episode { get; set; } = null!;
    public DbSet<TMDB_Season> TMDB_Season { get; set; } = null!;
    public DbSet<TMDB_Person> TMDB_Person { get; set; } = null!;
    public DbSet<TMDB_Image> TMDB_Image { get; set; } = null!;
    public DbSet<TMDB_Image_Entity> TMDB_Image_Entity { get; set; } = null!;
    public DbSet<TMDB_Company> TMDB_Company { get; set; } = null!;
    public DbSet<TMDB_Company_Entity> TMDB_Company_Entity { get; set; } = null!;
    public DbSet<TMDB_Collection> TMDB_Collection { get; set; } = null!;
    public DbSet<TMDB_Collection_Movie> TMDB_Collection_Movie { get; set; } = null!;
    public DbSet<TMDB_Network> TMDB_Network { get; set; } = null!;
    public DbSet<TMDB_Show_Network> TMDB_Show_Network { get; set; } = null!;
    public DbSet<TMDB_AlternateOrdering> TMDB_AlternateOrdering { get; set; } = null!;
    public DbSet<TMDB_AlternateOrdering_Season> TMDB_AlternateOrdering_Season { get; set; } = null!;
    public DbSet<TMDB_AlternateOrdering_Episode> TMDB_AlternateOrdering_Episode { get; set; } = null!;
    public DbSet<TMDB_Movie_Cast> TMDB_Movie_Cast { get; set; } = null!;
    public DbSet<TMDB_Movie_Crew> TMDB_Movie_Crew { get; set; } = null!;
    public DbSet<TMDB_Episode_Cast> TMDB_Episode_Cast { get; set; } = null!;
    public DbSet<TMDB_Episode_Crew> TMDB_Episode_Crew { get; set; } = null!;
    public DbSet<TMDB_Title> TMDB_Title { get; set; } = null!;
    public DbSet<TMDB_Overview> TMDB_Overview { get; set; } = null!;

    #endregion

    #region DbSet Properties — Shoko.Server.Models.CrossReference (6 entities)

    public DbSet<CrossRef_AniDB_MAL> CrossRef_AniDB_MAL { get; set; } = null!;
    public DbSet<CrossRef_File_Episode> CrossRef_File_Episode { get; set; } = null!;
    public DbSet<CrossRef_CustomTag> CrossRef_CustomTag { get; set; } = null!;
    public DbSet<CrossRef_AniDB_TMDB_Show> CrossRef_AniDB_TMDB_Show { get; set; } = null!;
    public DbSet<CrossRef_AniDB_TMDB_Movie> CrossRef_AniDB_TMDB_Movie { get; set; } = null!;
    public DbSet<CrossRef_AniDB_TMDB_Episode> CrossRef_AniDB_TMDB_Episode { get; set; } = null!;

    #endregion

    #region DbSet Properties — Shoko.Server.Models.Internal (3 entities)

    public DbSet<ScheduledUpdate> ScheduledUpdate { get; set; } = null!;
    public DbSet<Versions> Versions { get; set; } = null!;
    public DbSet<AuthTokens> AuthTokens { get; set; } = null!;

    #endregion

    #region DbSet Properties — Shoko.Server.Models.Legacy (3 entities)

    public DbSet<Playlist> Playlist { get; set; } = null!;
    public DbSet<Scan> Scan { get; set; } = null!;
    public DbSet<ScanFile> ScanFile { get; set; } = null!;

    #endregion

    #region OnModelCreating

    /// <summary>
    /// Configures the model before it has been finalized.
    ///
    /// Entity configurations are loaded via ApplyConfigurationsFromAssembly from
    /// Shoko.Server.Data.Configurations namespace.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ShokoDbContext).Assembly,
            type =>
                type != typeof(CrossRef_AniDB_TraktV2Configuration) &&
                type != typeof(Trakt_EpisodeConfiguration) &&
                type != typeof(Trakt_SeasonConfiguration) &&
                type != typeof(Trakt_ShowConfiguration));
        ApplySqlServerLegacyCompatibility(modelBuilder);
        modelBuilder.Ignore<AniDB_Season>();
        modelBuilder.Ignore<CrossRef_AniDB_TMDB_Season>();
        modelBuilder.Ignore<AnimeSeason>();
        modelBuilder.Ignore(typeof(TMDB_Studio<TMDB_Movie>));
        modelBuilder.Ignore(typeof(TMDB_Studio<TMDB_Show>));
        modelBuilder.Ignore(typeof(global::Shoko.Server.API.v3.Models.Shoko.File.HashDigest));
        modelBuilder.Ignore<global::Shoko.Abstractions.Video.Hashing.HashDigest>();
        modelBuilder.Ignore(typeof(global::Shoko.Server.Models.Shoko.JMMUser.UserImageMetadata));
        modelBuilder.Ignore(typeof(global::Shoko.Server.Models.TMDB.TMDB_Season_Cast));
        modelBuilder.Ignore(typeof(global::Shoko.Server.Models.TMDB.TMDB_Season_Crew));
        modelBuilder.Ignore(typeof(global::Shoko.Server.Models.TMDB.TMDB_Show_Cast));
        modelBuilder.Ignore(typeof(global::Shoko.Server.Models.TMDB.TMDB_Show_Crew));
        modelBuilder.Ignore<CrossRef_AniDB_TraktV2>();
        modelBuilder.Ignore<Trakt_Show>();
        modelBuilder.Ignore<Trakt_Season>();
        modelBuilder.Ignore<Trakt_Episode>();
    }

    private void ApplySqlServerLegacyCompatibility(ModelBuilder modelBuilder)
    {
        if (!Database.IsSqlServer())
            return;

        modelBuilder.Entity<AnimeSeries>()
            .Property(x => x.AirsOn)
            .HasConversion<string>();

        modelBuilder.Entity<AnimeEpisode>()
            .Property(x => x.IsHidden)
            .HasConversion<int>();

        modelBuilder.Entity<VideoLocal>()
            .Property(x => x.IsIgnored)
            .HasConversion<int>();

        modelBuilder.Entity<VideoLocal>()
            .Property(x => x.IsVariation)
            .HasConversion<int>();

        modelBuilder.Entity<AniDB_Episode>()
            .Property(x => x.EpisodeType)
            .HasConversion<int>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.IsCensored)
            .HasConversion<int?>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.IsChaptered)
            .HasConversion<int?>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.IsCreditless)
            .HasConversion<int?>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.IsCorrupted)
            .HasConversion<int>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.Source)
            .HasConversion<int>();

        modelBuilder.Entity<StoredReleaseInfo>()
            .Property(x => x.ReleasedAt)
            .HasConversion<NullableDateOnlyDateTimeConverter>();

        modelBuilder.Entity<TMDB_Show>()
            .Property(x => x.FirstAiredAt)
            .HasConversion<NullableDateOnlyDateTimeConverter>();

        modelBuilder.Entity<TMDB_Show>()
            .Property(x => x.LastAiredAt)
            .HasConversion<NullableDateOnlyDateTimeConverter>();

        modelBuilder.Entity<TMDB_Episode>()
            .Property(x => x.IsHidden)
            .HasConversion<int>();

        modelBuilder.Entity<TMDB_Episode>()
            .Property(x => x.AiredAt)
            .HasConversion<NullableDateOnlyDateTimeConverter>();
    }

    #endregion
}
