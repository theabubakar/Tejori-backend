using Microsoft.Extensions.Options;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Mappers;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;
using Tijori.Application.Options;

namespace Tijori.Application.Services;

public class HomeService : IHomeService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserStorageRepository _userStorageRepository;
    private readonly IBucketCategoryRepository _bucketCategoryRepository;
    private readonly IUserBucketRepository _userBucketRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IPaymentAlertRepository _paymentAlertRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StorageSettings _storageSettings;

    public HomeService(
        IUserRepository userRepository,
        IUserStorageRepository userStorageRepository,
        IBucketCategoryRepository bucketCategoryRepository,
        IUserBucketRepository userBucketRepository,
        IProjectRepository projectRepository,
        IMilestoneRepository milestoneRepository,
        IPaymentAlertRepository paymentAlertRepository,
        IUnitOfWork unitOfWork,
        IOptions<StorageSettings> storageSettings)
    {
        _userRepository = userRepository;
        _userStorageRepository = userStorageRepository;
        _bucketCategoryRepository = bucketCategoryRepository;
        _userBucketRepository = userBucketRepository;
        _projectRepository = projectRepository;
        _milestoneRepository = milestoneRepository;
        _paymentAlertRepository = paymentAlertRepository;
        _unitOfWork = unitOfWork;
        _storageSettings = storageSettings.Value;
    }

    public async Task<HomeDto> GetHomeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var storage = await EnsureUserStorageAsync(userId, cancellationToken);

        var categories = await _bucketCategoryRepository.GetAvailableForUserAsync(userId, cancellationToken);
        var userBuckets = await _userBucketRepository.GetByUserIdAsync(userId, cancellationToken);
        var documentCountsByCategory = userBuckets.ToDictionary(x => x.BucketCategoryId, x => x.DocumentCount);

        var buckets = categories
            .Select(category => HomeMapper.ToBucketDto(
                category,
                documentCountsByCategory.GetValueOrDefault(category.Id)))
            .ToList();

        var ongoingProjects = await _projectRepository.GetOngoingByUserIdAsync(userId, cancellationToken);
        var upcomingMilestones = await _milestoneRepository.GetUpcomingByUserIdAsync(userId, cancellationToken);
        var paymentAlerts = await _paymentAlertRepository.GetActiveByUserIdAsync(userId, cancellationToken);

        return new HomeDto
        {
            User = HomeMapper.ToUserSummaryDto(user),
            Storage = HomeMapper.ToStorageDto(storage),
            Buckets = buckets,
            OngoingProjects = ongoingProjects.Select(HomeMapper.ToProjectDto).ToList(),
            UpcomingMilestones = upcomingMilestones.Select(HomeMapper.ToMilestoneDto).ToList(),
            PaymentAlerts = paymentAlerts.Select(HomeMapper.ToPaymentAlertDto).ToList()
        };
    }

    private async Task<UserStorage> EnsureUserStorageAsync(Guid userId, CancellationToken cancellationToken)
    {
        var storage = await _userStorageRepository.GetByUserIdAsync(userId, cancellationToken);
        if (storage is not null)
        {
            return storage;
        }

        var now = DateTime.UtcNow;
        storage = new UserStorage
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UsedBytes = 0,
            TotalBytes = _storageSettings.DefaultTotalBytes,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userStorageRepository.AddAsync(storage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return storage;
    }
}
