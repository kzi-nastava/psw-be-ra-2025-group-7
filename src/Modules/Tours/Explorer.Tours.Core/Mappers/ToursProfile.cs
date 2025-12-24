using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.Mappers
{
    public class ToursProfile : Profile
    {
        public ToursProfile()
        {
            CreateMap<EquipmentDto, Equipment>().ReverseMap();
            CreateMap<FacilityDto, Facility>().ReverseMap();
            CreateMap<KeyPointDto, KeyPoint>().ReverseMap();
            CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();
            CreateMap<KeyPoint, KeyPointWithoutSecretDto>();

            CreateMap<TourDto, Tour>()
                .ForMember(dest => dest.KeyPoints,
                           opt => opt.MapFrom(src => src.KeyPoints ?? new List<KeyPointDto>()))
                .ForMember(dest => dest.TourDurations,
                           opt => opt.MapFrom(src => src.TourDurations ?? new List<TourDurationDto>()))
                .ForMember(dest => dest.RequiredEquipment,
                           opt => opt.MapFrom(src => src.RequiredEquipment ?? new List<EquipmentDto>()))
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

            CreateMap<TourProblemMessage, TourProblemMessageDto>().ReverseMap();

            CreateMap<TourProblemDto, TourProblem>()
               .ForMember(d => d.Category, opt => opt.MapFrom(s => Enum.Parse<ProblemCategory>(s.Category, true)))
               .ForMember(d => d.Priority, opt => opt.MapFrom(s => Enum.Parse<ProblemPriority>(s.Priority, true)))
               .ForMember(d => d.Status, opt => opt.MapFrom(s =>
                        string.IsNullOrWhiteSpace(s.Status)
                            ? ProblemStatus.Open
                            : Enum.Parse<ProblemStatus>(s.Status, true)
                    ));

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

            CreateMap<TourJournalDto, TourJournal>().ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<TouristEquipment, TouristEquipmentDto>().ReverseMap();

            CreateMap<TourDurationDto, TourDuration>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TravelType)src.Type))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));
            
            // Tour Purchase Token mapovi - includes purchased tour info
            CreateMap<TourPurchaseToken, TourPurchaseTokenDto>()
                .ForMember(dest => dest.Tour, 
                           opt => opt.MapFrom(src => src.Tour));
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
