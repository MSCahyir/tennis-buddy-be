using TennisBuddy.Domain.Entities;

namespace TennisBuddy.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetByLocationAsync(string location);
    Task<IEnumerable<User>> GetBySkillLevelAsync(int skillLevel);
    Task<IEnumerable<User>> GetBySkillLevelRangeAsync(int minSkillLevel, int maxSkillLevel);
}