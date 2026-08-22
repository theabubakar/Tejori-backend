using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class UserBucket : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BucketCategoryId { get; set; }
    public int DocumentCount { get; set; }

    public User User { get; set; } = null!;
    public BucketCategory BucketCategory { get; set; } = null!;
}
