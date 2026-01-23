using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class Sale : Entity
    {
        public long AuthorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int DiscountPercentage { get; set; }
        public SaleStatus Status { get; set; }
        public ICollection<SaleTour> SaleTours { get; set; } = new List<SaleTour>();
        public Sale() { }
    }

    public enum SaleStatus
    {
        Draft = 0,
        Active = 1,
        Expired = 2
    }
}
