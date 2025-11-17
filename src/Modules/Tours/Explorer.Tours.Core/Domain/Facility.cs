using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum FacilityCategory
    {
        WC,
        Restaurant,
        Parking,
        Other
    }

    public class Facility : Entity
    {
        public string Name { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public FacilityCategory Category { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public Facility(string name, double latitude, double longitude, FacilityCategory category)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");

            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Category = category;
        }
    }
}
