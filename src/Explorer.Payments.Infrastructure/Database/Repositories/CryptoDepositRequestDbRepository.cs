using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CryptoDepositRequestDbRepository : ICryptoDepositRequestRepository
    {
        private readonly PaymentsContext _context;

        public CryptoDepositRequestDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public CryptoDepositRequest Create(CryptoDepositRequest request)
        {
            _context.CryptoDepositRequests.Add(request);
            _context.SaveChanges();
            return request;
        }

        public CryptoDepositRequest? GetById(long id)
        {
            return _context.CryptoDepositRequests.FirstOrDefault(r => r.Id == id);
        }

        public CryptoDepositRequest? GetByTransactionId(string transactionId)
        {
            return _context.CryptoDepositRequests.FirstOrDefault(r => r.TransactionId == transactionId);
        }

        public List<CryptoDepositRequest> GetPendingDeposits()
        {
            return _context.CryptoDepositRequests
                .Where(r => r.Status == CryptoDepositStatus.Pending)
                .OrderBy(r => r.RequestedAt)
                .ToList();
        }

        public List<CryptoDepositRequest> GetByUserId(long userId)
        {
            return _context.CryptoDepositRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestedAt)
                .ToList();
        }

        public void Update(CryptoDepositRequest request)
        {
            _context.CryptoDepositRequests.Update(request);
            _context.SaveChanges();
        }
    }
}
