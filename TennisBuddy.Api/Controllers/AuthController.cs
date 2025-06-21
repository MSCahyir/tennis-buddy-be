using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TennisBuddy.Application.DTOs.Auth;
using TennisBuddy.Application.Services.Interfaces;
using TennisBuddy.Application.DTOs; // Add this line for UserDto


namespace TennisBuddy.Api.Controllers;

/// <summary>
/// Authentication controller for user registration, login, and profile management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="loginRequest">User login credentials</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginRequest);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="registerRequest">User registration information</param>
    /// <returns>Created user information</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Invalid request or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registerRequest);

        if (result == null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
    }

    /// <summary>
    /// Get current authenticated user profile
    /// </summary>
    /// <returns>Current user information</returns>
    /// <response code="200">User profile retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetCurrentUserAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="request">Token validation request</param>
    /// <returns>Token validation result</returns>
    /// <response code="200">Token validation result</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequestDto request)
    {
        var isValid = await _authService.ValidateTokenAsync(request.Token);
        return Ok(new { isValid });
    }
}