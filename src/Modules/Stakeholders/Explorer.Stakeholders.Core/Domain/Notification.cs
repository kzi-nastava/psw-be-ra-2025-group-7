using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{

    /// <summary>
    /// Unified Notification system supporting both club activities and follower messages
    /// </summary>
    public class Notification : Entity
    {
        // Generic user ID (supports all roles, not just tourists)
        public long UserId { get; init; }

        // Notification metadata
        public NotificationType Type { get; init; }
        public string Content { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsRead { get; private set; }

        // Resource linking (for follower messages)
        public long? ResourceId { get; init; }
        public ResourceType? ResourceType { get; init; }

        // Source references
        public long? SourceFollowerMessageId { get; init; }
        public long? SourceClubMessageId { get; init; }

        // Club-specific fields (for join requests)
        public long? ClubId { get; init; }

        private Notification() { } // EF Core

        // Constructor for follower messages (your functionality)
        public Notification(
            long userId,
            NotificationType type,
            string content,
            long? resourceId = null,
            ResourceType? resourceType = null,
            long? sourceFollowerMessageId = null,
            long? sourceClubMessageId = null)
        {
            UserId = userId;
            Type = type;
            Content = content;
            ResourceId = resourceId;
            ResourceType = resourceType;
            SourceFollowerMessageId = sourceFollowerMessageId;
            SourceClubMessageId = sourceClubMessageId;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        // Constructor for club join requests (existing functionality)
        public Notification(long userId, long clubId, NotificationType type, string content)
        {
            UserId = userId;
            ClubId = clubId;
            Type = type;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}