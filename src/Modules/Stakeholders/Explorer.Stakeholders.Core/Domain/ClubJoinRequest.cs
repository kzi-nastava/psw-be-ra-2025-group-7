using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum JoinRequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }

    public class ClubJoinRequest : Entity
    {
        public long TouristId { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public JoinRequestStatus Status { get; private set; }

        private ClubJoinRequest() { } // EF

        public ClubJoinRequest(long touristId)
        {
            TouristId = touristId;
            RequestedAt = DateTime.UtcNow;
            Status = JoinRequestStatus.Pending;
        }

        public void MarkAccepted()
        {
            Status = JoinRequestStatus.Accepted;
        }

        public void MarkRejected()
        {
            Status = JoinRequestStatus.Rejected;
        }
    }
}
