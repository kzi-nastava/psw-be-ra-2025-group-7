using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourReviewDbRepository : ITourReviewRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourReview> _dbSet;

    public TourReviewDbRepository(ToursContext context)
    {
        DbContext = context;
        _dbSet = DbContext.Set<TourReview>();
    }

    public TourReview Get(long id)
    {
        var entity = _dbSet
            .Include(tr => tr.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Include(tr => tr.TourExecution)
            .FirstOrDefault(tr => tr.Id == id);

        if (entity == null)
            throw new NotFoundException("Tour review not found: " + id);

        return entity;
    }

    public TourReview? GetByTouristAndTour(long touristId, long tourId)
    {
        return _dbSet
            .Include(tr => tr.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Include(tr => tr.TourExecution)
            .FirstOrDefault(tr => tr.TouristId == touristId && tr.TourId == tourId);
    }

    public PagedResult<TourReview> GetByTourId(long tourId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(tr => tr.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Include(tr => tr.TourExecution)
            .Where(tr => tr.TourId == tourId)
            .OrderByDescending(tr => tr.CreatedAt);

        var totalCount = query.Count();
        var results = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<TourReview>(results, totalCount);
    }

    public PagedResult<TourReview> GetByTouristId(long touristId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(tr => tr.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Include(tr => tr.TourExecution)
            .Where(tr => tr.TouristId == touristId)
            .OrderByDescending(tr => tr.CreatedAt);

        var totalCount = query.Count();
        var results = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<TourReview>(results, totalCount);
    }

    public TourReview Create(TourReview review)
    {
        _dbSet.Add(review);
        DbContext.SaveChanges();
        return review;
    }

    public TourReview Update(TourReview review)
    {
        try
        {
            var existingReview = _dbSet
                .Include(tr => tr.Tour)
                .ThenInclude(t => t.KeyPoints)
                .Include(tr => tr.TourExecution)
                .FirstOrDefault(tr => tr.Id == review.Id);

            if (existingReview == null)
                throw new NotFoundException($"Tour review with id {review.Id} not found.");

            // Ažuriraj svojstva
            DbContext.Entry(existingReview).CurrentValues.SetValues(review);

            // Ažuriraj _imageUrls kolekciju
            DbContext.Entry(existingReview).Property("_imageUrls")
                .CurrentValue = review.ImageUrls.ToList();

            DbContext.SaveChanges();

            // Ponovo učitaj sa svim Include-ovima
            DbContext.Entry(existingReview).State = EntityState.Detached;
            return Get(review.Id);
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException($"An error occurred while updating: {e.InnerException?.Message ?? e.Message}");
        }
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public List<TourReview> GetAll()
    {
        return _dbSet
            .Include(tr => tr.Tour)
            .ThenInclude(t => t.KeyPoints)
            .Include(tr => tr.TourExecution)
            .ToList();
    }

    public double GetAverageRatingForTour(long tourId)
    {
        var reviews = _dbSet.Where(tr => tr.TourId == tourId).ToList();
        
        if (!reviews.Any())
            return 0;

        return reviews.Average(tr => tr.Rating);
    }
}
