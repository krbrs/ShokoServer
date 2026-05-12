using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Movie"/> — TMDB movie cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_MovieMap.cs
/// </summary>
public class TMDB_MovieConfiguration : IEntityTypeConfiguration<TMDB_Movie>
{
    public void Configure(EntityTypeBuilder<TMDB_Movie> builder)
    {
        builder.ToTable("TMDB_Movie");

        builder.HasKey(x => x.TMDB_MovieID);

        builder.Property(x => x.TMDB_MovieID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbMovieID)
            .IsRequired();

        builder.Property(x => x.TmdbCollectionID);

        builder.Property(x => x.ImdbMovieID);

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

        builder.Property(x => x.IsVideo)
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

        builder.Property(x => x.RuntimeMinutes)
            .HasColumnName("Runtime");

        builder.Property(x => x.UserRating)
            .IsRequired();

        builder.Property(x => x.UserVotes)
            .IsRequired();

        builder.Property(x => x.ReleasedAt)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.CrossReferences);
        builder.Ignore(x => x.FileCrossReferences);
        builder.Ignore(x => x.TmdbCompanies);
        builder.Ignore(x => x.TmdbCompanyCrossReferences);
        builder.Ignore(x => x.Cast);
        builder.Ignore(x => x.Crew);
        builder.Ignore(x => x.Runtime);
    }
}
