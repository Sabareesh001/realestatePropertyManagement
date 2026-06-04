using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Currency
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
