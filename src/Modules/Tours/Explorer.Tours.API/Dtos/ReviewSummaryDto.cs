using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class ReviewSummaryDto
    {
        public DimensionRatingsDto AverageDimensions { get; set; } = new();
        public double OverallAverage { get; set; }

        public List<TagCountDto> TopTags { get; set; } = new();
        public List<TextCountDto> TopPros { get; set; } = new();
        public List<TextCountDto> TopCons { get; set; } = new();
    }

    public class TagCountDto
    {
        public string Tag { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TextCountDto
    {
        public string Text { get; set; } = string.Empty;
        public int Count { get; set; }
    }

}
