using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly StakeholdersContext _dbContext;

    public UserDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Exists(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }

    public User? GetActiveByName(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username && user.IsActive);
    }

    public User Create(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public long GetPersonId(long userId)
    {
        var person = _dbContext.People.FirstOrDefault(i => i.UserId == userId);
        if (person == null) throw new KeyNotFoundException("Not found.");
        return person.Id;
    }

    public User? GetById(long userId)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == userId);
    }

    public List<User> SearchByUsername(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<User>();

        return _dbContext.Users
            .Where(user => user.IsActive && user.Username.Contains(searchTerm))
            .OrderBy(user => user.Username)
            .Take(20)
            .ToList();
    }

    public List<User> GetAllActiveUsers()
    {
        return _dbContext.Users
            .Where(user => user.IsActive)
            .OrderBy(user => user.Username)
            .ToList();
    }
}