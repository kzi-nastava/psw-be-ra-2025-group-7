using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourProblemDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int TouristId { get; set; }
        public string Category { get; set; } = "";
        public string Priority { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime TimeReported { get; set; }
        public DateTime? ResolveDue { get; set; }
        public bool IsSolved { get; set; }
        public string Status { get; set; } = "";
        public List<TourProblemMessageDto> Comments { get; set; } = new();

        public long AuthorId { get; set; }


    }
}
