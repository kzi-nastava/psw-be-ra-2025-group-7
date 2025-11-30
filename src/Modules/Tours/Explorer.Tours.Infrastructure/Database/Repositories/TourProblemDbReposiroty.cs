
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;


namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourProblemDbRepository : ITourProblemRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourProblem> _dbSet;

        public TourProblemDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TourProblem>();
        }

        public PagedResult<TourProblem> GetByTourist(int touristId, int page, int pageSize)
        {
            var query = _dbSet.Where(tp => tp.TouristId == touristId).OrderBy(tp => tp.Id);
            var totalCount = query.Count();
            var items = query.Skip(page * pageSize).Take(pageSize).ToList();
            return new PagedResult<TourProblem>(items, totalCount);
        }



        public TourProblem Get(int id)
        {
            var entity = _dbSet.AsNoTracking().FirstOrDefault(e => e.Id == id);
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            return entity;
        }
        public TourProblem Create(TourProblem entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }
        public TourProblem Update(TourProblem entity)
        {
            var local = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
            {
                _dbContext.Entry(local).State = EntityState.Detached;
            }
            _dbContext.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new KeyNotFoundException(e.Message);
            }
            return entity;
        }
        public void Delete(int id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public List<TourProblem> GetAll()
        {
         return _dbSet.AsNoTracking().ToList();
        }
    }
}
