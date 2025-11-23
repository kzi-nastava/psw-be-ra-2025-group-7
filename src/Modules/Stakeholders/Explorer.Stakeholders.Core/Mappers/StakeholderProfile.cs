using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<AccountCreateDto, Account>();

        CreateMap<MonumentDto, Monument>()
            .ConstructUsing(src => new Monument(
                src.Name,
                src.Description,
                src.YearOfCreation,
                src.Latitude,
                src.Longitude
            ))
            .ReverseMap();
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        CreateMap<Domain.Message, API.Dtos.MessageDto>().ReverseMap();
    }
}
