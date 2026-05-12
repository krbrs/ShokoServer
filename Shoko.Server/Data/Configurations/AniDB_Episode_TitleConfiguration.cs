using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Episode_Title"/> — multi-language episode titles.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Episode_TitleMap.cs
/// </summary>
public class AniDB_Episode_TitleConfiguration : IEntityTypeConfiguration<AniDB_Episode_Title>
{
    public void Configure(EntityTypeBuilder<AniDB_Episode_Title> builder)
    {
        builder.ToTable("AniDB_Episode_Title");

        builder.HasKey(x => x.AniDB_Episode_TitleID);

        builder.Property(x => x.AniDB_Episode_TitleID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AniDB_EpisodeID)
            .IsRequired();

        builder.Property(x => x.Language)
            .IsRequired()
            .HasConversion(new TitleLanguageConverter());

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Ignore(x => x.LanguageCode);
    }
}
