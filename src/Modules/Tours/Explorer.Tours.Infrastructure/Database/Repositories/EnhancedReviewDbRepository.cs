using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class EnhancedReviewDbRepository : IEnhancedReviewRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<EnhancedReview> _dbSet;

        public EnhancedReviewDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<EnhancedReview>();
        }

        public EnhancedReview Create(EnhancedReview review)
        {
            _dbSet.Add(review);
            _dbContext.SaveChanges();
            return review;
        }

        public bool Exists(long tourId, long touristId)
        {
            return _dbSet.Any(r => r.TourId == tourId && r.TouristId == touristId);
        }

        public List<EnhancedReview> GetByTour(long tourId)
        {
            return _dbSet
                .Include(r => r.Pros)
                .Include(r => r.Cons)
                .Include(r => r.SentimentTags)
                .Include(r => r.Images)
                .Include(r => r.HelpfulVotes)
                .Where(r => r.TourId == tourId)
                .OrderByDescending(r => r.HelpfulVotes.Count)
                .ThenByDescending(r => r.CreatedAt)
                .ToList();

        }

        public EnhancedReview Get(long reviewId)
        {
            return _dbSet
                .Include(r => r.HelpfulVotes)
                .First(r => r.Id == reviewId);
        }

        public int AddImages(long reviewId, List<EnhancedReviewImage> images)
        {
            var review = _dbSet.Include(r => r.Images).First(r => r.Id == reviewId);

            if (review.Images.Count + images.Count > 5)
                throw new Exception("Max 5 images allowed.");

            foreach (var img in images)
                review.Images.Add(new EnhancedReviewImage { Url = img.Url, SizeBytes = img.SizeBytes });

            _dbContext.SaveChanges();
            return review.Images.Count;
        }



        public int ToggleHelpful(long reviewId, long touristId)
        {
            var review = _dbSet
                .Include(r => r.HelpfulVotes)
                .First(r => r.Id == reviewId);

            var existing = review.HelpfulVotes.FirstOrDefault(v => v.TouristId == touristId);

            if (existing != null)
                _dbContext.Remove(existing);
            else
                review.HelpfulVotes.Add(new EnhancedReviewHelpfulVote { TouristId = touristId });

            _dbContext.SaveChanges();

            return review.HelpfulVotes.Count;
        }

    }
}
