using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

public interface ISiteVisitService
{
    Task<SiteVisitResponseDto> RequestVisitAsync(Guid tenantId, int propertyId, SiteVisitRequestDto dto);
    Task<SiteVisitResponseDto> UpdateVisitStatusAsync(Guid ownerId, Guid visitId, UpdateSiteVisitStatusDto dto);
    Task<SiteVisitResponseDto> CancelVisitAsync(Guid tenantId, Guid visitId, string remarks);
    Task<IEnumerable<SiteVisitResponseDto>> GetTenantVisitsAsync(Guid tenantId);
    Task<IEnumerable<SiteVisitResponseDto>> GetOwnerVisitsAsync(Guid ownerId);
}
