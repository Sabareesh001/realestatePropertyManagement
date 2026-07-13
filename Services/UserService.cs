using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for user-related operations.
/// </summary>
public class UserService : IUserService
{
    private const int EmailVerificationTokenExpiryHours = 24;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly string _frontendUrl;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access.</param>
    /// <param name="emailService">The email service used to send verification emails.</param>
    /// <param name="configuration">The application configuration, used to build verification links.</param>
    public UserService(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:4200";
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerDto">The registration data containing email, password, and user details.</param>
    /// <returns>The created user response if successful.</returns>
    public async Task<UserResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var tenantRole = await _unitOfWork.Roles.GetByIdAsync(Role.Tenant);
        var userRoles = new List<UserRole>
        {
            new UserRole
            {
                RoleId = Role.Tenant,
                Role = tenantRole,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            }
        };

        if (registerDto.RoleId != Role.Tenant)
        {
            var requestedRole = await _unitOfWork.Roles.GetByIdAsync(registerDto.RoleId);
            userRoles.Add(new UserRole
            {
                RoleId = registerDto.RoleId,
                Role = requestedRole,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            });
        }

        var verificationHash = GenerateEmailVerificationHash();

        var user = new User
        {
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Phone = registerDto.Phone,
            DateOfBirth = registerDto.DateOfBirth,
            VerificationStatusId = UserVerificationStatus.Unverified,
            ActiveStatusId = UserActiveStatus.Active,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            UserRoles = userRoles,
            EmailVerified = false,
            EmailVerificationHash = verificationHash,
            EmailVerificationHashExpiresAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(EmailVerificationTokenExpiryHours), DateTimeKind.Unspecified)
        };

        var createdUser = await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(createdUser.Email, createdUser.FirstName, BuildVerificationLink(verificationHash));

        return MapToUserResponseDto(createdUser);
    }

    /// <summary>
    /// Authenticates a user with their email and password.
    /// </summary>
    /// <param name="loginDto">The login credentials containing email and password.</param>
    /// <returns>The authenticated user response if successful.</returns>
    public async Task<UserResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        if (!user.EmailVerified)
        {
            throw new EmailNotVerifiedException();
        }

        return MapToUserResponseDto(user);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A list of all users.</returns>
    public async Task<PagedResultDto<UserResponseDto>> GetAllUsersAsync(PaginationParams pagination)
    {
        var users = await _unitOfWork.Users.GetAllAsync(pagination.PageNumber, pagination.PageSize);
        return users.Select(MapToUserResponseDto);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user details if found.</returns>
    public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return MapToUserResponseDto(user);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="updateUserDto">The updated user information.</param>
    /// <returns>The updated user details.</returns>
    public async Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!string.IsNullOrEmpty(updateUserDto.FirstName))
            user.FirstName = updateUserDto.FirstName;

        if (!string.IsNullOrEmpty(updateUserDto.LastName))
            user.LastName = updateUserDto.LastName;

        if (!string.IsNullOrEmpty(updateUserDto.Phone))
            user.Phone = updateUserDto.Phone;

        if (updateUserDto.DateOfBirth.HasValue)
            user.DateOfBirth = updateUserDto.DateOfBirth.Value;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToUserResponseDto(user);
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        await _unitOfWork.Users.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns the "Owner" role to the specified user without modifying existing roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The updated user response.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when user is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when user is already an owner.</exception>
    public async Task<UserResponseDto> AssignOwnerRoleAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        user.UserRoles ??= new List<UserRole>();

        // Check if user is already an owner
        var isOwner = user.UserRoles.Any(ur => ur.RoleId == Role.Owner);
        if (isOwner)
        {
            throw new InvalidOperationException("User is already an owner.");
        }

        var ownerRole = await _unitOfWork.Roles.GetByIdAsync(Role.Owner);
        if (ownerRole == null)
        {
            throw new InvalidOperationException("Owner role does not exist in the database.");
        }

        user.UserRoles.Add(new UserRole
        {
            RoleId = Role.Owner,
            Role = ownerRole,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        });

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToUserResponseDto(user);
    }

    /// <summary>
    /// Confirms a user's email address using the verification hash sent to them.
    /// </summary>
    /// <param name="hash">The email verification hash.</param>
    /// <exception cref="KeyNotFoundException">Thrown when no user matches the given hash.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the verification hash has expired.</exception>
    public async Task VerifyEmailAsync(string hash)
    {
        var user = await _unitOfWork.Users.GetByEmailVerificationHashAsync(hash);
        if (user == null)
        {
            throw new KeyNotFoundException("Invalid or unknown verification link.");
        }

        if (user.EmailVerificationHashExpiresAt == null || user.EmailVerificationHashExpiresAt < DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified))
        {
            throw new InvalidOperationException("Verification link has expired.");
        }

        user.EmailVerified = true;
        user.EmailVerificationHash = null;
        user.EmailVerificationHashExpiresAt = null;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Regenerates and resends the email verification link for the given email address, if it belongs to an unverified account.
    /// </summary>
    /// <param name="email">The email address to resend the verification link to.</param>
    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        if (user == null || user.EmailVerified)
        {
            return;
        }

        var verificationHash = GenerateEmailVerificationHash();
        user.EmailVerificationHash = verificationHash;
        user.EmailVerificationHashExpiresAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(EmailVerificationTokenExpiryHours), DateTimeKind.Unspecified);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, user.FirstName, BuildVerificationLink(verificationHash));
    }

    /// <summary>
    /// Generates a cryptographically random, URL-safe email verification hash.
    /// </summary>
    private static string GenerateEmailVerificationHash()
    {
        return WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    }

    /// <summary>
    /// Builds the frontend email verification link for the given hash.
    /// </summary>
    private string BuildVerificationLink(string hash)
    {
        return $"{_frontendUrl.TrimEnd('/')}/auth/verify-email/{hash}";
    }

    /// <summary>
    /// Maps a User entity to a UserResponseDto.
    /// </summary>
    /// <param name="user">The user entity to map.</param>
    /// <returns>The mapped UserResponseDto.</returns>
    private static UserResponseDto MapToUserResponseDto(User user)
    {
        var userRole = user.UserRoles?.FirstOrDefault(ur => ur.Role != null);

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Role = userRole != null ? new RoleResponseDto
            {
                Id = userRole.Role!.Id,
                Name = userRole.Role.Name
            } : null,
            Roles = user.UserRoles?.Where(ur => ur.Role != null).Select(ur => new RoleResponseDto
            {
                Id = ur.Role!.Id,
                Name = ur.Role.Name
            }).ToList() ?? new List<RoleResponseDto>(),
            VerificationStatusId = user.VerificationStatusId,
            ActiveStatusId = user.ActiveStatusId,
            EmailVerified = user.EmailVerified
        };
    }
}
