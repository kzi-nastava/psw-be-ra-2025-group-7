
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;


namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourProblemDbRepository: ITourProblemRepository
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

            var task = query.GetPagedById(page, pageSize);
            task.Wait();

            return task.Result;
        }


        public TourProblem Get(int id)
        {
            var entity = _dbSet.Find(id);
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
            try
            {
                _dbContext.Update(entity);
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
    }
}
