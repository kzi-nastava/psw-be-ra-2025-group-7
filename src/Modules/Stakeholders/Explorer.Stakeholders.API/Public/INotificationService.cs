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
        
        // Notification generation for messaging system
        void CreateFollowerMessageNotifications(FollowerMessageDto message, List<long> followerIds);
        void CreateClubMessageNotifications(ClubMessageDto message, List<long> memberIds);
        void DeleteFollowerMessageNotifications(long followerMessageId);
        void DeleteClubMessageNotifications(long clubMessageId);
        
        // Unified notification methods
        List<UnifiedNotificationDto> GetAllNotificationsForUser(long userId, bool onlyUnread = false);
        int GetAllUnreadCount(long userId);
        void MarkNotificationAsRead(long notificationId, string source, long userId);
        void MarkAllNotificationsAsRead(long userId);
    }
}
