using System;

namespace propertyManagement.DTOs;

/// <summary>
/// DTO for updating property site visit preferences.
/// </summary>
public class UpdatePropertyVisitPreferencesDto
{
    public string? VisitPreferences { get; set; }
    public string? SpecificVisitDays { get; set; }
    public TimeSpan? VisitStartTime { get; set; }
    public TimeSpan? VisitEndTime { get; set; }
}
