using System.ComponentModel.DataAnnotations;

namespace TennisBuddy.Application.DTOs.Auth;

public class ValidateTokenRequestDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}