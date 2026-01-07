using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IWalletRepository
    {
        Wallet GetByUserId(long userId);
        bool ExistsForUser(long userId);
        Wallet Create(Wallet wallet);
        void Update(Wallet wallet);
    }
}
