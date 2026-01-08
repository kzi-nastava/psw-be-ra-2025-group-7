using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Coupon : AggregateRoot
    {
        public string Code { get; private set; }
        public int DiscountPercentage { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public long AuthorId { get; private set; }
        public long? TourId { get; private set; }
        public bool IsActive { get; private set; }

        private Coupon() { }

        public Coupon(long authorId, int discountPercentage, DateTime? expirationDate, long? tourId)
        {
            AuthorId = authorId;
            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            TourId = tourId;
            IsActive = true;
            Code = GenerateRandomCode();

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code) || Code.Length != 8)
                throw new ArgumentException("Coupon code must be exactly 8 characters.");

            if (DiscountPercentage <= 0 || DiscountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 1 and 100.");

            if (AuthorId == 0)
                throw new ArgumentException("Author ID must be valid.");
        }

        public void Update(int discountPercentage, DateTime? expirationDate, long? tourId)
        {
            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            TourId = tourId;

            Validate();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public bool IsValid()
        {
            if (!IsActive)
                return false;

            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow)
                return false;

            return true;
        }

        public bool AppliesTo(long tourId, long authorId)
        {
            if (AuthorId != authorId)
                return false;

            if (TourId.HasValue)
                return TourId.Value == tourId;

            return true;
        }

        public static string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
