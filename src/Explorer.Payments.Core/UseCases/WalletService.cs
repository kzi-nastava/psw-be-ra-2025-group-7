using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
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

        public void AddFunds(long touristUserId, decimal amount)
        {
            var wallet = _walletRepository.GetByUserId(touristUserId);
            wallet.AddFunds(amount);
            _walletRepository.Update(wallet);
        }

        public WalletDto GetWallet(long userId)
        {
            var wallet = _walletRepository.GetByUserId(userId);
            return new WalletDto(wallet.Balance);
        }
    }
}
