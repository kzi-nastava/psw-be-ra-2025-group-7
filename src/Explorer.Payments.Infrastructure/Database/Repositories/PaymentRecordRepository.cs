using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentRecordRepository : IPaymentRecordRepository
    {
        private readonly PaymentsContext _context;

        public PaymentRecordRepository(PaymentsContext context)
        {
            _context = context;
        }

        public PaymentRecord Create(PaymentRecord record)
        {
            var entity = _context.Add(record).Entity;
            _context.SaveChanges();
            return entity;
        }
    }
}
