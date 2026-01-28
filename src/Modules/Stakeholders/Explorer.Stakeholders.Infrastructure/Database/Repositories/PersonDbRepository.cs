using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : IPersonRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet;

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public Person Create(Person entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Person Get(long personId)
    {
        var person = _dbSet.AsNoTracking().FirstOrDefault(p => p.Id == personId);
        if (person == null) throw new NotFoundException($"Person with id {personId} not found.");
        return person;
    }

    public Person GetByUserId(long userId)   
    {
        var person = _dbSet.AsNoTracking().FirstOrDefault(p => p.UserId == userId);
        if (person == null)
            throw new NotFoundException($"Person not found for userId={userId}");

        return person;
    }
}