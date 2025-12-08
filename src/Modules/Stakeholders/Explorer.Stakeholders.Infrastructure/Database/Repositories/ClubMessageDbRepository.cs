using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ClubMessageDbRepository : IClubMessageRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<ClubMessage> _dbSet;

    public ClubMessageDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<ClubMessage>();
    }

    public ClubMessage Create(ClubMessage message)
    {
        _dbSet.Add(message);
        _dbContext.SaveChanges();
        return message;
    }

    public ClubMessage Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null)
            throw new NotFoundException($"ClubMessage with id {id} not found");
        return entity;
    }

    public ClubMessage Update(ClubMessage message)
    {
        try
        {
            _dbContext.Update(message);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return message;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    public PagedResult<ClubMessage> GetByClub(long clubId, int page, int pageSize)
    {
        var query = _dbSet.Where(m => m.ClubId == clubId).OrderByDescending(m => m.CreatedAt);
        var task = query.GetPaged(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Club GetClub(long clubId)
    {
        var club = _dbContext.Clubs.Find(clubId);
        if (club == null)
            throw new NotFoundException($"Club with id {clubId} not found");
        return club;
    }
}
