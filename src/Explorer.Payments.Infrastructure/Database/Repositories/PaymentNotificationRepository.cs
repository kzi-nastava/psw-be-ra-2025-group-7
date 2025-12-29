using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentNotificationRepository : IPaymentNotificationRepository
    {
        private readonly PaymentsContext _context;

        public PaymentNotificationRepository(PaymentsContext context)
        {
            _context = context;
        }

        public void Create(PaymentNotification notification)
        {
            _context.PaymentNotifications.Add(notification);
            _context.SaveChanges();
        }

        public PaymentNotification Get(long id)
        {
            return _context.PaymentNotifications
                .FirstOrDefault(n => n.Id == id);
        }

        public void Update(PaymentNotification notification)
        {
            _context.PaymentNotifications.Update(notification);
            _context.SaveChanges();
        }

        public List<PaymentNotification> GetUnreadByUser(long userId)
        {
            return _context.PaymentNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public List<PaymentNotification> GetAllByUser(long userId)
        {
            return _context.PaymentNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}
