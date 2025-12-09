using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class DeleteCommentDto
    {
        public long CommentId { get; set; }
        public long UserId { get; set; }
    }
}

