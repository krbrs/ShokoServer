using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Episode_Crew"/> — TMDB episode crew join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Episode_CrewMap.cs
/// </summary>
public class TMDB_Episode_CrewConfiguration : IEntityTypeConfiguration<TMDB_Episode_Crew>
{
    public void Configure(EntityTypeBuilder<TMDB_Episode_Crew> builder)
    {
        builder.ToTable("TMDB_Episode_Crew");

        builder.HasKey(x => x.TMDB_Episode_CrewID);

        builder.Property(x => x.TMDB_Episode_CrewID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbSeasonID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeID)
            .IsRequired();

        builder.Property(x => x.TmdbPersonID)
            .IsRequired();

        builder.Property(x => x.TmdbCreditID)
            .IsRequired();

        builder.Property(x => x.Job)
            .IsRequired();

        builder.Property(x => x.Department)
            .IsRequired();
    }
}
