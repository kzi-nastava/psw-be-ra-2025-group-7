using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ToursContext _dbContext;

        public NotificationRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Notification Create(Notification notification)
        {
            _dbContext.Notifications.Add(notification);
            _dbContext.SaveChanges();
            return notification;
        }

        public List<Notification> GetForUser(long userId, bool onlyUnread)
        {
            var query = _dbContext.Notifications
                .Where(n => n.UserId == userId);

            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            return query
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }


        public void MarkAsRead(int id, long userId)
        {
            var notif = _dbContext.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notif == null) return;

            notif.MarkAsRead();
            _dbContext.SaveChanges();
        }
    }
}
