using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectDocument : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ExtensionDate { get; set; }

    public Project Project { get; set; } = null!;
}
