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
        CreateMap<Club, ClubDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => (ClubStatusDto)s.Status));
        CreateMap<ClubDto, Club>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => (ClubStatus)s.Status));
        CreateMap<TourPreferences, TourPreferencesDto>().ReverseMap();
        CreateMap<Notification, NotificationDto>();


    }
}
