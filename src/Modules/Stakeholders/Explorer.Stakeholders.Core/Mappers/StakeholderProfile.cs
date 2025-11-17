using AutoMapper;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<Domain.Message, API.Dtos.MessageDto>().ReverseMap();
    }
}