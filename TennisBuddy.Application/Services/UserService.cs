using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.User;
using TennisBuddy.Application.Services.Interfaces;
using TennisBuddy.Domain.Entities;
using TennisBuddy.Domain.Interfaces;

namespace TennisBuddy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToUserDto).ToList();
    }

    public async Task<UserDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateDto.FullName))
            user.FullName = updateDto.FullName;

        if (updateDto.Location != null)
            user.Location = updateDto.Location;

        if (updateDto.PhoneNumber != null)
            user.PhoneNumber = updateDto.PhoneNumber;

        if (updateDto.SkillLevel.HasValue)
            user.SkillLevel = updateDto.SkillLevel;

        if (updateDto.PreferredPlayTimes != null)
            user.PreferredPlayTimes = updateDto.PreferredPlayTimes;

        if (updateDto.ProfilePicture != null)
            user.ProfilePicture = updateDto.ProfilePicture;

        var updatedUser = await _userRepository.UpdateAsync(user);
        return MapToUserDto(updatedUser);
    }

    public async Task<UserSearchResultDto> SearchUsersAsync(UserSearchDto searchDto)
    {
        var users = await _userRepository.GetAllAsync();

        // Apply filters
        var filteredUsers = users.AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            filteredUsers = filteredUsers.Where(u =>
                u.Location != null &&
                u.Location.Contains(searchDto.Location, StringComparison.OrdinalIgnoreCase));
        }

        if (searchDto.MinSkillLevel.HasValue)
        {
            filteredUsers = filteredUsers.Where(u =>
                u.SkillLevel >= searchDto.MinSkillLevel);
        }

        if (searchDto.MaxSkillLevel.HasValue)
        {
            filteredUsers = filteredUsers.Where(u =>
                u.SkillLevel <= searchDto.MaxSkillLevel);
        }

        if (!string.IsNullOrEmpty(searchDto.PreferredPlayTime))
        {
            filteredUsers = filteredUsers.Where(u =>
                u.PreferredPlayTimes != null &&
                u.PreferredPlayTimes.Contains(searchDto.PreferredPlayTime));
        }

        var totalCount = filteredUsers.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

        var paginatedUsers = filteredUsers
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(MapToUserDto)
            .ToList();

        return new UserSearchResultDto
        {
            Users = paginatedUsers,
            TotalCount = totalCount,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize,
            TotalPages = totalPages,
            HasNextPage = searchDto.PageNumber < totalPages,
            HasPreviousPage = searchDto.PageNumber > 1
        };
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        return await _userRepository.DeleteAsync(userId);
    }

    public async Task<List<UserDto>> GetNearbyUsersAsync(string currentUserId, string location, int radiusKm = 10)
    {
        var users = await _userRepository.GetByLocationAsync(location);
        return users
            .Where(u => u.Id != currentUserId)
            .Select(MapToUserDto)
            .ToList();
    }

    public async Task<List<UserDto>> GetUsersBySimilarSkillLevelAsync(string currentUserId, int skillLevelRange = 2)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId);
        if (currentUser?.SkillLevel == null)
        {
            return new List<UserDto>();
        }

        var minSkill = Math.Max(1, currentUser.SkillLevel.Value - skillLevelRange);
        var maxSkill = Math.Min(10, currentUser.SkillLevel.Value + skillLevelRange);

        var users = await _userRepository.GetBySkillLevelRangeAsync(minSkill, maxSkill);
        return users
            .Where(u => u.Id != currentUserId)
            .Select(MapToUserDto)
            .ToList();
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            ProfilePicture = user.ProfilePicture,
            Location = user.Location,
            PhoneNumber = user.PhoneNumber,
            SkillLevel = user.SkillLevel,
            PreferredPlayTimes = user.PreferredPlayTimes,
            CreatedAt = user.CreatedAt
        };
    }
}