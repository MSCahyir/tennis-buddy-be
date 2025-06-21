using System.ComponentModel.DataAnnotations;

namespace TennisBuddy.Application.DTOs.User;

public class UserSearchDto
{
    public string? Location { get; set; }

    [Range(1, 10)]
    public int? MinSkillLevel { get; set; }

    [Range(1, 10)]
    public int? MaxSkillLevel { get; set; }

    public string? PreferredPlayTime { get; set; }

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}