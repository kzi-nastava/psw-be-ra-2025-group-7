using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class BundlePurchaseService : IBundlePurchaseService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IBundlePurchaseRepository _bundlePurchaseRepository;
        private readonly IWalletInternalService _walletInternalService;

        public BundlePurchaseService(
            IBundleRepository bundleRepository,
            ITourRepository tourRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IBundlePurchaseRepository bundlePurchaseRepository,
            IWalletInternalService walletInternalService)
        {
            _bundleRepository = bundleRepository;
            _tourRepository = tourRepository;
            _tokenRepository = tokenRepository;
            _bundlePurchaseRepository = bundlePurchaseRepository;
            _walletInternalService = walletInternalService;
        }

        public void PurchaseBundle(long touristId, long bundleId)
        {
            var bundle = _bundleRepository.Get(bundleId);

            if (bundle.Status != BundleStatus.Published)
                throw new InvalidOperationException("Only published bundles can be purchased.");

            if (_bundlePurchaseRepository.HasUserPurchasedBundle(touristId, bundleId))
                throw new InvalidOperationException("Bundle has already been purchased.");

            foreach (var item in bundle.Items)
            {
                var tour = _tourRepository.Get(item.TourId);

                if (tour.Status == TourStatus.Archived)
                    throw new InvalidOperationException(
                        $"Archived tour {tour.Name} cannot be purchased.");

                if (tour.Status != TourStatus.Published)
                    throw new InvalidOperationException(
                        $"Tour {tour.Name} is not published.");

                // korisnik već ima token?
                if (_tokenRepository.HasUserPurchasedTour(touristId, tour.Id))
                    throw new InvalidOperationException(
                        $"Tour {tour.Name} has already been purchased.");
            }

            var balance = _walletInternalService.GetBalance(touristId);
            if (balance < bundle.Price)
                throw new InvalidOperationException("Insufficient funds.");

            _walletInternalService.Withdraw(touristId, bundle.Price);

            var purchase = new BundlePurchase(
                touristId,
                bundle.Id,
                bundle.Price);

            _bundlePurchaseRepository.Create(purchase);

            foreach (var item in bundle.Items)
            {
                var tour = _tourRepository.Get(item.TourId);

                var token = TourPurchaseToken.CreateForTour(touristId, tour);
                _tokenRepository.Create(token);
            }
        }
    }
}
