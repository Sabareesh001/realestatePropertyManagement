namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a user role response payload.
/// </summary>
public class RoleResponseDto
{
    /// <summary>
    /// The unique identifier of the role.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the role.
    /// </summary>
    public string Name { get; set; } = null!;
}
