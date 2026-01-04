using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PurchaseNotificationRepository : IPurchaseNotificationRepository
    {
        private readonly PaymentsContext _context;

        public PurchaseNotificationRepository(PaymentsContext context)
        {
            _context = context;
        }

        public PurchaseNotification Create(PurchaseNotification notification)
        {
            _context.PurchaseNotifications.Add(notification);
            _context.SaveChanges();
            return notification;
        }

        public PurchaseNotification? Get(long id)
        {
            return _context.PurchaseNotifications.FirstOrDefault(n => n.Id == id);
        }

        public List<PurchaseNotification> GetAllByTouristId(long touristId)
        {
            return _context.PurchaseNotifications
                .Where(n => n.TouristId == touristId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public List<PurchaseNotification> GetUnreadByTouristId(long touristId)
        {
            return _context.PurchaseNotifications
                .Where(n => n.TouristId == touristId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public PurchaseNotification Update(PurchaseNotification notification)
        {
            _context.PurchaseNotifications.Update(notification);
            _context.SaveChanges();
            return notification;
        }
    }
}
