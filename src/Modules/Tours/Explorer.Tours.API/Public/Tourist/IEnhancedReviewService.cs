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
        Task CreateReview(long tourId, EnhancedReviewDto dto, long touristId);
        Task<List<EnhancedReviewDto>> GetReviews(long tourId);
    }
}
