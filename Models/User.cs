using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the verification status identifier of the user.
    /// </summary>
    public int? VerificationStatusId { get; set; }

    /// <summary>
    /// Gets or sets the active status identifier of the user.
    /// </summary>
    public int? ActiveStatusId { get; set; }

    /// <summary>
    /// Gets or sets the verification status associated with this user.
    /// </summary>
    public virtual UserVerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Gets or sets the active status associated with this user.
    /// </summary>
    public virtual UserActiveStatus? ActiveStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Complaint> ComplaintResolvedByNavigations { get; set; } = new List<Complaint>();

    public virtual ICollection<Complaint> ComplaintTenants { get; set; } = new List<Complaint>();

    public virtual ICollection<LeaseProposal> LeaseProposalReviewedByNavigations { get; set; } = new List<LeaseProposal>();

    public virtual ICollection<LeaseProposal> LeaseProposalTenants { get; set; } = new List<LeaseProposal>();

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<Property> PropertiesVerified { get; set; } = new List<Property>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets the collection of verification requests submitted by this user.
    /// </summary>
    public virtual ICollection<UserVerification> UserVerifications { get; set; } = new List<UserVerification>();

    /// <summary>
    /// Gets or sets the collection of verification requests reviewed/performed by this user.
    /// </summary>
    public virtual ICollection<UserVerification> VerificationsPerformed { get; set; } = new List<UserVerification>();

    /// <summary>
    /// Gets or sets the bank accounts associated with this user.
    /// </summary>
    public virtual ICollection<UserBankAccount> UserBankAccounts { get; set; } = new List<UserBankAccount>();
}
