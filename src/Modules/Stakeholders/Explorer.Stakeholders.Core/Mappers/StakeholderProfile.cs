using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Domain.Message, API.Dtos.MessageDto>().ReverseMap();
        CreateMap<Account, AccountDto>();
        CreateMap<AccountCreateDto, Account>();
        CreateMap<MonumentDto, Monument>().ReverseMap();
    }
}