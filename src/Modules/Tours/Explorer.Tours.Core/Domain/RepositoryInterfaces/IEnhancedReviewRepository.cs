using Explorer.Tours.Core.Domain;

public interface IEnhancedReviewRepository
{
    EnhancedReview Create(EnhancedReview review);
    bool Exists(long tourId, long touristId);
    List<EnhancedReview> GetByTour(long tourId);
    int ToggleHelpful(long reviewId, long touristId);
    EnhancedReview Get(long reviewId);

    int AddImages(long reviewId, List<EnhancedReviewImage> images);
}
