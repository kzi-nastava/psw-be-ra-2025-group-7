using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Core.UseCases;
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

        public PublicPointRequest Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException($"PublicPointRequest with id {id} not found.");
            return entity;
        }

        public PublicPointRequest Update(PublicPointRequest entity)
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public PagedResult<PublicPointRequest> GetPaged(int page, int pageSize)
        {
            var query = _dbSet.OrderByDescending(r => r.CreatedAt);

            var totalCount = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<PublicPointRequest>(items, totalCount);
        }

        public List<PublicPointRequest> GetPendingRequests()
        {
            return _dbSet
                .Where(r => r.Status == PublicPointRequestStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .ToList();
        }
    }
}