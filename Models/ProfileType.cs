using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ProfileType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
