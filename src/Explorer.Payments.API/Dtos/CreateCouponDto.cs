namespace Explorer.Payments.API.Dtos
{
    public class CreateCouponDto
    {
        public int DiscountPercentage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public long? TourId { get; set; }
    }
}
