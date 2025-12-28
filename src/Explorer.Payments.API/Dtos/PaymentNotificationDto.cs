using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class PaymentNotificationDto
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public PaymentNotificationDto(string message, DateTime createdAt, bool isRead)
        {
            Message = message;
            CreatedAt = createdAt;
            IsRead = isRead;
        }

        public PaymentNotificationDto(long id, string message, DateTime createdAt, bool isRead)
        {
            Id = id;
            Message = message;
            CreatedAt = createdAt;
            IsRead = isRead;
        }
    }
}
