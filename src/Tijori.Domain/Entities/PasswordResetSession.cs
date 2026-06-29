using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class PasswordResetSession : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }

    public User User { get; set; } = null!;
}
