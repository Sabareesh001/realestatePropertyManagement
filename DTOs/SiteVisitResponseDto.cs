using System;

namespace propertyManagement.DTOs;

public class SiteVisitResponseDto
{
    public Guid Id { get; set; }
    public int PropertyId { get; set; }
    public Guid TenantId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime VisitDate { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public string? Remarks { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public PropertyResponseDto? Property { get; set; }
    public UserResponseDto? Tenant { get; set; }
    public UserResponseDto? Owner { get; set; }
}
