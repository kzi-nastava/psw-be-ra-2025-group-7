namespace Explorer.Payments.API.Dtos
{
    public class CouponDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public long AuthorId { get; set; }
        public long? TourId { get; set; }
        public bool IsActive { get; set; }
    }
}
