using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class User : BaseEntity
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public bool TermsAccepted { get; set; }
    public bool IsGuest { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? PasswordUpdatedAt { get; set; }

    public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    public ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();
    public ICollection<PasswordResetSession> PasswordResetSessions { get; set; } = new List<PasswordResetSession>();
}
