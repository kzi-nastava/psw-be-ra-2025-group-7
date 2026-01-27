using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAccountRepository
    {
        Account Create(Account account);

        IEnumerable<Account> GetAll();

        Account? Get(int id);

        void Update(Account account);
    }
}
