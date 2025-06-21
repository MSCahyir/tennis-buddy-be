using Microsoft.EntityFrameworkCore;
using TennisBuddy.Domain.Entities;
using TennisBuddy.Domain.Interfaces;
using TennisBuddy.Infrastructure.Data;

namespace TennisBuddy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        // Soft delete - just mark as inactive
        user.IsActive = false;
        await UpdateAsync(user);
        return true;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByLocationAsync(string location)
    {
        return await _context.Users
            .Where(u => u.IsActive && u.Location != null && u.Location.ToLower().Contains(location.ToLower()))
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetBySkillLevelAsync(int skillLevel)
    {
        return await _context.Users
            .Where(u => u.IsActive && u.SkillLevel == skillLevel)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetBySkillLevelRangeAsync(int minSkillLevel, int maxSkillLevel)
    {
        return await _context.Users
            .Where(u => u.IsActive && u.SkillLevel >= minSkillLevel && u.SkillLevel <= maxSkillLevel)
            .OrderBy(u => u.SkillLevel)
            .ThenBy(u => u.Name)
            .ToListAsync();
    }
}