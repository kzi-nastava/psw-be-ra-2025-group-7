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
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Status { get; set; }
        public decimal Price { get; set; }


        public List<KeyPointDto> KeyPoints { get; set; } = new();

        public DateTime? PublishedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public List<TourDurationDto> TourDurations { get; set; } = new();

        public List<EquipmentDto> RequiredEquipment { get; set; } = new();
    }
}
