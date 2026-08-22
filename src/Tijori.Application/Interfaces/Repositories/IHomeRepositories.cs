using Tijori.Domain.Entities;

namespace Tijori.Application.Interfaces.Repositories;

public interface IUserStorageRepository
{
    Task<UserStorage?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserStorage storage, CancellationToken cancellationToken = default);
    void Update(UserStorage storage);
}

public interface IBucketCategoryRepository
{
    Task<IReadOnlyList<BucketCategory>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BucketCategory>> GetAvailableForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BucketCategory>> GetCustomSubCategoriesForUserAsync(
        Guid userId,
        Guid parentCategoryId,
        CancellationToken cancellationToken = default);
    Task<BucketCategory?> GetSystemCustomGroupAsync(CancellationToken cancellationToken = default);
    Task<BucketCategory?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BucketCategory>> GetDraftsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsSubCategoryNameForUserAsync(
        string name,
        Guid userId,
        Guid parentCategoryId,
        CancellationToken cancellationToken = default);
    Task AddAsync(BucketCategory category, CancellationToken cancellationToken = default);
    void Update(BucketCategory category);
    void Remove(BucketCategory category);
}

public interface ICategoryFormFieldRepository
{
    Task<IReadOnlyList<CategoryFormField>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default);
    Task<CategoryFormField?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByFieldKeyAsync(
        Guid categoryId,
        string fieldKey,
        CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(CategoryFormField field, CancellationToken cancellationToken = default);
}

public interface IProjectDocumentRepository
{
    Task AddRangeAsync(IEnumerable<ProjectDocument> documents, CancellationToken cancellationToken = default);
}

public interface IUserBucketRepository
{
    Task<IReadOnlyList<UserBucket>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserBucket?> GetByUserAndCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
    Task AddAsync(UserBucket bucket, CancellationToken cancellationToken = default);
    void Update(UserBucket bucket);
}

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetOngoingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdForUserAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task<int> GetNextSortOrderAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAndCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}

public interface IMilestoneRepository
{
    Task<IReadOnlyList<Milestone>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IPaymentAlertRepository
{
    Task<IReadOnlyList<PaymentAlert>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
