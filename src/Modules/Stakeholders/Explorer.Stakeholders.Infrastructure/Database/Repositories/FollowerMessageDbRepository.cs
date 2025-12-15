using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class FollowerMessageDbRepository : IFollowerMessageRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<FollowerMessage> _dbSet;

    public FollowerMessageDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<FollowerMessage>();
    }

    public FollowerMessage Create(FollowerMessage message)
    {
        _dbSet.Add(message);
        _dbContext.SaveChanges();
        return message;
    }

    public FollowerMessage Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null)
            throw new NotFoundException($"FollowerMessage with id {id} not found");
        return entity;
    }

    public PagedResult<FollowerMessage> GetByAuthor(long authorId, int page, int pageSize)
    {
        var query = _dbSet.Where(m => m.AuthorId == authorId).OrderByDescending(m => m.CreatedAt);
        var task = query.GetPaged(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }
}
