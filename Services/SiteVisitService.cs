using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using propertyManagement.Data;
using propertyManagement.DTOs;
using propertyManagement.Exceptions;
using propertyManagement.Models;

namespace propertyManagement.Services;

public class SiteVisitService : ISiteVisitService
{
    private readonly PropertyManagementDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SiteVisitService> _logger;

    public SiteVisitService(
        PropertyManagementDbContext context,
        INotificationService notificationService,
        ILogger<SiteVisitService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<SiteVisitResponseDto> RequestVisitAsync(Guid tenantId, int propertyId, SiteVisitRequestDto dto)
    {
        var property = await _context.Properties
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.DeletedAt == null);

        if (property == null)
            throw new KeyNotFoundException("Property not found.");

        // Here we could add logic to validate `dto.VisitDate` against property.VisitPreferences, 
        // VisitStartTime, and VisitEndTime.

        var existingVisit = await _context.SiteVisits
            .FirstOrDefaultAsync(v => v.PropertyId == propertyId && v.TenantId == tenantId && v.StatusId != 3 && v.DeletedAt == null);

        if (existingVisit != null)
        {
            throw new InvalidOperationException("You already have an active site visit request for this property.");
        }

        var visit = new SiteVisit
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            TenantId = tenantId,
            OwnerId = property.OwnerId,
            VisitDate = DateTime.SpecifyKind(dto.VisitDate, DateTimeKind.Unspecified),
            StatusId = 1, // Pending
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.SiteVisits.Add(visit);
        await _context.SaveChangesAsync();

        var tenant = await _context.Users.FindAsync(tenantId);

        // Notify Owner
        await _notificationService.NotifyAsync(
            new[] { property.OwnerId },
            1, // notificationTypeId
            "New Site Visit Request",
            $"{tenant?.FirstName} {tenant?.LastName} has requested a site visit for {property.Title} on {dto.VisitDate:g}."
        );

        return await GetVisitByIdAsync(visit.Id);
    }

    public async Task<SiteVisitResponseDto> UpdateVisitStatusAsync(Guid ownerId, Guid visitId, UpdateSiteVisitStatusDto dto)
    {
        var visit = await _context.SiteVisits
            .Include(v => v.Property)
            .Include(v => v.Tenant)
            .FirstOrDefaultAsync(v => v.Id == visitId && v.DeletedAt == null);

        if (visit == null)
            throw new KeyNotFoundException("Site visit not found.");

        if (visit.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You are not authorized to update this site visit.");

        visit.StatusId = dto.StatusId;
        visit.Remarks = dto.Remarks;
        visit.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.SaveChangesAsync();

        string statusName = dto.StatusId == 2 ? "Approved" : "Cancelled";

        // Notify Tenant
        await _notificationService.NotifyAsync(
            new[] { visit.TenantId },
            dto.StatusId == 2 ? 1 : 3, // info or warning
            $"Site Visit {statusName}",
            $"Your site visit request for {visit.Property.Title} on {visit.VisitDate:g} has been {statusName.ToLower()}."
        );

        return await GetVisitByIdAsync(visit.Id);
    }

    public async Task<IEnumerable<SiteVisitResponseDto>> GetTenantVisitsAsync(Guid tenantId)
    {
        var visits = await _context.SiteVisits
            .Include(v => v.Property)
            .Include(v => v.Property.PropertyImages)
            .Include(v => v.Owner)
            .Include(v => v.Status)
            .Where(v => v.TenantId == tenantId && v.DeletedAt == null)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return visits.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<SiteVisitResponseDto>> GetOwnerVisitsAsync(Guid ownerId)
    {
        var visits = await _context.SiteVisits
            .Include(v => v.Property)
            .Include(v => v.Tenant)
            .Include(v => v.Status)
            .Where(v => v.OwnerId == ownerId && v.DeletedAt == null)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return visits.Select(MapToResponseDto);
    }

    private async Task<SiteVisitResponseDto> GetVisitByIdAsync(Guid visitId)
    {
        var visit = await _context.SiteVisits
            .Include(v => v.Property)
            .Include(v => v.Tenant)
            .Include(v => v.Owner)
            .Include(v => v.Status)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        return MapToResponseDto(visit!);
    }

    private SiteVisitResponseDto MapToResponseDto(SiteVisit v)
    {
        return new SiteVisitResponseDto
        {
            Id = v.Id,
            PropertyId = v.PropertyId,
            TenantId = v.TenantId,
            OwnerId = v.OwnerId,
            VisitDate = DateTime.SpecifyKind(v.VisitDate, DateTimeKind.Utc),
            StatusId = v.StatusId,
            StatusName = v.Status?.Name ?? "",
            Remarks = v.Remarks,
            CreatedAt = v.CreatedAt.HasValue ? DateTime.SpecifyKind(v.CreatedAt.Value, DateTimeKind.Utc) : null,
            UpdatedAt = v.UpdatedAt.HasValue ? DateTime.SpecifyKind(v.UpdatedAt.Value, DateTimeKind.Utc) : null,
            Property = v.Property == null ? null : new PropertyResponseDto
            {
                Id = v.Property.Id,
                Title = v.Property.Title!,
                ThumbnailImgUrl = v.Property.ThumbnailImgUrl,
                AddressLine = v.Property.AddressLine!,
                MonthlyRent = v.Property.MonthlyRent,
                VisitPreferences = v.Property.VisitPreferences,
                SpecificVisitDays = v.Property.SpecificVisitDays,
                VisitStartTime = v.Property.VisitStartTime,
                VisitEndTime = v.Property.VisitEndTime
            },
            Tenant = v.Tenant == null ? null : new UserResponseDto
            {
                Id = v.Tenant.Id,
                FirstName = v.Tenant.FirstName,
                LastName = v.Tenant.LastName,
                Email = v.Tenant.Email,
                Phone = v.Tenant.Phone
            },
            Owner = v.Owner == null ? null : new UserResponseDto
            {
                Id = v.Owner.Id,
                FirstName = v.Owner.FirstName,
                LastName = v.Owner.LastName,
                Email = v.Owner.Email,
                Phone = v.Owner.Phone
            }
        };
    }
}
