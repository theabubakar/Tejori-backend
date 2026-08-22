using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class BucketCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDraft { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public BucketCategory? ParentCategory { get; set; }
    public ICollection<BucketCategory> ChildCategories { get; set; } = new List<BucketCategory>();
    public ICollection<UserBucket> UserBuckets { get; set; } = new List<UserBucket>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<CategoryFormField> FormFields { get; set; } = new List<CategoryFormField>();
}
