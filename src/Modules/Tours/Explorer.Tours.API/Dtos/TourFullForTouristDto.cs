using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourFullForTouristDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public decimal Price { get; set; }
        public DateTime? PublishedAt { get; set; }
        public double LengthInKm { get; set; }

        public List<TourImageDto> Images { get; set; } = new();
        public List<EquipmentDto> RequiredEquipment { get; set; } = new();
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<TourDurationDto> TourDurations { get; set; } = new();
    }

}