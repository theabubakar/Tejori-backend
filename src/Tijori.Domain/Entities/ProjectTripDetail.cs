using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectTripDetail : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string? Destination { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }

    public Project Project { get; set; } = null!;
}
