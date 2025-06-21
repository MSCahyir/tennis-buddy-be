using System.ComponentModel.DataAnnotations;

namespace TennisBuddy.Application.DTOs.User;

public class UpdateUserProfileDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Range(1, 10, ErrorMessage = "Skill level must be between 1 and 10")]
    public int? SkillLevel { get; set; }

    public List<string>? PreferredPlayTimes { get; set; }

    [StringLength(500)]
    public string? ProfilePicture { get; set; }
}