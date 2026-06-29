using Tijori.Application.Common;
using Tijori.Domain.Entities;

namespace Tijori.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterPendingDto> RegisterAsync(DTOs.Auth.RegisterRequest request, CancellationToken cancellationToken = default);
    Task<RegistrationOtpVerifiedDto> VerifyRegistrationOtpAsync(DTOs.Auth.VerifyRegistrationOtpRequest request, CancellationToken cancellationToken = default);
    Task<OtpSentDto> ResendRegistrationOtpByEmailAsync(DTOs.Auth.ResendRegistrationOtpByEmailRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokenDto> LoginAsync(DTOs.Auth.LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokenDto> GuestAccessAsync(CancellationToken cancellationToken = default);
    Task<OtpSentDto> SendForgotPasswordOtpByEmailAsync(DTOs.Auth.SendForgotPasswordOtpByEmailRequest request, CancellationToken cancellationToken = default);
    Task<OtpVerifiedDto> VerifyForgotPasswordOtpAsync(DTOs.Auth.VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task<OtpSentDto> ResendForgotPasswordOtpByEmailAsync(DTOs.Auth.ResendOtpByEmailRequest request, CancellationToken cancellationToken = default);
    Task<PasswordChangedDto> ResetPasswordAsync(DTOs.Auth.ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokenDto> AppleSignInAsync(DTOs.Auth.SocialLoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokenDto> GoogleSignInAsync(DTOs.Auth.SocialLoginRequest request, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IOtpHasher
{
    string Hash(string code);
    bool Verify(string code, string codeHash);
}

public interface IJwtTokenService
{
    AuthTokenDto GenerateToken(User user);
}

public interface IOtpService
{
    string GenerateCode();
}

public interface IEmailService
{
    Task SendOtpAsync(string email, string code, CancellationToken cancellationToken = default);
}

public interface IOtpDeliveryService
{
    Task SendEmailOtpAsync(string email, string code, CancellationToken cancellationToken = default);
}

public interface ISocialTokenValidator
{
    Task<SocialUserInfo> ValidateAppleTokenAsync(string idToken, CancellationToken cancellationToken = default);
    Task<SocialUserInfo> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default);
}

public class SocialUserInfo
{
    public string ProviderKey { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? FullName { get; init; }
}

public interface IMaskingService
{
    string MaskEmail(string email);
}
