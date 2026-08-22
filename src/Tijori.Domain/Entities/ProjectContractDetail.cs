using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectContractDetail : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string? CompanyName { get; set; }
    public string? RepresentativeName { get; set; }
    public string? CompanyPhoneCountryCode { get; set; }
    public string? CompanyPhone { get; set; }
    public string? WhatsAppCountryCode { get; set; }
    public string? WhatsApp { get; set; }
    public string? CompanyEmail { get; set; }
    public string ContractName { get; set; } = string.Empty;
    public DateTime? ContractDate { get; set; }
    public decimal? ContractAmount { get; set; }
    public string Currency { get; set; } = "KD";
    public int? NumberOfPayments { get; set; }
    public string? PaymentMethod { get; set; }
    public string? AlertListType { get; set; }

    public Project Project { get; set; } = null!;
}
