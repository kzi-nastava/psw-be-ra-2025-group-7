using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<ShoppingCart> _dbSet;

        public ShoppingCartDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ShoppingCart>();
        }

        public ShoppingCart GetByTouristId(long touristId)
        {
            return _dbSet
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.TouristId == touristId);
        }

        public ShoppingCart Create(ShoppingCart cart)
        {
            _dbSet.Add(cart);
            _dbContext.SaveChanges();
            return cart;
        }

        public ShoppingCart Update(ShoppingCart cart)
        {
            _dbContext.ChangeTracker.Clear();

            var existingCart = _dbSet
                .Include(sc => sc.Items)
                .AsNoTracking()
                .FirstOrDefault(sc => sc.Id == cart.Id);

            if (existingCart == null)
                throw new NotFoundException($"Shopping cart with id {cart.Id} not found.");

            var existingItemIds = existingCart.Items.Select(i => i.Id).ToHashSet();
            var newItemIds = cart.Items.Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();

            var itemIdsToDelete = existingItemIds.Except(newItemIds).ToList();
            
            if (itemIdsToDelete.Any())
            {
                _dbContext.Database.ExecuteSqlRaw(
                    "DELETE FROM tours.\"OrderItems\" WHERE \"Id\" = ANY(@p0)",
                    itemIdsToDelete.ToArray()
                );
            }

            _dbContext.Entry(cart).State = EntityState.Modified;
            _dbContext.Entry(cart).Property(c => c.Id).IsModified = false;

            foreach (var item in cart.Items)
            {
                if (item.Id == 0)
                {
                    _dbContext.Entry(item).State = EntityState.Added;
                }
                else if (newItemIds.Contains(item.Id) && existingItemIds.Contains(item.Id))
                {
                    _dbContext.Entry(item).State = EntityState.Modified;
                }
            }

            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();

            return _dbSet
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.Id == cart.Id);
        }

        public void Delete(long id)
        {
            var cart = _dbSet
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.Id == id);

            if (cart == null)
                throw new NotFoundException($"Shopping cart with id {id} not found.");

            _dbSet.Remove(cart);
            _dbContext.SaveChanges();
        }
    }
}
