using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public long AuthorId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public List<BlogImage> Images { get; private set; } = new();

        protected BlogPost() { }  

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images = null)
        {
            if (authorId <= 0)
                throw new ArgumentException("AuthorId must be a positive number.", nameof(authorId));

            SetTitle(title);
            SetDescription(description);

            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;

            if (images != null)
            {
                Images = images.ToList();
            }
        }

        public void Edit(string title, string description, IEnumerable<BlogImage>? images)
        {
            SetTitle(title);
            SetDescription(description);

            Images.Clear();
            if (images != null)
            {
                Images.AddRange(images);
            }
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

            Title = title;
        }

        private void SetDescription(string description)
        {
            Description = description ?? string.Empty;
            // Renderovanje Markdown -> HTML radi API ili frontend.
        }
    }

}
