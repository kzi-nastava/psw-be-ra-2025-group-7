using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class UpdateMyResponseDto
    {
        public decimal ProposedPrice { get; set; }
        public string? Message { get; set; }

        
        public string? ProposalDescription { get; set; }

        public long? TourId { get; set; }
    }

}
