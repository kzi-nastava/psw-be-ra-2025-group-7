using Explorer.Payments.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Public
{
    public interface IWalletService
    {
        void CreateWallet(long userId);
        decimal GetBalance(long userId);
        void AddFunds(long touristUserId, decimal amount);
        WalletDto GetWallet(long userId);
    }
}
