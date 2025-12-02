using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum TravelType
    {
        Walk,
        Bike,
        Car
    }
    public class TourDuration : ValueObject
    {
        public TravelType Type { get; }
        public int Minutes { get; }

        protected TourDuration() { }

        public TourDuration(TravelType type, int minutes)
        {
            Type = type;
            Minutes = minutes;
            if (minutes <= 0)
                throw new ArgumentException("Duration must be positive.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Minutes;
        }
    }
}
