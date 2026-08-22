using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class UserStorage : BaseEntity
{
    public Guid UserId { get; set; }
    public long UsedBytes { get; set; }
    public long TotalBytes { get; set; }

    public User User { get; set; } = null!;
}
