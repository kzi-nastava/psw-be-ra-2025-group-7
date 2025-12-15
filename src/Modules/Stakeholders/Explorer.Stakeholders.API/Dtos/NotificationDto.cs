using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long? ClubId { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public long? ResourceId { get; set; }
        public int? ResourceType { get; set; }
    }
}
