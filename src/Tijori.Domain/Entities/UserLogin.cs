using Tijori.Domain.Common;
using Tijori.Domain.Enums;

namespace Tijori.Domain.Entities;

public class UserLogin : BaseEntity
{
    public Guid UserId { get; set; }
    public SocialLoginProvider Provider { get; set; }
    public string ProviderKey { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
