namespace Tijori.Application.Common;

public class AuthUserDto
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CountryCode { get; init; }
    public bool IsGuest { get; init; }
    public bool IsEmailVerified { get; init; }
}

public class AuthTokenDto
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public AuthUserDto User { get; init; } = null!;
}

public class RegisterPendingDto
{
    public Guid UserId { get; init; }
    public string MaskedEmail { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
}

public class RegistrationOtpVerifiedDto
{
    public bool IsEmailVerified { get; init; }
    public AuthTokenDto AuthToken { get; init; } = null!;
}

public class OtpSentDto
{
    public string MaskedRecipient { get; init; } = string.Empty;
}

public class OtpVerifiedDto
{
    public string ResetToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

public class PasswordChangedDto
{
    public string Message { get; init; } = string.Empty;
}
