using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Services
{
    public interface IAccountService
    {
        
        AccountDto CreateAccount(AccountCreateDto request);

        
        IEnumerable<AccountDto> GetAllAccounts();

        
        AccountDto BlockAccount(int accountId);
    }
}
