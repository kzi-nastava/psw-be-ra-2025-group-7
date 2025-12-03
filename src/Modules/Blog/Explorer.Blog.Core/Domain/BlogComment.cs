using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Blog.Core.Domain
{
    public class BlogComment : Entity
    {
        public long BlogPostId { get; private set; }
        public long UserId { get; private set; }
        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        protected BlogComment() { }

        public BlogComment(long blogId, long authorId, string text)
        {
            if (blogId == 0)
                throw new ArgumentException("BlogId must be a positive number.", nameof(blogId));

            if (authorId == 0)
                throw new ArgumentException("AuthorId must be a positive number.", nameof(authorId));

            BlogPostId = blogId;
            UserId = authorId;
            CreatedAt = DateTime.UtcNow;

            SetText(text);
        }

        public void EditText(string newText)
        {
            SetText(newText);
            LastModifiedAt = DateTime.UtcNow;
        }

        private void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Comment text cannot be empty.", nameof(text));

            if (text.Length > 1000)
                throw new ArgumentException("Comment text cannot exceed 1000 characters.", nameof(text));

            Text = text.Trim();
        }
    }
}