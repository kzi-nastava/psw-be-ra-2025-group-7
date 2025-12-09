using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Shopping
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public ShoppingCartDto GetByTouristId(long touristId)
        {
            var cart = GetOrCreateCart(touristId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto AddToCart(long touristId, long tourId)
        {
            var tour = _tourRepository.Get(tourId);

            if (tour.Status == TourStatus.Archived)
                throw new InvalidOperationException("Archived tours cannot be purchased.");

            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("Only published tours can be added to cart.");

            var cart = GetOrCreateCart(touristId);

            cart.AddItem(tour.Id, tour.Name, tour.Price);

            var updated = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(updated);
        }

        public ShoppingCartDto RemoveFromCart(long touristId, long orderItemId)
        {
            var cart = GetOrCreateCart(touristId);

            cart.RemoveItem(orderItemId);

            var updated = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(updated);
        }

        public void ClearCart(long touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart != null)
            {
                _cartRepository.Delete(cart.Id);
            }
        }

        private ShoppingCart GetOrCreateCart(long touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                cart = _cartRepository.Create(cart);
            }
            return cart;
        }
    }
}
