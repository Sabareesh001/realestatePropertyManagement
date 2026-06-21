using System;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Models;

namespace propertyManagement.Data;

/// <summary>
/// Partial class for <see cref="PropertyManagementDbContext"/> to isolate seeding logic.
/// </summary>
public partial class PropertyManagementDbContext
{
    /// <summary>
    /// Configures the seed data for country, state, districts, and cities.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        var seedTime = new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Unspecified);

        // Seed Country
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "India", CreatedAt = seedTime }
        );

        // Seed State
        modelBuilder.Entity<State>().HasData(
            new State { Id = 1, CountryId = 1, Name = "Tamil Nadu", CreatedAt = seedTime }
        );

        // Seed Districts
        modelBuilder.Entity<District>().HasData(
            new District { Id = 1, StateId = 1, Name = "Ariyalur", CreatedAt = seedTime },
            new District { Id = 2, StateId = 1, Name = "Chengalpattu", CreatedAt = seedTime },
            new District { Id = 3, StateId = 1, Name = "Chennai", CreatedAt = seedTime },
            new District { Id = 4, StateId = 1, Name = "Coimbatore", CreatedAt = seedTime },
            new District { Id = 5, StateId = 1, Name = "Cuddalore", CreatedAt = seedTime },
            new District { Id = 6, StateId = 1, Name = "Dharmapuri", CreatedAt = seedTime },
            new District { Id = 7, StateId = 1, Name = "Dindigul", CreatedAt = seedTime },
            new District { Id = 8, StateId = 1, Name = "Erode", CreatedAt = seedTime },
            new District { Id = 9, StateId = 1, Name = "Kallakurichi", CreatedAt = seedTime },
            new District { Id = 10, StateId = 1, Name = "Kanchipuram", CreatedAt = seedTime },
            new District { Id = 11, StateId = 1, Name = "Kanyakumari", CreatedAt = seedTime },
            new District { Id = 12, StateId = 1, Name = "Karur", CreatedAt = seedTime },
            new District { Id = 13, StateId = 1, Name = "Krishnagiri", CreatedAt = seedTime },
            new District { Id = 14, StateId = 1, Name = "Madurai", CreatedAt = seedTime },
            new District { Id = 15, StateId = 1, Name = "Mayiladuthurai", CreatedAt = seedTime },
            new District { Id = 16, StateId = 1, Name = "Nagapattinam", CreatedAt = seedTime },
            new District { Id = 17, StateId = 1, Name = "Namakkal", CreatedAt = seedTime },
            new District { Id = 18, StateId = 1, Name = "Nilgiris", CreatedAt = seedTime },
            new District { Id = 19, StateId = 1, Name = "Perambalur", CreatedAt = seedTime },
            new District { Id = 20, StateId = 1, Name = "Pudukkottai", CreatedAt = seedTime },
            new District { Id = 21, StateId = 1, Name = "Ramanathapuram", CreatedAt = seedTime },
            new District { Id = 22, StateId = 1, Name = "Ranipet", CreatedAt = seedTime },
            new District { Id = 23, StateId = 1, Name = "Salem", CreatedAt = seedTime },
            new District { Id = 24, StateId = 1, Name = "Sivaganga", CreatedAt = seedTime },
            new District { Id = 25, StateId = 1, Name = "Tenkasi", CreatedAt = seedTime },
            new District { Id = 26, StateId = 1, Name = "Thanjavur", CreatedAt = seedTime },
            new District { Id = 27, StateId = 1, Name = "Theni", CreatedAt = seedTime },
            new District { Id = 28, StateId = 1, Name = "Thoothukudi", CreatedAt = seedTime },
            new District { Id = 29, StateId = 1, Name = "Tiruchirappalli", CreatedAt = seedTime },
            new District { Id = 30, StateId = 1, Name = "Tirunelveli", CreatedAt = seedTime },
            new District { Id = 31, StateId = 1, Name = "Tirupattur", CreatedAt = seedTime },
            new District { Id = 32, StateId = 1, Name = "Tiruppur", CreatedAt = seedTime },
            new District { Id = 33, StateId = 1, Name = "Tiruvallur", CreatedAt = seedTime },
            new District { Id = 34, StateId = 1, Name = "Tiruvannamalai", CreatedAt = seedTime },
            new District { Id = 35, StateId = 1, Name = "Tiruvarur", CreatedAt = seedTime },
            new District { Id = 36, StateId = 1, Name = "Vellore", CreatedAt = seedTime },
            new District { Id = 37, StateId = 1, Name = "Viluppuram", CreatedAt = seedTime },
            new District { Id = 38, StateId = 1, Name = "Virudhunagar", CreatedAt = seedTime }
        );

        // Seed Cities (Capitals of respective districts)
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, DistrictId = 1, Name = "Ariyalur", CreatedAt = seedTime },
            new City { Id = 2, DistrictId = 2, Name = "Chengalpattu", CreatedAt = seedTime },
            new City { Id = 3, DistrictId = 3, Name = "Chennai", CreatedAt = seedTime },
            new City { Id = 4, DistrictId = 4, Name = "Coimbatore", CreatedAt = seedTime },
            new City { Id = 5, DistrictId = 5, Name = "Cuddalore", CreatedAt = seedTime },
            new City { Id = 6, DistrictId = 6, Name = "Dharmapuri", CreatedAt = seedTime },
            new City { Id = 7, DistrictId = 7, Name = "Dindigul", CreatedAt = seedTime },
            new City { Id = 8, DistrictId = 8, Name = "Erode", CreatedAt = seedTime },
            new City { Id = 9, DistrictId = 9, Name = "Kallakurichi", CreatedAt = seedTime },
            new City { Id = 10, DistrictId = 10, Name = "Kanchipuram", CreatedAt = seedTime },
            new City { Id = 11, DistrictId = 11, Name = "Nagercoil", CreatedAt = seedTime },
            new City { Id = 12, DistrictId = 12, Name = "Karur", CreatedAt = seedTime },
            new City { Id = 13, DistrictId = 13, Name = "Krishnagiri", CreatedAt = seedTime },
            new City { Id = 14, DistrictId = 14, Name = "Madurai", CreatedAt = seedTime },
            new City { Id = 15, DistrictId = 15, Name = "Mayiladuthurai", CreatedAt = seedTime },
            new City { Id = 16, DistrictId = 16, Name = "Nagapattinam", CreatedAt = seedTime },
            new City { Id = 17, DistrictId = 17, Name = "Namakkal", CreatedAt = seedTime },
            new City { Id = 18, DistrictId = 18, Name = "Udhagamandalam", CreatedAt = seedTime },
            new City { Id = 19, DistrictId = 19, Name = "Perambalur", CreatedAt = seedTime },
            new City { Id = 20, DistrictId = 20, Name = "Pudukkottai", CreatedAt = seedTime },
            new City { Id = 21, DistrictId = 21, Name = "Ramanathapuram", CreatedAt = seedTime },
            new City { Id = 22, DistrictId = 22, Name = "Ranipet", CreatedAt = seedTime },
            new City { Id = 23, DistrictId = 23, Name = "Salem", CreatedAt = seedTime },
            new City { Id = 24, DistrictId = 24, Name = "Sivaganga", CreatedAt = seedTime },
            new City { Id = 25, DistrictId = 25, Name = "Tenkasi", CreatedAt = seedTime },
            new City { Id = 26, DistrictId = 26, Name = "Thanjavur", CreatedAt = seedTime },
            new City { Id = 27, DistrictId = 27, Name = "Theni", CreatedAt = seedTime },
            new City { Id = 28, DistrictId = 28, Name = "Thoothukudi", CreatedAt = seedTime },
            new City { Id = 29, DistrictId = 29, Name = "Tiruchirappalli", CreatedAt = seedTime },
            new City { Id = 30, DistrictId = 30, Name = "Tirunelveli", CreatedAt = seedTime },
            new City { Id = 31, DistrictId = 31, Name = "Tirupattur", CreatedAt = seedTime },
            new City { Id = 32, DistrictId = 32, Name = "Tiruppur", CreatedAt = seedTime },
            new City { Id = 33, DistrictId = 33, Name = "Tiruvallur", CreatedAt = seedTime },
            new City { Id = 34, DistrictId = 34, Name = "Tiruvannamalai", CreatedAt = seedTime },
            new City { Id = 35, DistrictId = 35, Name = "Tiruvarur", CreatedAt = seedTime },
            new City { Id = 36, DistrictId = 36, Name = "Vellore", CreatedAt = seedTime },
            new City { Id = 37, DistrictId = 37, Name = "Viluppuram", CreatedAt = seedTime },
            new City { Id = 38, DistrictId = 38, Name = "Virudhunagar", CreatedAt = seedTime }
        );

        // Seed PropertyVerificationStatus
        modelBuilder.Entity<PropertyVerificationStatus>().HasData(
            new PropertyVerificationStatus { Id = PropertyVerificationStatus.Draft, Name = "Draft", CreatedAt = seedTime },
            new PropertyVerificationStatus { Id = PropertyVerificationStatus.Submitted, Name = "Submitted", CreatedAt = seedTime },
            new PropertyVerificationStatus { Id = PropertyVerificationStatus.Verified, Name = "Verified", CreatedAt = seedTime },
            new PropertyVerificationStatus { Id = PropertyVerificationStatus.Rejected, Name = "Rejected", CreatedAt = seedTime }
        );

        // Seed PropertyAvailabilityStatus
        modelBuilder.Entity<PropertyAvailabilityStatus>().HasData(
            new PropertyAvailabilityStatus { Id = PropertyAvailabilityStatus.Available, Name = "Available", CreatedAt = seedTime },
            new PropertyAvailabilityStatus { Id = PropertyAvailabilityStatus.Occupied, Name = "Occupied", CreatedAt = seedTime },
            new PropertyAvailabilityStatus { Id = PropertyAvailabilityStatus.Unavailable, Name = "Unavailable", CreatedAt = seedTime }
        );

        // Seed UserVerificationStatus
        modelBuilder.Entity<UserVerificationStatus>().HasData(
            new UserVerificationStatus { Id = UserVerificationStatus.Unverified, Name = "Unverified", CreatedAt = seedTime },
            new UserVerificationStatus { Id = UserVerificationStatus.Pending, Name = "Pending", CreatedAt = seedTime },
            new UserVerificationStatus { Id = UserVerificationStatus.Verified, Name = "Verified", CreatedAt = seedTime },
            new UserVerificationStatus { Id = UserVerificationStatus.Rejected, Name = "Rejected", CreatedAt = seedTime }
        );

        // Seed UserActiveStatus
        modelBuilder.Entity<UserActiveStatus>().HasData(
            new UserActiveStatus { Id = UserActiveStatus.Active, Name = "Active", CreatedAt = seedTime },
            new UserActiveStatus { Id = UserActiveStatus.Inactive, Name = "Inactive", CreatedAt = seedTime },
            new UserActiveStatus { Id = UserActiveStatus.Suspended, Name = "Suspended", CreatedAt = seedTime }
        );

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Role.Tenant, Name = "Tenant", CreatedAt = seedTime },
            new Role { Id = Role.Owner, Name = "Owner", CreatedAt = seedTime },
            new Role { Id = Role.Admin, Name = "Admin", CreatedAt = seedTime }
        );

        // Seed DocumentTypes
        modelBuilder.Entity<DocumentType>().HasData(
            new DocumentType { Id = DocumentType.PanCard, Name = "Pan card", CreatedAt = seedTime },
            new DocumentType { Id = DocumentType.PropertyDeed, Name = "Property Deed", CreatedAt = seedTime },
            new DocumentType { Id = DocumentType.SalarySlip, Name = "Salary Slip", CreatedAt = seedTime },
            new DocumentType { Id = DocumentType.LeaseAgreement, Name = "Lease Agreement", CreatedAt = seedTime },
            new DocumentType { Id = DocumentType.SignedLeaseAgreement, Name = "Signed Lease Agreement", CreatedAt = seedTime }
        );

        // Seed ProposalStatus
        modelBuilder.Entity<ProposalStatus>().HasData(
            new ProposalStatus { Id = ProposalStatus.Draft, Name = "DRAFT", CreatedAt = seedTime },
            new ProposalStatus { Id = ProposalStatus.Submitted, Name = "SUBMITTED", CreatedAt = seedTime },
            new ProposalStatus { Id = ProposalStatus.Approved, Name = "APPROVED", CreatedAt = seedTime },
            new ProposalStatus { Id = ProposalStatus.Rejected, Name = "REJECTED", CreatedAt = seedTime },
            new ProposalStatus { Id = ProposalStatus.Expired, Name = "EXPIRED", CreatedAt = seedTime },
            new ProposalStatus { Id = ProposalStatus.Cancelled, Name = "CANCELLED", CreatedAt = seedTime }
        );

        // Seed LeaseStatus
        modelBuilder.Entity<LeaseStatus>().HasData(
            new LeaseStatus { Id = LeaseStatus.Draft, Name = "DRAFT", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.Submitted, Name = "SUBMITTED", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.PendingSignature, Name = "PENDING_SIGNATURE", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.TenantSigned, Name = "TENANT_SIGNED", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.Active, Name = "ACTIVE", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.Rejected, Name = "REJECTED", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.Terminated, Name = "TERMINATED", CreatedAt = seedTime },
            new LeaseStatus { Id = LeaseStatus.Expired, Name = "EXPIRED", CreatedAt = seedTime }
        );

        // Seed ChargeType
        modelBuilder.Entity<ChargeType>().HasData(
            new ChargeType { Id = ChargeType.MonthlyRent, Name = "Monthly Rent", CreatedAt = seedTime },
            new ChargeType { Id = ChargeType.SecurityDeposit, Name = "Security Deposit", CreatedAt = seedTime },
            new ChargeType { Id = ChargeType.UpfrontPayment, Name = "Upfront Payment", CreatedAt = seedTime },
            new ChargeType { Id = ChargeType.Maintenance, Name = "Maintenance", CreatedAt = seedTime },
            new ChargeType { Id = ChargeType.Penalty, Name = "Penalty", CreatedAt = seedTime },
            new ChargeType { Id = ChargeType.Other, Name = "Other", CreatedAt = seedTime }
        );

        // Seed ChargeStatus
        modelBuilder.Entity<ChargeStatus>().HasData(
            new ChargeStatus { Id = ChargeStatus.Pending, Name = "PENDING", CreatedAt = seedTime },
            new ChargeStatus { Id = ChargeStatus.PartiallyPaid, Name = "PARTIALLY_PAID", CreatedAt = seedTime },
            new ChargeStatus { Id = ChargeStatus.Paid, Name = "PAID", CreatedAt = seedTime },
            new ChargeStatus { Id = ChargeStatus.Overdue, Name = "OVERDUE", CreatedAt = seedTime },
            new ChargeStatus { Id = ChargeStatus.Cancelled, Name = "CANCELLED", CreatedAt = seedTime }
        );

        // Seed PaymentStatus
        modelBuilder.Entity<PaymentStatus>().HasData(
            new PaymentStatus { Id = PaymentStatus.Pending, Name = "PENDING", CreatedAt = seedTime },
            new PaymentStatus { Id = PaymentStatus.Completed, Name = "COMPLETED", CreatedAt = seedTime },
            new PaymentStatus { Id = PaymentStatus.Failed, Name = "FAILED", CreatedAt = seedTime },
            new PaymentStatus { Id = PaymentStatus.Refunded, Name = "REFUNDED", CreatedAt = seedTime }
        );

        // Seed PaymentMethod
        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Name = "Cash", Category = "Offline", CreatedAt = seedTime },
            new PaymentMethod { Id = 2, Name = "Bank Transfer", Category = "Online", CreatedAt = seedTime },
            new PaymentMethod { Id = 3, Name = "UPI", Category = "Online", CreatedAt = seedTime },
            new PaymentMethod { Id = 4, Name = "Credit Card", Category = "Online", CreatedAt = seedTime },
            new PaymentMethod { Id = 5, Name = "Debit Card", Category = "Online", CreatedAt = seedTime },
            new PaymentMethod { Id = 6, Name = "Cheque", Category = "Offline", CreatedAt = seedTime }
        );

        // Seed Currency
        modelBuilder.Entity<Currency>().HasData(
            new Currency { Id = 1, Code = "INR", Name = "Indian Rupee", CreatedAt = seedTime }
        );

    }
}
