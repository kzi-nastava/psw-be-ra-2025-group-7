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
        CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();
        CreateMap<TourProblemMessage, TourProblemMessageDto>().ReverseMap();
        CreateMap<TourProblemDto, TourProblem>()
           .ForMember(d => d.Category, opt => opt.MapFrom(s => Enum.Parse<ProblemCategory>(s.Category, true)))
           .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<ProblemPriority>(s.Priority, true)))
           .ForMember(d => d.Status, opt => opt.MapFrom(s=>Enum.Parse<ProblemStatus>(s.Status,true)));
        CreateMap<TourProblem, TourProblemDto>()
             .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
             .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()))
             .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
             .ForMember(d => d.Comments, opt => opt.MapFrom(s =>
                 // Ako nema komentara, kreiramo listu sa prvom porukom iz Description
                 s.Comments.Any()
                     ? s.Comments
                     : new List<TourProblemMessage> { new TourProblemMessage(s.TouristId, s.Description, s.TimeReported) }
             ));
        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<TourJournalDto, TourJournal>().ReverseMap()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

    }
}