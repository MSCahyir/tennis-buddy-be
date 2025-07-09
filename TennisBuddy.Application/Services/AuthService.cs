using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;

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
        var existingUser = await _userRepository.GetByEmailAsync(registerRequest.Email);
        if (existingUser != null)
        {
            return null;
        }

        var user = _mapper.Map<User>(registerRequest);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

        var createdUser = await _userRepository.CreateAsync(user);
        return _mapper.Map<UserDto>(createdUser);
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
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
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
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePicture,
            Location = user.Location,
            PhoneNumber = user.PhoneNumber,
            SkillLevel = user.SkillLevel,
            PreferredPlayTimes = user.PreferredPlayTimes,
            CreatedAt = user.CreatedAt
        };
    }
}