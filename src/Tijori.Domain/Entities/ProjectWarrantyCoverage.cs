using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectWarrantyCoverage : BaseEntity
{
    public Guid ProjectId { get; set; }
    public int SortOrder { get; set; }
    public string? CoverageArea { get; set; }
    public string? CoverageOption { get; set; }

    public Project Project { get; set; } = null!;
}
