using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;


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

        public Task CreateReview(long tourId, EnhancedReviewDto dto, long touristId)
        {
            if (_enhancedReviewRepository.Exists(tourId, touristId))
                throw new EntityValidationException("You have already reviewed this tour");

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

            _enhancedReviewRepository.Create(review);

            return Task.CompletedTask;
        }


        public Task<List<EnhancedReviewDto>> GetReviews(long tourId)
        {
            var reviews = _enhancedReviewRepository.GetByTour(tourId);

            var result = reviews.Select(r => MapToDto(r)).ToList();
            return Task.FromResult(result);
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
                TextReview = review.TextReview
            };
        }


    }
}
