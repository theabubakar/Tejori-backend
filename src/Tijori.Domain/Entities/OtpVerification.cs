using Tijori.Domain.Common;
using Tijori.Domain.Enums;

namespace Tijori.Domain.Entities;

public class OtpVerification : BaseEntity
{
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public OtpChannel Channel { get; set; }
    public OtpPurpose Purpose { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int FailedAttempts { get; set; }

    public User User { get; set; } = null!;
}
