using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public enum ClubStatusDto
    {
        Active = 1,
        Closed = 2
    }
    public class TouristLookupDto
    {
        public long Id { get; set; }        
        public string Username { get; set; }
    }


    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string>? ImageUrls { get; set; } = null;
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ClubStatusDto Status { get; set; }
        public List<ClubMemberDto> Members { get; set; } = new();
        public List<ClubJoinRequestDto> JoinRequests { get; set; } = new();
        public List<ClubInvitationDto> Invitations { get; set; } = new();
    }
}
