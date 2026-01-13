using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IEnhancedReviewService
    {
        Task<long> CreateReview(long tourId, EnhancedReviewDto dto, long touristId);
        Task AddImages(long reviewId, List<EnhancedReviewImageDto> images);
        Task<List<EnhancedReviewDto>> GetReviews(long tourId);
        Task<int> ToggleHelpful(long reviewId, long touristId);
        Task<ReviewSummaryDto> GetSummary(long tourId);
    }

}
