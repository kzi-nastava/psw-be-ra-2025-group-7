using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourPurchaseToken> _dbSet;

    public TourPurchaseTokenDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourPurchaseToken>();
    }

    public TourPurchaseToken Create(TourPurchaseToken token)
    {
        _dbSet.Add(token);
        DbContext.SaveChanges();
        return token;
    }

    public TourPurchaseToken? GetByUserAndTour(long userId, long tourId)
    {
        return _dbSet
            .Include(t => t.Tour)
            .FirstOrDefault(t => t.UserId == userId && t.TourId == tourId);
    }

    public PagedResult<TourPurchaseToken> GetPagedByUser(int page, int pageSize, long userId)
    {
        var query = _dbSet
            .Include(t => t.Tour)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.PurchaseDate);

        var task = query.GetPaged(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public bool HasUserPurchasedTour(long userId, long tourId)
    {
        return _dbSet.Any(t => t.UserId == userId && t.TourId == tourId);
    }
}
