using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourImage : ValueObject
    {
        public string Url { get; }
        public int Order { get; }

        private TourImage() { } // EF Core

        public TourImage(string url, int order = 0)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.", nameof(url));

            Url = url;
            Order = order;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return Order;
        }
    }
}