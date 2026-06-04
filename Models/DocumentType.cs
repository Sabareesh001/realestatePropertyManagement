using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class DocumentType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
