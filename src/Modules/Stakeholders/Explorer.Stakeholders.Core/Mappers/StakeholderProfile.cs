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
        CreateMap<MonumentDto, Monument>().ReverseMap();


        CreateMap<TourPreferences, TourPreferencesDto>();/*
                .ForMember(dest => dest.TransportDifficulties,
                    opt => opt.MapFrom(src =>
                        src.TransportDifficulties.Select(x => new TransportDifficultyDto
                        {
                            TransportMode = (TransportModeDto)x.Key,
                            Difficulty = (DifficultyDto)x.Value
                        }).ToList()
                    ));

        CreateMap<TourPreferencesDto, TourPreferences>()
            .ForMember(dest => dest.TransportDifficulties,
                opt => opt.MapFrom(src =>
                    src.TransportDifficulties == null
                    ? new Dictionary<TransportMode, Difficulty>()
                        : src.TransportDifficulties.ToDictionary(
                            x => (TransportMode)x.TransportMode,
                            x => (Difficulty)x.Difficulty
                        )
                ));*/
    }
}