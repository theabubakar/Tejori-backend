using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectTripItem : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string? Title { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public Project Project { get; set; } = null!;
}
