using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum TravelType
    {
        Walk,
        Bike,
        Car,
        Boat
    }
    public class TourDuration : ValueObject
    {
        public TravelType Type { get; }
        public int Minutes { get; }

        private TourDuration() { }

        [JsonConstructor]
        public TourDuration(TravelType type, int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentException("Duration must be positive.");

            Type = type;
            Minutes = minutes;
        }


        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Type;
            yield return Minutes;
        }
    }
}
