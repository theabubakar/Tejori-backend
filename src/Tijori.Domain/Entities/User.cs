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
    public string? ProfileImageUrl { get; set; }
    public string Language { get; set; } = "ENGLISH";
    public string NotificationPreference { get; set; } = "ALL";

    public UserStorage? Storage { get; set; }
    public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    public ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();
    public ICollection<PasswordResetSession> PasswordResetSessions { get; set; } = new List<PasswordResetSession>();
    public ICollection<UserBucket> Buckets { get; set; } = new List<UserBucket>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public ICollection<PaymentAlert> PaymentAlerts { get; set; } = new List<PaymentAlert>();
}
