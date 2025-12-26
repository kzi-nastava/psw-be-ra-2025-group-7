using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Stakeholders.Infrastructure.Database;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserProfileDbRepository : IUserProfileRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<UserProfile> _dbSet;

    public UserProfileDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<UserProfile>();
    }

    public UserProfile? GetByUserId(long userId)
    {
        return _dbSet.FirstOrDefault(up => up.UserId == userId);
    }

    public UserProfile GetById(long userId)
    {
        return _dbSet.FirstOrDefault(up => up.UserId == userId);
    }

    public UserProfile Create(UserProfile profile)
    {
        _dbSet.Add(profile);
        _dbContext.SaveChanges();
        return profile;
    }

    public UserProfile Update(UserProfile profile)
    {
        _dbContext.Update(profile);
        _dbContext.SaveChanges();
        return profile;
    }
}