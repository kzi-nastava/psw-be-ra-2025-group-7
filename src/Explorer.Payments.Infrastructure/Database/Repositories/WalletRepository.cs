using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly PaymentsContext _context;

        public WalletRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Wallet GetByUserId(long userId)
        {
            return _context.Wallets
                .SingleOrDefault(w => w.UserId == userId)
                ?? throw new KeyNotFoundException("Wallet not found.");
        }

        public Wallet? GetByUserSolanaAddress(string solanaAddress)
        {
            if (string.IsNullOrWhiteSpace(solanaAddress))
                return null;

            return _context.Wallets
                .FirstOrDefault(w => w.SolanaWalletAddress == solanaAddress);
        }

        public bool ExistsForUser(long userId)
        {
            return _context.Wallets.Any(w => w.UserId == userId);
        }

        public Wallet Create(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet;
        }

        public void Update(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}
