using System;

namespace propertyManagement.DTOs;

public class UpdateSiteVisitStatusDto
{
    public int StatusId { get; set; } // 2 for Approved, 3 for Cancelled
    public string? Remarks { get; set; }
}
