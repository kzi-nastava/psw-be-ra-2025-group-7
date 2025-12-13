using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMember
    {
        public long Id { get; private set; }          // EF key
        public long TouristId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        private ClubMember() { } // EF

        public ClubMember(long touristId)
        {
            TouristId = touristId;
            JoinedAt = DateTime.UtcNow;
        }
    }
}
