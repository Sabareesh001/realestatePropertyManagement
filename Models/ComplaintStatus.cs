using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ComplaintStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
