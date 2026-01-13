using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class BundlePurchaseDbRepository : IBundlePurchaseRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<BundlePurchase> _dbSet;

        public BundlePurchaseDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<BundlePurchase>();
        }

        public BundlePurchase Create(BundlePurchase purchase)
        {
            _dbSet.Add(purchase);
            _dbContext.SaveChanges();
            return purchase;
        }

        public bool HasUserPurchasedBundle(long touristId, long bundleId)
        {
            return _dbSet.Any(p =>
                p.TouristId == touristId &&
                p.BundleId == bundleId);
        }
    }
}
