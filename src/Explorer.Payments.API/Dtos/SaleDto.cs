using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public enum SaleStatusDto
    {
        Draft,
        Active,
        Expired
    }

    public class SaleDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public List<long> TourIds { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int DiscountPercentage { get; set; }
        public SaleStatusDto Status { get; set; }
    }

}
