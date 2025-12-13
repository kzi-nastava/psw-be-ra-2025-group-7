using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class OrderItem : Entity
    {
        public long TourId { get; private set; }
        public string TourName { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }

        public OrderItem(long tourId, string tourName, decimal price)
        {
            if (tourId == 0)
                throw new ArgumentException("Tour ID cannot be zero.", nameof(tourId));

            if (string.IsNullOrWhiteSpace(tourName))
                throw new ArgumentException("Tour name is required.", nameof(tourName));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            TourId = tourId;
            TourName = tourName.Trim();
            Price = price;
        }
    }
}
