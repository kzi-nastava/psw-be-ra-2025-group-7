using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogFilterDto
    {
        public bool? Active { get; set; }      // true - samo ACTIVE
        public bool? Famous { get; set; }      // true - samo FAMOUS
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        

    }
}
