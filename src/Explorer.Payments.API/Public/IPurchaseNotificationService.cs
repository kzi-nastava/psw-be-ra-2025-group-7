using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IPurchaseNotificationService
    {
        List<PurchaseNotificationDto> GetAll(long touristId);
        List<PurchaseNotificationDto> GetUnread(long touristId);
        void MarkAsRead(long id, long touristId);

        // Ovo zove ShoppingCartService nakon uspešne kupovine
        void NotifyPurchaseSuccess(long touristId, IReadOnlyList<string> tourNames);
    }
}
