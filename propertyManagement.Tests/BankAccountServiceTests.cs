using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;
using propertyManagement.Services;

namespace propertyManagement.Tests;

[TestFixture]
public class BankAccountServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IBankAccountRepository> _mockBankAccountRepository;
    private BankAccountService _bankAccountService;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.BankAccounts).Returns(_mockBankAccountRepository.Object);

        _bankAccountService = new BankAccountService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task CreateBankAccountAsync_ValidUser_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var dto = new CreateBankAccountDto
        {
            BankName = "Chase",
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0001234"
        };

        // Act
        var result = await _bankAccountService.CreateBankAccountAsync(userId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.BankName, Is.EqualTo("Chase"));
        Assert.That(result.AccountNumber, Is.EqualTo("1234567890"));
        Assert.That(result.AccountHolderName, Is.EqualTo("John Doe"));
        Assert.That(result.IfscCode, Is.EqualTo("CHAS0001234"));

        _mockBankAccountRepository.Verify(r => r.CreateAsync(It.IsAny<BankAccount>()), Times.Once);
        _mockBankAccountRepository.Verify(r => r.AddUserMappingAsync(userId, It.IsAny<Guid>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void CreateBankAccountAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        var dto = new CreateBankAccountDto();

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _bankAccountService.CreateBankAccountAsync(userId, dto));
    }

    [Test]
    public async Task GetUserBankAccountsAsync_ReturnsUserAccounts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccounts = new List<BankAccount>
        {
            new() { Id = Guid.NewGuid(), BankName = "Chase", AccountNumber = "1", AccountHolderName = "John", IfscCode = "C" },
            new() { Id = Guid.NewGuid(), BankName = "Citi", AccountNumber = "2", AccountHolderName = "John", IfscCode = "CI" }
        };

        _mockBankAccountRepository.Setup(r => r.GetBankAccountsByUserIdAsync(userId)).ReturnsAsync(bankAccounts);

        // Act
        var result = (await _bankAccountService.GetUserBankAccountsAsync(userId)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].BankName, Is.EqualTo("Chase"));
        Assert.That(result[1].BankName, Is.EqualTo("Citi"));
    }

    [Test]
    public async Task GetBankAccountByIdAsync_UserIsOwner_ReturnsAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        var bankAccount = new BankAccount { Id = bankAccountId, BankName = "Chase", AccountNumber = "1", AccountHolderName = "John", IfscCode = "C" };

        _mockBankAccountRepository.Setup(r => r.IsOwnerAsync(userId, bankAccountId)).ReturnsAsync(true);
        _mockBankAccountRepository.Setup(r => r.GetByIdAsync(bankAccountId)).ReturnsAsync(bankAccount);

        // Act
        var result = await _bankAccountService.GetBankAccountByIdAsync(userId, bankAccountId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.BankName, Is.EqualTo("Chase"));
    }

    [Test]
    public void GetBankAccountByIdAsync_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();

        _mockBankAccountRepository.Setup(r => r.IsOwnerAsync(userId, bankAccountId)).ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _bankAccountService.GetBankAccountByIdAsync(userId, bankAccountId));
    }

    [Test]
    public void GetBankAccountByIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();

        _mockBankAccountRepository.Setup(r => r.IsOwnerAsync(userId, bankAccountId)).ReturnsAsync(true);
        _mockBankAccountRepository.Setup(r => r.GetByIdAsync(bankAccountId)).ReturnsAsync((BankAccount?)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _bankAccountService.GetBankAccountByIdAsync(userId, bankAccountId));
    }

    [Test]
    public async Task UpdateBankAccountAsync_UserIsOwner_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();
        var bankAccount = new BankAccount { Id = bankAccountId, BankName = "Chase", AccountNumber = "1", AccountHolderName = "John", IfscCode = "C" };

        _mockBankAccountRepository.Setup(r => r.IsOwnerAsync(userId, bankAccountId)).ReturnsAsync(true);
        _mockBankAccountRepository.Setup(r => r.GetByIdAsync(bankAccountId)).ReturnsAsync(bankAccount);

        var dto = new UpdateBankAccountDto
        {
            BankName = "Chase New",
            AccountNumber = "999",
            AccountHolderName = "John New",
            IfscCode = "NEWC123"
        };

        // Act
        var result = await _bankAccountService.UpdateBankAccountAsync(userId, bankAccountId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.BankName, Is.EqualTo("Chase New"));
        Assert.That(result.AccountNumber, Is.EqualTo("999"));
        Assert.That(result.AccountHolderName, Is.EqualTo("John New"));
        Assert.That(result.IfscCode, Is.EqualTo("NEWC123"));

        _mockBankAccountRepository.Verify(r => r.UpdateAsync(bankAccount), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteBankAccountAsync_UserIsOwner_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bankAccountId = Guid.NewGuid();

        _mockBankAccountRepository.Setup(r => r.IsOwnerAsync(userId, bankAccountId)).ReturnsAsync(true);

        // Act
        await _bankAccountService.DeleteBankAccountAsync(userId, bankAccountId);

        // Assert
        _mockBankAccountRepository.Verify(r => r.RemoveUserMappingAsync(userId, bankAccountId), Times.Once);
        _mockBankAccountRepository.Verify(r => r.DeleteAsync(bankAccountId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
