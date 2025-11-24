using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class MonumentDbRepository : IMonumentRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Monument> _dbSet;

    public MonumentDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Monument>();
    }

    public PagedResult<Monument> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Monument Create(Monument entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Monument Update(Monument entity)
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
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
