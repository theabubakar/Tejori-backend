using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Interfaces.Services;
using Tijori.Infrastructure.Data;
using Tijori.Infrastructure.Repositories;
using Tijori.Infrastructure.Services;

namespace Tijori.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<SocialAuthSettings>(configuration.GetSection(SocialAuthSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
        services.AddScoped<IPasswordResetSessionRepository, PasswordResetSessionRepository>();
        services.AddScoped<IUserLoginRepository, UserLoginRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IOtpHasher, OtpHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IOtpDeliveryService, OtpDeliveryService>();
        services.AddScoped<ISocialTokenValidator, SocialTokenValidator>();
        services.AddScoped<IMaskingService, MaskingService>();

        return services;
    }
}
