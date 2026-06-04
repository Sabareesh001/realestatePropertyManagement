using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class TenantProfile
{
    public Guid UserId { get; set; }

    public decimal? MonthlyIncome { get; set; }

    public string? Occupation { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public virtual UserProfile User { get; set; } = null!;
}
