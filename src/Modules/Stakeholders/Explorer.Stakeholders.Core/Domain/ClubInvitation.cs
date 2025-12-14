using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum InvitationStatus
    {
        Sent = 1,
        Accepted = 2,
        Rejected = 3
    }

    public class ClubInvitation : Entity
    {
        public long TouristId { get; private set; }
        public DateTime SentAt { get; private set; }
        public InvitationStatus Status { get; private set; }

        private ClubInvitation() { } // EF

        public ClubInvitation(long touristId)
        {
            TouristId = touristId;
            SentAt = DateTime.UtcNow;
            Status = InvitationStatus.Sent;
        }

        public void MarkAccepted()
        {
            Status = InvitationStatus.Accepted;
        }

        public void MarkRejected()
        {
            Status = InvitationStatus.Rejected;
        }
    }
}
