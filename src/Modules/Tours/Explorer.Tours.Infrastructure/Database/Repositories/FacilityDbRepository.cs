using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class FacilityDbRepository : IFacilityRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<Facility> _dbSet;

        public FacilityDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Facility>();
        }

        public PagedResult<Facility> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Facility? GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public Facility Create(Facility facility)
        {
            _dbSet.Add(facility);
            _dbContext.SaveChanges();
            return facility;
        }

        public Facility Update(Facility facility)
        {
            try
            {
                _dbContext.Update(facility);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }

            return facility;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            if (entity == null)
                throw new NotFoundException("Facility not found: " + id);

            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
