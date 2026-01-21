using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain
{
    public class SaleTour
    {
        public long SaleId { get; set; }
        public Sale Sale { get; set; } = null!;
        public long TourId { get; set; }
    }
}
