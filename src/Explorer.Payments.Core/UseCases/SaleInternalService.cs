using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class SaleInternalService : ISaleInternalService
    {
        private readonly ISaleRepository _saleRepository;

        public SaleInternalService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public decimal? GetActiveDiscountForTour(long tourId)
        {
            var sale = _saleRepository.GetActiveSaleForTour(tourId, DateTime.UtcNow);
            return sale?.DiscountPercentage;
        }

        public decimal? GetDiscountedPrice(long tourId, decimal basePrice)
        {
            var sale = _saleRepository.GetActiveSaleForTour(tourId, DateTime.UtcNow);
            if (sale == null) return null;

            return basePrice * (1 - sale.DiscountPercentage / 100m);
        }

        public List<long> GetToursWithActiveSales()
        {
            var now = DateTime.UtcNow;
            var activeSales = _saleRepository.GetAllActive(now);

            return activeSales
                .SelectMany(s => s.SaleTours.Select(st => st.TourId))
                .Distinct()
                .ToList();
        }
    }
}
