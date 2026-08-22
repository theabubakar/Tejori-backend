namespace Tijori.Application.Common;

public class BucketCategoryOptionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string IconKey { get; init; } = string.Empty;
    public bool IsCustom { get; init; }
}

public class WarrantySubCategoryOptionDto
{
    public string Key { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class BucketSetupDto
{
    public IReadOnlyList<BucketCategoryOptionDto> Categories { get; init; } = Array.Empty<BucketCategoryOptionDto>();
    public Guid? CustomGroupCategoryId { get; init; }
    public IReadOnlyList<BucketCategoryOptionDto> CustomCategories { get; init; } = Array.Empty<BucketCategoryOptionDto>();
    public IReadOnlyList<WarrantySubCategoryOptionDto> WarrantySubCategories { get; init; } =
        Array.Empty<WarrantySubCategoryOptionDto>();
}

public class AddCustomCategoryRequest
{
    public string Name { get; init; } = string.Empty;
}

public class UploadedBucketFileDto
{
    public string FileToken { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
}

public class CreateBucketRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public bool ScanWithAiOcr { get; init; }
    public string? WarrantySubCategoryKey { get; init; }
    public string? Remarks { get; init; }
    public CreateBucketContractDetailDto? Contract { get; init; }
    public CreateBucketWarrantyDetailDto? Warranty { get; init; }
    public CreateBucketTripDetailDto? Trip { get; init; }
    public CreateBucketAppointmentDetailDto? Appointment { get; init; }
    public IReadOnlyList<CreateBucketMedicineRecordDto> Medicine { get; init; } = Array.Empty<CreateBucketMedicineRecordDto>();
    public IReadOnlyList<CreateBucketCustomFieldValueDto> CustomFieldValues { get; init; } = Array.Empty<CreateBucketCustomFieldValueDto>();
    public IReadOnlyList<CreateBucketDocumentDto> Documents { get; init; } = Array.Empty<CreateBucketDocumentDto>();
}

public class CreateBucketContractDetailDto
{
    public string? CompanyName { get; init; }
    public string? RepresentativeName { get; init; }
    public string? CompanyPhoneCountryCode { get; init; }
    public string? CompanyPhone { get; init; }
    public string? WhatsAppCountryCode { get; init; }
    public string? WhatsApp { get; init; }
    public string? CompanyEmail { get; init; }
    public string ContractName { get; init; } = string.Empty;
    public DateTime? ContractDate { get; init; }
    public decimal? ContractAmount { get; init; }
    public string Currency { get; init; } = "KD";
    public int? NumberOfPayments { get; init; }
    public string? PaymentMethod { get; init; }
    public string? AlertListType { get; init; }
    public IReadOnlyList<CreateBucketContractPaymentDto> Payments { get; init; } = Array.Empty<CreateBucketContractPaymentDto>();
    public IReadOnlyList<CreateBucketContractPhaseDto> Phases { get; init; } = Array.Empty<CreateBucketContractPhaseDto>();
}

public class CreateBucketContractPaymentDto
{
    public decimal Amount { get; init; }
    public DateTime? DueDate { get; init; }
    public string? NotificationTiming { get; init; }
}

public class CreateBucketContractPhaseDto
{
    public string Title { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
    public string? NotificationTiming { get; init; }
    public int? ProgressPercentage { get; init; }
}

public class CreateBucketWarrantyDetailDto
{
    public string? BrandName { get; init; }
    public decimal? Price { get; init; }
    public string Currency { get; init; } = "KD";
    public string? SerialNumber { get; init; }
    public string? SellerName { get; init; }
    public string? SellerPhoneCountryCode { get; init; }
    public string? SellerPhone { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? PurchaseLocation { get; init; }
    public string? CountryOfManufacture { get; init; }
    public string? StoreLocationUrl { get; init; }
    public bool ExpiryReminderEnabled { get; init; }
    public string? ExpiryReminderTiming { get; init; }
    public IReadOnlyList<CreateBucketWarrantyCoverageDto> Coverages { get; init; } = Array.Empty<CreateBucketWarrantyCoverageDto>();
}

public class CreateBucketWarrantyCoverageDto
{
    public string? CoverageArea { get; init; }
    public string? CoverageOption { get; init; }
}

public class CreateBucketTripDetailDto
{
    public string? Destination { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<CreateBucketTripItemDto> Items { get; init; } = Array.Empty<CreateBucketTripItemDto>();
}

public class CreateBucketTripItemDto
{
    public string ItemType { get; init; } = string.Empty;
    public string? Title { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Location { get; init; }
    public string? ReferenceNumber { get; init; }
    public string? Notes { get; init; }
    public string? FileToken { get; init; }
}

public class CreateBucketAppointmentDetailDto
{
    public string? ProviderName { get; init; }
    public string? FacilityName { get; init; }
    public string? Specialty { get; init; }
    public string? PhoneCountryCode { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<CreateBucketAppointmentRecordDto> Records { get; init; } = Array.Empty<CreateBucketAppointmentRecordDto>();
}

public class CreateBucketAppointmentRecordDto
{
    public string? Title { get; init; }
    public DateTime? AppointmentDate { get; init; }
    public string? AppointmentTime { get; init; }
    public string? Status { get; init; }
    public string? Notes { get; init; }
    public string? FileToken { get; init; }
}

public class CreateBucketMedicineRecordDto
{
    public string Section { get; init; } = string.Empty;
    public string? Label { get; init; }
    public string? Value { get; init; }
    public string? FileToken { get; init; }
}

public class CreateBucketCustomFieldValueDto
{
    public Guid FieldId { get; init; }
    public string? Value { get; init; }
    public string? FileToken { get; init; }
}

public class CreateBucketDocumentDto
{
    public string DocumentType { get; init; } = string.Empty;
    public string? FileToken { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime? ExtensionDate { get; init; }
}

public class CategoryFormFieldDto
{
    public Guid Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string FieldKey { get; init; } = string.Empty;
    public string FieldType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
    public string? OptionsJson { get; init; }
}

public class AddCategoryFormFieldRequest
{
    public string Label { get; init; } = string.Empty;
    public string FieldKey { get; init; } = string.Empty;
    public string FieldType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public string? OptionsJson { get; init; }
}

public class CreateBucketResultDto
{
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIconKey { get; init; } = string.Empty;
    public string FlowType { get; init; } = string.Empty;
    public string SuccessTitle { get; init; } = string.Empty;
    public string SuccessMessage { get; init; } = string.Empty;
}
