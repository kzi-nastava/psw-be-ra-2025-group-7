using System;
using System.Collections.Generic;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AccountDto CreateAccount(AccountCreateDto request)
        {
            var account = _mapper.Map<Account>(request);
            var created = _repository.Create(account);
            return _mapper.Map<AccountDto>(created);
        }

        public IEnumerable<AccountDto> GetAllAccounts()
        {
            var accounts = _repository.GetAll();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        //public AccountDto BlockAccount(int accountId)
        //{
        //    var account = _repository.Get(accountId)
        //                  ?? throw new ArgumentException($"Account with id {accountId} not found.");

            
        //    if (account.Role == "Author" || account.Role == "Tourist")
        //    {
        //        account.IsBlocked = true;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Only authors and tourists can be blocked.");
        //    }

        //    _repository.Update(account);
        //    return _mapper.Map<AccountDto>(account);
        //}

        public AccountDto BlockAccount(int accountId)
        {
            var account = _repository.Get(accountId)
                          ?? throw new ArgumentException($"Account with id {accountId} not found.");

            // Ne dozvoljavamo blokiranje admina, sve ostalo može
            if (account.Role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ||
                account.Role.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Administrators cannot be blocked.");
            }

            account.IsBlocked = true;

            _repository.Update(account);

            return _mapper.Map<AccountDto>(account);
        }
    }
}
