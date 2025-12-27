using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class Wallet
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Wallet() { } 

        public Wallet(long userId)
        {
            UserId = userId;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddFunds(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Balance += amount;
        }
    }
}
