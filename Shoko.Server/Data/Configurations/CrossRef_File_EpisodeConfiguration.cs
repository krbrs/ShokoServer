using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_File_Episode"/> — file-to-episode mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossRef_File_EpisodeMap.cs
/// </summary>
public class CrossRef_File_EpisodeConfiguration : IEntityTypeConfiguration<CrossRef_File_Episode>
{
    public void Configure(EntityTypeBuilder<CrossRef_File_Episode> builder)
    {
        builder.ToTable("CrossRef_File_Episode");

        builder.HasKey(x => x.CrossRef_File_EpisodeID);

        builder.Property(x => x.CrossRef_File_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EpisodeID)
            .IsRequired();

        builder.Property(x => x.EpisodeOrder)
            .IsRequired();

        builder.Property(x => x.Hash)
            .IsRequired();

        builder.Property(x => x.Percentage)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.AnimeID)
            .IsRequired();
    }
}
