using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentNotificationService : IPaymentNotificationService
    {
        private readonly IPaymentNotificationRepository _repository;

        public PaymentNotificationService(IPaymentNotificationRepository repository)
        {
            _repository = repository;
        }

        public void Create(long userId, string message)
        {
            var notification = new PaymentNotification(userId, message);
            _repository.Create(notification);
        }

        public List<PaymentNotificationDto> GetUnread(long userId)
        {
            return _repository.GetUnreadByUser(userId)
                .Select(n => new PaymentNotificationDto(
                    n.Id,
                    n.Content,
                    n.CreatedAt,
                    n.IsRead))
                .ToList();
        }

        public List<PaymentNotificationDto> GetAll(long userId)
        {
            return _repository.GetAllByUser(userId)
                .Select(n => new PaymentNotificationDto(
                    n.Content,
                    n.CreatedAt,
                    n.IsRead))
                .ToList();
        }

        public void MarkAsRead(long notificationId, long userId)
        {
            var notification = _repository.Get(notificationId);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            notification.MarkAsRead();
            _repository.Update(notification);
        }
    }
}
