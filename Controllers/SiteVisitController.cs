using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SiteVisitController : ControllerBase
{
    private readonly ISiteVisitService _siteVisitService;

    public SiteVisitController(ISiteVisitService siteVisitService)
    {
        _siteVisitService = siteVisitService;
    }

    [HttpPost("property/{propertyId}")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> RequestVisit(int propertyId, [FromBody] SiteVisitRequestDto dto)
    {
        var tenantId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _siteVisitService.RequestVisitAsync(tenantId, propertyId, dto);
        return Ok(response);
    }

    [HttpPut("{visitId}/status")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> UpdateStatus(Guid visitId, [FromBody] UpdateSiteVisitStatusDto dto)
    {
        var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _siteVisitService.UpdateVisitStatusAsync(ownerId, visitId, dto);
        return Ok(response);
    }

    [HttpPut("{visitId}/cancel")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> CancelVisit(Guid visitId, [FromBody] CancelSiteVisitDto dto)
    {
        var tenantId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _siteVisitService.CancelVisitAsync(tenantId, visitId, dto.Remarks ?? "Cancelled by tenant");
        return Ok(response);
    }

    [HttpGet("my-requests")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> GetMyRequests()
    {
        var tenantId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var visits = await _siteVisitService.GetTenantVisitsAsync(tenantId);
        return Ok(visits);
    }

    [HttpGet("owner-requests")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetOwnerRequests()
    {
        var ownerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var visits = await _siteVisitService.GetOwnerVisitsAsync(ownerId);
        return Ok(visits);
    }
}
