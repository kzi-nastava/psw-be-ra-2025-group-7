using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class MonumentProfile : Profile
{
    public MonumentProfile()
    {
        CreateMap<MonumentDto, Monument>()
            .ConstructUsing(src => new Monument(
                src.Name,
                src.Description,
                src.YearOfCreation,
                src.Latitude,
                src.Longitude
            ))
            .ReverseMap();
    }
}
