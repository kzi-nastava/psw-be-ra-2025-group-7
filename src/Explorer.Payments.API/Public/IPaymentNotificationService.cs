using Explorer.Payments.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public
{
    public interface IPaymentNotificationService
    {
        void Create(long userId, string message);
        List<PaymentNotificationDto> GetUnread(long userId);
        List<PaymentNotificationDto> GetAll(long userId);
        void MarkAsRead(long notificationId, long userId);
    }
}
