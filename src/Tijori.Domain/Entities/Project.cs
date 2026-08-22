using Tijori.Domain.Common;
using Tijori.Domain.Enums;

namespace Tijori.Domain.Entities;

public class Project : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? BucketCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WarrantySubCategoryKey { get; set; }
    public bool ScanWithAiOcr { get; set; }
    public string? Remarks { get; set; }
    public int DocumentCount { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Ongoing;
    public int SortOrder { get; set; }

    public User User { get; set; } = null!;
    public BucketCategory? BucketCategory { get; set; }
    public ProjectContractDetail? ContractDetail { get; set; }
    public ProjectWarrantyDetail? WarrantyDetail { get; set; }
    public ProjectTripDetail? TripDetail { get; set; }
    public ProjectAppointmentDetail? AppointmentDetail { get; set; }
    public ICollection<ProjectContractPayment> ContractPayments { get; set; } = new List<ProjectContractPayment>();
    public ICollection<ProjectContractPhase> ContractPhases { get; set; } = new List<ProjectContractPhase>();
    public ICollection<ProjectWarrantyCoverage> WarrantyCoverages { get; set; } = new List<ProjectWarrantyCoverage>();
    public ICollection<ProjectTripItem> TripItems { get; set; } = new List<ProjectTripItem>();
    public ICollection<ProjectAppointmentRecord> AppointmentRecords { get; set; } = new List<ProjectAppointmentRecord>();
    public ICollection<ProjectMedicineRecord> MedicineRecords { get; set; } = new List<ProjectMedicineRecord>();
    public ICollection<ProjectCustomFieldValue> CustomFieldValues { get; set; } = new List<ProjectCustomFieldValue>();
    public ICollection<ProjectDocument> Documents { get; set; } = new List<ProjectDocument>();
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public ICollection<PaymentAlert> PaymentAlerts { get; set; } = new List<PaymentAlert>();
}
