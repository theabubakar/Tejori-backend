using Tijori.Application.Common;

namespace Tijori.Application.Interfaces.Services;

public interface IBucketService
{
    Task<BucketSetupDto> GetSetupAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BucketCategoryOptionDto> AddCustomCategoryAsync(
        Guid userId,
        AddCustomCategoryRequest request,
        CancellationToken cancellationToken = default);
    Task DeleteCustomCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
    Task<UploadedBucketFileDto> UploadFileAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
    Task<CreateBucketResultDto> CreateBucketAsync(
        Guid userId,
        CreateBucketRequest request,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryFormFieldDto>> GetCategoryFormFieldsAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
    Task<CategoryFormFieldDto> AddCategoryFormFieldAsync(
        Guid userId,
        Guid categoryId,
        AddCategoryFormFieldRequest request,
        CancellationToken cancellationToken = default);
}
