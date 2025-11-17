using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class CreateBlogPostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // markdown
        public List<BlogImageDto> Images { get; set; } = new();
    }
}