using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.User;

namespace TennisBuddy.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto);
    Task<UserSearchResultDto> SearchUsersAsync(UserSearchDto searchDto);
    Task<bool> DeleteUserAsync(string userId);
    Task<List<UserDto>> GetNearbyUsersAsync(string currentUserId, string location, int radiusKm = 10);
    Task<List<UserDto>> GetUsersBySimilarSkillLevelAsync(string currentUserId, int skillLevelRange = 2);
}