using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectCustomFieldValue : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Guid CategoryFormFieldId { get; set; }
    public string? Value { get; set; }
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public Project Project { get; set; } = null!;
    public CategoryFormField CategoryFormField { get; set; } = null!;
}
