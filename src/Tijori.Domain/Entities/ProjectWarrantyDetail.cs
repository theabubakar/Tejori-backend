using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectWarrantyDetail : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string? BrandName { get; set; }
    public decimal? Price { get; set; }
    public string Currency { get; set; } = "KD";
    public string? SerialNumber { get; set; }
    public string? SellerName { get; set; }
    public string? SellerPhoneCountryCode { get; set; }
    public string? SellerPhone { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? PurchaseLocation { get; set; }
    public string? CountryOfManufacture { get; set; }
    public string? StoreLocationUrl { get; set; }
    public bool ExpiryReminderEnabled { get; set; }
    public string? ExpiryReminderTiming { get; set; }

    public Project Project { get; set; } = null!;
}
