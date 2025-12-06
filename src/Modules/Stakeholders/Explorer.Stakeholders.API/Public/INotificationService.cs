using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        List<NotificationDto> GetForTourist(long touristId);
        void MarkAsRead(long notificationId, long touristId); 
    }
}
