using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tijori.Domain.Entities;

namespace Tijori.Infrastructure.Data.Configurations;

public class HomeEntityConfigurations
{
    public class UserStorageConfiguration : IEntityTypeConfiguration<UserStorage>
    {
        public void Configure(EntityTypeBuilder<UserStorage> builder)
        {
            builder.ToTable("UserStorages");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithOne(x => x.Storage)
                .HasForeignKey<UserStorage>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BucketCategoryConfiguration : IEntityTypeConfiguration<BucketCategory>
    {
        public void Configure(EntityTypeBuilder<BucketCategory> builder)
        {
            builder.ToTable("BucketCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IconKey)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("[IsDraft] = 0 AND [ParentCategoryId] IS NULL");

            builder.HasIndex(x => new { x.Name, x.CreatedByUserId, x.ParentCategoryId })
                .IsUnique()
                .HasFilter("[IsDraft] = 0 AND [ParentCategoryId] IS NOT NULL");

            builder.HasOne(x => x.ParentCategory)
                .WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class UserBucketConfiguration : IEntityTypeConfiguration<UserBucket>
    {
        public void Configure(EntityTypeBuilder<UserBucket> builder)
        {
            builder.ToTable("UserBuckets");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.BucketCategoryId })
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Buckets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BucketCategory)
                .WithMany(x => x.UserBuckets)
                .HasForeignKey(x => x.BucketCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.WarrantySubCategoryKey)
                .HasMaxLength(50);

            builder.Property(x => x.Remarks)
                .HasMaxLength(2000);

            builder.HasOne(x => x.ContractDetail)
                .WithOne(x => x.Project)
                .HasForeignKey<ProjectContractDetail>(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.WarrantyDetail)
                .WithOne(x => x.Project)
                .HasForeignKey<ProjectWarrantyDetail>(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TripDetail)
                .WithOne(x => x.Project)
                .HasForeignKey<ProjectTripDetail>(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AppointmentDetail)
                .WithOne(x => x.Project)
                .HasForeignKey<ProjectAppointmentDetail>(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TripItems)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.AppointmentRecords)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.MedicineRecords)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CustomFieldValues)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ContractPayments)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ContractPhases)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.WarrantyCoverages)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BucketCategory)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.BucketCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.ToTable("Milestones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Milestones)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Milestones)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PaymentAlertConfiguration : IEntityTypeConfiguration<PaymentAlert>
    {
        public void Configure(EntityTypeBuilder<PaymentAlert> builder)
        {
            builder.ToTable("PaymentAlerts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.HasOne(x => x.User)
                .WithMany(x => x.PaymentAlerts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.PaymentAlerts)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class ProjectContractDetailConfiguration : IEntityTypeConfiguration<ProjectContractDetail>
    {
        public void Configure(EntityTypeBuilder<ProjectContractDetail> builder)
        {
            builder.ToTable("ProjectContractDetails");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectId).IsUnique();
            builder.Property(x => x.ContractName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            builder.Property(x => x.ContractAmount).HasPrecision(18, 2);
        }
    }

    public class ProjectWarrantyDetailConfiguration : IEntityTypeConfiguration<ProjectWarrantyDetail>
    {
        public void Configure(EntityTypeBuilder<ProjectWarrantyDetail> builder)
        {
            builder.ToTable("ProjectWarrantyDetails");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectId).IsUnique();
            builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Price).HasPrecision(18, 2);
        }
    }

    public class ProjectContractPaymentConfiguration : IEntityTypeConfiguration<ProjectContractPayment>
    {
        public void Configure(EntityTypeBuilder<ProjectContractPayment> builder)
        {
            builder.ToTable("ProjectContractPayments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
        }
    }

    public class ProjectContractPhaseConfiguration : IEntityTypeConfiguration<ProjectContractPhase>
    {
        public void Configure(EntityTypeBuilder<ProjectContractPhase> builder)
        {
            builder.ToTable("ProjectContractPhases");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        }
    }

    public class ProjectWarrantyCoverageConfiguration : IEntityTypeConfiguration<ProjectWarrantyCoverage>
    {
        public void Configure(EntityTypeBuilder<ProjectWarrantyCoverage> builder)
        {
            builder.ToTable("ProjectWarrantyCoverages");
            builder.HasKey(x => x.Id);
        }
    }

    public class ProjectDocumentConfiguration : IEntityTypeConfiguration<ProjectDocument>
    {
        public void Configure(EntityTypeBuilder<ProjectDocument> builder)
        {
            builder.ToTable("ProjectDocuments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DocumentType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.StoredFileName).HasMaxLength(255);
            builder.Property(x => x.ContentType).HasMaxLength(100);
        }
    }

    public class ProjectTripDetailConfiguration : IEntityTypeConfiguration<ProjectTripDetail>
    {
        public void Configure(EntityTypeBuilder<ProjectTripDetail> builder)
        {
            builder.ToTable("ProjectTripDetails");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectId).IsUnique();
            builder.Property(x => x.Destination).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(2000);
        }
    }

    public class ProjectTripItemConfiguration : IEntityTypeConfiguration<ProjectTripItem>
    {
        public void Configure(EntityTypeBuilder<ProjectTripItem> builder)
        {
            builder.ToTable("ProjectTripItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ItemType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Location).HasMaxLength(300);
            builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.StoredFileName).HasMaxLength(255);
            builder.Property(x => x.ContentType).HasMaxLength(100);
        }
    }

    public class ProjectAppointmentDetailConfiguration : IEntityTypeConfiguration<ProjectAppointmentDetail>
    {
        public void Configure(EntityTypeBuilder<ProjectAppointmentDetail> builder)
        {
            builder.ToTable("ProjectAppointmentDetails");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectId).IsUnique();
            builder.Property(x => x.ProviderName).HasMaxLength(200);
            builder.Property(x => x.FacilityName).HasMaxLength(200);
            builder.Property(x => x.Specialty).HasMaxLength(200);
            builder.Property(x => x.PhoneCountryCode).HasMaxLength(10);
            builder.Property(x => x.Phone).HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(256);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(2000);
        }
    }

    public class ProjectAppointmentRecordConfiguration : IEntityTypeConfiguration<ProjectAppointmentRecord>
    {
        public void Configure(EntityTypeBuilder<ProjectAppointmentRecord> builder)
        {
            builder.ToTable("ProjectAppointmentRecords");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.AppointmentTime).HasMaxLength(20);
            builder.Property(x => x.Status).HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.StoredFileName).HasMaxLength(255);
            builder.Property(x => x.ContentType).HasMaxLength(100);
        }
    }

    public class ProjectMedicineRecordConfiguration : IEntityTypeConfiguration<ProjectMedicineRecord>
    {
        public void Configure(EntityTypeBuilder<ProjectMedicineRecord> builder)
        {
            builder.ToTable("ProjectMedicineRecords");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Label).HasMaxLength(200);
            builder.Property(x => x.Value).HasMaxLength(2000);
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.StoredFileName).HasMaxLength(255);
            builder.Property(x => x.ContentType).HasMaxLength(100);
        }
    }

    public class CategoryFormFieldConfiguration : IEntityTypeConfiguration<CategoryFormField>
    {
        public void Configure(EntityTypeBuilder<CategoryFormField> builder)
        {
            builder.ToTable("CategoryFormFields");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Label).HasMaxLength(200).IsRequired();
            builder.Property(x => x.FieldKey).HasMaxLength(100).IsRequired();
            builder.Property(x => x.FieldType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.OptionsJson).HasMaxLength(2000);

            builder.HasIndex(x => new { x.BucketCategoryId, x.FieldKey }).IsUnique();

            builder.HasOne(x => x.BucketCategory)
                .WithMany(x => x.FormFields)
                .HasForeignKey(x => x.BucketCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ProjectCustomFieldValueConfiguration : IEntityTypeConfiguration<ProjectCustomFieldValue>
    {
        public void Configure(EntityTypeBuilder<ProjectCustomFieldValue> builder)
        {
            builder.ToTable("ProjectCustomFieldValues");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Value).HasMaxLength(2000);
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.StoredFileName).HasMaxLength(255);
            builder.Property(x => x.ContentType).HasMaxLength(100);

            builder.HasIndex(x => new { x.ProjectId, x.CategoryFormFieldId }).IsUnique();

            builder.HasOne(x => x.CategoryFormField)
                .WithMany(x => x.ProjectValues)
                .HasForeignKey(x => x.CategoryFormFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
