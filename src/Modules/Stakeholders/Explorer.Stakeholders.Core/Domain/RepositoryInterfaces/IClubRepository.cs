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
        void Delete(Club club);
        List<Club> GetAll();

    }
}
