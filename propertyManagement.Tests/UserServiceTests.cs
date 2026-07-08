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

/// <summary>
/// Unit tests for the UserService class.
/// </summary>
[TestFixture]
public class UserServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRoleRepository> _mockRoleRepository;
    private UserService _userService;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.Roles).Returns(_mockRoleRepository.Object);

        _userService = new UserService(_mockUnitOfWork.Object);
    }

    /// <summary>
    /// Verifies that RegisterAsync throws InvalidOperationException when the email already exists.
    /// </summary>
    [Test]
    public async Task RegisterAsync_EmailAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "password",
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            DateOfBirth = new DateOnly(1990, 1, 1),
            RoleId = 1
        };

        var existingUser = new User { Email = registerDto.Email };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.RegisterAsync(registerDto));
        Assert.That(ex.Message, Is.EqualTo("Email already exists"));

        _mockUserRepository.Verify(r => r.GetByEmailAsync(registerDto.Email), Times.Once);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that RegisterAsync assigns both Tenant and Owner roles when registering as Owner.
    /// </summary>
    [Test]
    public async Task RegisterAsync_AsOwner_AssignsTenantAndOwnerRoles()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "newuser@example.com",
            Password = "SecurePassword123",
            FirstName = "Jane",
            LastName = "Doe",
            Phone = "9876543210",
            DateOfBirth = new DateOnly(1992, 2, 2),
            RoleId = Role.Owner
        };

        var tenantRole = new Role { Id = Role.Tenant, Name = "Tenant" };
        var ownerRole = new Role { Id = Role.Owner, Name = "Owner" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User?)null);
        _mockRoleRepository.Setup(r => r.GetByIdAsync(Role.Tenant)).ReturnsAsync(tenantRole);
        _mockRoleRepository.Setup(r => r.GetByIdAsync(Role.Owner)).ReturnsAsync(ownerRole);

        User? savedUser = null;
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => { savedUser = u; u.Id = Guid.NewGuid(); })
            .ReturnsAsync((User u) => u);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.RegisterAsync(registerDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(registerDto.Email));
        Assert.That(result.FirstName, Is.EqualTo(registerDto.FirstName));
        Assert.That(result.LastName, Is.EqualTo(registerDto.LastName));
        Assert.That(result.Phone, Is.EqualTo(registerDto.Phone));
        Assert.That(result.DateOfBirth, Is.EqualTo(registerDto.DateOfBirth));
        Assert.That(result.Roles, Has.Count.EqualTo(2));
        Assert.That(result.Roles.Any(r => r.Id == Role.Tenant), Is.True);
        Assert.That(result.Roles.Any(r => r.Id == Role.Owner), Is.True);

        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.VerificationStatusId, Is.EqualTo(UserVerificationStatus.Unverified));
        Assert.That(savedUser.ActiveStatusId, Is.EqualTo(UserActiveStatus.Active));
        Assert.That(BCrypt.Net.BCrypt.Verify(registerDto.Password, savedUser.PasswordHash), Is.True);

        _mockUserRepository.Verify(r => r.GetByEmailAsync(registerDto.Email), Times.Once);
        _mockRoleRepository.Verify(r => r.GetByIdAsync(Role.Tenant), Times.Once);
        _mockRoleRepository.Verify(r => r.GetByIdAsync(Role.Owner), Times.Once);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that RegisterAsync assigns only the Tenant role when registering as Tenant.
    /// </summary>
    [Test]
    public async Task RegisterAsync_AsTenant_AssignsOnlyTenantRole()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "tenant@example.com",
            Password = "SecurePassword123",
            FirstName = "John",
            LastName = "Smith",
            Phone = "1234567890",
            DateOfBirth = new DateOnly(1990, 5, 15),
            RoleId = Role.Tenant
        };

        var tenantRole = new Role { Id = Role.Tenant, Name = "Tenant" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User?)null);
        _mockRoleRepository.Setup(r => r.GetByIdAsync(Role.Tenant)).ReturnsAsync(tenantRole);

        User? savedUser = null;
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => { savedUser = u; u.Id = Guid.NewGuid(); })
            .ReturnsAsync((User u) => u);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.RegisterAsync(registerDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Roles, Has.Count.EqualTo(1));
        Assert.That(result.Roles.First().Id, Is.EqualTo(Role.Tenant));

        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser.UserRoles, Has.Count.EqualTo(1));

        _mockRoleRepository.Verify(r => r.GetByIdAsync(Role.Tenant), Times.Once);
        _mockRoleRepository.Verify(r => r.GetByIdAsync(Role.Owner), Times.Never);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that LoginAsync throws InvalidOperationException when the user does not exist.
    /// </summary>
    [Test]
    public async Task LoginAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "password" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.LoginAsync(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Invalid email or password"));

        _mockUserRepository.Verify(r => r.GetByEmailAsync(loginDto.Email), Times.Once);
    }

    /// <summary>
    /// Verifies that LoginAsync throws InvalidOperationException when password verification fails.
    /// </summary>
    [Test]
    public async Task LoginAsync_IncorrectPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "user@example.com", Password = "wrongpassword" };
        var user = new User
        {
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.LoginAsync(loginDto));
        Assert.That(ex.Message, Is.EqualTo("Invalid email or password"));

        _mockUserRepository.Verify(r => r.GetByEmailAsync(loginDto.Email), Times.Once);
    }

    /// <summary>
    /// Verifies that LoginAsync successfully authenticates a user with correct credentials.
    /// </summary>
    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsUserResponseDto()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "user@example.com", Password = "correctpassword" };
        var role = new Role { Id = 1, Name = "Admin" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            FirstName = "Alice",
            LastName = "Smith",
            Phone = "555-5555",
            DateOfBirth = new DateOnly(1985, 5, 5),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        user.UserRoles.Add(new UserRole { RoleId = 1, Role = role });

        _mockUserRepository.Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(user.Email));
        Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
        Assert.That(result.LastName, Is.EqualTo(user.LastName));
        Assert.That(result.Role, Is.Not.Null);
        Assert.That(result.Role.Name, Is.EqualTo("Admin"));

        _mockUserRepository.Verify(r => r.GetByEmailAsync(loginDto.Email), Times.Once);
    }

    /// <summary>
    /// Verifies that GetAllUsersAsync returns all users mapped correctly.
    /// </summary>
    [Test]
    public async Task GetAllUsersAsync_ReturnsAllUsersMapped()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = "user1@example.com", FirstName = "User1", LastName = "One", UserRoles = null! },
            new User { Id = Guid.NewGuid(), Email = "user2@example.com", FirstName = "User2", LastName = "Two" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        var list = result.ToList();
        Assert.That(list[0].Email, Is.EqualTo("user1@example.com"));
        Assert.That(list[1].Email, Is.EqualTo("user2@example.com"));

        _mockUserRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that GetUserByIdAsync throws InvalidOperationException when the user is not found.
    /// </summary>
    [Test]
    public async Task GetUserByIdAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.GetUserByIdAsync(userId));
        Assert.That(ex.Message, Is.EqualTo("User not found"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    /// <summary>
    /// Verifies that GetUserByIdAsync returns user details if found.
    /// </summary>
    [Test]
    public async Task GetUserByIdAsync_UserFound_ReturnsUserResponseDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "test@example.com", FirstName = "Jane", LastName = "Doe" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(userId));
        Assert.That(result.Email, Is.EqualTo("test@example.com"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    /// <summary>
    /// Verifies that UpdateUserAsync throws InvalidOperationException when the user is not found.
    /// </summary>
    [Test]
    public async Task UpdateUserAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDto { FirstName = "New" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.UpdateUserAsync(userId, updateDto));
        Assert.That(ex.Message, Is.EqualTo("User not found"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that UpdateUserAsync successfully updates all fields when they are provided.
    /// </summary>
    [Test]
    public async Task UpdateUserAsync_AllFieldsProvided_UpdatesUserAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FirstName = "OldFirst",
            LastName = "OldLast",
            Phone = "000",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Email = "test@example.com"
        };
        var updateDto = new UpdateUserDto
        {
            FirstName = "NewFirst",
            LastName = "NewLast",
            Phone = "111",
            DateOfBirth = new DateOnly(2001, 1, 1)
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(existingUser)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("NewFirst"));
        Assert.That(result.LastName, Is.EqualTo("NewLast"));
        Assert.That(result.Phone, Is.EqualTo("111"));
        Assert.That(result.DateOfBirth, Is.EqualTo(new DateOnly(2001, 1, 1)));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that UpdateUserAsync does not update properties when their corresponding DTO values are null or empty.
    /// </summary>
    [Test]
    public async Task UpdateUserAsync_NullOrEmptyFields_DoesNotChangeExistingValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            FirstName = "OldFirst",
            LastName = "OldLast",
            Phone = "000",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Email = "test@example.com"
        };
        var updateDto = new UpdateUserDto
        {
            FirstName = null,
            LastName = "",
            Phone = null,
            DateOfBirth = null
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(existingUser)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("OldFirst"));
        Assert.That(result.LastName, Is.EqualTo("OldLast"));
        Assert.That(result.Phone, Is.EqualTo("000"));
        Assert.That(result.DateOfBirth, Is.EqualTo(new DateOnly(2000, 1, 1)));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that DeleteUserAsync throws InvalidOperationException when the user is not found.
    /// </summary>
    [Test]
    public async Task DeleteUserAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.DeleteUserAsync(userId));
        Assert.That(ex.Message, Is.EqualTo("User not found"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that DeleteUserAsync successfully deletes the user and saves changes if the user exists.
    /// </summary>
    [Test]
    public async Task DeleteUserAsync_UserExists_DeletesAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that AssignOwnerRoleAsync throws KeyNotFoundException when the user does not exist.
    /// </summary>
    [Test]
    public void AssignOwnerRoleAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _userService.AssignOwnerRoleAsync(userId));
        Assert.That(ex.Message, Is.EqualTo("User not found"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that AssignOwnerRoleAsync throws InvalidOperationException when the user is already an owner.
    /// </summary>
    [Test]
    public void AssignOwnerRoleAsync_UserAlreadyOwner_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        user.UserRoles.Add(new UserRole { RoleId = Role.Owner }); // Already an Owner

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userService.AssignOwnerRoleAsync(userId));
        Assert.That(ex.Message, Is.EqualTo("User is already an owner."));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that AssignOwnerRoleAsync successfully appends the Owner role to the user's role collection.
    /// </summary>
    [Test]
    public async Task AssignOwnerRoleAsync_ValidUser_AppendsOwnerRoleAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "test@example.com", FirstName = "John", LastName = "Doe" };
        var tenantRole = new Role { Id = Role.Tenant, Name = "Tenant" };
        var ownerRole = new Role { Id = Role.Owner, Name = "Owner" };

        user.UserRoles.Add(new UserRole { RoleId = Role.Tenant, Role = tenantRole });

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRoleRepository.Setup(r => r.GetByIdAsync(Role.Owner)).ReturnsAsync(ownerRole);
        _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.AssignOwnerRoleAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(userId));
        
        // Ensure both Tenant and Owner roles are present
        Assert.That(result.Roles, Has.Count.EqualTo(2));
        var rolesList = result.Roles.ToList();
        Assert.That(rolesList[0].Id, Is.EqualTo(Role.Tenant));
        Assert.That(rolesList[0].Name, Is.EqualTo("Tenant"));
        Assert.That(rolesList[1].Id, Is.EqualTo(Role.Owner));
        Assert.That(rolesList[1].Name, Is.EqualTo("Owner"));

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRoleRepository.Verify(r => r.GetByIdAsync(Role.Owner), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
