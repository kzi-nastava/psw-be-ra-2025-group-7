using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class WalletDepositDto
    {
        public long TouristUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
