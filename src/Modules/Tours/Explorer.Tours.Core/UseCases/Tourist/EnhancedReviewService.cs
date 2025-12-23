using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class EnhancedReviewService : IEnhancedReviewService
    {
        private readonly ITourPurchaseTokenRepository _tourPurchaseTokenRepository;
        private readonly IEnhancedReviewRepository _enhancedReviewRepository;

        public EnhancedReviewService(ITourPurchaseTokenRepository tourPurchaseTokenRepository, IEnhancedReviewRepository enhancedReviewRepository)
        {
            _tourPurchaseTokenRepository = tourPurchaseTokenRepository;
            _enhancedReviewRepository = enhancedReviewRepository;
        }

        public Task CreateReview(long tourId, EnhancedReviewDto dto, long touristId, List<EnhancedReviewImageDto> images)
        {
            if (_enhancedReviewRepository.Exists(tourId, touristId))
                throw new EntityValidationException("You have already reviewed this tour");

            if (!_tourPurchaseTokenRepository.HasUserPurchasedTour(touristId, tourId))
                throw new ForbiddenException("You must complete the tour first");

            ValidateRating(dto.OverallRating, "OverallRating");
            ValidateRating(dto.DimensionRatings.GuideQuality, "GuideQuality");
            ValidateRating(dto.DimensionRatings.ValueForMoney, "ValueForMoney");
            ValidateRating(dto.DimensionRatings.RouteScenery, "RouteScenery");
            ValidateRating(dto.DimensionRatings.Difficulty, "Difficulty");
            ValidateRating(dto.DimensionRatings.GroupSize, "GroupSize");

            if (dto.SentimentTags == null || dto.SentimentTags.Count < 1)
                throw new EntityValidationException("At least one sentiment tag is required.");

            if (dto.Pros != null && dto.Pros.Count > 5)
                throw new EntityValidationException("Max 5 pros allowed.");

            if (dto.Cons != null && dto.Cons.Count > 5)
                throw new EntityValidationException("Max 5 cons allowed.");

            if (dto.TextReview != null && dto.TextReview.Length > 2000)
                throw new EntityValidationException("TextReview must be <= 2000 characters.");

            var review = new EnhancedReview(
                tourId,
                touristId,
                dto.OverallRating,
                dto.DimensionRatings.GuideQuality,
                dto.DimensionRatings.ValueForMoney,
                dto.DimensionRatings.RouteScenery,
                dto.DimensionRatings.Difficulty,
                dto.DimensionRatings.GroupSize,
                dto.TextReview
            );

            if (dto.Pros != null)
            {
                foreach (var p in dto.Pros)
                    review.Pros.Add(new EnhancedReviewPro { Text = p });
            }

            if (dto.Cons != null)
            {
                foreach (var c in dto.Cons)
                    review.Cons.Add(new EnhancedReviewCon { Text = c });
            }

            if (dto.SentimentTags != null)
            {
                foreach (var t in dto.SentimentTags)
                    review.SentimentTags.Add(new EnhancedReviewTag { Tag = ParseTag(t) });
            }

            if (images != null)
            {
                foreach (var img in images)
                    review.Images.Add(new EnhancedReviewImage { Url = img.Url, SizeBytes = img.SizeBytes });
            }

            _enhancedReviewRepository.Create(review);

            return Task.CompletedTask;
        }


        public Task<List<EnhancedReviewDto>> GetReviews(long tourId)
        {
            var reviews = _enhancedReviewRepository.GetByTour(tourId);

            var result = reviews.Select(r => MapToDto(r)).ToList();
            return Task.FromResult(result);
        }

        public Task<int> ToggleHelpful(long reviewId, long touristId)
        {
            var review = _enhancedReviewRepository.Get(reviewId);

            if (review.TouristId == touristId)
                throw new EntityValidationException("You cannot vote helpful on your own review.");

            var count = _enhancedReviewRepository.ToggleHelpful(reviewId, touristId);
            return Task.FromResult(count);
        }
        public Task<ReviewSummaryDto> GetSummary(long tourId)
        {
            var reviews = _enhancedReviewRepository.GetByTour(tourId);

            if (reviews.Count == 0)
                return Task.FromResult(new ReviewSummaryDto()); 

            var summary = new ReviewSummaryDto
            {
                OverallAverage = reviews.Average(r => r.OverallRating),
                AverageDimensions = new DimensionRatingsDto
                {
                    GuideQuality = (int)Math.Round(reviews.Average(r => r.GuideQuality)),
                    ValueForMoney = (int)Math.Round(reviews.Average(r => r.ValueForMoney)),
                    RouteScenery = (int)Math.Round(reviews.Average(r => r.RouteScenery)),
                    Difficulty = (int)Math.Round(reviews.Average(r => r.Difficulty)),
                    GroupSize = (int)Math.Round(reviews.Average(r => r.GroupSize)),
                },
                TopTags = reviews
                    .SelectMany(r => r.SentimentTags)
                    .GroupBy(t => t.Tag)
                    .Select(g => new TagCountDto { Tag = g.Key.ToString(), Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList(),
                TopPros = reviews
                    .SelectMany(r => r.Pros)
                    .GroupBy(p => p.Text.Trim())
                    .Select(g => new TextCountDto { Text = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList(),
                TopCons = reviews
                    .SelectMany(r => r.Cons)
                    .GroupBy(c => c.Text.Trim())
                    .Select(g => new TextCountDto { Text = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList(),
            };

            return Task.FromResult(summary);
        }

        private static void ValidateRating(int value, string field)
        {
            if (value < 1 || value > 5)
                throw new EntityValidationException($"{field} must be between 1 and 5.");
        }

        private static ReviewSentimentTag ParseTag(string tag)
        {
            if (!Enum.TryParse<ReviewSentimentTag>(tag, true, out var parsed))
                throw new EntityValidationException($"Invalid sentiment tag: {tag}");

            return parsed;
        }

        private static EnhancedReviewDto MapToDto(EnhancedReview review)
        {
            return new EnhancedReviewDto
            {
                OverallRating = review.OverallRating,
                DimensionRatings = new DimensionRatingsDto
                {
                    GuideQuality = review.GuideQuality,
                    ValueForMoney = review.ValueForMoney,
                    RouteScenery = review.RouteScenery,
                    Difficulty = review.Difficulty,
                    GroupSize = review.GroupSize
                },
                Pros = review.Pros.Select(p => p.Text).ToList(),
                Cons = review.Cons.Select(c => c.Text).ToList(),
                SentimentTags = review.SentimentTags.Select(t => t.Tag.ToString()).ToList(),
                TextReview = review.TextReview,
                Id = review.Id,
                CreatedAt = review.CreatedAt,
                HelpfulCount = review.HelpfulVotes.Count,
                ImageUrls = review.Images.Select(i => i.Url).ToList(),

            };
        }


    }
}
