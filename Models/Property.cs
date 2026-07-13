using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Property
{
    public int Id { get; set; }

    public Guid OwnerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the verification status identifier of the property.
    /// </summary>
    public int? VerificationStatusId { get; set; }

    /// <summary>
    /// Gets or sets the availability status identifier of the property.
    /// </summary>
    public int? AvailabilityStatusId { get; set; }

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

    /// <summary>
    /// Gets or sets optional verification or rejection remarks from the administrator.
    /// </summary>
    public string? Remarks { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual ICollection<LeaseProposal> LeaseProposals { get; set; } = new List<LeaseProposal>();

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual User Owner { get; set; } = null!;

    /// <summary>
    /// Gets or sets the verification status associated with the property.
    /// </summary>
    public virtual PropertyVerificationStatus? VerificationStatus { get; set; }

    /// <summary>
    /// Gets or sets the availability status associated with the property.
    /// </summary>
    public virtual PropertyAvailabilityStatus? AvailabilityStatus { get; set; }

    public virtual User? VerifiedByNavigation { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets the collection of images associated with this property.
    /// </summary>
    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public string? VisitPreferences { get; set; }

    public string? SpecificVisitDays { get; set; }

    public TimeSpan? VisitStartTime { get; set; }

    public TimeSpan? VisitEndTime { get; set; }

    public virtual ICollection<SiteVisit> SiteVisits { get; set; } = new List<SiteVisit>();
}
