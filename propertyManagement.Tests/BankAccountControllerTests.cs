using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using propertyManagement.Controllers;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Tests;

[TestFixture]
public class BankAccountControllerTests
{
    private Mock<IBankAccountService> _mockBankAccountService;
    private BankAccountController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockBankAccountService = new Mock<IBankAccountService>();
        _controller = new BankAccountController(_mockBankAccountService.Object);
    }

    private void SetupUserContext(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Test]
    public async Task CreateBankAccount_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var dto = new CreateBankAccountDto
        {
            BankName = "Chase",
            AccountNumber = "123",
            AccountHolderName = "John",
            IfscCode = "C1"
        };

        var responseDto = new BankAccountResponseDto
        {
            Id = Guid.NewGuid(),
            BankName = "Chase",
            AccountNumber = "123",
            AccountHolderName = "John",
            IfscCode = "C1",
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _mockBankAccountService.Setup(s => s.CreateBankAccountAsync(userId, dto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.CreateBankAccount(dto);

        // Assert
        var actionResult = result.Result as CreatedAtActionResult;
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(actionResult.ActionName, Is.EqualTo(nameof(BankAccountController.GetBankAccountById)));
        Assert.That(actionResult.Value, Is.EqualTo(responseDto));
    }

    [Test]
    public async Task GetUserBankAccounts_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var accounts = new List<BankAccountResponseDto>
        {
            new() { Id = Guid.NewGuid(), BankName = "Chase" }
        };
        var pagedResult = new PagedResultDto<BankAccountResponseDto>
        {
            Items = accounts,
            PageNumber = 1,
            PageSize = 20,
            TotalCount = accounts.Count,
            TotalPages = 1
        };

        _mockBankAccountService.Setup(s => s.GetUserBankAccountsAsync(userId, It.IsAny<PaginationParams>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetUserBankAccounts(new PaginationParams());

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(pagedResult));
    }

    [Test]
    public async Task GetBankAccountById_ValidAccess_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        SetupUserContext(userId);

        var account = new BankAccountResponseDto { Id = bankAccountId, BankName = "Chase" };
        _mockBankAccountService.Setup(s => s.GetBankAccountByIdAsync(userId, bankAccountId)).ReturnsAsync(account);

        // Act
        var result = await _controller.GetBankAccountById(bankAccountId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(account));
    }

    [Test]
    public void GetBankAccountById_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        SetupUserContext(userId);

        _mockBankAccountService.Setup(s => s.GetBankAccountByIdAsync(userId, bankAccountId))
            .ThrowsAsync(new UnauthorizedAccessException());

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.GetBankAccountById(bankAccountId));
    }

    [Test]
    public void GetBankAccountById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        SetupUserContext(userId);

        _mockBankAccountService.Setup(s => s.GetBankAccountByIdAsync(userId, bankAccountId))
            .ThrowsAsync(new KeyNotFoundException());

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.GetBankAccountById(bankAccountId));
    }

    [Test]
    public async Task UpdateBankAccount_ValidAccess_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        SetupUserContext(userId);

        var dto = new UpdateBankAccountDto { BankName = "Chase New" };
        var response = new BankAccountResponseDto { Id = bankAccountId, BankName = "Chase New" };

        _mockBankAccountService.Setup(s => s.UpdateBankAccountAsync(userId, bankAccountId, dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.UpdateBankAccount(bankAccountId, dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task DeleteBankAccount_ValidAccess_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        SetupUserContext(userId);

        _mockBankAccountService.Setup(s => s.DeleteBankAccountAsync(userId, bankAccountId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteBankAccount(bankAccountId);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
