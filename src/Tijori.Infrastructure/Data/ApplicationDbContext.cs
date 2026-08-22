using Microsoft.EntityFrameworkCore;
using Tijori.Domain.Entities;
using Tijori.Infrastructure.Data.Configurations;

namespace Tijori.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
    public DbSet<PasswordResetSession> PasswordResetSessions => Set<PasswordResetSession>();
    public DbSet<UserStorage> UserStorages => Set<UserStorage>();
    public DbSet<BucketCategory> BucketCategories => Set<BucketCategory>();
    public DbSet<UserBucket> UserBuckets => Set<UserBucket>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectContractDetail> ProjectContractDetails => Set<ProjectContractDetail>();
    public DbSet<ProjectWarrantyDetail> ProjectWarrantyDetails => Set<ProjectWarrantyDetail>();
    public DbSet<ProjectContractPayment> ProjectContractPayments => Set<ProjectContractPayment>();
    public DbSet<ProjectContractPhase> ProjectContractPhases => Set<ProjectContractPhase>();
    public DbSet<ProjectWarrantyCoverage> ProjectWarrantyCoverages => Set<ProjectWarrantyCoverage>();
    public DbSet<ProjectDocument> ProjectDocuments => Set<ProjectDocument>();
    public DbSet<ProjectTripDetail> ProjectTripDetails => Set<ProjectTripDetail>();
    public DbSet<ProjectTripItem> ProjectTripItems => Set<ProjectTripItem>();
    public DbSet<ProjectAppointmentDetail> ProjectAppointmentDetails => Set<ProjectAppointmentDetail>();
    public DbSet<ProjectAppointmentRecord> ProjectAppointmentRecords => Set<ProjectAppointmentRecord>();
    public DbSet<ProjectMedicineRecord> ProjectMedicineRecords => Set<ProjectMedicineRecord>();
    public DbSet<CategoryFormField> CategoryFormFields => Set<CategoryFormField>();
    public DbSet<ProjectCustomFieldValue> ProjectCustomFieldValues => Set<ProjectCustomFieldValue>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<PaymentAlert> PaymentAlerts => Set<PaymentAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new OtpVerificationConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetSessionConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.UserStorageConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.BucketCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.UserBucketConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectContractDetailConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectWarrantyDetailConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectContractPaymentConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectContractPhaseConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectWarrantyCoverageConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectTripDetailConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectTripItemConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectAppointmentDetailConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectAppointmentRecordConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectMedicineRecordConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.CategoryFormFieldConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.ProjectCustomFieldValueConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.MilestoneConfiguration());
        modelBuilder.ApplyConfiguration(new HomeEntityConfigurations.PaymentAlertConfiguration());
    }
}
