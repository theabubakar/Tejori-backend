using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class CategoryFormField : BaseEntity
{
    public Guid BucketCategoryId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? OptionsJson { get; set; }

    public BucketCategory BucketCategory { get; set; } = null!;
    public ICollection<ProjectCustomFieldValue> ProjectValues { get; set; } = new List<ProjectCustomFieldValue>();
}
