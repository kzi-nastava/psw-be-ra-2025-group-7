using System;

namespace Explorer.Blog.API.Dtos
{
    public class BlogCommentDto
    {
        public long Id { get; set; }
        public long BlogId { get; set; }
        public long AuthorId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastEditedAt { get; set; }
    }
}