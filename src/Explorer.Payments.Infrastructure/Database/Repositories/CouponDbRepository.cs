using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponDbRepository : ICouponRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<Coupon> _dbSet;

        public CouponDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Coupon>();
        }

        public Coupon Get(long id)
        {
            var coupon = _dbSet.FirstOrDefault(c => c.Id == id);
            if (coupon == null) throw new NotFoundException($"Coupon with id {id} not found.");
            return coupon;
        }

        public Coupon? GetByCode(string code)
        {
            return _dbSet.FirstOrDefault(c => c.Code == code);
        }

        public List<Coupon> GetByAuthor(long authorId)
        {
            return _dbSet
                .Where(c => c.AuthorId == authorId)
                .OrderByDescending(c => c.Id)
                .ToList();
        }

        public Coupon Create(Coupon coupon)
        {
            _dbSet.Add(coupon);
            _dbContext.SaveChanges();
            return Get(coupon.Id);
        }

        public Coupon Update(Coupon coupon)
        {
            _dbContext.ChangeTracker.Clear();
            _dbSet.Update(coupon);
            _dbContext.SaveChanges();
            return Get(coupon.Id);
        }

        public void Delete(long id)
        {
            var coupon = _dbSet.FirstOrDefault(c => c.Id == id);
            if (coupon == null) throw new NotFoundException($"Coupon with id {id} not found.");

            _dbSet.Remove(coupon);
            _dbContext.SaveChanges();
        }
    }
}
