using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TouristKeyPointMapDto
    {
        public long KeyPointId { get; set; }
        public int Index { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsUnlocked { get; set; }
        public bool IsNext { get; set; }

        public string Secret { get; set; } = string.Empty;
    }
}
