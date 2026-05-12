using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Movie_Crew"/> — TMDB movie crew join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Movie_CrewMap.cs
/// </summary>
public class TMDB_Movie_CrewConfiguration : IEntityTypeConfiguration<TMDB_Movie_Crew>
{
    public void Configure(EntityTypeBuilder<TMDB_Movie_Crew> builder)
    {
        builder.ToTable("TMDB_Movie_Crew");

        builder.HasKey(x => x.TMDB_Movie_CrewID);

        builder.Property(x => x.TMDB_Movie_CrewID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbMovieID)
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
