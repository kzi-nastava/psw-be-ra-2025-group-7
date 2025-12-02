using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class UpdatePublishedBlogDescriptionDto
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
