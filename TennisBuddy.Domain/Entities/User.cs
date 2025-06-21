using System.ComponentModel.DataAnnotations;

namespace TennisBuddy.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? ProfilePicture { get; set; }

    public string? Location { get; set; }

    public string? PhoneNumber { get; set; }

    [Range(1, 10)]
    public int? SkillLevel { get; set; } // 1-10 tennis skill level

    public List<string>? PreferredPlayTimes { get; set; }

    public DateTime? CreatedAt { get; set; }

    // For authentication
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;
}