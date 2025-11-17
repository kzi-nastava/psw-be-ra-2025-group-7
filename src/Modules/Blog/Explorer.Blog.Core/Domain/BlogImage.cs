using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogImage : ValueObject
    {
        public string Url { get; }
        public int Order { get; }

        private BlogImage() { }   // EF Core traži prazan konstruktor za ValueObject-e

        public BlogImage(string url, int order = 0)
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