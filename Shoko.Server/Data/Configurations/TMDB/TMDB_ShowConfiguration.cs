using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Show"/> — TMDB show cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_ShowMap.cs
/// </summary>
public class TMDB_ShowConfiguration : IEntityTypeConfiguration<TMDB_Show>
{
    public void Configure(EntityTypeBuilder<TMDB_Show> builder)
    {
        builder.ToTable("TMDB_Show");

        builder.HasKey(x => x.TMDB_ShowID);

        builder.Property(x => x.TMDB_ShowID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TvdbShowID);

        builder.Property(x => x.PosterPath);

        builder.Property(x => x.BackdropPath);

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EnglishOverview)
            .IsRequired();

        builder.Property(x => x.OriginalTitle)
            .IsRequired();

        builder.Property(x => x.OriginalLanguageCode)
            .IsRequired();

        builder.Property(x => x.IsRestricted)
            .IsRequired();

        builder.Property(x => x.Genres)
            .IsRequired()
            .HasConversion(new StringListConverter());

        builder.Property(x => x.Keywords)
            .IsRequired()
            .HasConversion(new StringListConverter());

        builder.Property(x => x.ContentRatings)
            .IsRequired()
            .HasConversion(new TmdbContentRatingConverter());

        builder.Property(x => x.ProductionCountries)
            .IsRequired()
            .HasConversion(new TmdbProductionCountryConverter());

        builder.Property(x => x.EpisodeCount)
            .IsRequired();

        builder.Property(x => x.HiddenEpisodeCount)
            .IsRequired();

        builder.Property(x => x.SeasonCount)
            .IsRequired();

        builder.Property(x => x.AlternateOrderingCount)
            .IsRequired();

        builder.Property(x => x.UserRating)
            .IsRequired();

        builder.Property(x => x.UserVotes)
            .IsRequired();

        builder.Property(x => x.FirstAiredAt)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.LastAiredAt)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Property(x => x.PreferredAlternateOrderingID);

        builder.Ignore(x => x.CrossReferences);
        builder.Ignore(x => x.EpisodeCrossReferences);
        builder.Ignore(x => x.TmdbAlternateOrdering);
        builder.Ignore(x => x.TmdbCompanies);
        builder.Ignore(x => x.TmdbCompanyCrossReferences);
        builder.Ignore(x => x.TmdbEpisodes);
        builder.Ignore(x => x.TmdbNetworks);
        builder.Ignore(x => x.TmdbSeasons);
        builder.Ignore(x => x.TmdbNetworkCrossReferences);
    }
}
