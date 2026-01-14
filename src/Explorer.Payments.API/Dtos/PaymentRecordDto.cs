namespace Explorer.Payments.API.Dtos
{
    public class PaymentRecordDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public long? TourId { get; set; }
        public long? BundleId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string? CouponCode { get; set; }
    }
}
