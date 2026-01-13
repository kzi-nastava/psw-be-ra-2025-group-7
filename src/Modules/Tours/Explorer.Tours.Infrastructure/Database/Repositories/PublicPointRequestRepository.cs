using System.Linq;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

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

        public PublicPointRequest Update(PublicPointRequest entity)
        {
            _dbSet.Update(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public PublicPointRequest? Get(long id)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public PublicPointRequest? GetPending(long tourId, int keyPointIndex)
        {
            return _dbSet.AsNoTracking()
                .FirstOrDefault(x => x.TourId == tourId
                                  && x.KeyPointIndex == keyPointIndex
                                  && x.Status == PublicPointRequestStatus.Pending);
        }

        public PagedResult<PublicPointRequest> GetPaged(int page, int pageSize, PublicPointRequestStatus? status = null)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var total = query.Count();

            var items = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<PublicPointRequest>(items, total);
        }

        public PagedResult<PublicPointRequest> GetByAuthor(long authorId, int page, int pageSize)
        {
            var query = _dbSet.AsNoTracking()
                .Where(x => x.AuthorId == authorId);

            var total = query.Count();

            var items = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<PublicPointRequest>(items, total);
        }
    }
}
