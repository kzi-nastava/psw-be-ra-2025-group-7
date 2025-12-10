using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Notification : Entity
    {
        public long TouristId { get; private set; }
        public long ClubId { get; private set; }
        public NotificationType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRead { get; private set; }

        private Notification() { }   // EF

        public Notification(long touristId, long clubId, NotificationType type)
        {
            TouristId = touristId;
            ClubId = clubId;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;

            if (CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                CreatedAt = DateTime.SpecifyKind(CreatedAt, DateTimeKind.Utc);
            }
        }
    }
}
