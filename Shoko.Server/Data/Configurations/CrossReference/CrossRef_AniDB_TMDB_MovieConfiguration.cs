using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_AniDB_TMDB_Movie"/> — AniDB ↔ TMDB movie mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossReference/CrossRef_AniDB_TMDB_MovieMap.cs
/// </summary>
public class CrossRef_AniDB_TMDB_MovieConfiguration : IEntityTypeConfiguration<CrossRef_AniDB_TMDB_Movie>
{
    public void Configure(EntityTypeBuilder<CrossRef_AniDB_TMDB_Movie> builder)
    {
        builder.ToTable("CrossRef_AniDB_TMDB_Movie");

        builder.HasKey(x => x.CrossRef_AniDB_TMDB_MovieID);

        builder.Property(x => x.CrossRef_AniDB_TMDB_MovieID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnidbAnimeID)
            .IsRequired();

        builder.Property(x => x.AnidbEpisodeID)
            .IsRequired();

        builder.Property(x => x.TmdbMovieID)
            .IsRequired();

        builder.Property(x => x.MatchRating)
            .HasConversion<byte>()
            .IsRequired();
    }
}
