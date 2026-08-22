using Tijori.Domain.Common;
using Tijori.Domain.Enums;

namespace Tijori.Domain.Entities;

public class ProjectMedicineRecord : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public MedicineRecordSection Section { get; set; }
    public string? Label { get; set; }
    public string? Value { get; set; }
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public Project Project { get; set; } = null!;
}
