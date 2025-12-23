using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class EnhancedReview
    {
        public long Id { get; private set; }

        public long TourId { get; private set; }
        public long TouristId { get; private set; }

        public int OverallRating { get; private set; }

        public int GuideQuality { get; private set; }
        public int ValueForMoney { get; private set; }
        public int RouteScenery { get; private set; }
        public int Difficulty { get; private set; }
        public int GroupSize { get; private set; }

        public string? TextReview { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public ICollection<EnhancedReviewPro> Pros { get; private set; } = new List<EnhancedReviewPro>();
        public ICollection<EnhancedReviewCon> Cons { get; private set; } = new List<EnhancedReviewCon>();
        public ICollection<EnhancedReviewTag> SentimentTags { get; private set; } = new List<EnhancedReviewTag>();
        public ICollection<EnhancedReviewImage> Images { get; private set; } = new List<EnhancedReviewImage>();

        public EnhancedReview(
            long tourId,
            long touristId,
            int overallRating,
            int guideQuality,
            int valueForMoney,
            int routeScenery,
            int difficulty,
            int groupSize,
            string? textReview)
        {
            TourId = tourId;
            TouristId = touristId;
            OverallRating = overallRating;
            GuideQuality = guideQuality;
            ValueForMoney = valueForMoney;
            RouteScenery = routeScenery;
            Difficulty = difficulty;
            GroupSize = groupSize;
            TextReview = textReview;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class EnhancedReviewPro
    {
        public long Id { get; set; }
        public long EnhancedReviewId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class EnhancedReviewCon
    {
        public long Id { get; set; }
        public long EnhancedReviewId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class EnhancedReviewImage
    {
        public long Id { get; set; }
        public long EnhancedReviewId { get; set; }
        public string Url { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
    }

    public enum ReviewSentimentTag
    {
        FriendlyGuide,
        GreatViews,
        GoodValue,
        WellOrganized,
        Challenging,
        TooCrowded
    }

    public class EnhancedReviewTag
    {
        public long Id { get; set; }
        public long EnhancedReviewId { get; set; }
        public ReviewSentimentTag Tag { get; set; }
    }

}
