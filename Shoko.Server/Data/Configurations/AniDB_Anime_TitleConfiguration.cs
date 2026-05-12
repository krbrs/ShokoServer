using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Title"/> — multi-language anime titles.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_TitleMap.cs
/// </summary>
public class AniDB_Anime_TitleConfiguration : IEntityTypeConfiguration<AniDB_Anime_Title>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Title> builder)
    {
        builder.ToTable("AniDB_Anime_Title");

        builder.HasKey(x => x.AniDB_Anime_TitleID);

        builder.Property(x => x.AniDB_Anime_TitleID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.Language)
            .IsRequired()
            .HasConversion(new TitleLanguageConverter());

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.TitleType)
            .IsRequired()
            .HasConversion(new TitleTypeConverter());

        builder.Ignore(x => x.LanguageCode);
    }
}
