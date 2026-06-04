using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing user operations including authentication and CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">The user service for handling user operations.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userResponseDto = await _userService.LoginAsync(loginDto);
        return Ok(userResponseDto);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var userDtos = await _userService.GetAllUsersAsync();
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


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
}
