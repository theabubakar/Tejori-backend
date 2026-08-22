using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectContractPhase : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public string? NotificationTiming { get; set; }
    public int? ProgressPercentage { get; set; }

    public Project Project { get; set; } = null!;
}
