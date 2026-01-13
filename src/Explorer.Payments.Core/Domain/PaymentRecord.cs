using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class PaymentRecord : Entity
    {
        public long TouristId { get; private set; }
        public long? TourId { get; private set; }
        public long? BundleId { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal FinalPrice { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public string? CouponCode { get; private set; }

        private PaymentRecord() { }

        public PaymentRecord(long touristId, long? tourId, long? bundleId, decimal originalPrice, decimal discountPercentage, string? couponCode)
        {
            TouristId = touristId;
            TourId = tourId;
            BundleId = bundleId;
            OriginalPrice = originalPrice;
            DiscountPercentage = discountPercentage;
            CouponCode = couponCode;
            PurchaseDate = DateTime.UtcNow;

            FinalPrice = CalculateFinalPrice();

            Validate();
        }

        private void Validate()
        {
            if (TouristId == 0)
                throw new ArgumentException("Tourist ID must be valid.");

            if (!TourId.HasValue && !BundleId.HasValue)
                throw new ArgumentException("Either TourId or BundleId must be specified.");

            if (OriginalPrice < 0)
                throw new ArgumentException("Original price cannot be negative.");

            if (DiscountPercentage < 0 || DiscountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100.");
        }

        private decimal CalculateFinalPrice()
        {
            return OriginalPrice * (1 - DiscountPercentage / 100m);
        }
    }
}
