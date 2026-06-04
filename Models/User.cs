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

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
