using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class UpdateBlogPostDto
    {
        public long Id { get; set; }             // koji blog editujemo
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<BlogImageDto> Images { get; set; } = new();
    }
}

