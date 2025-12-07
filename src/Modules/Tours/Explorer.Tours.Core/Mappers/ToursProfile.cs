using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using System.Security.Cryptography;

namespace Explorer.Tours.Core.Mappers
{
    public class ToursProfile : Profile
    {
        public ToursProfile()
        {
            // Osnovni DTO-i
            CreateMap<EquipmentDto, Equipment>().ReverseMap();
            CreateMap<FacilityDto, Facility>().ReverseMap();

            // Kartica 3 – mapiranje KeyPoint <-> KeyPointDto
            CreateMap<KeyPointDto, KeyPoint>().ReverseMap();

            // Tour <-> TourDto, uključujući KeyPoints
            CreateMap<TourDto, Tour>()
                .ForMember(dest => dest.KeyPoints,
                           opt => opt.MapFrom(src => src.KeyPoints ?? new List<KeyPointDto>()))
                .ForMember(dest => dest.TourDurations,
                           opt => opt.MapFrom(src => src.TourDurations ?? new List<TourDurationDto>()))
                .ForMember(dest => dest.RequiredEquipment,
                           opt => opt.MapFrom(src => src.RequiredEquipment ?? new List<EquipmentDto>()))
                .ReverseMap();

            // Mapiranje za kreiranje ture (priča člana 1)
            CreateMap<CreateTourDto, Tour>();

            // Ostali postojeći mapovi
            CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

            CreateMap<TourProblemDto, TourProblem>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => Enum.Parse<ProblemCategory>(s.Category, true)))
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<ProblemPriority>(s.Priority, true)));

            CreateMap<TourProblem, TourProblemDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.ToString()))
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()));

            CreateMap<TourJournalDto, TourJournal>().ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

            CreateMap<TourDurationDto, TourDuration>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TravelType)src.Type))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

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
}
