using System;

namespace Explorer.Stakeholders.API.Dtos
{
    /// <summary>
    /// Unified notification DTO that combines both Stakeholder and Tour notifications
    /// </summary>
    public class UnifiedNotificationDto
    {
        public long Id { get; set; }
        public string Source { get; set; } // "Stakeholder" or "Tour"
        
        // Common fields
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        
        // Stakeholder-specific fields
        public long? ClubId { get; set; }
        public int? Type { get; set; } // NotificationType for Stakeholder notifications
        public long? ResourceId { get; set; }
        public int? ResourceType { get; set; }
        public long? SourceFollowerMessageId { get; set; }
        public long? SourceClubMessageId { get; set; }
        
        // Tour-specific fields
        public int? ProblemId { get; set; }
    }
}
