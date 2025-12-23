using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IEnhancedReviewRepository
    {
        EnhancedReview Create(EnhancedReview review);
        bool Exists(long tourId, long touristId);
        List<EnhancedReview> GetByTour(long tourId);
        int ToggleHelpful(long reviewId, long touristId);
        EnhancedReview Get(long reviewId);

    }
}
