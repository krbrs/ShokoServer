using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Image"/> — TMDB image cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_ImageMap.cs
/// </summary>
public class TMDB_ImageConfiguration : IEntityTypeConfiguration<TMDB_Image>
{
    public void Configure(EntityTypeBuilder<TMDB_Image> builder)
    {
        builder.ToTable("TMDB_Image");

        builder.HasKey(x => x.TMDB_ImageID);

        builder.Property(x => x.TMDB_ImageID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IsEnabled);

        builder.Property(x => x.Width)
            .IsRequired();

        builder.Property(x => x.Height)
            .IsRequired();

        builder.Property(x => x.Language)
            .IsRequired()
            .HasConversion(new TitleLanguageConverter());

        builder.Property(x => x.RemoteFileName)
            .IsRequired();

        builder.Property(x => x.UserRating)
            .IsRequired();

        builder.Property(x => x.UserVotes)
            .IsRequired();

        builder.Ignore(x => x.ImageType);
        builder.Ignore(x => x.IsPreferred);
        builder.Ignore(x => x.LanguageCode);
        builder.Ignore(x => x.IsRemoteAvailable);
        builder.Ignore(x => x.LocalPath);
        builder.Ignore(x => x.RemoteURL);
    }
}
