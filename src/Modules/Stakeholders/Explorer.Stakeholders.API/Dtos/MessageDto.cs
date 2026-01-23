using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long SentByUserId { get; set; }
        public long SentToUserId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        
        // Additional info for display purposes
        public string? SentByUsername { get; set; }
        public string? SentByDisplayName { get; set; }
        public string? SentByProfilePicture { get; set; }
        
        public string? SentToUsername { get; set; }
        public string? SentToDisplayName { get; set; }
        public string? SentToProfilePicture { get; set; }
    }
}
