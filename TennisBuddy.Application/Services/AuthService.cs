using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.Auth;
using TennisBuddy.Application.Services.Interfaces;
using TennisBuddy.Domain.Entities;
using TennisBuddy.Domain.Interfaces;

namespace TennisBuddy.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

        if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
        {
            return null;
        }

        if (!user.IsActive)
        {
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var userDto = MapToUserDto(user);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequest)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(registerRequest.Email))
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = registerRequest.Email,
            Name = registerRequest.Name,
            PasswordHash = HashPassword(registerRequest.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfilePicture = user.ProfilePicture,
            Location = user.Location,
            PhoneNumber = user.PhoneNumber,
            SkillLevel = user.SkillLevel,
            PreferredPlayTimes = user.PreferredPlayTimes,
            CreatedAt = user.CreatedAt
        };
    }
}