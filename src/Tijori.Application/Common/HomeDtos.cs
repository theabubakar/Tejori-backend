using Tijori.Domain.Entities;

namespace Tijori.Application.Common;

public class HomeDto
{
    public HomeUserSummaryDto User { get; init; } = null!;
    public HomeStorageDto Storage { get; init; } = null!;
    public IReadOnlyList<HomeBucketDto> Buckets { get; init; } = Array.Empty<HomeBucketDto>();
    public IReadOnlyList<HomeProjectDto> OngoingProjects { get; init; } = Array.Empty<HomeProjectDto>();
    public IReadOnlyList<HomeMilestoneDto> UpcomingMilestones { get; init; } = Array.Empty<HomeMilestoneDto>();
    public IReadOnlyList<HomePaymentAlertDto> PaymentAlerts { get; init; } = Array.Empty<HomePaymentAlertDto>();
}

public class HomeUserSummaryDto
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? ProfileImageUrl { get; init; }
}

public class HomeStorageDto
{
    public long UsedBytes { get; init; }
    public long TotalBytes { get; init; }
    public decimal PercentageUsed { get; init; }
}

public class HomeBucketDto
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string IconKey { get; init; } = string.Empty;
    public int DocumentCount { get; init; }
}

public class HomeProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public int DocumentCount { get; init; }
    public string Status { get; init; } = string.Empty;
}

public class HomeMilestoneDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public DateTime DueDate { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int ProgressPercentage { get; init; }
    public string Status { get; init; } = string.Empty;
}

public class HomePaymentAlertDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public DateTime DueDate { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int ProgressPercentage { get; init; }
    public string Status { get; init; } = string.Empty;
}
