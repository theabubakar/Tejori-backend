using Tijori.Domain.Common;

namespace Tijori.Domain.Entities;

public class ProjectAppointmentDetail : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string? ProviderName { get; set; }
    public string? FacilityName { get; set; }
    public string? Specialty { get; set; }
    public string? PhoneCountryCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public Project Project { get; set; } = null!;
}
