using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectAppointmentRecord : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public string? Title { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public string? AppointmentTime { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? FileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public Project Project { get; set; } = null!;
}
