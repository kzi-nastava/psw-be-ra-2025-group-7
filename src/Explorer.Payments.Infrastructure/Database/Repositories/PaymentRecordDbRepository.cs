using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentRecordDbRepository : IPaymentRecordRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<PaymentRecord> _dbSet;

        public PaymentRecordDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<PaymentRecord>();
        }

        public PaymentRecord Create(PaymentRecord record)
        {
            _dbSet.Add(record);
            _dbContext.SaveChanges();
            return record;
        }

        public List<PaymentRecord> GetByTourist(long touristId)
        {
            return _dbSet
                .Where(pr => pr.TouristId == touristId)
                .OrderByDescending(pr => pr.PurchaseDate)
                .ToList();
        }
    }
}
