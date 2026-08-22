using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Options;
using Tijori.Application.Services;
using Tijori.Application.Validators;

namespace Tijori.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IBucketService, BucketService>();
        services.AddScoped<IProfileService, ProfileService>();

        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }

    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StorageSettings>(configuration.GetSection(StorageSettings.SectionName));
        return services;
    }
}
