namespace Explorer.Payments.API.Dtos
{
    public class PurchaseNotificationDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
