using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        List<NotificationDto> GetForUser(long userId, bool onlyUnread = false);
        int GetUnreadCount(long userId);
        void MarkAsRead(long notificationId, long userId); 
        void MarkAllAsRead(long userId);
        void Delete(long notificationId, long userId);
    }
}
