using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Person"/> — TMDB person (cast/crew) cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_PersonMap.cs
/// </summary>
public class TMDB_PersonConfiguration : IEntityTypeConfiguration<TMDB_Person>
{
    public void Configure(EntityTypeBuilder<TMDB_Person> builder)
    {
        builder.ToTable("TMDB_Person");

        builder.HasKey(x => x.TMDB_PersonID);

        builder.Property(x => x.TMDB_PersonID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbPersonID)
            .IsRequired();

        builder.Property(x => x.EnglishName)
            .IsRequired();

        builder.Property(x => x.EnglishBiography)
            .IsRequired();

        builder.Property(x => x.Aliases)
            .IsRequired()
            .HasConversion(new StringListConverter());

        builder.Property(x => x.Gender)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(x => x.IsRestricted)
            .IsRequired();

        builder.Property(x => x.BirthDay)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.DeathDay)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.PlaceOfBirth);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Property(x => x.LastOrphanedAt);
    }
}
