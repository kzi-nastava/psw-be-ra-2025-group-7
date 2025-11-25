using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        // Existing mappings
        CreateMap<EquipmentDto, Equipment>().ReverseMap();

        CreateMap<TourProblemDto, TourProblem>()
           .ForMember(d => d.Category, opt => opt.MapFrom(s => Enum.Parse<ProblemCategory>(s.Category, true)))
           .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<ProblemPriority>(s.Priority, true)));

        CreateMap<TourProblem, TourProblemDto>()
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()));

        CreateMap<FacilityDto, Facility>().ReverseMap();
        CreateMap<TourDto, Tour>().ReverseMap();
        CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();


        CreateMap<AnnualAward, AnnualAwardDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<AnnualAwardDto, AnnualAward>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<AwardStatus>(s.Status, true)));
        CreateMap<CreateAnnualAwardDto, AnnualAward>()
            .ForMember(d => d.Status, opt => opt.Ignore()); 
        CreateMap<UpdateAnnualAwardDto, AnnualAward>()
            .ForMember(d => d.Status, opt => opt.Ignore()); 
    }
}