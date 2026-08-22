using Microsoft.Extensions.Options;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Options;
using Tijori.Domain.Entities;
using Tijori.Domain.Enums;

namespace Tijori.Application.Services;

public class BucketService : IBucketService
{
    private readonly IUserRepository _userRepository;
    private readonly IBucketCategoryRepository _bucketCategoryRepository;
    private readonly ICategoryFormFieldRepository _categoryFormFieldRepository;
    private readonly IUserBucketRepository _userBucketRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectDocumentRepository _projectDocumentRepository;
    private readonly IUserStorageRepository _userStorageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUploadedFileStore _uploadedFileStore;
    private readonly StorageSettings _storageSettings;

    public BucketService(
        IUserRepository userRepository,
        IBucketCategoryRepository bucketCategoryRepository,
        ICategoryFormFieldRepository categoryFormFieldRepository,
        IUserBucketRepository userBucketRepository,
        IProjectRepository projectRepository,
        IProjectDocumentRepository projectDocumentRepository,
        IUserStorageRepository userStorageRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IUploadedFileStore uploadedFileStore,
        IOptions<StorageSettings> storageSettings)
    {
        _userRepository = userRepository;
        _bucketCategoryRepository = bucketCategoryRepository;
        _categoryFormFieldRepository = categoryFormFieldRepository;
        _userBucketRepository = userBucketRepository;
        _projectRepository = projectRepository;
        _projectDocumentRepository = projectDocumentRepository;
        _userStorageRepository = userStorageRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _uploadedFileStore = uploadedFileStore;
        _storageSettings = storageSettings.Value;
    }

    public async Task<BucketSetupDto> GetSetupAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var categories = await _bucketCategoryRepository.GetAvailableForUserAsync(userId, cancellationToken);
        var customGroup = categories.FirstOrDefault(x => BucketCategoryKeys.IsCustomGroup(x.IconKey))
            ?? await _bucketCategoryRepository.GetSystemCustomGroupAsync(cancellationToken);

        IReadOnlyList<BucketCategory> customCategories = Array.Empty<BucketCategory>();
        if (customGroup is not null)
        {
            customCategories = await _bucketCategoryRepository.GetCustomSubCategoriesForUserAsync(
                userId,
                customGroup.Id,
                cancellationToken);
        }

        return new BucketSetupDto
        {
            Categories = categories.Select(ToCategoryOptionDto).ToList(),
            CustomGroupCategoryId = customGroup?.Id,
            CustomCategories = customCategories.Select(ToCategoryOptionDto).ToList(),
            WarrantySubCategories = GetWarrantySubCategoryOptions()
        };
    }

    public async Task<BucketCategoryOptionDto> AddCustomCategoryAsync(
        Guid userId,
        AddCustomCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var customGroup = await _bucketCategoryRepository.GetSystemCustomGroupAsync(cancellationToken)
            ?? throw new NotFoundAppException("Custom category group not found.");

        var trimmedName = request.Name.Trim();
        if (await _bucketCategoryRepository.ExistsSubCategoryNameForUserAsync(
                trimmedName,
                userId,
                customGroup.Id,
                cancellationToken))
        {
            throw new ConflictAppException("A custom category with this name already exists.");
        }

        var existingDrafts = await _bucketCategoryRepository.GetDraftsByUserIdAsync(userId, cancellationToken);
        foreach (var draft in existingDrafts)
        {
            _bucketCategoryRepository.Remove(draft);
        }

        var now = DateTime.UtcNow;
        var category = new BucketCategory
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            IconKey = BucketCategoryKeys.CustomSub,
            ParentCategoryId = customGroup.Id,
            SortOrder = 0,
            IsActive = true,
            IsDraft = true,
            CreatedByUserId = userId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _bucketCategoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToCategoryOptionDto(category);
    }

    public async Task DeleteCustomCategoryAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var category = await _bucketCategoryRepository.GetActiveByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundAppException("Custom category not found.");

        EnsureCategoryAccessible(category, userId);

        if (!BucketCategoryKeys.IsCustomSubCategory(category.IconKey) || category.ParentCategoryId is null)
        {
            throw new ValidationAppException(new[] { "Only user custom categories can be deleted." });
        }

        if (await _projectRepository.ExistsForUserAndCategoryAsync(userId, categoryId, cancellationToken))
        {
            throw new ValidationAppException(new[] { "This custom category is already used by a bucket and cannot be deleted." });
        }

        _bucketCategoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static BucketCategoryOptionDto ToCategoryOptionDto(BucketCategory category) =>
        new()
        {
            Id = category.Id,
            Name = category.Name,
            IconKey = category.IconKey,
            IsCustom = category.CreatedByUserId.HasValue
        };

    public async Task<UploadedBucketFileDto> UploadFileAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        if (fileStream.CanSeek && fileStream.Length > _storageSettings.MaxUploadBytes)
        {
            throw new ValidationAppException(new[] { "File exceeds the maximum upload size." });
        }

        var (storedFileName, sizeBytes) = await _fileStorageService.SaveUserFileAsync(
            userId,
            fileStream,
            fileName,
            cancellationToken);

        if (sizeBytes > _storageSettings.MaxUploadBytes)
        {
            await _fileStorageService.DeleteUserFileAsync(userId, storedFileName, cancellationToken);
            throw new ValidationAppException(new[] { "File exceeds the maximum upload size." });
        }

        var fileToken = Guid.NewGuid().ToString("N");
        _uploadedFileStore.Store(userId, fileToken, new UploadedFileMetadata
        {
            StoredFileName = storedFileName,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = sizeBytes
        });

        return new UploadedBucketFileDto
        {
            FileToken = fileToken,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = sizeBytes
        };
    }

    public async Task<CreateBucketResultDto> CreateBucketAsync(
        Guid userId,
        CreateBucketRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var category = await _bucketCategoryRepository.GetActiveByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundAppException("Bucket category not found.");

        if (category.CreatedByUserId.HasValue && category.CreatedByUserId != userId)
        {
            throw new UnauthorizedAppException("You cannot use this category.");
        }

        ValidateFlowRequest(category.IconKey, request);

        var now = DateTime.UtcNow;
        var sortOrder = await _projectRepository.GetNextSortOrderAsync(userId, cancellationToken);
        var projectId = Guid.NewGuid();
        var documentBuildResult = BuildDocuments(userId, projectId, request.Documents, now);
        var flowAttachmentResult = new FlowAttachmentResult([], 0, 0);

        var project = new Project
        {
            Id = projectId,
            UserId = userId,
            BucketCategoryId = category.Id,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            WarrantySubCategoryKey = request.WarrantySubCategoryKey,
            ScanWithAiOcr = request.ScanWithAiOcr,
            Remarks = request.Remarks,
            DocumentCount = documentBuildResult.Entities.Count,
            Status = ProjectStatus.Ongoing,
            SortOrder = sortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _projectRepository.AddAsync(project, cancellationToken);

        if (request.Contract is not null)
        {
            await ApplyContractDetailsAsync(userId, project, request.Contract, now, cancellationToken);
        }

        if (request.Warranty is not null)
        {
            ApplyWarrantyDetails(project, request.Warranty, now);
        }

        if (request.Trip is not null)
        {
            flowAttachmentResult = flowAttachmentResult.Merge(
                ApplyTripDetailsAsync(userId, project, request.Trip, now));
        }

        if (request.Appointment is not null)
        {
            flowAttachmentResult = flowAttachmentResult.Merge(
                ApplyAppointmentDetailsAsync(userId, project, request.Appointment, now));
        }

        if (request.Medicine.Count > 0)
        {
            flowAttachmentResult = flowAttachmentResult.Merge(
                ApplyMedicineDetailsAsync(userId, project, request.Medicine, now));
        }

        if (request.CustomFieldValues.Count > 0)
        {
            flowAttachmentResult = flowAttachmentResult.Merge(
                await ApplyCustomFieldValuesAsync(userId, project, category.Id, request.CustomFieldValues, now, cancellationToken));
        }

        var totalDocumentCount = documentBuildResult.Entities.Count + flowAttachmentResult.FileCount;
        project.DocumentCount = totalDocumentCount;

        if (documentBuildResult.Entities.Count > 0)
        {
            await _projectDocumentRepository.AddRangeAsync(documentBuildResult.Entities, cancellationToken);
        }

        var totalBytes = documentBuildResult.TotalBytes + flowAttachmentResult.TotalBytes;
        var consumedFileTokens = documentBuildResult.ConsumedFileTokens
            .Concat(flowAttachmentResult.ConsumedFileTokens)
            .ToList();

        await EnsureUserBucketAsync(userId, category.Id, totalDocumentCount, now, cancellationToken);
        await UpdateStorageUsageAsync(userId, totalBytes, now, cancellationToken);

        if (category.IsDraft)
        {
            category.IsDraft = false;
            category.UpdatedAt = now;
            _bucketCategoryRepository.Update(category);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var fileToken in consumedFileTokens)
        {
            _uploadedFileStore.Remove(userId, fileToken);
        }

        return BuildSuccessResult(project, category);
    }

    public async Task<IReadOnlyList<CategoryFormFieldDto>> GetCategoryFormFieldsAsync(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var category = await _bucketCategoryRepository.GetActiveByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundAppException("Bucket category not found.");

        EnsureCategoryAccessible(category, userId);

        if (!BucketCategoryKeys.IsCustomSubCategory(category.IconKey))
        {
            throw new ValidationAppException(new[] { "Form fields are only available for custom categories." });
        }

        var fields = await _categoryFormFieldRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        return fields.Select(ToCategoryFormFieldDto).ToList();
    }

    public async Task<CategoryFormFieldDto> AddCategoryFormFieldAsync(
        Guid userId,
        Guid categoryId,
        AddCategoryFormFieldRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException("User not found.");

        var category = await _bucketCategoryRepository.GetActiveByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundAppException("Bucket category not found.");

        EnsureCategoryAccessible(category, userId);

        if (!BucketCategoryKeys.IsCustomSubCategory(category.IconKey))
        {
            throw new ValidationAppException(new[] { "Form fields can only be added to custom categories." });
        }

        var trimmedLabel = request.Label.Trim();
        var trimmedFieldKey = request.FieldKey.Trim();
        var trimmedFieldType = request.FieldType.Trim();

        if (string.IsNullOrWhiteSpace(trimmedLabel) ||
            string.IsNullOrWhiteSpace(trimmedFieldKey) ||
            string.IsNullOrWhiteSpace(trimmedFieldType))
        {
            throw new ValidationAppException(new[] { "Label, field key, and field type are required." });
        }

        if (await _categoryFormFieldRepository.ExistsByFieldKeyAsync(categoryId, trimmedFieldKey, cancellationToken))
        {
            throw new ConflictAppException("A field with this key already exists for the category.");
        }

        var now = DateTime.UtcNow;
        var sortOrder = await _categoryFormFieldRepository.GetNextSortOrderAsync(categoryId, cancellationToken);
        var field = new CategoryFormField
        {
            Id = Guid.NewGuid(),
            BucketCategoryId = categoryId,
            Label = trimmedLabel,
            FieldKey = trimmedFieldKey,
            FieldType = trimmedFieldType,
            IsRequired = request.IsRequired,
            SortOrder = sortOrder,
            OptionsJson = request.OptionsJson,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _categoryFormFieldRepository.AddAsync(field, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToCategoryFormFieldDto(field);
    }

    private static void EnsureCategoryAccessible(BucketCategory category, Guid userId)
    {
        if (category.CreatedByUserId.HasValue && category.CreatedByUserId != userId)
        {
            throw new UnauthorizedAppException("You cannot use this category.");
        }
    }

    private static CategoryFormFieldDto ToCategoryFormFieldDto(CategoryFormField field) =>
        new()
        {
            Id = field.Id,
            Label = field.Label,
            FieldKey = field.FieldKey,
            FieldType = field.FieldType,
            IsRequired = field.IsRequired,
            SortOrder = field.SortOrder,
            OptionsJson = field.OptionsJson
        };

    private static IReadOnlyList<WarrantySubCategoryOptionDto> GetWarrantySubCategoryOptions() =>
        new List<WarrantySubCategoryOptionDto>
        {
            new() { Key = WarrantySubCategoryKeys.Watches, Name = "Watches", Description = "This type of warranty is for all kind of original watches." },
            new() { Key = WarrantySubCategoryKeys.Jewelaries, Name = "Jewelaries", Description = "This type of warranty is for jewelry items." },
            new() { Key = WarrantySubCategoryKeys.Bags, Name = "Bags", Description = "This type of warranty is for bags and accessories." },
            new() { Key = WarrantySubCategoryKeys.Others, Name = "Others", Description = "This type of warranty is for other items." }
        };

    private void ValidateFlowRequest(string iconKey, CreateBucketRequest request)
    {
        if (BucketCategoryKeys.IsContract(iconKey) && request.Contract is null)
        {
            throw new ValidationAppException(new[] { "Contract details are required for this bucket type." });
        }

        if (BucketCategoryKeys.IsWarranty(iconKey))
        {
            if (string.IsNullOrWhiteSpace(request.WarrantySubCategoryKey))
            {
                throw new ValidationAppException(new[] { "Warranty category is required." });
            }

            if (request.Warranty is null)
            {
                throw new ValidationAppException(new[] { "Warranty details are required." });
            }
        }

        if (BucketCategoryKeys.IsMyTrips(iconKey) && request.Trip is null)
        {
            throw new ValidationAppException(new[] { "Trip details are required for this bucket type." });
        }

        if (BucketCategoryKeys.IsMyAppointments(iconKey) && request.Appointment is null)
        {
            throw new ValidationAppException(new[] { "Appointment details are required for this bucket type." });
        }

        if (BucketCategoryKeys.IsMyMedicine(iconKey) && request.Medicine.Count == 0)
        {
            throw new ValidationAppException(new[] { "Medicine records are required for this bucket type." });
        }

        if (BucketCategoryKeys.IsCustomSubCategory(iconKey) && request.CustomFieldValues.Count == 0)
        {
            throw new ValidationAppException(new[] { "Custom field values are required for this bucket type." });
        }
    }

    private sealed record FlowAttachmentResult(
        List<string> ConsumedFileTokens,
        long TotalBytes,
        int FileCount)
    {
        public FlowAttachmentResult Merge(FlowAttachmentResult other) =>
            new(
                ConsumedFileTokens.Concat(other.ConsumedFileTokens).ToList(),
                TotalBytes + other.TotalBytes,
                FileCount + other.FileCount);
    }

    private static CreateBucketResultDto BuildSuccessResult(Project project, BucketCategory category)
    {
        if (BucketCategoryKeys.IsWarranty(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "warranty",
                SuccessTitle = "Warranty successfully Added",
                SuccessMessage = $"Congrats! Your warranty was added to '{project.Name}' bucket."
            };
        }

        if (BucketCategoryKeys.IsContract(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "contract",
                SuccessTitle = "Contract successfully Added",
                SuccessMessage = $"Congrats! Your contract was added to '{project.Name}' bucket."
            };
        }

        if (BucketCategoryKeys.IsMyTrips(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "trip",
                SuccessTitle = "Trip successfully Added",
                SuccessMessage = $"Congrats! Your trip was added to '{project.Name}' bucket."
            };
        }

        if (BucketCategoryKeys.IsMyAppointments(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "appointment",
                SuccessTitle = "Appointment successfully Added",
                SuccessMessage = $"Congrats! Your appointment was added to '{project.Name}' bucket."
            };
        }

        if (BucketCategoryKeys.IsMyMedicine(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "medicine",
                SuccessTitle = "Medicine successfully Added",
                SuccessMessage = $"Congrats! Your medicine record was added to '{project.Name}' bucket."
            };
        }

        if (BucketCategoryKeys.IsCustomSubCategory(category.IconKey))
        {
            return new CreateBucketResultDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey,
                FlowType = "custom",
                SuccessTitle = "Bucket successfully Added",
                SuccessMessage = $"Congrats! Your custom bucket '{project.Name}' was added successfully."
            };
        }

        return new CreateBucketResultDto
        {
            ProjectId = project.Id,
            Name = project.Name,
            CategoryName = category.Name,
            CategoryIconKey = category.IconKey,
            FlowType = "bucket",
            SuccessTitle = "Bucket successfully Added",
            SuccessMessage = "Congrats! Your bucket was added successfully."
        };
    }

    private sealed record DocumentBuildResult(
        List<ProjectDocument> Entities,
        long TotalBytes,
        List<string> ConsumedFileTokens);

    private DocumentBuildResult BuildDocuments(
        Guid userId,
        Guid projectId,
        IReadOnlyList<CreateBucketDocumentDto> documents,
        DateTime now)
    {
        var entities = new List<ProjectDocument>();
        var consumedFileTokens = new List<string>();
        var seenFileTokens = new HashSet<string>(StringComparer.Ordinal);
        var sortOrder = 1;
        var totalBytes = 0L;

        foreach (var document in documents)
        {
            if (string.IsNullOrWhiteSpace(document.DocumentType))
            {
                continue;
            }

            string? storedFileName = null;
            string? fileName = null;
            string? contentType = null;
            long fileSize = 0;

            if (!string.IsNullOrWhiteSpace(document.FileToken))
            {
                if (!seenFileTokens.Add(document.FileToken))
                {
                    throw new ValidationAppException(new[] { "The same uploaded file was attached more than once." });
                }

                var metadata = _uploadedFileStore.Get(userId, document.FileToken)
                    ?? throw new ValidationAppException(new[] { "One or more uploaded files are invalid or expired. Please upload the file again." });

                storedFileName = metadata.StoredFileName;
                fileName = metadata.FileName;
                contentType = metadata.ContentType;
                fileSize = metadata.FileSizeBytes;
                totalBytes += fileSize;
                consumedFileTokens.Add(document.FileToken);
            }

            entities.Add(new ProjectDocument
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                SortOrder = sortOrder++,
                DocumentType = document.DocumentType.Trim(),
                FileName = fileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                FileSizeBytes = fileSize,
                StartDate = document.StartDate,
                EndDate = document.EndDate,
                ExtensionDate = document.ExtensionDate,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        return new DocumentBuildResult(entities, totalBytes, consumedFileTokens);
    }

    private FlowAttachmentResult ResolveFileAttachment(
        Guid userId,
        string? fileToken,
        HashSet<string> seenFileTokens)
    {
        if (string.IsNullOrWhiteSpace(fileToken))
        {
            return new FlowAttachmentResult([], 0, 0);
        }

        if (!seenFileTokens.Add(fileToken))
        {
            throw new ValidationAppException(new[] { "The same uploaded file was attached more than once." });
        }

        var metadata = _uploadedFileStore.Get(userId, fileToken)
            ?? throw new ValidationAppException(new[] { "One or more uploaded files are invalid or expired. Please upload the file again." });

        return new FlowAttachmentResult(
            [fileToken],
            metadata.FileSizeBytes,
            1);
    }

    private FlowAttachmentResult ApplyTripDetailsAsync(
        Guid userId,
        Project project,
        CreateBucketTripDetailDto trip,
        DateTime now)
    {
        project.TripDetail = new ProjectTripDetail
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Destination = trip.Destination?.Trim(),
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Notes = trip.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        var result = new FlowAttachmentResult([], 0, 0);
        var seenFileTokens = new HashSet<string>(StringComparer.Ordinal);
        var sortOrder = 1;

        foreach (var item in trip.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ItemType))
            {
                continue;
            }

            if (!TripItemTypeKeys.IsValid(item.ItemType))
            {
                throw new ValidationAppException(new[] { $"Invalid trip item type: {item.ItemType}" });
            }

            var attachment = ResolveFileAttachment(userId, item.FileToken, seenFileTokens);
            UploadedFileMetadata? metadata = attachment.FileCount > 0
                ? _uploadedFileStore.Get(userId, item.FileToken!)
                : null;

            project.TripItems.Add(new ProjectTripItem
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = sortOrder++,
                ItemType = item.ItemType.Trim(),
                Title = item.Title?.Trim(),
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Location = item.Location?.Trim(),
                ReferenceNumber = item.ReferenceNumber?.Trim(),
                Notes = item.Notes,
                FileName = metadata?.FileName,
                StoredFileName = metadata?.StoredFileName,
                ContentType = metadata?.ContentType,
                FileSizeBytes = attachment.TotalBytes,
                CreatedAt = now,
                UpdatedAt = now
            });

            result = result.Merge(attachment);
        }

        return result;
    }

    private FlowAttachmentResult ApplyAppointmentDetailsAsync(
        Guid userId,
        Project project,
        CreateBucketAppointmentDetailDto appointment,
        DateTime now)
    {
        project.AppointmentDetail = new ProjectAppointmentDetail
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            ProviderName = appointment.ProviderName?.Trim(),
            FacilityName = appointment.FacilityName?.Trim(),
            Specialty = appointment.Specialty?.Trim(),
            PhoneCountryCode = appointment.PhoneCountryCode,
            Phone = appointment.Phone,
            Email = appointment.Email?.Trim(),
            Address = appointment.Address?.Trim(),
            Notes = appointment.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        var result = new FlowAttachmentResult([], 0, 0);
        var seenFileTokens = new HashSet<string>(StringComparer.Ordinal);
        var sortOrder = 1;

        foreach (var record in appointment.Records)
        {
            if (string.IsNullOrWhiteSpace(record.Title) &&
                !record.AppointmentDate.HasValue &&
                string.IsNullOrWhiteSpace(record.FileToken))
            {
                continue;
            }

            var attachment = ResolveFileAttachment(userId, record.FileToken, seenFileTokens);
            UploadedFileMetadata? metadata = attachment.FileCount > 0
                ? _uploadedFileStore.Get(userId, record.FileToken!)
                : null;

            project.AppointmentRecords.Add(new ProjectAppointmentRecord
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = sortOrder++,
                Title = record.Title?.Trim(),
                AppointmentDate = record.AppointmentDate,
                AppointmentTime = record.AppointmentTime,
                Status = record.Status?.Trim(),
                Notes = record.Notes,
                FileName = metadata?.FileName,
                StoredFileName = metadata?.StoredFileName,
                ContentType = metadata?.ContentType,
                FileSizeBytes = attachment.TotalBytes,
                CreatedAt = now,
                UpdatedAt = now
            });

            result = result.Merge(attachment);
        }

        return result;
    }

    private FlowAttachmentResult ApplyMedicineDetailsAsync(
        Guid userId,
        Project project,
        IReadOnlyList<CreateBucketMedicineRecordDto> medicineRecords,
        DateTime now)
    {
        var result = new FlowAttachmentResult([], 0, 0);
        var seenFileTokens = new HashSet<string>(StringComparer.Ordinal);
        var sortOrder = 1;

        foreach (var record in medicineRecords)
        {
            if (string.IsNullOrWhiteSpace(record.Section))
            {
                throw new ValidationAppException(new[] { "Medicine record section is required." });
            }

            if (!Enum.TryParse<MedicineRecordSection>(record.Section, true, out var section))
            {
                throw new ValidationAppException(new[] { $"Invalid medicine record section: {record.Section}" });
            }

            var attachment = ResolveFileAttachment(userId, record.FileToken, seenFileTokens);
            UploadedFileMetadata? metadata = attachment.FileCount > 0
                ? _uploadedFileStore.Get(userId, record.FileToken!)
                : null;

            project.MedicineRecords.Add(new ProjectMedicineRecord
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = sortOrder++,
                Section = section,
                Label = record.Label?.Trim(),
                Value = record.Value,
                FileName = metadata?.FileName,
                StoredFileName = metadata?.StoredFileName,
                ContentType = metadata?.ContentType,
                FileSizeBytes = attachment.TotalBytes,
                CreatedAt = now,
                UpdatedAt = now
            });

            result = result.Merge(attachment);
        }

        return result;
    }

    private async Task<FlowAttachmentResult> ApplyCustomFieldValuesAsync(
        Guid userId,
        Project project,
        Guid categoryId,
        IReadOnlyList<CreateBucketCustomFieldValueDto> customFieldValues,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var categoryFields = await _categoryFormFieldRepository.GetByCategoryIdAsync(categoryId, cancellationToken);
        var fieldsById = categoryFields.ToDictionary(x => x.Id);
        var providedFieldIds = customFieldValues.Select(x => x.FieldId).ToHashSet();
        var result = new FlowAttachmentResult([], 0, 0);
        var seenFileTokens = new HashSet<string>(StringComparer.Ordinal);

        foreach (var requiredField in categoryFields.Where(x => x.IsRequired))
        {
            if (!providedFieldIds.Contains(requiredField.Id))
            {
                throw new ValidationAppException(new[] { $"Required field '{requiredField.Label}' is missing." });
            }
        }

        foreach (var fieldValue in customFieldValues)
        {
            if (!fieldsById.TryGetValue(fieldValue.FieldId, out var field))
            {
                throw new ValidationAppException(new[] { "One or more custom fields are invalid for this category." });
            }

            var hasValue = !string.IsNullOrWhiteSpace(fieldValue.Value);
            var hasFile = !string.IsNullOrWhiteSpace(fieldValue.FileToken);

            if (field.IsRequired && !hasValue && !hasFile)
            {
                throw new ValidationAppException(new[] { $"Required field '{field.Label}' must have a value." });
            }

            var attachment = ResolveFileAttachment(userId, fieldValue.FileToken, seenFileTokens);
            UploadedFileMetadata? metadata = attachment.FileCount > 0
                ? _uploadedFileStore.Get(userId, fieldValue.FileToken!)
                : null;

            project.CustomFieldValues.Add(new ProjectCustomFieldValue
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                CategoryFormFieldId = field.Id,
                Value = fieldValue.Value,
                FileName = metadata?.FileName,
                StoredFileName = metadata?.StoredFileName,
                ContentType = metadata?.ContentType,
                FileSizeBytes = attachment.TotalBytes,
                CreatedAt = now,
                UpdatedAt = now
            });

            result = result.Merge(attachment);
        }

        return result;
    }

    private async Task ApplyContractDetailsAsync(
        Guid userId,
        Project project,
        CreateBucketContractDetailDto contract,
        DateTime now,
        CancellationToken cancellationToken)
    {
        project.ContractDetail = new ProjectContractDetail
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            CompanyName = contract.CompanyName,
            RepresentativeName = contract.RepresentativeName,
            CompanyPhoneCountryCode = contract.CompanyPhoneCountryCode,
            CompanyPhone = contract.CompanyPhone,
            WhatsAppCountryCode = contract.WhatsAppCountryCode,
            WhatsApp = contract.WhatsApp,
            CompanyEmail = contract.CompanyEmail,
            ContractName = contract.ContractName.Trim(),
            ContractDate = contract.ContractDate,
            ContractAmount = contract.ContractAmount,
            Currency = string.IsNullOrWhiteSpace(contract.Currency) ? "KD" : contract.Currency,
            NumberOfPayments = contract.NumberOfPayments,
            PaymentMethod = contract.PaymentMethod,
            AlertListType = contract.AlertListType,
            CreatedAt = now,
            UpdatedAt = now
        };

        var paymentSort = 1;
        foreach (var payment in contract.Payments)
        {
            project.ContractPayments.Add(new ProjectContractPayment
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = paymentSort++,
                Amount = payment.Amount,
                DueDate = payment.DueDate,
                NotificationTiming = payment.NotificationTiming,
                CreatedAt = now,
                UpdatedAt = now
            });

            if (payment.DueDate.HasValue)
            {
                project.PaymentAlerts.Add(new PaymentAlert
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProjectId = project.Id,
                    Title = $"{contract.ContractName.Trim()} - Payment {paymentSort - 1}",
                    DueDate = payment.DueDate.Value,
                    Amount = payment.Amount,
                    Currency = string.IsNullOrWhiteSpace(contract.Currency) ? "KD" : contract.Currency,
                    ProgressPercentage = 0,
                    Status = PaymentAlertStatus.Next,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }

        var phaseSort = 1;
        foreach (var phase in contract.Phases)
        {
            if (string.IsNullOrWhiteSpace(phase.Title))
            {
                continue;
            }

            var phaseTitle = phase.Title.Trim();

            project.ContractPhases.Add(new ProjectContractPhase
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = phaseSort++,
                Title = phaseTitle,
                DueDate = phase.DueDate,
                NotificationTiming = phase.NotificationTiming,
                ProgressPercentage = phase.ProgressPercentage,
                CreatedAt = now,
                UpdatedAt = now
            });

            if (phase.DueDate.HasValue)
            {
                project.Milestones.Add(new Milestone
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProjectId = project.Id,
                    Title = phaseTitle,
                    DueDate = phase.DueDate.Value,
                    Amount = contract.ContractAmount ?? 0,
                    Currency = string.IsNullOrWhiteSpace(contract.Currency) ? "KD" : contract.Currency,
                    ProgressPercentage = phase.ProgressPercentage ?? 0,
                    Status = MilestoneStatus.Upcoming,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }
    }

    private static void ApplyWarrantyDetails(
        Project project,
        CreateBucketWarrantyDetailDto warranty,
        DateTime now)
    {
        project.WarrantyDetail = new ProjectWarrantyDetail
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            BrandName = warranty.BrandName,
            Price = warranty.Price,
            Currency = string.IsNullOrWhiteSpace(warranty.Currency) ? "KD" : warranty.Currency,
            SerialNumber = warranty.SerialNumber,
            SellerName = warranty.SellerName,
            SellerPhoneCountryCode = warranty.SellerPhoneCountryCode,
            SellerPhone = warranty.SellerPhone,
            StartDate = warranty.StartDate,
            ExpiryDate = warranty.ExpiryDate,
            PurchaseLocation = warranty.PurchaseLocation,
            CountryOfManufacture = warranty.CountryOfManufacture,
            StoreLocationUrl = warranty.StoreLocationUrl,
            ExpiryReminderEnabled = warranty.ExpiryReminderEnabled,
            ExpiryReminderTiming = warranty.ExpiryReminderTiming,
            CreatedAt = now,
            UpdatedAt = now
        };

        var coverageSort = 1;
        foreach (var coverage in warranty.Coverages)
        {
            project.WarrantyCoverages.Add(new ProjectWarrantyCoverage
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                SortOrder = coverageSort++,
                CoverageArea = coverage.CoverageArea,
                CoverageOption = coverage.CoverageOption,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }

    private async Task EnsureUserBucketAsync(
        Guid userId,
        Guid categoryId,
        int documentCount,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var existingUserBucket = await _userBucketRepository.GetByUserAndCategoryAsync(
            userId,
            categoryId,
            cancellationToken);

        if (existingUserBucket is null)
        {
            await _userBucketRepository.AddAsync(new UserBucket
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BucketCategoryId = categoryId,
                DocumentCount = documentCount,
                CreatedAt = now,
                UpdatedAt = now
            }, cancellationToken);
            return;
        }

        existingUserBucket.DocumentCount += documentCount;
        existingUserBucket.UpdatedAt = now;
        _userBucketRepository.Update(existingUserBucket);
    }

    private async Task UpdateStorageUsageAsync(
        Guid userId,
        long addedBytes,
        DateTime now,
        CancellationToken cancellationToken)
    {
        if (addedBytes <= 0)
        {
            return;
        }

        var storage = await _userStorageRepository.GetByUserIdAsync(userId, cancellationToken);
        if (storage is null)
        {
            storage = new UserStorage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UsedBytes = addedBytes,
                TotalBytes = _storageSettings.DefaultTotalBytes,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _userStorageRepository.AddAsync(storage, cancellationToken);
            return;
        }

        storage.UsedBytes += addedBytes;
        storage.UpdatedAt = now;
        _userStorageRepository.Update(storage);
    }
}
