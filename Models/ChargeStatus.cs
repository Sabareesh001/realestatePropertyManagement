using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ChargeStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();
}
