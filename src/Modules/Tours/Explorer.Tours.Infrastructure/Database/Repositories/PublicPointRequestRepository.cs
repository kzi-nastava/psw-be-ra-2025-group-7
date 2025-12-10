using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class PublicPointRequestRepository : IPublicPointRequestRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<PublicPointRequest> _dbSet;

        public PublicPointRequestRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<PublicPointRequest>();
        }

        public PublicPointRequest Create(PublicPointRequest entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        // Ako vam kasnije zatreba:
        // public PublicPointRequest Get(long id) { ... }
        // public void Delete(long id) { ... }
        // itd.
    }
}
