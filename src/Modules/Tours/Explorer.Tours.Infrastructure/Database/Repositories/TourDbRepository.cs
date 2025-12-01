using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public PagedResult<Tour> GetPagedByAuthor(int page, int pageSize, long authorId)
    {
        var task = _dbSet
            .Where(t => t.AuthorId == authorId)
            .GetPagedById(page, pageSize);

        task.Wait();
        return task.Result;
    }

    public PagedResult<Tour> GetPublishedTours(int page, int pageSize)
    {
        var task = _dbSet
            .Where(t => t.Status == TourStatus.Published)
            .GetPagedById(page, pageSize);

        task.Wait();
        return task.Result;
    }

    public Tour Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public Tour Create(Tour entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Tour Update(Tour entity)
    {
        try
        {
            var existingEntity = _dbSet.Find(entity.Id);

            if (existingEntity == null)
                throw new NotFoundException($"Tour with id {entity.Id} not found.");

            DbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            DbContext.SaveChanges();

            return existingEntity;
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