namespace TennisBuddy.Application.DTOs.User;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserStatsDto Stats { get; set; } = new();
    public UserPreferencesDto Preferences { get; set; } = new();
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public DateTime JoinedDate { get; set; }
}

public class UserStatsDto
{
    public int? SkillLevel { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public double WinRate { get; set; }
    public string? DominantHand { get; set; }
    public List<string> PlayingStyles { get; set; } = new();
    public Dictionary<string, int> CourtTypePreferences { get; set; } = new();
}

public class UserPreferencesDto
{
    public List<string> PreferredPlayTimes { get; set; } = new();
    public List<string> PreferredCourtTypes { get; set; } = new();
    public int MaxTravelDistance { get; set; }
    public bool IsAvailableForMatches { get; set; }
    public bool ReceiveNotifications { get; set; }
    public string PrivacyLevel { get; set; } = string.Empty;
}