using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class FollowerDbRepository : IFollowerRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<Follower> _dbSet;

    public FollowerDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Follower>();
    }

    public Follower Create(Follower follower)
    {
        _dbSet.Add(follower);
        _dbContext.SaveChanges();
        return follower;
    }

    public void Delete(long followerId, long followedId)
    {
        var follower = _dbSet.FirstOrDefault(f => f.FollowerId == followerId && f.FollowedId == followedId);
        if (follower == null)
            throw new NotFoundException($"Follower relationship not found");

        _dbSet.Remove(follower);
        _dbContext.SaveChanges();
    }

    public Follower? Get(long followerId, long followedId)
    {
        return _dbSet.FirstOrDefault(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public bool Exists(long followerId, long followedId)
    {
        return _dbSet.Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public PagedResult<Follower> GetFollowers(long followedId, int page, int pageSize)
    {
        var query = _dbSet.Where(f => f.FollowedId == followedId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public PagedResult<Follower> GetFollowing(long followerId, int page, int pageSize)
    {
        var query = _dbSet.Where(f => f.FollowerId == followerId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public List<long> GetFollowerIds(long followedId)
    {
        return _dbSet
            .Where(f => f.FollowedId == followedId)
            .Select(f => f.FollowerId)
            .ToList();
    }
}
