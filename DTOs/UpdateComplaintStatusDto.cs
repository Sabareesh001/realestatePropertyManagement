namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for updating a complaint's status.
/// </summary>
public class UpdateComplaintStatusDto
{
    /// <summary>Gets or sets the new status identifier (1–5).</summary>
    public int StatusId { get; set; }

    /// <summary>Gets or sets an optional note to attach as a comment when transitioning status.</summary>
    public string? Note { get; set; }
}
