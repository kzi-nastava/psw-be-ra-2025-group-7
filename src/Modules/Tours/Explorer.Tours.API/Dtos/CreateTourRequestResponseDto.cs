using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateTourRequestResponseDto
    {
        public long TourRequestId { get; set; }
        public int ResponseType { get; set; } // 0 ExistingTour, 1 CustomProposal

        public long? TourId { get; set; } // obavezno ako ExistingTour
        public string? ProposalDescription { get; set; } // obavezno ako CustomProposal

        public decimal ProposedPrice { get; set; }
        public string? Message { get; set; } // opciono
    }
}
