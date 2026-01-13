using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourReviewRepository
{
    TourReview Get(long id);
    TourReview? GetByTouristAndTour(long touristId, long tourId);
    PagedResult<TourReview> GetByTourId(long tourId, int page, int pageSize);
    PagedResult<TourReview> GetByTouristId(long touristId, int page, int pageSize);
    TourReview Create(TourReview review);
    TourReview Update(TourReview review);
    void Delete(long id);
    List<TourReview> GetAll();
    double GetAverageRatingForTour(long tourId);
}
