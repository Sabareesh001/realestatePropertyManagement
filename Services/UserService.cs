using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for user-related operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access.</param>
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        var role = await _unitOfWork.Roles.GetByIdAsync(registerDto.RoleId);

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
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    RoleId = registerDto.RoleId,
                    Role = role,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                }
            }
        };

        var createdUser = await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();

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

        return MapToUserResponseDto(user);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A list of all users.</returns>
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Select(MapToUserResponseDto).ToList();
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
            ActiveStatusId = user.ActiveStatusId
        };
    }
}
