namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for adding a comment to a complaint thread.
/// </summary>
public class AddCommentDto
{
    /// <summary>Gets or sets the comment message (1–2000 chars).</summary>
    public string Message { get; set; } = null!;
}
