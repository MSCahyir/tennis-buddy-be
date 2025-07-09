using AutoMapper;
using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.User;
using TennisBuddy.Application.Services.Interfaces;
using TennisBuddy.Domain.Entities;
using TennisBuddy.Domain.Interfaces;

namespace TennisBuddy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto?> GetUserProfileByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserProfileDto>(user) : null;
    }

    public async Task<UserProfileDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        // Use AutoMapper to update only non-null properties
        _mapper.Map(updateDto, user);

        var updatedUser = await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserProfileDto>(updatedUser);
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
            .ToList();

        var userProfileDtos = _mapper.Map<List<UserProfileDto>>(paginatedUsers);
        var userDtos = _mapper.Map<List<UserDto>>(userProfileDtos);

        return new UserSearchResultDto
        {
            Users = userDtos,
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

    public async Task<List<UserProfileDto>> GetNearbyUsersAsync(string currentUserId, string location, int radiusKm = 10)
    {
        var users = await _userRepository.GetByLocationAsync(location);
        var filteredUsers = users.Where(u => u.Id != currentUserId).ToList();
        return _mapper.Map<List<UserProfileDto>>(filteredUsers);
    }

    public async Task<List<UserProfileDto>> GetUsersBySimilarSkillLevelAsync(string currentUserId, int skillLevelRange = 2)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId);
        if (currentUser?.SkillLevel == null)
        {
            return new List<UserProfileDto>();
        }

        var minSkill = Math.Max(1, currentUser.SkillLevel.Value - skillLevelRange);
        var maxSkill = Math.Min(10, currentUser.SkillLevel.Value + skillLevelRange);

        var users = await _userRepository.GetBySkillLevelRangeAsync(minSkill, maxSkill);
        var filteredUsers = users.Where(u => u.Id != currentUserId).ToList();
        return _mapper.Map<List<UserProfileDto>>(filteredUsers);
    }
}