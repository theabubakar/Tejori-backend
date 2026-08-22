using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;

namespace Tijori.API.Controllers;

[Authorize]
[ApiController]
[Route("api/buckets")]
public class BucketsController : ControllerBase
{
    private readonly IBucketService _bucketService;
    private readonly ICurrentUserService _currentUserService;

    public BucketsController(IBucketService bucketService, ICurrentUserService currentUserService)
    {
        _bucketService = bucketService;
        _currentUserService = currentUserService;
    }

    [HttpGet("setup")]
    [ProducesResponseType(typeof(ApiResponse<BucketSetupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSetup(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _bucketService.GetSetupAsync(userId, cancellationToken);
        return Ok(ApiResponse<BucketSetupDto>.Ok(result, "Bucket setup fetched successfully."));
    }

    [HttpPost("categories")]
    [ProducesResponseType(typeof(ApiResponse<BucketCategoryOptionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddCustomCategory(
        [FromBody] AddCustomCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _bucketService.AddCustomCategoryAsync(userId, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<BucketCategoryOptionDto>.Ok(result, "Category added successfully."));
    }

    [HttpDelete("categories/{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCustomCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        await _bucketService.DeleteCustomCategoryAsync(userId, categoryId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Custom category deleted successfully."));
    }

    [HttpPost("files")]
    [ProducesResponseType(typeof(ApiResponse<UploadedBucketFileDto>), StatusCodes.Status201Created)]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse.Fail("A file is required."));
        }

        var userId = _currentUserService.GetRequiredUserId();
        await using var stream = file.OpenReadStream();
        var result = await _bucketService.UploadFileAsync(
            userId,
            stream,
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<UploadedBucketFileDto>.Ok(result, "File uploaded successfully."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateBucketResultDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBucket(
        [FromBody] CreateBucketRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _bucketService.CreateBucketAsync(userId, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<CreateBucketResultDto>.Ok(result, "Bucket created successfully."));
    }

    [HttpGet("categories/{categoryId:guid}/fields")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CategoryFormFieldDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryFormFields(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _bucketService.GetCategoryFormFieldsAsync(userId, categoryId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CategoryFormFieldDto>>.Ok(result, "Category form fields fetched successfully."));
    }

    [HttpPost("categories/{categoryId:guid}/fields")]
    [ProducesResponseType(typeof(ApiResponse<CategoryFormFieldDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddCategoryFormField(
        Guid categoryId,
        [FromBody] AddCategoryFormFieldRequest request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var result = await _bucketService.AddCategoryFormFieldAsync(userId, categoryId, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<CategoryFormFieldDto>.Ok(result, "Category form field added successfully."));
    }
}
