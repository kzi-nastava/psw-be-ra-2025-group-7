namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User Create(User user);
    long GetPersonId(long userId);
    User? GetById(long userId);
    List<User> SearchByUsername(string searchTerm);
    List<User> GetAllActiveUsers();
}