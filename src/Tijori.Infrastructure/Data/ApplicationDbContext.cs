using Microsoft.EntityFrameworkCore;
using Tijori.Domain.Entities;
using Tijori.Infrastructure.Data.Configurations;

namespace Tijori.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
    public DbSet<PasswordResetSession> PasswordResetSessions => Set<PasswordResetSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new OtpVerificationConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetSessionConfiguration());
    }
}
