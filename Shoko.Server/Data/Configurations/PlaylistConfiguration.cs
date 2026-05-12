using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Legacy;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Playlist"/> — user playlists.
/// Based on NHibernate mapping: Shoko.Server/Mappings/PlaylistMap.cs
/// </summary>
public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.ToTable("Playlist");

        builder.HasKey(x => x.PlaylistID);

        builder.Property(x => x.PlaylistID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PlaylistName);

        builder.Property(x => x.PlaylistItems);

        builder.Property(x => x.DefaultPlayOrder)
            .IsRequired();

        builder.Property(x => x.PlayWatched)
            .IsRequired();

        builder.Property(x => x.PlayUnwatched)
            .IsRequired();
    }
}
