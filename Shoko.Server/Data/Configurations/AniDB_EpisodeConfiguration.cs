using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Episode"/> — raw AniDB episode cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_EpisodeMap.cs
/// </summary>
public class AniDB_EpisodeConfiguration : IEntityTypeConfiguration<AniDB_Episode>
{
    public void Configure(EntityTypeBuilder<AniDB_Episode> builder)
    {
        builder.ToTable("AniDB_Episode");

        builder.HasKey(x => x.AniDB_EpisodeID);

        builder.Property(x => x.AniDB_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EpisodeID)
            .IsRequired();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.LengthSeconds)
            .IsRequired();

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Votes)
            .IsRequired();

        builder.Property(x => x.EpisodeNumber)
            .IsRequired();

        builder.Property(x => x.EpisodeType)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.AirDate)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Ignore(x => x.TmdbEpisodeCrossReferences);
        builder.Ignore(x => x.TmdbMovieCrossReferences);
        builder.Ignore(x => x.TmdbEpisodes);
        builder.Ignore(x => x.TmdbMovies);
    }
}
