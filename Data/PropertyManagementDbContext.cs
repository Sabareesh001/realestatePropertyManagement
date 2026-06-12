using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Models;

namespace propertyManagement.Data;
public partial class PropertyManagementDbContext : DbContext
{
    public PropertyManagementDbContext()
    {
    }

    public PropertyManagementDbContext(DbContextOptions<PropertyManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<ChargePayment> ChargePayments { get; set; }

    public virtual DbSet<ChargeStatus> ChargeStatuses { get; set; }

    public virtual DbSet<ChargeType> ChargeTypes { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<ComplaintPriority> ComplaintPriorities { get; set; }

    public virtual DbSet<ComplaintStatus> ComplaintStatuses { get; set; }

    public virtual DbSet<ComplaintType> ComplaintTypes { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Lease> Leases { get; set; }

    public virtual DbSet<LeaseProposal> LeaseProposals { get; set; }

    public virtual DbSet<LeaseStatus> LeaseStatuses { get; set; }

    public virtual DbSet<OwnerProfile> OwnerProfiles { get; set; }

    public virtual DbSet<OwnerType> OwnerTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<ProfileType> ProfileTypes { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    /// <summary>
    /// Gets or sets the database set for property verification statuses.
    /// </summary>
    public virtual DbSet<PropertyVerificationStatus> PropertyVerificationStatuses { get; set; }

    /// <summary>
    /// Gets or sets the database set for property availability statuses.
    /// </summary>
    public virtual DbSet<PropertyAvailabilityStatus> PropertyAvailabilityStatuses { get; set; }

    /// <summary>
    /// Gets or sets the database set for user verification statuses.
    /// </summary>
    public virtual DbSet<UserVerificationStatus> UserVerificationStatuses { get; set; }

    /// <summary>
    /// Gets or sets the database set for user active statuses.
    /// </summary>
    public virtual DbSet<UserActiveStatus> UserActiveStatuses { get; set; }

    public virtual DbSet<ProposalStatus> ProposalStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<TenantProfile> TenantProfiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    /// <summary>
    /// Gets or sets the database set for user verifications.
    /// </summary>
    public virtual DbSet<UserVerification> UserVerifications { get; set; }

    /// <summary>
    /// Gets or sets the database set for user verification document mappings.
    /// </summary>
    public virtual DbSet<UserVerificationDocument> UserVerificationDocuments { get; set; }

    /// <summary>
    /// Gets or sets the database set for bank accounts.
    /// </summary>
    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    /// <summary>
    /// Gets or sets the database set for user bank accounts.
    /// </summary>
    public virtual DbSet<UserBankAccount> UserBankAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("charges_pkey");

            entity.ToTable("charges");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ChargeTypeId).HasColumnName("charge_type_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Desc).HasColumnName("desc");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.ChargeType).WithMany(p => p.Charges)
                .HasForeignKey(d => d.ChargeTypeId)
                .HasConstraintName("charges_charge_type_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Charges)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("charges_status_id_fkey");

            entity.HasMany(d => d.Leases).WithMany(p => p.Charges)
                .UsingEntity<Dictionary<string, object>>(
                    "LeaseCharge",
                    r => r.HasOne<Lease>().WithMany()
                        .HasForeignKey("LeaseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lease_charges_lease_id_fkey"),
                    l => l.HasOne<Charge>().WithMany()
                        .HasForeignKey("ChargeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lease_charges_charge_id_fkey"),
                    j =>
                    {
                        j.HasKey("ChargeId", "LeaseId").HasName("lease_charges_pkey");
                        j.ToTable("lease_charges");
                        j.IndexerProperty<Guid>("ChargeId").HasColumnName("charge_id");
                        j.IndexerProperty<Guid>("LeaseId").HasColumnName("lease_id");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.ChargesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UserCharge",
                    r => r.HasOne<Lease>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_charges_user_id_fkey"),
                    l => l.HasOne<Charge>().WithMany()
                        .HasForeignKey("ChargeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_charges_charge_id_fkey"),
                    j =>
                    {
                        j.HasKey("ChargeId", "UserId").HasName("user_charges_pkey");
                        j.ToTable("user_charges");
                        j.IndexerProperty<Guid>("ChargeId").HasColumnName("charge_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                    });
        
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ChargePayment>(entity =>
        {
            entity.HasKey(e => new { e.ChargeId, e.PaymentId }).HasName("charge_payments_pkey");

            entity.ToTable("charge_payments");

            entity.Property(e => e.ChargeId).HasColumnName("charge_id");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AmountApplied)
                .HasPrecision(12, 2)
                .HasColumnName("amount_applied");

            entity.HasOne(d => d.Charge).WithMany(p => p.ChargePayments)
                .HasForeignKey(d => d.ChargeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("charge_payments_charge_id_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.ChargePayments)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("charge_payments_payment_id_fkey");
        });

        modelBuilder.Entity<ChargeStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("charge_statuses_pkey");

            entity.ToTable("charge_statuses");

            entity.HasIndex(e => e.Name, "charge_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ChargeType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("charge_types_pkey");

            entity.ToTable("charge_types");

            entity.HasIndex(e => e.Name, "charge_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.District).WithMany(p => p.Cities)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("cities_district_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaints_pkey");

            entity.ToTable("complaints");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComplaintTypeId).HasColumnName("complaint_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PriorityId).HasColumnName("priority_id");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resolved_at");
            entity.Property(e => e.ResolvedBy).HasColumnName("resolved_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.ComplaintType).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.ComplaintTypeId)
                .HasConstraintName("complaints_complaint_type_id_fkey");

            entity.HasOne(d => d.Priority).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("complaints_priority_id_fkey");

            entity.HasOne(d => d.Property).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("complaints_property_id_fkey");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.ComplaintResolvedByNavigations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("complaints_resolved_by_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("complaints_status_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ComplaintTenants)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("complaints_tenant_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ComplaintPriority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_priorities_pkey");

            entity.ToTable("complaint_priorities");

            entity.HasIndex(e => e.Name, "complaint_priorities_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ComplaintStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_statuses_pkey");

            entity.ToTable("complaint_statuses");

            entity.HasIndex(e => e.Name, "complaint_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ComplaintType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_types_pkey");

            entity.ToTable("complaint_types");

            entity.HasIndex(e => e.Name, "complaint_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.HasIndex(e => e.Name, "countries_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("currencies_pkey");

            entity.ToTable("currencies");

            entity.HasIndex(e => e.Code, "currencies_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("districts_pkey");

            entity.ToTable("districts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Districts)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("districts_state_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("documents_pkey");

            entity.ToTable("documents");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(50)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");
            entity.Property(e => e.DocumentUrl).HasColumnName("document_url");

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents)
                .HasForeignKey(d => d.DocumentTypeId)
                .HasConstraintName("documents_document_type_id_fkey");
        
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("document_types_pkey");

            entity.ToTable("document_types");

            entity.HasIndex(e => e.Name, "document_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Lease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leases_pkey");

            entity.ToTable("leases");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MonthlyRent)
                .HasPrecision(12, 2)
                .HasColumnName("monthly_rent");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.ProposalId).HasColumnName("proposal_id");
            entity.Property(e => e.SecurityDeposit)
                .HasPrecision(12, 2)
                .HasColumnName("security_deposit");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpfrontPayment)
                .HasPrecision(12, 2)
                .HasColumnName("upfront_payment");

            entity.HasOne(d => d.PropertyNavigation).WithMany(p => p.Leases)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("leases_property_id_fkey");

            entity.HasOne(d => d.Proposal).WithMany(p => p.Leases)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("leases_proposal_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Leases)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("leases_status_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Leases)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("leases_tenant_id_fkey");

            entity.HasMany(d => d.Documents).WithMany(p => p.Leases)
                .UsingEntity<Dictionary<string, object>>(
                    "LeaseDocument",
                    r => r.HasOne<Document>().WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lease_documents_document_id_fkey"),
                    l => l.HasOne<Lease>().WithMany()
                        .HasForeignKey("LeaseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("lease_documents_lease_id_fkey"),
                    j =>
                    {
                        j.HasKey("LeaseId", "DocumentId").HasName("lease_documents_pkey");
                        j.ToTable("lease_documents");
                        j.IndexerProperty<Guid>("LeaseId").HasColumnName("lease_id");
                        j.IndexerProperty<Guid>("DocumentId").HasColumnName("document_id");
                    });
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<LeaseProposal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lease_proposals_pkey");

            entity.ToTable("lease_proposals");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MonthlyRent)
                .HasPrecision(12, 2)
                .HasColumnName("monthly_rent");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.SecurityDeposit)
                .HasPrecision(12, 2)
                .HasColumnName("security_deposit");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpfrontPayment)
                .HasPrecision(12, 2)
                .HasColumnName("upfront_payment");

            entity.HasOne(d => d.Property).WithMany(p => p.LeaseProposals)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("lease_proposals_property_id_fkey");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.LeaseProposalReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("lease_proposals_reviewed_by_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.LeaseProposals)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("lease_proposals_status_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.LeaseProposalTenants)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("lease_proposals_tenant_id_fkey");
        
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<LeaseStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lease_statuses_pkey");

            entity.ToTable("lease_statuses");

            entity.HasIndex(e => e.Name, "lease_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });


        modelBuilder.Entity<OwnerProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("owner_profiles_pkey");

            entity.ToTable("owner_profiles");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Gstin)
                .HasMaxLength(15)
                .HasColumnName("gstin");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(100)
                .HasColumnName("organization_name");
            entity.Property(e => e.OwnerTypeId).HasColumnName("owner_type_id");

            entity.HasOne(d => d.OwnerType).WithMany(p => p.OwnerProfiles)
                .HasForeignKey(d => d.OwnerTypeId)
                .HasConstraintName("owner_profiles_owner_type_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.OwnerProfile)
                .HasForeignKey<OwnerProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("owner_profiles_user_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<OwnerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("owner_types_pkey");

            entity.ToTable("owner_types");

            entity.HasIndex(e => e.Name, "owner_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.HasIndex(e => e.TransactionRef, "payments_transaction_ref_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyId).HasColumnName("currency_id");
            entity.Property(e => e.PaidAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaidBy).HasColumnName("paid_by");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TransactionRef)
                .HasColumnType("character varying")
                .HasColumnName("transaction_ref");

            entity.HasOne(d => d.Currency).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("payments_currency_id_fkey");

            entity.HasOne(d => d.PaidByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaidBy)
                .HasConstraintName("payments_paid_by_fkey");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("payments_payment_method_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("payments_status_id_fkey");
        
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_methods_pkey");

            entity.ToTable("payment_methods");

            entity.HasIndex(e => e.Name, "payment_methods_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(30)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_statuses_pkey");

            entity.ToTable("payment_statuses");

            entity.HasIndex(e => e.Name, "payment_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ProfileType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("profile_types_pkey");

            entity.ToTable("profile_types");

            entity.HasIndex(e => e.Name, "profile_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("properties_pkey");

            entity.ToTable("properties");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressLine)
                .HasMaxLength(255)
                .HasColumnName("address_line");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(10, 7)
                .HasColumnName("longitude");
            entity.Property(e => e.MonthlyRent)
                .HasPrecision(12, 2)
                .HasColumnName("monthly_rent");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.SecurityDeposit)
                .HasPrecision(12, 2)
                .HasColumnName("security_deposit");
            entity.Property(e => e.VerificationStatusId).HasColumnName("verification_status_id");
            entity.Property(e => e.AvailabilityStatusId).HasColumnName("availability_status_id");
            entity.Property(e => e.ThumbnailImgUrl)
                .HasMaxLength(255)
                .HasColumnName("thumbnail_img_url");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpfrontPayment)
                .HasPrecision(12, 2)
                .HasColumnName("upfront_payment");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");
            entity.Property(e => e.Remarks).HasColumnName("remarks");

            entity.HasOne(d => d.City).WithMany(p => p.Properties)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("properties_city_id_fkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Properties)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("properties_owner_id_fkey");

            entity.HasOne(d => d.VerificationStatus).WithMany(p => p.Properties)
                .HasForeignKey(d => d.VerificationStatusId)
                .HasConstraintName("properties_verification_status_id_fkey");

            entity.HasOne(d => d.AvailabilityStatus).WithMany(p => p.Properties)
                .HasForeignKey(d => d.AvailabilityStatusId)
                .HasConstraintName("properties_availability_status_id_fkey");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.PropertiesVerified)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("properties_verified_by_fkey");

            entity.HasMany(d => d.Documents).WithMany(p => p.Properties)
                .UsingEntity<Dictionary<string, object>>(
                    "PropertyDocument",
                    r => r.HasOne<Document>().WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("property_documents_document_id_fkey"),
                    l => l.HasOne<Property>().WithMany()
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("property_documents_property_id_fkey"),
                    j =>
                    {
                        j.HasKey("PropertyId", "DocumentId").HasName("property_documents_pkey");
                        j.ToTable("property_documents");
                        j.IndexerProperty<int>("PropertyId").HasColumnName("property_id");
                        j.IndexerProperty<Guid>("DocumentId").HasColumnName("document_id");
                    });
        });

        modelBuilder.Entity<PropertyVerificationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("property_verification_statuses_pkey");

            entity.ToTable("property_verification_statuses");

            entity.HasIndex(e => e.Name, "property_verification_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<PropertyAvailabilityStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("property_availability_statuses_pkey");

            entity.ToTable("property_availability_statuses");

            entity.HasIndex(e => e.Name, "property_availability_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<ProposalStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proposal_statuses_pkey");

            entity.ToTable("proposal_statuses");

            entity.HasIndex(e => e.Name, "proposal_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("states_pkey");

            entity.ToTable("states");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("states_country_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<TenantProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("tenant_profiles_pkey");

            entity.ToTable("tenant_profiles");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(100)
                .HasColumnName("emergency_contact_name");
            entity.Property(e => e.EmergencyContactNumber)
                .HasMaxLength(20)
                .HasColumnName("emergency_contact_number");
            entity.Property(e => e.MonthlyIncome)
                .HasPrecision(12, 2)
                .HasColumnName("monthly_income");
            entity.Property(e => e.Occupation)
                .HasMaxLength(100)
                .HasColumnName("occupation");

            entity.HasOne(d => d.User).WithOne(p => p.TenantProfile)
                .HasForeignKey<TenantProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tenant_profiles_user_id_fkey");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Documents).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDocument",
                    r => r.HasOne<Document>().WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_documents_document_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_documents_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "DocumentId").HasName("user_documents_pkey");
                        j.ToTable("user_documents");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("DocumentId").HasColumnName("document_id");
                    });

            entity.Property(e => e.VerificationStatusId)
                .HasColumnName("verification_status_id")
                .HasDefaultValue(1);
            entity.Property(e => e.ActiveStatusId)
                .HasColumnName("active_status_id")
                .HasDefaultValue(1);

            entity.HasOne(d => d.VerificationStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.VerificationStatusId)
                .HasConstraintName("users_verification_status_id_fkey");

            entity.HasOne(d => d.ActiveStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.ActiveStatusId)
                .HasConstraintName("users_active_status_id_fkey");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_profiles_pkey");

            entity.ToTable("user_profiles");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProfileTypeId).HasColumnName("profile_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ProfileType).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.ProfileTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_profiles_profile_type_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_profiles_user_id_fkey");
        
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_roles_pkey");

            entity.ToTable("user_roles");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "user_roles_user_id_role_id_idx").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_roles_role_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_roles_user_id_fkey");
        });

        modelBuilder.Entity<UserVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_verifications_pkey");

            entity.ToTable("user_verifications", t => t.HasCheckConstraint("CK_UserVerification_Status", "status IN ('Pending', 'Verified', 'Rejected')"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status")
                .HasDefaultValue("Pending");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithMany(p => p.UserVerifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_verifications_user_id_fkey");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.VerificationsPerformed)
                .HasForeignKey(d => d.VerifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_verifications_verified_by_fkey");
        
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<UserVerificationDocument>(entity =>
        {
            entity.HasKey(e => new { e.UserVerificationId, e.DocumentId }).HasName("user_verification_documents_pkey");

            entity.ToTable("user_verification_documents");

            entity.Property(e => e.UserVerificationId).HasColumnName("user_verification_id");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");

            entity.HasOne(d => d.UserVerification).WithMany(p => p.UserVerificationDocuments)
                .HasForeignKey(d => d.UserVerificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_verification_documents_user_verification_id_fkey");

            entity.HasOne(d => d.Document).WithMany(p => p.UserVerificationDocuments)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_verification_documents_document_id_fkey");
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bank_accounts_pkey");

            entity.ToTable("bank_accounts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .HasColumnName("account_number");
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(100)
                .HasColumnName("account_holder_name");
            entity.Property(e => e.IfscCode)
                .HasMaxLength(20)
                .HasColumnName("ifsc_code");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<UserBankAccount>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BankAccountId }).HasName("user_bank_accounts_pkey");

            entity.ToTable("user_bank_accounts");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.BankAccountId).HasColumnName("bank_account_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserBankAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_bank_accounts_user_id_fkey");

            entity.HasOne(d => d.BankAccount).WithMany(p => p.UserBankAccounts)
                .HasForeignKey(d => d.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_bank_accounts_bank_account_id_fkey");
        });

        modelBuilder.Entity<UserVerificationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_verification_statuses_pkey");

            entity.ToTable("user_verification_statuses");

            entity.HasIndex(e => e.Name, "user_verification_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<UserActiveStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_active_statuses_pkey");

            entity.ToTable("user_active_statuses");

            entity.HasIndex(e => e.Name, "user_active_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
