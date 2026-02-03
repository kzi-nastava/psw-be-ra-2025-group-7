using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases.Internal;

public class PersonNameInternalService : IPersonNameInternalService
{
    private readonly IPersonRepository _personRepository;

    public PersonNameInternalService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public string GetFullName(long personId)
    {
        var person = _personRepository.Get(personId);
        return $"{person.Name} {person.Surname}";
    }
}
