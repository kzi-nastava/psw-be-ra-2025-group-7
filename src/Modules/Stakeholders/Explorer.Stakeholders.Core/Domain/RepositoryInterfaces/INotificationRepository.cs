using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        List<Notification> GetForUser(long userId, bool onlyUnread = false);
        int GetUnreadCount(long userId);
        Notification Get(long id);       
        void Update(Notification notification);
        void MarkAllAsRead(long userId);
        void Delete(long id);
    }
}
