using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;
using Tijori.Domain.Entities;

namespace Tijori.Infrastructure.Services;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 60;
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public AuthTokenDto GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("is_guest", user.IsGuest.ToString().ToLowerInvariant())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.FullName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthTokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = MapUser(user)
        };
    }

    private static AuthUserDto MapUser(User user) =>
        new()
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CountryCode = user.CountryCode,
            IsGuest = user.IsGuest,
            IsEmailVerified = user.IsEmailVerified
        };
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}

public class OtpHasher : IOtpHasher
{
    public string Hash(string code) =>
        BCrypt.Net.BCrypt.HashPassword(code);

    public bool Verify(string code, string codeHash) =>
        BCrypt.Net.BCrypt.Verify(code, codeHash);
}

public class OtpService : IOtpService
{
    public string GenerateCode() =>
        Random.Shared.Next(0, 10000).ToString("D4");
}

public class MaskingService : IMaskingService
{
    public string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return email;
        }

        var localPart = parts[0];
        var domainPart = parts[1];
        var maskedLocal = localPart.Length <= 2
            ? $"{localPart[0]}..."
            : $"{localPart[..2]}...";
        var domainSegments = domainPart.Split('.');
        var maskedDomain = domainSegments.Length > 1
            ? $"...{domainSegments[^1]}"
            : "...";

        return $"{maskedLocal}@{maskedDomain}";
    }
}
