using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class EnhancedReviewDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HelpfulCount { get; set; }
        public int OverallRating { get; set; }
        public DimensionRatingsDto DimensionRatings { get; set; } = default!;
        public string? TextReview { get; set; }
        public List<string> SentimentTags { get; set; } = new();
        public List<string> Pros { get; set; } = new();
        public List<string> Cons { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
    }


    public class DimensionRatingsDto
    {
        public int GuideQuality { get; set; }
        public int ValueForMoney { get; set; }
        public int RouteScenery { get; set; }
        public int Difficulty { get; set; }
        public int GroupSize { get; set; }
    }

    public class EnhancedReviewImageDto
    {
        public string Url { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
    }
}
