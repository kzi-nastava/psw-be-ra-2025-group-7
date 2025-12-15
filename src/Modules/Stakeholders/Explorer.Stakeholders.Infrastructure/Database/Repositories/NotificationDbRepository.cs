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

        public List<Notification> GetForTourist(long touristId)
        {
            return _dbSet
                .Where(n => n.UserId == touristId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
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
    }
}
