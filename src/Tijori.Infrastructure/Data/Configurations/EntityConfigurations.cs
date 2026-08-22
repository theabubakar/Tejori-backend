using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tijori.Domain.Entities;

namespace Tijori.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .HasMaxLength(256);

        builder.Property(x => x.CountryCode)
            .HasMaxLength(10);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512);

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(512);

        builder.Property(x => x.Language)
            .HasMaxLength(20)
            .HasDefaultValue("ENGLISH");

        builder.Property(x => x.NotificationPreference)
            .HasMaxLength(50)
            .HasDefaultValue("ALL");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(x => new { x.CountryCode, x.PhoneNumber })
            .IsUnique()
            .HasFilter("[PhoneNumber] IS NOT NULL AND [CountryCode] IS NOT NULL");
    }
}

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("UserLogins");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => new { x.Provider, x.ProviderKey })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Logins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
{
    public void Configure(EntityTypeBuilder<OtpVerification> builder)
    {
        builder.ToTable("OtpVerifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CodeHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.OtpVerifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PasswordResetSessionConfiguration : IEntityTypeConfiguration<PasswordResetSession>
{
    public void Configure(EntityTypeBuilder<PasswordResetSession> builder)
    {
        builder.ToTable("PasswordResetSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetSessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
