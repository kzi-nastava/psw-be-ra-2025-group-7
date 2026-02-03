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
        private readonly ISaleRepository _saleRepository;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper,
            IWalletInternalService walletInternalService,
            IPurchaseNotificationService purchaseNotificationService,
            ICouponRepository couponRepository,
            IPaymentRecordRepository paymentRecordRepository,
            ISaleRepository saleRepository)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _walletInternalService = walletInternalService;
            _purchaseNotificationService = purchaseNotificationService;
            _couponRepository = couponRepository;
            _paymentRecordRepository = paymentRecordRepository;
            _saleRepository = saleRepository;
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
                    throw new InvalidOperationException($"Tour with ID {tourId} has already been purchased.");
            }

            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                coupon = _couponRepository.GetByCode(couponCode);
                if (coupon == null || !coupon.IsValid())
                    throw new InvalidOperationException("Invalid or expired coupon code.");
            }

            var tours = tourIds.Select(id => _tourRepository.Get(id)).ToList();

            var sales = tours.ToDictionary(
                t => t.Id,
                t => _saleRepository.GetActiveSaleForTour(t.Id, DateTime.UtcNow)
            );

            long? couponTargetTourId = null;

            if (coupon != null)
            {
                if (coupon.IsUniversal)
                {
                    couponTargetTourId = tours.OrderByDescending(t => t.Price).First().Id;
                }
                else if (coupon.TourId.HasValue)
                {
                    var targetTour = tours.FirstOrDefault(t =>
                        t.Id == coupon.TourId.Value && t.AuthorId == coupon.AuthorId);

                    if (targetTour == null)
                        throw new InvalidOperationException("Coupon does not apply to any tour in the cart.");

                    couponTargetTourId = targetTour.Id;
                }
                else
                {
                    var authorTours = tours.Where(t => t.AuthorId == coupon.AuthorId).ToList();
                    if (!authorTours.Any())
                        throw new InvalidOperationException("Coupon does not apply to any tour in the cart.");

                    couponTargetTourId = authorTours.OrderByDescending(t => t.Price).First().Id;
                }

                if (!couponTargetTourId.HasValue)
                    throw new InvalidOperationException("Coupon target tour could not be determined.");

                if (!coupon.IsUniversal)
                {
                    var targetTour = tours.First(t => t.Id == couponTargetTourId.Value);
                    if (!coupon.AppliesTo(targetTour.Id, targetTour.AuthorId))
                        throw new InvalidOperationException("Coupon does not apply to selected tour.");
                }
            }

            // ✅ 1) Izračunaj ukupno (sale + coupon na target samo)
            decimal totalAfterDiscount = 0m;

            foreach (var tour in tours)
            {
                var tourFinalPrice = tour.Price;

                // Sale discount prvo
                var sale = sales[tour.Id];
                if (sale != null)
                {
                    tourFinalPrice = tourFinalPrice * (1 - sale.DiscountPercentage / 100m);
                }

                // Coupon discount samo na target
                var appliesCouponToThisTour =
                    coupon != null &&
                    couponTargetTourId.HasValue &&
                    tour.Id == couponTargetTourId.Value &&
                    (coupon.IsUniversal || tour.AuthorId == coupon.AuthorId);

                if (appliesCouponToThisTour)
                {
                    tourFinalPrice = tourFinalPrice * (1 - coupon!.DiscountPercentage / 100m);
                }

                totalAfterDiscount += tourFinalPrice;
            }

            // ✅ 2) Tek sad proveri i skini pare
            var balance = _walletInternalService.GetBalance(touristId);
            if (balance < totalAfterDiscount)
                throw new InvalidOperationException("Insufficient funds");

            _walletInternalService.Withdraw(touristId, totalAfterDiscount);

            // ✅ 3) Tek posle withdraw-a pravi tokene + records
            var createdTokenDtos = new List<ToursDto.TourPurchaseTokenDto>();
            var purchasedTourNames = new List<string>();

            foreach (var tour in tours)
            {
                purchasedTourNames.Add(tour.Name);

                var tourDiscountPercentage = 0m;
                var tourFinalPrice = tour.Price;

                // Sale
                var sale = sales[tour.Id];
                if (sale != null)
                {
                    tourDiscountPercentage = sale.DiscountPercentage;
                    tourFinalPrice = tourFinalPrice * (1 - sale.DiscountPercentage / 100m);
                }

                var appliesCouponToThisTour =
                    coupon != null &&
                    couponTargetTourId.HasValue &&
                    tour.Id == couponTargetTourId.Value &&
                    (coupon.IsUniversal || tour.AuthorId == coupon.AuthorId);

                if (appliesCouponToThisTour)
                {
                    // ovde po želji možeš da sabereš popuste, ali ti si htela da kupon "prepiše" discountPercentage
                    tourDiscountPercentage = coupon!.DiscountPercentage;
                    tourFinalPrice = tourFinalPrice * (1 - coupon.DiscountPercentage / 100m);
                }

                var token = TourPurchaseToken.CreateForTour(touristId, tour);
                var createdToken = _tokenRepository.Create(token);

                var tokenDto = _mapper.Map<ToursDto.TourPurchaseTokenDto>(createdToken);
                tokenDto.OriginalPrice = tour.Price;
                tokenDto.DiscountPercentage = tourDiscountPercentage;
                tokenDto.FinalPrice = tourFinalPrice;
                tokenDto.CouponCode = appliesCouponToThisTour ? couponCode : null;

                createdTokenDtos.Add(tokenDto);

                var paymentRecord = new PaymentRecord(
                    touristId,
                    tour.Id,
                    null,
                    tourFinalPrice,
                    tourDiscountPercentage,
                    appliesCouponToThisTour ? couponCode : null
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
