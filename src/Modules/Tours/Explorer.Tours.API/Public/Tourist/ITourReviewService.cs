using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourReviewService
{
    TourReviewDto CreateReview(long touristId, CreateTourReviewDto dto);
    TourReviewDto UpdateReview(long touristId, long reviewId, UpdateTourReviewDto dto);
    void DeleteReview(long touristId, long reviewId);
    TourReviewDto GetReviewByTouristAndTour(long touristId, long tourId);
    PagedResult<TourReviewDto> GetReviewsForTour(long tourId, int page, int pageSize);
    PagedResult<TourReviewDto> GetReviewsByTourist(long touristId, int page, int pageSize);
    double GetAverageRatingForTour(long tourId);
    bool CanLeaveReview(long touristId, long tourExecutionId);
}
