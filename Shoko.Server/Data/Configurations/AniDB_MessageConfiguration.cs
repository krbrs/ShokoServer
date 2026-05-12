using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Message"/> — AniDB message storage.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_MessageMap.cs
/// </summary>
public class AniDB_MessageConfiguration : IEntityTypeConfiguration<AniDB_Message>
{
    public void Configure(EntityTypeBuilder<AniDB_Message> builder)
    {
        builder.ToTable("AniDB_Message");

        builder.HasKey(x => x.AniDB_MessageID);

        builder.Property(x => x.AniDB_MessageID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MessageID)
            .IsRequired();

        builder.Property(x => x.FromUserId)
            .HasColumnName("FromUserID")
            .IsRequired();

        builder.Property(x => x.FromUserName)
            .IsRequired();

        builder.Property(x => x.SentAt)
            .IsRequired();

        builder.Property(x => x.FetchedAt)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired();

        builder.Property(x => x.Flags)
            .IsRequired();

        builder.Ignore(x => x.IsReadOnAniDB);
        builder.Ignore(x => x.IsReadOnShoko);
        builder.Ignore(x => x.IsFileMoved);
        builder.Ignore(x => x.IsFileMoveHandled);
    }
}
