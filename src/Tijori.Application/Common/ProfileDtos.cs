namespace Tijori.Application.Common;

public class ProfileDto
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? CountryCode { get; init; }
    public string? PhoneNumber { get; init; }
    public string? ProfileImageUrl { get; init; }
    public string Language { get; init; } = "ENGLISH";
    public string NotificationPreference { get; init; } = "ALL";
}
