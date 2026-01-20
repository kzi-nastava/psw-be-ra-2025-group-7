using System;

namespace Explorer.Blog.API.Dtos
{
    public class BlogCommentDto
    {
        public long Id { get; set; }
        public long BlogPostId { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty; 
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}