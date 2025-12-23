using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class EnhancedReviewDto
    {
        public int OverallRating { get; set; }
        public DimensionRatingsDto DimensionRatings { get; set; } = default!;
        public List<string> SentimentTags { get; set; } = new();
        public List<string> Pros { get; set; } = new();
        public List<string> Cons { get; set; } = new();
        public string? TextReview { get; set; }
    }

    public class DimensionRatingsDto
    {
        public int GuideQuality { get; set; }
        public int ValueForMoney { get; set; }
        public int RouteScenery { get; set; }
        public int Difficulty { get; set; }
        public int GroupSize { get; set; }
    }
}
