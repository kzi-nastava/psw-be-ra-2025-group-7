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
        private readonly IPurchaseNotificationService _purchaseNotificationService;
        private readonly ICouponRepository _couponRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper,
            IWalletInternalService walletInternalService,
            IPurchaseNotificationService purchaseNotificationService,
            ICouponRepository couponRepository,
            IPaymentRecordRepository paymentRecordRepository)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _walletInternalService = walletInternalService;
            _purchaseNotificationService = purchaseNotificationService;
            _couponRepository = couponRepository;
            _paymentRecordRepository = paymentRecordRepository;
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

        public List<object> PurchaseCart(long touristId)
        {
            return PurchaseCartWithCoupon(touristId, null);
        }

        public List<object> PurchaseCartWithCoupon(long touristId, string? couponCode)
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

            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                coupon = _couponRepository.GetByCode(couponCode);
                if (coupon == null || !coupon.IsValid())
                    throw new InvalidOperationException("Invalid or expired coupon code.");
            }

            var tours = tourIds.Select(id => _tourRepository.Get(id)).ToList();
            var totalBeforeDiscount = cart.TotalPrice;
            var totalAfterDiscount = totalBeforeDiscount;
            var discountPercentage = 0m;

            if (coupon != null)
            {
                if (coupon.TourId.HasValue)
                {
                    var targetTour = tours.FirstOrDefault(t => t.Id == coupon.TourId.Value && t.AuthorId == coupon.AuthorId);
                    if (targetTour == null)
                        throw new InvalidOperationException("Coupon does not apply to any tour in the cart.");

                    var discount = targetTour.Price * (coupon.DiscountPercentage / 100m);
                    totalAfterDiscount -= discount;
                    discountPercentage = (discount / totalBeforeDiscount) * 100m;
                }
                else
                {
                    var authorTours = tours.Where(t => t.AuthorId == coupon.AuthorId).ToList();
                    if (!authorTours.Any())
                        throw new InvalidOperationException("Coupon does not apply to any tour in the cart.");

                    var mostExpensiveTour = authorTours.OrderByDescending(t => t.Price).First();
                    var discount = mostExpensiveTour.Price * (coupon.DiscountPercentage / 100m);
                    totalAfterDiscount -= discount;
                    discountPercentage = (discount / totalBeforeDiscount) * 100m;
                }
            }

            var balance = _walletInternalService.GetBalance(touristId);
            if (balance < totalAfterDiscount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            _walletInternalService.Withdraw(touristId, totalAfterDiscount);

            var createdTokenDtos = new List<ToursDto.TourPurchaseTokenDto>();
            var purchasedTourNames = new List<string>();

            foreach (var tour in tours)
            {
                purchasedTourNames.Add(tour.Name);

                // Check if this specific tour got a discount
                var tourDiscountPercentage = 0m;
                var tourFinalPrice = tour.Price;

                if (coupon != null && coupon.AppliesTo(tour.Id, tour.AuthorId))
                {
                    tourDiscountPercentage = coupon.DiscountPercentage;
                    tourFinalPrice = tour.Price * (1 - tourDiscountPercentage / 100m);
                }

                var token = TourPurchaseToken.CreateForTour(touristId, tour);
                var createdToken = _tokenRepository.Create(token);

                // Map to DTO and add payment information
                var tokenDto = _mapper.Map<ToursDto.TourPurchaseTokenDto>(createdToken);
                tokenDto.OriginalPrice = tour.Price;
                tokenDto.DiscountPercentage = tourDiscountPercentage;
                tokenDto.FinalPrice = tourFinalPrice;
                tokenDto.CouponCode = coupon != null && coupon.AppliesTo(tour.Id, tour.AuthorId) ? couponCode : null;

                createdTokenDtos.Add(tokenDto);

                var paymentRecord = new PaymentRecord(
                    touristId,
                    tour.Id,
                    null,
                    tour.Price,
                    tourDiscountPercentage,
                    coupon != null && coupon.AppliesTo(tour.Id, tour.AuthorId) ? couponCode : null
                );
                _paymentRecordRepository.Create(paymentRecord);
            }

            cart.ClearAfterPurchase();
            _cartRepository.Update(cart);

            _purchaseNotificationService.NotifyPurchaseSuccess(touristId, purchasedTourNames);

            return createdTokenDtos.Cast<object>().ToList();
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
