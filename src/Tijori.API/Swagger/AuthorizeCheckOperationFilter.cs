using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tijori.API.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionAttributes = context.MethodInfo.GetCustomAttributes(true);
        var controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? Array.Empty<object>();

        var hasAllowAnonymous = actionAttributes.OfType<AllowAnonymousAttribute>().Any()
            || controllerAttributes.OfType<AllowAnonymousAttribute>().Any();

        var hasAuthorize = actionAttributes.OfType<AuthorizeAttribute>().Any()
            || controllerAttributes.OfType<AuthorizeAttribute>().Any();

        if (hasAllowAnonymous || !hasAuthorize)
        {
            return;
        }

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        ];

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
    }
}
