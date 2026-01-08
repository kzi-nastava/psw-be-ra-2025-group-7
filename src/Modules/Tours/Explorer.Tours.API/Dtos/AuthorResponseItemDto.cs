using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class AuthorResponseItemDto
    {
        public long ResponseId { get; set; }
        public long TourRequestId { get; set; }

        public string TourRequestTitle { get; set; } = "";  

        public int ResponseType { get; set; }              
        public decimal ProposedPrice { get; set; }         
        public DateTime SentAt { get; set; }                

        public int Status { get; set; }                    

        public string TouristName { get; set; } = "";
        public string? TouristProfilePicture { get; set; }

        public string? TouristEmail { get; set; }
        public string? TouristBiography { get; set; }
        public string? TouristMotto { get; set; }

        public string? Message { get; set; }
        public string? ProposalDescription { get; set; } 
        public long? TourId { get; set; }

        public string? TourName { get; set; }

    }
}