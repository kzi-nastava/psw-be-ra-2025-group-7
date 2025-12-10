using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PublicPointRequestDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public int KeyPointIndex { get; set; }
        public long AuthorId { get; set; }
        public string Status { get; set; } 
        public string? AdminComment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        // Dodatne informacije za prikaz
        public string? KeyPointName { get; set; }
        public string? TourName { get; set; }
    }
}