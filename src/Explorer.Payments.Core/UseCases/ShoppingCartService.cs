using AutoMapper;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using PaymentsDto = Explorer.Payments.API.Dtos;
using ToursDto = Explorer.Tours.API.Dtos;

namespace Explorer.Payments.Core.UseCases
{
    public class ShoppingCartService : Explorer.Payments.API.Public.IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
        }

        public PaymentsDto.ShoppingCartDto GetByTouristId(long touristId)
        {
            var cart = GetOrCreateCart(touristId);
            return _mapper.Map<PaymentsDto.ShoppingCartDto>(cart);
        }

        public PaymentsDto.ShoppingCartDto AddToCart(long touristId, long tourId)
        {
            var tour = _tourRepository.Get(tourId);

            if (tour.Status == TourStatus.Archived)
                throw new InvalidOperationException("Archived tours cannot be purchased.");

            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("Only published tours can be added to cart.");

            var cart = GetOrCreateCart(touristId);

            cart.AddItem(tour.Id, tour.Name, tour.Price);

            var updated = _cartRepository.Update(cart);
            return _mapper.Map<PaymentsDto.ShoppingCartDto>(updated);
        }

        public PaymentsDto.ShoppingCartDto RemoveFromCart(long touristId, long orderItemId)
        {
            var cart = GetOrCreateCart(touristId);

            cart.RemoveItem(orderItemId);

            var updated = _cartRepository.Update(cart);
            return _mapper.Map<PaymentsDto.ShoppingCartDto>(updated);
        }

        public void ClearCart(long touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart != null)
            {
                _cartRepository.Delete(cart.Id);
            }
        }

        /// <summary>
        /// Purchases all items in the cart by creating TourPurchaseTokens for each item.
        /// Uses domain-driven design: the ShoppingCart aggregate validates the purchase,
        /// then we create tokens for each tour, and finally clear the cart.
        /// </summary>
        public List<object> PurchaseCart(long touristId)
        {
            var cart = GetOrCreateCart(touristId);

            // Validates that cart can be purchased and returns tour IDs
            var tourIds = cart.PreparePurchase();

            var createdTokens = new List<TourPurchaseToken>();

            // Create tokens for each tour in the cart
            foreach (var tourId in tourIds)
            {
                // Check if user already purchased this tour
                if (_tokenRepository.HasUserPurchasedTour(touristId, tourId))
                {
                    throw new InvalidOperationException($"Tour with ID {tourId} has already been purchased.");
                }

                // Get the tour to validate purchase rules
                var tour = _tourRepository.Get(tourId);
                
                // Use factory method to create token with all business rules enforced
                var token = TourPurchaseToken.CreateForTour(touristId, tour);
                
                // Persist the token
                var createdToken = _tokenRepository.Create(token);
                createdTokens.Add(createdToken);
            }

            // Clear the cart after successful purchase (domain method)
            cart.ClearAfterPurchase();
            _cartRepository.Update(cart);

            // Return DTOs
            return _mapper.Map<List<ToursDto.TourPurchaseTokenDto>>(createdTokens).Cast<object>().ToList();
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
