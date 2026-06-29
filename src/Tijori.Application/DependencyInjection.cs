using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Services;
using Tijori.Application.Validators;

namespace Tijori.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }
}
