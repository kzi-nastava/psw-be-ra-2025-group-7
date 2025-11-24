using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IReviewService
    {
        ReviewDto CreateReview(CreateReviewDto dto);
        ReviewDto GetMyReview(long personId);
        ReviewDto UpdateReview(int reviewId, UpdateReviewDto dto);
        void DeleteReview(int reviewId);
        List<ReviewDto> GetAll();
    }
}
