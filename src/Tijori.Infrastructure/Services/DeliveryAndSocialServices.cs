using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;

namespace Tijori.Infrastructure.Services;

public class SocialAuthSettings
{
    public const string SectionName = "SocialAuth";

    public string AppleClientId { get; set; } = string.Empty;
    public string GoogleClientId { get; set; } = string.Empty;
}

public class SocialTokenValidator : ISocialTokenValidator
{
    private readonly SocialAuthSettings _settings;

    public SocialTokenValidator(IOptions<SocialAuthSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<SocialUserInfo> ValidateAppleTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var principal = ValidateToken(idToken, _settings.AppleClientId);
        return Task.FromResult(MapSocialUser(principal, "apple"));
    }

    public Task<SocialUserInfo> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var principal = ValidateToken(idToken, _settings.GoogleClientId);
        return Task.FromResult(MapSocialUser(principal, "google"));
    }

    private static ClaimsPrincipal ValidateToken(string idToken, string audience)
    {
        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new UnauthorizedAppException("Social authentication is not configured.");
        }

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            SignatureValidator = (token, _) => new JwtSecurityToken(token)
        };

        try
        {
            return handler.ValidateToken(idToken, parameters, out _);
        }
        catch
        {
            throw new UnauthorizedAppException("Invalid social authentication token.");
        }
    }

    private static SocialUserInfo MapSocialUser(ClaimsPrincipal principal, string provider)
    {
        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new UnauthorizedAppException("Invalid social authentication token.");
        }

        return new SocialUserInfo
        {
            ProviderKey = $"{provider}:{subject}",
            Email = principal.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? principal.FindFirstValue(ClaimTypes.Email),
            FullName = principal.FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? principal.FindFirstValue(ClaimTypes.Name)
        };
    }
}
