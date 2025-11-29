using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Core.UseCases;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TourPreferencesDbRepository : ITourPreferencesRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<TourPreferences> _dbSet;

        TourPreferencesDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourPreferences>(); ;
        }

        public TourPreferences Create(TourPreferences entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            _dbSet.Remove(Get(id));
            DbContext.SaveChanges();
        }

        public TourPreferences Get(long id)
        {
            return _dbSet.Find(id) ?? throw new NotFoundException("Not found: " + id);
        }

        public TourPreferences GetByTouristId(long TouristId) 
        {
            return _dbSet.FirstOrDefault(pref => pref.TouristId == TouristId);

        }

        public TourPreferences Update(TourPreferences entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }

    }
}
