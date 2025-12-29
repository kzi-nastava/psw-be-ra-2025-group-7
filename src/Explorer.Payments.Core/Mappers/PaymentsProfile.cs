using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Mappers
{
    public class PaymentsProfile : Profile
    {
        public PaymentsProfile()
        {
            // Shopping Cart mapovi
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<ShoppingCart, ShoppingCartDto>().ReverseMap();

            // Bundle mapovi
            CreateMap<Bundle, BundleDto>()
                .ForMember(d => d.TourIds, opt => opt.MapFrom(s => s.Items.Select(i => i.TourId).ToList()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (int)s.Status));
        }
    }
}
