using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IPurchaseNotificationRepository
    {
        PurchaseNotification Create(PurchaseNotification notification);
        PurchaseNotification? Get(long id);

        List<PurchaseNotification> GetAllByTouristId(long touristId);
        List<PurchaseNotification> GetUnreadByTouristId(long touristId);

        PurchaseNotification Update(PurchaseNotification notification);
    }
}
