using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class EditCommentDto
    {
        public long CommentId { get; set; }
        public long UserId { get; set; }
        public string NewText { get; set; } = string.Empty;
    }
}

