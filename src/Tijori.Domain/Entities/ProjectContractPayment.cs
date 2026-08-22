using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectContractPayment : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public decimal Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public string? NotificationTiming { get; set; }

    public Project Project { get; set; } = null!;
}
