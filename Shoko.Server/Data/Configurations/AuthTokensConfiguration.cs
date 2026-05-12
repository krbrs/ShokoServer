using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Internal;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AuthTokens"/> — API key auth tokens.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AuthTokensMap.cs
/// </summary>
public class AuthTokensConfiguration : IEntityTypeConfiguration<AuthTokens>
{
    public void Configure(EntityTypeBuilder<AuthTokens> builder)
    {
        builder.ToTable("AuthTokens");

        builder.HasKey(x => x.AuthID);

        builder.Property(x => x.AuthID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserID)
            .IsRequired();

        builder.Property(x => x.DeviceName)
            .IsRequired();

        builder.Property(x => x.Token)
            .IsRequired();
    }
}
