using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletInternalService : IWalletInternalService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletInternalService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public void CreateWallet(long userId)
        {
            if (_walletRepository.ExistsForUser(userId))
                return;

            var wallet = new Wallet(userId);
            _walletRepository.Create(wallet);
        }

        public decimal GetBalance(long userId)
        {
            return _walletRepository.GetByUserId(userId).Balance;
        }
    }
}
