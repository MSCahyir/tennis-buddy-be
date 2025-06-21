using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.User;
using TennisBuddy.Application.Services.Interfaces;

namespace TennisBuddy.Api.Controllers;

/// <summary>
/// User profile management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User profile information</returns>
    /// <response code="200">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Get All Users
    /// </summary>
    /// <returns>List of all users</returns>
    /// <response code="200">Users retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// [HttpGet("all")]
    /// [ProducesResponseType(typeof(List<UserDto>), 200)]
    /// [ProducesResponseType(401)]
    /// [ProducesResponseType(500)]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    /// <param name="updateDto">Profile update information</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var updatedUser = await _userService.UpdateUserProfileAsync(userId, updateDto);

        if (updatedUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(updatedUser);
    }

    /// <summary>
    /// Search for tennis partners
    /// </summary>
    /// <param name="searchDto">Search criteria</param>
    /// <returns>List of matching users</returns>
    /// <response code="200">Search completed successfully</response>
    /// <response code="400">Invalid search criteria</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(UserSearchResultDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SearchUsers([FromBody] UserSearchDto searchDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.SearchUsersAsync(searchDto);
        return Ok(result);
    }

    /// <summary>
    /// Get nearby tennis players
    /// </summary>
    /// <param name="location">Location to search around</param>
    /// <param name="radiusKm">Search radius in kilometers (default: 10)</param>
    /// <returns>List of nearby users</returns>
    /// <response code="200">Nearby users found</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(List<UserDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetNearbyUsers([FromQuery] string location, [FromQuery] int radiusKm = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var users = await _userService.GetNearbyUsersAsync(userId, location, radiusKm);
        return Ok(users);
    }

    /// <summary>
    /// Get users with similar skill level
    /// </summary>
    /// <param name="skillRange">Skill level range (default: 2)</param>
    /// <returns>List of users with similar skill level</returns>
    /// <response code="200">Similar skill level users found</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("similar-skill")]
    [ProducesResponseType(typeof(List<UserDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetSimilarSkillUsers([FromQuery] int skillRange = 2)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var users = await _userService.GetUsersBySimilarSkillLevelAsync(userId, skillRange);
        return Ok(users);
    }

    /// <summary>
    /// Delete current user account
    /// </summary>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Account deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpDelete("account")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var deleted = await _userService.DeleteUserAsync(userId);

        if (!deleted)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new { message = "Account deleted successfully" });
    }
}