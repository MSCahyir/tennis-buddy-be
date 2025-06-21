namespace TennisBuddy.Application.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }
    public int? SkillLevel { get; set; }
    public List<string>? PreferredPlayTimes { get; set; }
    public DateTime? CreatedAt { get; set; }
}