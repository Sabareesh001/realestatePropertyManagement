using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ComplaintType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
