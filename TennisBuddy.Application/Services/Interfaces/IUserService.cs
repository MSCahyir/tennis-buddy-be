using TennisBuddy.Application.DTOs.User;

namespace TennisBuddy.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetUserProfileByIdAsync(string userId);
    Task<UserProfileDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto);
    Task<UserSearchResultDto> SearchUsersAsync(UserSearchDto searchDto);
    Task<bool> DeleteUserAsync(string userId);
    Task<List<UserProfileDto>> GetNearbyUsersAsync(string currentUserId, string location, int radiusKm = 10);
    Task<List<UserProfileDto>> GetUsersBySimilarSkillLevelAsync(string currentUserId, int skillLevelRange = 2);
}