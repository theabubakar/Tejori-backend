using Microsoft.EntityFrameworkCore;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;
using Tijori.Infrastructure.Data;

namespace Tijori.Infrastructure.Repositories;

public class UserStorageRepository : IUserStorageRepository
{
    private readonly ApplicationDbContext _context;

    public UserStorageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<UserStorage?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.UserStorages.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task AddAsync(UserStorage storage, CancellationToken cancellationToken = default) =>
        await _context.UserStorages.AddAsync(storage, cancellationToken);

    public void Update(UserStorage storage) =>
        _context.UserStorages.Update(storage);
}

public class BucketCategoryRepository : IBucketCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public BucketCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BucketCategory>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await _context.BucketCategories
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<BucketCategory?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.BucketCategories
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

    public async Task<IReadOnlyList<BucketCategory>> GetAvailableForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.BucketCategories
            .Where(x =>
                x.IsActive &&
                !x.IsDraft &&
                x.ParentCategoryId == null &&
                (x.CreatedByUserId == null || x.CreatedByUserId == userId))
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<BucketCategory>> GetCustomSubCategoriesForUserAsync(
        Guid userId,
        Guid parentCategoryId,
        CancellationToken cancellationToken = default) =>
        await _context.BucketCategories
            .Where(x =>
                x.IsActive &&
                !x.IsDraft &&
                x.ParentCategoryId == parentCategoryId &&
                x.CreatedByUserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<BucketCategory?> GetSystemCustomGroupAsync(CancellationToken cancellationToken = default) =>
        _context.BucketCategories
            .FirstOrDefaultAsync(
                x => x.IsActive &&
                     x.ParentCategoryId == null &&
                     x.CreatedByUserId == null &&
                     x.IconKey == BucketCategoryKeys.Custom,
                cancellationToken);

    public async Task<IReadOnlyList<BucketCategory>> GetDraftsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _context.BucketCategories
            .Where(x => x.IsActive && x.IsDraft && x.CreatedByUserId == userId)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        _context.BucketCategories.AnyAsync(
            x => !x.IsDraft &&
                 x.ParentCategoryId == null &&
                 x.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public Task<bool> ExistsSubCategoryNameForUserAsync(
        string name,
        Guid userId,
        Guid parentCategoryId,
        CancellationToken cancellationToken = default) =>
        _context.BucketCategories.AnyAsync(
            x => !x.IsDraft &&
                 x.ParentCategoryId == parentCategoryId &&
                 x.CreatedByUserId == userId &&
                 x.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public async Task AddAsync(BucketCategory category, CancellationToken cancellationToken = default) =>
        await _context.BucketCategories.AddAsync(category, cancellationToken);

    public void Update(BucketCategory category) =>
        _context.BucketCategories.Update(category);

    public void Remove(BucketCategory category) =>
        _context.BucketCategories.Remove(category);
}

public class CategoryFormFieldRepository : ICategoryFormFieldRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryFormFieldRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryFormField>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default) =>
        await _context.CategoryFormFields
            .Where(x => x.BucketCategoryId == categoryId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Label)
            .ToListAsync(cancellationToken);

    public Task<CategoryFormField?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.CategoryFormFields.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByFieldKeyAsync(
        Guid categoryId,
        string fieldKey,
        CancellationToken cancellationToken = default) =>
        _context.CategoryFormFields.AnyAsync(
            x => x.BucketCategoryId == categoryId &&
                 x.FieldKey.ToLower() == fieldKey.ToLower(),
            cancellationToken);

    public async Task<int> GetNextSortOrderAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var maxSortOrder = await _context.CategoryFormFields
            .Where(x => x.BucketCategoryId == categoryId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync(cancellationToken);

        return (maxSortOrder ?? 0) + 1;
    }

    public async Task AddAsync(CategoryFormField field, CancellationToken cancellationToken = default) =>
        await _context.CategoryFormFields.AddAsync(field, cancellationToken);
}

public class UserBucketRepository : IUserBucketRepository
{
    private readonly ApplicationDbContext _context;

    public UserBucketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserBucket>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.UserBuckets
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

    public Task<UserBucket?> GetByUserAndCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default) =>
        _context.UserBuckets
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.BucketCategoryId == categoryId,
                cancellationToken);

    public async Task AddAsync(UserBucket bucket, CancellationToken cancellationToken = default) =>
        await _context.UserBuckets.AddAsync(bucket, cancellationToken);

    public void Update(UserBucket bucket) =>
        _context.UserBuckets.Update(bucket);
}

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetOngoingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Projects
            .Include(x => x.BucketCategory)
            .Where(x => x.UserId == userId && x.Status == ProjectStatus.Ongoing)
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);

    public Task<Project?> GetByIdForUserAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        _context.Projects
            .Include(x => x.BucketCategory)
            .FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId, cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await _context.Projects.AddAsync(project, cancellationToken);

    public async Task<int> GetNextSortOrderAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var maxSortOrder = await _context.Projects
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync(cancellationToken);

        return (maxSortOrder ?? 0) + 1;
    }

    public Task<bool> ExistsForUserAndCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default) =>
        _context.Projects.AnyAsync(
            x => x.UserId == userId && x.BucketCategoryId == categoryId,
            cancellationToken);
}

public class MilestoneRepository : IMilestoneRepository
{
    private readonly ApplicationDbContext _context;

    public MilestoneRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Milestone>> GetUpcomingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Milestones
            .Include(x => x.Project)
            .Where(x =>
                x.UserId == userId &&
                x.Status != MilestoneStatus.Completed)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
}

public class PaymentAlertRepository : IPaymentAlertRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentAlertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PaymentAlert>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.PaymentAlerts
            .Include(x => x.Project)
            .Where(x => x.UserId == userId && x.Status != PaymentAlertStatus.Paid)
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
}
