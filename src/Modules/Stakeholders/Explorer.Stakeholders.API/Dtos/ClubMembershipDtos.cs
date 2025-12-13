using System;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMemberDto
    {
        public long TouristId { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? TouristName { get; set; }  
    }

    public class ClubJoinRequestDto
    {
        public long TouristId { get; set; }
        public DateTime RequestedAt { get; set; }
        public string? TouristName { get; set; }
    }

    public class ClubInvitationDto
    {
        public long TouristId { get; set; }
        public DateTime SentAt { get; set; }
        public string? TouristName { get; set; }
    }
}
