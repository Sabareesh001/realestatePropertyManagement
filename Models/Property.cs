using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Property
{
    public int Id { get; set; }

    public Guid OwnerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? StatusId { get; set; }

    public int? CityId { get; set; }

    public string? AddressLine { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? ThumbnailImgUrl { get; set; }

    public decimal MonthlyRent { get; set; }

    public decimal UpfrontPayment { get; set; }

    public decimal SecurityDeposit { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid? VerifiedBy { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual ICollection<LeaseProposal> LeaseProposals { get; set; } = new List<LeaseProposal>();

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual User Owner { get; set; } = null!;

    public virtual PropertyStatus? Status { get; set; }

    public virtual User? VerifiedByNavigation { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
