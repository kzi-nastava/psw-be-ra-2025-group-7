using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface IWalletInternalService
    {
        void CreateWallet(long userId);
        decimal GetBalance(long userId);
    }
}
