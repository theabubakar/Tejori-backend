using Tijori.Application.Common;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;

namespace Tijori.Application.Mappers;

public static class HomeMapper
{
    public static HomeUserSummaryDto ToUserSummaryDto(User user) =>
        new()
        {
            UserId = user.Id,
            FullName = user.FullName,
            ProfileImageUrl = user.ProfileImageUrl
        };

    public static HomeStorageDto ToStorageDto(UserStorage storage)
    {
        var percentageUsed = storage.TotalBytes <= 0
            ? 0
            : Math.Round((decimal)storage.UsedBytes / storage.TotalBytes * 100, 1);

        return new HomeStorageDto
        {
            UsedBytes = storage.UsedBytes,
            TotalBytes = storage.TotalBytes,
            PercentageUsed = percentageUsed
        };
    }

    public static HomeBucketDto ToBucketDto(BucketCategory category, int documentCount) =>
        new()
        {
            Id = category.Id,
            CategoryId = category.Id,
            Name = category.Name,
            IconKey = category.IconKey,
            DocumentCount = documentCount
        };

    public static HomeProjectDto ToProjectDto(Project project) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            CategoryId = project.BucketCategoryId,
            CategoryName = project.BucketCategory?.Name,
            DocumentCount = project.DocumentCount,
            Status = project.Status.ToString()
        };

    public static HomeMilestoneDto ToMilestoneDto(Milestone milestone) =>
        new()
        {
            Id = milestone.Id,
            Title = milestone.Title,
            ProjectId = milestone.ProjectId,
            ProjectName = milestone.Project.Name,
            DueDate = milestone.DueDate,
            Amount = milestone.Amount,
            Currency = milestone.Currency,
            ProgressPercentage = milestone.ProgressPercentage,
            Status = milestone.Status.ToString()
        };

    public static HomePaymentAlertDto ToPaymentAlertDto(PaymentAlert paymentAlert) =>
        new()
        {
            Id = paymentAlert.Id,
            Title = paymentAlert.Title,
            ProjectId = paymentAlert.ProjectId,
            ProjectName = paymentAlert.Project?.Name,
            DueDate = paymentAlert.DueDate,
            Amount = paymentAlert.Amount,
            Currency = paymentAlert.Currency,
            ProgressPercentage = paymentAlert.ProgressPercentage,
            Status = paymentAlert.Status.ToString()
        };
}
