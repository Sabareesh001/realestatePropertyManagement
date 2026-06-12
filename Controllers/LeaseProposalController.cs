using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing lease proposals/renting requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaseProposalController : BaseApiController
{
    private readonly ILeaseProposalService _leaseProposalService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseProposalController"/> class.
    /// </summary>
    /// <param name="leaseProposalService">The lease proposal service.</param>
    public LeaseProposalController(ILeaseProposalService leaseProposalService)
    {
        _leaseProposalService = leaseProposalService;
    }

    /// <summary>
    /// Submits a new lease proposal to rent a property. Requires the user to be verified.
    /// </summary>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>The created lease proposal details.</returns>
    /// <response code="201">Lease proposal successfully submitted.</response>
    /// <response code="400">If user is not verified or request is invalid.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LeaseProposalResponseDto>> CreateLeaseProposal([FromBody] CreateLeaseProposalDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _leaseProposalService.CreateLeaseProposalAsync(userId, dto);
        return CreatedAtAction(null, result);
    }

}
