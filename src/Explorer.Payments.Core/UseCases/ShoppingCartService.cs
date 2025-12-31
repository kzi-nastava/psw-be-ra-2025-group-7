using AutoMapper;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using PaymentsDto = Explorer.Payments.API.Dtos;
using ToursDto = Explorer.Tours.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;

namespace Explorer.Payments.Core.UseCases
{
    public class ShoppingCartService : Explorer.Payments.API.Public.IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IMapper _mapper;
        private readonly IWalletInternalService _walletInternalService;

        // NEW
        private readonly IPurchaseNotificationService _purchaseNotificationService;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper,
            IWalletInternalService walletInternalService,
            IPurchaseNotificationService purchaseNotificationService) // NEW
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _walletInternalService = walletInternalService;

            _purchaseNotificationService = purchaseNotificationService; // NEW
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

            if (_tokenRepository.HasUserPurchasedTour(touristId, tourId))
            {
                throw new InvalidOperationException("You have already purchased this tour.");
            }

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
        /// </summary>
        public List<object> PurchaseCart(long touristId)
        {
            var cart = GetOrCreateCart(touristId);

            var tourIds = cart.PreparePurchase();

            foreach (var tourId in tourIds)
            {
                if (_tokenRepository.HasUserPurchasedTour(touristId, tourId))
                {
                    throw new InvalidOperationException($"Tour with ID {tourId} has already been purchased.");
                }
            }

            var total = cart.TotalPrice;
            var balance = _walletInternalService.GetBalance(touristId);
            if (balance < total)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            _walletInternalService.Withdraw(touristId, total);

            var createdTokens = new List<TourPurchaseToken>();
            var purchasedTourNames = new List<string>(); // NEW

            foreach (var tourId in tourIds)
            {
                var tour = _tourRepository.Get(tourId);

                purchasedTourNames.Add(tour.Name); // NEW

                var token = TourPurchaseToken.CreateForTour(touristId, tour);

                var createdToken = _tokenRepository.Create(token);
                createdTokens.Add(createdToken);
            }

            cart.ClearAfterPurchase();
            _cartRepository.Update(cart);

            // NEW: notifikacija nakon uspešne kupovine
            _purchaseNotificationService.NotifyPurchaseSuccess(touristId, purchasedTourNames);

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
