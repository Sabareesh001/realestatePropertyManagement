using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class OwnerProfile
{
    public Guid UserId { get; set; }

    public int? OwnerTypeId { get; set; }

    public string? OrganizationName { get; set; }

    public string? Gstin { get; set; }

    public virtual OwnerType? OwnerType { get; set; }

    public virtual UserProfile User { get; set; } = null!;
}
