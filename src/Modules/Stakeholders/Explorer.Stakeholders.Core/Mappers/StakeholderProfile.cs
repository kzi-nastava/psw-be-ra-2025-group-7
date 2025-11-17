using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
<<<<<<< HEAD
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
=======
        CreateMap<Account, AccountDto>();
        CreateMap<AccountCreateDto, Account>();
        CreateMap<MonumentDto, Monument>().ReverseMap();
>>>>>>> development
    }
}