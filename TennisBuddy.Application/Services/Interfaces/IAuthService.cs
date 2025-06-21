using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.Auth;

namespace TennisBuddy.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
    Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequest);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetCurrentUserAsync(string userId);
}