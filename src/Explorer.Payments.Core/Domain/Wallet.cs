using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class Wallet : Entity
    {
        //public long Id { get; private set; }
        public long UserId { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? SolanaWalletAddress { get; private set; }

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

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");

            Balance -= amount;
        }

        public void RegisterSolanaAddress(string solanaAddress)
        {
            if (string.IsNullOrWhiteSpace(solanaAddress))
                throw new ArgumentException("Solana address cannot be empty.", nameof(solanaAddress));

            // Basic validation: Solana addresses are base58 encoded, typically 32-44 characters
            if (solanaAddress.Length < 32 || solanaAddress.Length > 44)
                throw new ArgumentException("Invalid Solana address format.", nameof(solanaAddress));

            SolanaWalletAddress = solanaAddress;
        }
    }
}
