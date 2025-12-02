using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Blog.API.Dtos
{
    public class BlogPostDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // markdown tekst

        public DateTime? LastModifiedAt { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<BlogImageDto> Images { get; set; } = new();
        public int Score { get; set; }
    }
}