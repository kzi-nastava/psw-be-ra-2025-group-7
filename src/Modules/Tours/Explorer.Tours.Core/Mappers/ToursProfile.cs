using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;

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
            
            // KeyPoint to KeyPointWithoutSecretDto (excludes secret)
            CreateMap<KeyPoint, KeyPointWithoutSecretDto>();

            // Tour <-> TourDto, uključujući KeyPoints
            CreateMap<TourDto, Tour>()
                .ForMember(dest => dest.KeyPoints,
                           opt => opt.MapFrom(src => src.KeyPoints ?? new List<KeyPointDto>()))
                .ForMember(dest => dest.TourDurations,
                           opt => opt.MapFrom(src => src.TourDurations ?? new List<TourDurationDto>()))
                .ReverseMap();
            
            // Tour to PurchasedTourInfoDto (for purchased tours without secrets)
            CreateMap<Tour, PurchasedTourInfoDto>()
                .ForMember(dest => dest.KeyPoints,
                           opt => opt.MapFrom(src => src.KeyPoints))
                .ForMember(dest => dest.TourDurations,
                           opt => opt.MapFrom(src => src.TourDurations));
            
            // Tour to TourPreviewDto (for browsing - limited info before purchase)
            CreateMap<Tour, TourPreviewDto>()
                .ForMember(dest => dest.Difficulty,
                           opt => opt.MapFrom(src => src.Difficulty.ToString()))
                .ForMember(dest => dest.IsPurchasable,
                           opt => opt.MapFrom(src => src.Status == TourStatus.Published))
                .ForMember(dest => dest.StartingPoint,
                           opt => opt.MapFrom(src => src.KeyPoints != null && src.KeyPoints.Any() 
                               ? src.KeyPoints.First() 
                               : null))
                .ForMember(dest => dest.TourDurations,
                           opt => opt.MapFrom(src => src.TourDurations ?? new List<TourDuration>()));

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

            // Shopping Cart mapovi
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();
            
            // Tour Purchase Token mapovi - includes purchased tour info
            CreateMap<TourPurchaseToken, TourPurchaseTokenDto>()
                .ForMember(dest => dest.Tour, 
                           opt => opt.MapFrom(src => src.Tour));
        }
    }
}
