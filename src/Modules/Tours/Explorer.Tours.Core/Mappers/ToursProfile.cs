using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<CreateTourDto, Tour>();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

        CreateMap<TourProblemDto, TourProblem>()
           .ForMember(d => d.Category, opt => opt.MapFrom(s => Enum.Parse<ProblemCategory>(s.Category, true)))
           .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<ProblemPriority>(s.Priority, true)));
        CreateMap<TourProblem, TourProblemDto>()
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()));
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<TourJournalDto, TourJournal>().ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

    }
}