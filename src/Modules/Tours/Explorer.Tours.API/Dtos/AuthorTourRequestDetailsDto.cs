using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class AuthorTourRequestDetailsDto
    {
        
        public long Id { get; set; }
        public long TouristId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Budget { get; set; }
        public int? PreferredDifficulty { get; set; }
        public int NumberOfParticipants { get; set; }
        public DateTime? PreferredDate { get; set; }

      
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Radius { get; set; }

        
        public string TouristName { get; set; } = "";
        public string ContactDisclaimer { get; set; } = "You only get contact information if the tourist accepts your offer.";

        public string? TouristProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int DaysUntilExpiration { get; set; }
        public int ResponseCount { get; set; }
    }
}
