using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Internal
{
    public interface IPaymentInternalService
    {
        bool ChargeWallet(long userId, long? tourId, long? bundleId, decimal amount, string description);
    }
}
