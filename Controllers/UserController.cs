using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing user operations including authentication and CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    /// <summary>
    /// Initializes a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">The user service for handling user operations.</param>
    /// <param name="jwtService">The JWT service for generating authentication tokens.</param>
    public UserController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerDto">The registration data containing email, password, and user details.</param>
    /// <returns>The created user response if successful.</returns>
    /// <response code="201">User successfully registered.</response>
    /// <response code="400">Invalid input or email already exists.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        Console.WriteLine($"Registering user with email");
        var userResponseDto = await _userService.RegisterAsync(registerDto);
        return CreatedAtAction(nameof(GetUserById), new { id = userResponseDto.Id }, userResponseDto);
    }

    /// <summary>
    /// Authenticates a user with their email and password.
    /// </summary>
    /// <param name="loginDto">The login credentials containing email and password.</param>
    /// <returns>The authenticated user response if successful.</returns>
    /// <response code="200">User successfully authenticated.</response>
    /// <response code="400">Invalid email or password.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var userResponseDto = await _userService.LoginAsync(loginDto);
        var token = _jwtService.GenerateToken(userResponseDto);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        Console.WriteLine("token generated: " + token);
        Response.Cookies.Append("jwt_token", token, cookieOptions);

        return Ok(userResponseDto);
    }

    /// <summary>
    /// Confirms a user's email address using the verification hash sent to them at registration.
    /// </summary>
    /// <param name="hash">The email verification hash from the verification link.</param>
    /// <returns>A success message and the verification status.</returns>
    /// <response code="200">Email successfully verified.</response>
    /// <response code="400">The verification link has expired.</response>
    /// <response code="404">The verification link is invalid or unknown.</response>
    [HttpGet("verify-email/{hash}")]
    public async Task<ActionResult> VerifyEmail(string hash)
    {
        await _userService.VerifyEmailAsync(hash);
        return Ok(new { message = "Email verified successfully.", emailVerified = true });
    }

    /// <summary>
    /// Resends the email verification link for an unverified account.
    /// </summary>
    /// <param name="resendVerificationDto">The email address to resend the verification link to.</param>
    /// <returns>A generic success message, regardless of whether the email is registered or already verified.</returns>
    /// <response code="200">Request processed.</response>
    [HttpPost("resend-verification")]
    public async Task<ActionResult> ResendVerification([FromBody] ResendVerificationDto resendVerificationDto)
    {
        await _userService.ResendVerificationEmailAsync(resendVerificationDto.Email);
        return Ok(new { message = "If an account with that email exists and is not yet verified, a new verification link has been sent." });
    }

    /// <summary>
    /// Retrieves a page of users from the database.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserResponseDto>>> GetAllUsers([FromQuery] PaginationParams pagination)
    {
        var userDtos = await _userService.GetAllUsersAsync(pagination);
        return Ok(userDtos);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user details if found.</returns>
    /// <response code="200">User retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
    {
        var userResponseDto = await _userService.GetUserByIdAsync(id);
        return Ok(userResponseDto);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="updateUserDto">The updated user information.</param>
    /// <returns>The updated user details.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        var userResponseDto = await _userService.UpdateUserAsync(id, updateUserDto);
        return Ok(userResponseDto);
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>A success message if the user was deleted.</returns>
    /// <response code="200">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Promotes the currently authenticated user to the Owner role.
    /// </summary>
    /// <returns>The updated user details.</returns>
    /// <response code="200">Successfully assigned Owner role.</response>
    /// <response code="400">If the user is already an owner.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [Authorize]
    [HttpPost("become-owner")]
    public async Task<ActionResult<UserResponseDto>> BecomeOwner()
    {
        var userId = GetCurrentUserId();
        var userResponseDto = await _userService.AssignOwnerRoleAsync(userId);

        var token = _jwtService.GenerateToken(userResponseDto);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        Response.Cookies.Append("jwt_token", token, cookieOptions);

        return Ok(userResponseDto);
    }
}
