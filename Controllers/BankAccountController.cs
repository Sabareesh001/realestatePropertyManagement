using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing bank accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BankAccountController : BaseApiController
{
    private readonly IBankAccountService _bankAccountService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankAccountController"/> class.
    /// </summary>
    /// <param name="bankAccountService">The bank account service.</param>
    public BankAccountController(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService ?? throw new ArgumentNullException(nameof(bankAccountService));
    }

    /// <summary>
    /// Creates a new bank account associated with the authenticated user.
    /// </summary>
    /// <param name="dto">The details of the bank account to create.</param>
    /// <returns>The created bank account details.</returns>
    /// <response code="201">Returns the newly created bank account.</response>
    /// <response code="400">If the input details are invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BankAccountResponseDto>> CreateBankAccount([FromBody] CreateBankAccountDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _bankAccountService.CreateBankAccountAsync(userId, dto);
        return CreatedAtAction(nameof(GetBankAccountById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves all bank accounts associated with the authenticated user.
    /// </summary>
    /// <returns>A list of bank accounts.</returns>
    /// <response code="200">Returns the list of bank accounts.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankAccountResponseDto>>> GetUserBankAccounts()
    {
        var userId = GetCurrentUserId();
        var result = await _bankAccountService.GetUserBankAccountsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific bank account by its unique identifier if the authenticated user has access.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account.</param>
    /// <returns>The bank account details.</returns>
    /// <response code="200">Returns the bank account.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have permission to access the bank account.</response>
    /// <response code="404">If the bank account is not found.</response>
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BankAccountResponseDto>> GetBankAccountById(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _bankAccountService.GetBankAccountByIdAsync(userId, id);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing bank account details.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account.</param>
    /// <param name="dto">The updated details of the bank account.</param>
    /// <returns>The updated bank account details.</returns>
    /// <response code="200">Returns the updated bank account.</response>
    /// <response code="400">If the input details are invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have permission to update the bank account.</response>
    /// <response code="404">If the bank account is not found.</response>
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BankAccountResponseDto>> UpdateBankAccount(Guid id, [FromBody] UpdateBankAccountDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _bankAccountService.UpdateBankAccountAsync(userId, id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an existing bank account and removes its mapping.
    /// </summary>
    /// <param name="id">The unique identifier of the bank account.</param>
    /// <returns>No Content response.</returns>
    /// <response code="24">If the bank account was deleted successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user does not have permission to delete the bank account.</response>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBankAccount(Guid id)
    {
        var userId = GetCurrentUserId();
        await _bankAccountService.DeleteBankAccountAsync(userId, id);
        return NoContent();
    }
}
