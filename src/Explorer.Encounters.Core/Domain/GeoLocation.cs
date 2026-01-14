using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class GeoLocation : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double? Radius { get; set; }

        private GeoLocation() { } // EF

        [JsonConstructor]
        public GeoLocation(double latitude, double longitude, double? radius)
        {

            if (latitude < -90 || latitude > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
            if (longitude < -180 || longitude > 180) throw new ArgumentOutOfRangeException(nameof(longitude));

            Latitude = latitude;
            Longitude = longitude;
            Radius = radius;
        }
        public GeoLocation(double latitude, double longitude)
        {

            if (latitude < -90 || latitude > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
            if (longitude < -180 || longitude > 180) throw new ArgumentOutOfRangeException(nameof(longitude));

            Latitude = latitude;
            Longitude = longitude;
            Radius = null;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            
            yield return Latitude;
            yield return Longitude;
            yield return Radius;
        }
    }

}
