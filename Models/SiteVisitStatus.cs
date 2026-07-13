using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class SiteVisitStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<SiteVisit> SiteVisits { get; set; } = new List<SiteVisit>();
}
