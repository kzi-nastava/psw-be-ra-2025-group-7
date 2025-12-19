using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionDbRepository : ITourExecutionRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourExecution> _dbSet;
    
    public TourExecutionDbRepository(ToursContext context)
    {
        DbContext = context;
        _dbSet = DbContext.Set<TourExecution>();
    }

    public TourExecution? GetActiveExecutionForTourist(long touristId, long tourId)
    {
        return _dbSet
            .Include(te => te.Tour)
            .ThenInclude(t => t.KeyPoints)
            .FirstOrDefault(te => te.TouristId == touristId 
                                  && te.TourId == tourId 
                                  && te.Status == TourExecutionStatus.Active);
    }

    public PagedResult<TourExecution> GetByTouristId(long touristId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(te => te.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Where(te => te.TouristId == touristId)
            .OrderByDescending(te => te.StartedAt);

        var totalCount = query.Count();
        var results = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<TourExecution>(results, totalCount);
    }

    public List<TourExecution> GetAllActiveExecutionsByTourist(long touristId)
    {
        return _dbSet
            .Include(te => te.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Where(te => te.TouristId == touristId && te.Status == TourExecutionStatus.Active)
            .ToList();
    }

    public TourExecution Get(long id)
    {
        var entity = _dbSet
            .Include(te => te.Tour)
            .ThenInclude(t => t.KeyPoints)
            .FirstOrDefault(te => te.Id == id);
        
        if (entity == null) 
            throw new NotFoundException("Tour execution not found: " + id);
        
        return entity;
    }

    public TourExecution Create(TourExecution execution)
    {
        _dbSet.Add(execution);
        DbContext.SaveChanges();
        return execution;
    }

    public TourExecution Update(TourExecution execution)
    {
        try
        {
            var existingExecution = _dbSet
                .Include(te => te.Tour)
                .ThenInclude(t => t.KeyPoints)
                .FirstOrDefault(te => te.Id == execution.Id);

            if (existingExecution == null)
                throw new NotFoundException($"Tour execution with id {execution.Id} not found.");

            // Ažuriraj svojstva
            DbContext.Entry(existingExecution).CurrentValues.SetValues(execution);
            
            // Ažuriraj _unlockedKeyPointIndices kolekciju
            DbContext.Entry(existingExecution).Property("_unlockedKeyPointIndices")
                .CurrentValue = execution.UnlockedKeyPointIndices.ToList();

            // Ažuriraj _keyPointUnlockTimes dictionary
            DbContext.Entry(existingExecution).Property("_keyPointUnlockTimes")
                .CurrentValue = new Dictionary<int, DateTime>(execution.KeyPointUnlockTimes);

            DbContext.SaveChanges();

            // Ponovo učitaj sa svim Include-ovima
            DbContext.Entry(existingExecution).State = EntityState.Detached;
            return Get(execution.Id);
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException($"An error occurred while updating: {e.InnerException?.Message ?? e.Message}");
        }
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
