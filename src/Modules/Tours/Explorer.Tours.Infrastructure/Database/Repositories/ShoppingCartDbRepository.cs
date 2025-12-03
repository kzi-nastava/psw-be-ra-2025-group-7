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

            // 1. Ucitaj postojeci cart iz baze (BEZ AsNoTracking, jer želimo da EF Core prati izmene)
            var existingCart = _dbSet
                .Include(sc => sc.Items)
                .FirstOrDefault(sc => sc.Id == cart.Id);

            if (existingCart == null)
                throw new NotFoundException($"Shopping cart with id {cart.Id} not found.");

            // 2. Identifikuj stavke koje treba obrisati
            var existingItemIds = existingCart.Items.Select(i => i.Id).ToHashSet();
            var newItemIds = cart.Items.Where(i => i.Id != 0).Select(i => i.Id).ToHashSet();
            var itemIdsToDelete = existingItemIds.Except(newItemIds).ToList();

            // 3. Eksplicitno obrisi stavke iz DbContext PRE nego što ih ukloniš iz kolekcije
            if (itemIdsToDelete.Any())
            {
                var itemsToRemove = existingCart.Items.Where(i => itemIdsToDelete.Contains(i.Id)).ToList();
                foreach (var item in itemsToRemove)
                {
                    _dbContext.Remove(item);  // Eksplicitno oznaci za brisanje
                    existingCart.Items.Remove(item);
                }
            }

            // 4. Dodaj nove stavke (one sa Id == 0)
            var newItems = cart.Items.Where(i => i.Id == 0).ToList();
            foreach (var newItem in newItems)
            {
                existingCart.Items.Add(newItem);
            }

            // 5. Rekalkuši TotalPrice pozivom domain metode
            existingCart.UpdateTotalPrice();

            // 6. Sa?uvaj izmene
            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();

            // 7. Vrati osveženi cart iz baze
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
