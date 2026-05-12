using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_GroupStatus"/> — release group status cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_GroupStatusMap.cs
/// </summary>
public class AniDB_GroupStatusConfiguration : IEntityTypeConfiguration<AniDB_GroupStatus>
{
    public void Configure(EntityTypeBuilder<AniDB_GroupStatus> builder)
    {
        builder.ToTable("AniDB_GroupStatus");

        builder.HasKey(x => x.AniDB_GroupStatusID);

        builder.Property(x => x.AniDB_GroupStatusID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.GroupID)
            .IsRequired();

        builder.Property(x => x.GroupName);

        builder.Property(x => x.CompletionState)
            .IsRequired();

        builder.Property(x => x.LastEpisodeNumber)
            .IsRequired();

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Votes)
            .IsRequired();

        builder.Property(x => x.EpisodeRange);
    }
}
