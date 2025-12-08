using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Domain.Message, API.Dtos.MessageDto>().ReverseMap();
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        CreateMap<Account, AccountDto>();
        CreateMap<AccountCreateDto, Account>();
        CreateMap<Club, ClubDto>().ReverseMap();
        CreateMap<TourPreferences, TourPreferencesDto>().ReverseMap();
        
        // New mappings for follower system
        CreateMap<Follower, FollowerDto>();
        
        CreateMap<FollowerMessage, FollowerMessageDto>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.ResourceType.HasValue ? src.ResourceType.ToString() : null));
        
        CreateMap<FollowerMessageDto, FollowerMessage>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.ResourceType) ? (ResourceType?)null : Enum.Parse<ResourceType>(src.ResourceType)));
        
        CreateMap<ClubMessage, ClubMessageDto>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => src.ResourceType.HasValue ? src.ResourceType.ToString() : null));
        
        CreateMap<ClubMessageDto, ClubMessage>()
            .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.ResourceType) ? (ResourceType?)null : Enum.Parse<ResourceType>(src.ResourceType)));
    }
}
