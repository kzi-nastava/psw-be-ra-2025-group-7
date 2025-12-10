using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        Club Create(Club club);
        Club Update(Club club);
        void Delete(long id);
        List<Club> GetAll();
        Club Get(long id);
        List<Club> GetByOwner(long ownerId);

    }
}
