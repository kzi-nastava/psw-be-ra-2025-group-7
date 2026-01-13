using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class BundlePurchase : Entity
    {
        public long TouristId { get; private set; }
        public long BundleId { get; private set; }
        public decimal Price { get; private set; }
        public DateTime PurchasedAt { get; private set; }
        public BundlePurchase() { }
        public BundlePurchase(long touristId, long bundleId, decimal price)
        {
            if (price <= 0) throw new ArgumentException("Invalid price");

            TouristId = touristId;
            BundleId = bundleId;
            Price = price;
            PurchasedAt = DateTime.UtcNow;
        }
    }
}
