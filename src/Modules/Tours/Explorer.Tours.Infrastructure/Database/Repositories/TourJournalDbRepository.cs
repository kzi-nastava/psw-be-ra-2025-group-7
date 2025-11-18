using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourJournalDbRepository : ITourJournalRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourJournal> _dbSet;

    public TourJournalDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourJournal>();
    }

    public PagedResult<TourJournal> GetPagedByTourist(long touristId, int page, int pageSize)
    {
        var query = _dbSet.Where(tj => tj.TouristId == touristId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public TourJournal Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Tour journal not found: " + id);
        return entity;
    }

    public TourJournal Create(TourJournal entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public TourJournal Update(TourJournal entity)
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

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
