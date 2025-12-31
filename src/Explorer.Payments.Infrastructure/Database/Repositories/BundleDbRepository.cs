using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class BundleDbRepository : IBundleRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<Bundle> _dbSet;

        public BundleDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Bundle>();
        }

        public Bundle Get(long id)
        {
            var bundle = _dbSet.Include(b => b.Items).FirstOrDefault(b => b.Id == id);
            if (bundle == null) throw new NotFoundException($"Bundle with id {id} not found.");
            return bundle;
        }

        public List<Bundle> GetByAuthor(long authorId)
        {
            return _dbSet
                .Include(b => b.Items)
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }

        public Bundle Create(Bundle bundle)
        {
            _dbSet.Add(bundle);
            _dbContext.SaveChanges();
            return Get(bundle.Id);
        }

        public Bundle Update(Bundle detachedBundle)
        {
            // Bitno: detachedBundle dolazi iz servisa i nije tracked.
            _dbContext.ChangeTracker.Clear();

            // 1) Ucitaj postojeći bundle iz baze + items (tracked)
            var existing = _dbSet
                .Include(b => b.Items)
                .FirstOrDefault(b => b.Id == detachedBundle.Id);

            if (existing == null)
                throw new NotFoundException($"Bundle with id {detachedBundle.Id} not found.");

            // 2) Iz requesta izvuci željene tourId-eve
            var desiredTourIds = detachedBundle.Items
                .Select(i => i.TourId)
                .Distinct()
                .ToList();

            using var tx = _dbContext.Database.BeginTransaction();

            // 3) Obrisi SVE postojeće item-e iz baze (da ne udarimo duplikate / unique constraint)
            if (existing.Items != null && existing.Items.Any())
            {
                _dbContext.RemoveRange(existing.Items);
                existing.Items.Clear();
                _dbContext.SaveChanges();
            }


            // 4) Očisti tracker da izbegnemo "already being tracked" probleme sa istim key-evima
            _dbContext.ChangeTracker.Clear();

            // 5) Ponovo ucitaj bundle (sada bez starih items) i primeni domen update
            existing = _dbSet
                .Include(b => b.Items)
                .First(b => b.Id == detachedBundle.Id);

            existing.Update(detachedBundle.Name, detachedBundle.Price, desiredTourIds);

            _dbContext.SaveChanges();
            tx.Commit();

            _dbContext.ChangeTracker.Clear();

            // 6) Vrati osveženi bundle iz baze
            return _dbSet
                .Include(b => b.Items)
                .First(b => b.Id == detachedBundle.Id);
        }


        public void Delete(long id)
        {
            var bundle = _dbSet.Include(b => b.Items).FirstOrDefault(b => b.Id == id);
            if (bundle == null) throw new NotFoundException($"Bundle with id {id} not found.");

            _dbSet.Remove(bundle);
            _dbContext.SaveChanges();
        }

        public void Save(Bundle bundle)
        {
            _dbContext.ChangeTracker.Clear();

            _dbSet.Attach(bundle);
            _dbContext.Entry(bundle).Property(b => b.Status).IsModified = true;
            _dbContext.Entry(bundle).Property(b => b.UpdatedAt).IsModified = true;

            _dbContext.SaveChanges();
        }

        public List<Bundle> GetPublished()
        {
            return _dbSet
                .Include(b => b.Items)
                .Where(b => b.Status == BundleStatus.Published)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
        }
    }
}
