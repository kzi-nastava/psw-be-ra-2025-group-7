using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class NotificationDbRepository : INotificationRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<Notification> _dbSet;

        public NotificationDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Notification>();
        }

        public Notification Create(Notification notification)
        {
            var entity = _dbSet.Add(notification).Entity;
            _dbContext.SaveChanges();
            return entity;
        }

        public List<Notification> GetForUser(long userId, bool onlyUnread = false)
        {
            var query = _dbSet.Where(n => n.UserId == userId);
            
            if (onlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }
            
            return query.OrderByDescending(n => n.CreatedAt).ToList();
        }

        public int GetUnreadCount(long userId)
        {
            return _dbSet.Count(n => n.UserId == userId && !n.IsRead);
        }

        public Notification Get(long id)
        {
            return _dbSet.FirstOrDefault(n => n.Id == id);
        }

        public void Update(Notification notification)
        {
            _dbSet.Update(notification);
            _dbContext.SaveChanges();
        }

        public void MarkAllAsRead(long userId)
        {
            var unreadNotifications = _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            _dbContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var notification = _dbSet.FirstOrDefault(n => n.Id == id);
            if (notification != null)
            {
                _dbSet.Remove(notification);
                _dbContext.SaveChanges();
            }
        }
    }
}
