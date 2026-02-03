using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ContactDto
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? LastMessageContent { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool? IsLastMessageByMe { get; set; }
    }
}
