using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class UserProfile
{
    public Guid UserId { get; set; }

    public int ProfileTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OwnerProfile? OwnerProfile { get; set; }

    public virtual ProfileType ProfileType { get; set; } = null!;

    public virtual TenantProfile? TenantProfile { get; set; }

    public virtual User User { get; set; } = null!;
}
