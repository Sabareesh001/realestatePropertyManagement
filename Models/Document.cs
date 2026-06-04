using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int? DocumentTypeId { get; set; }

    public string? DocumentNumber { get; set; }

    public string? DocumentUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual DocumentType? DocumentType { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
