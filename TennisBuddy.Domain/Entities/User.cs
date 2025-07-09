using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TennisBuddy.Domain.Entities;

public class User
{
    [Key]
    [StringLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? ProfilePicture { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Range(1, 10)]
    public int? SkillLevel { get; set; }

    public int MatchesPlayed { get; set; } = 0;
    public int MatchesWon { get; set; } = 0;
    public int MatchesLost { get; set; } = 0;

    [StringLength(10)]
    public string? DominantHand { get; set; }

    // JSON columns for PostgreSQL
    [Column(TypeName = "jsonb")]
    public List<string>? PlayingStyles { get; set; }

    [Column(TypeName = "jsonb")]
    public Dictionary<string, int>? CourtTypePreferences { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? PreferredPlayTimes { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? PreferredCourtTypes { get; set; }

    public int MaxTravelDistance { get; set; } = 15;
    public bool IsAvailableForMatches { get; set; } = true;
    public bool ReceiveNotifications { get; set; } = true;

    [StringLength(20)]
    public string PrivacyLevel { get; set; } = "public";

    public bool IsOnline { get; set; } = false;
    public DateTime? LastSeen { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    [NotMapped]
    public double WinRate => MatchesPlayed > 0 ? Math.Round((double)MatchesWon / MatchesPlayed * 100, 1) : 0;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
}