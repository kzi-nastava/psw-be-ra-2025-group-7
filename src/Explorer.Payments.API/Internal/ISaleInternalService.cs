using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface ISaleInternalService
    {
        decimal? GetActiveDiscountForTour(long tourId);
        decimal? GetDiscountedPrice(long tourId, decimal basePrice);
        List<long> GetToursWithActiveSales();
    }
}
