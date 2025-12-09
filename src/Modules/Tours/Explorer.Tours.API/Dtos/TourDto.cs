using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class TourDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Enum u domenu, int u DTO
        public int Difficulty { get; set; }

        public List<string> Tags { get; set; } = new();

        // Enum u domenu, int u DTO
        public int Status { get; set; }

        public decimal Price { get; set; }

        // Kartica 3 – ključne tačke ture
        public List<KeyPointDto> KeyPoints { get; set; } = new();

        // Životni ciklus ture (priča člana 1)
        public DateTime? PublishedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public List<TourDurationDto> TourDurations { get; set; } = new();
    }
}
