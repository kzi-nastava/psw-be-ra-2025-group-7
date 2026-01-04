namespace Explorer.Payments.Core.Domain
{
    public class PurchaseNotification
    {
        public long Id { get; private set; }
        public long TouristId { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF
        private PurchaseNotification() { }

        public PurchaseNotification(long touristId, string message)
        {
            TouristId = touristId;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
