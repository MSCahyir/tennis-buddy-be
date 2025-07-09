using AutoMapper;
using TennisBuddy.Application.DTOs;
using TennisBuddy.Application.DTOs.Auth;
using TennisBuddy.Application.DTOs.User;
using TennisBuddy.Domain.Entities;

namespace TennisBuddy.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.JoinedDate, opt => opt.MapFrom(src => src.CreatedAt));

        // User Stats mapping
        CreateMap<User, UserStatsDto>()
            .ForMember(dest => dest.WinRate, opt => opt.MapFrom(src => src.WinRate))
            .ForMember(dest => dest.PlayingStyles, opt => opt.MapFrom(src => src.PlayingStyles ?? new List<string>()))
            .ForMember(dest => dest.CourtTypePreferences, opt => opt.MapFrom(src => src.CourtTypePreferences ?? new Dictionary<string, int>()));

        // User Preferences mapping
        CreateMap<User, UserPreferencesDto>()
            .ForMember(dest => dest.PreferredPlayTimes, opt => opt.MapFrom(src => src.PreferredPlayTimes ?? new List<string>()))
            .ForMember(dest => dest.PreferredCourtTypes, opt => opt.MapFrom(src => src.PreferredCourtTypes ?? new List<string>()));

        // Registration mapping
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Will be set manually

        // Update mapping
        CreateMap<UpdateUserProfileDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Search result mapping
        CreateMap<UserProfileDto, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.SkillLevel, opt => opt.MapFrom(src => src.Stats.SkillLevel))
            .ForMember(dest => dest.PreferredPlayTimes, opt => opt.MapFrom(src => src.Preferences.PreferredPlayTimes))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.JoinedDate));
    }
}