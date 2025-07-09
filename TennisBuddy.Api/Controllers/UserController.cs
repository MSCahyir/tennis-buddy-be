using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TennisBuddy.Application.DTOs.Common;
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
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserProfileByIdAsync(id);

        if (user == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
        }

        return Ok(ApiResponse<UserProfileDto>.SuccessResponse(user));
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    /// <param name="updateDto">Profile update information</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", errors));
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated"));
        }

        var updatedUser = await _userService.UpdateUserProfileAsync(userId, updateDto);

        if (updatedUser == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
        }

        return Ok(ApiResponse<UserProfileDto>.SuccessResponse(updatedUser, "Profile updated successfully"));
    }

    /// <summary>
    /// Search for tennis partners
    /// </summary>
    /// <param name="searchDto">Search criteria</param>
    /// <returns>List of matching users</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(ApiResponse<UserSearchResultDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> SearchUsers([FromBody] UserSearchDto searchDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid search criteria", errors));
        }

        var result = await _userService.SearchUsersAsync(searchDto);
        return Ok(ApiResponse<UserSearchResultDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Get nearby tennis players
    /// </summary>
    /// <param name="location">Location to search around</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <returns>List of nearby users</returns>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(ApiResponse<List<UserProfileDto>>), 200)]
    public async Task<IActionResult> GetNearbyUsers([FromQuery] string location, [FromQuery] int radiusKm = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated"));
        }

        var users = await _userService.GetNearbyUsersAsync(userId, location, radiusKm);
        return Ok(ApiResponse<List<UserProfileDto>>.SuccessResponse(users));
    }
}