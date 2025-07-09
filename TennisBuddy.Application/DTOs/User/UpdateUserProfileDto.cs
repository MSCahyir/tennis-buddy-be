using System.ComponentModel.DataAnnotations;

namespace TennisBuddy.Application.DTOs.User;

public class UpdateUserProfileDto
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Range(1, 10, ErrorMessage = "Skill level must be between 1 and 10")]
    public int? SkillLevel { get; set; }

    public string? DominantHand { get; set; }
    public List<string>? PlayingStyles { get; set; }
    public Dictionary<string, int>? CourtTypePreferences { get; set; }
    public List<string>? PreferredPlayTimes { get; set; }
    public List<string>? PreferredCourtTypes { get; set; }

    [Range(1, 100)]
    public int? MaxTravelDistance { get; set; }

    public bool? IsAvailableForMatches { get; set; }
    public bool? ReceiveNotifications { get; set; }
    public string? PrivacyLevel { get; set; }

    [StringLength(500)]
    public string? ProfilePicture { get; set; }
}