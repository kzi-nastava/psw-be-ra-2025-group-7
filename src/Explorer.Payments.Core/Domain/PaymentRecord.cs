using System;

namespace Explorer.Payments.Core.Domain
{
    public class PaymentRecord
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public long? TourId { get; private set; }
        public long? BundleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Description { get; private set; }

        protected PaymentRecord() { }

        public PaymentRecord(long userId, long? tourId, long? bundleId, decimal amount, DateTime createdAt, string? description)
        {
            UserId = userId;
            TourId = tourId;
            BundleId = bundleId;
            Amount = amount;
            CreatedAt = createdAt;
            Description = description;
        }
    }
}
