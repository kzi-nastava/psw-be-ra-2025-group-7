namespace Explorer.Payments.API.Dtos
{
    public class OrderItemDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
