using Explorer.Blog.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Blog.API.Public
{
    public interface IBlogCommentService
    {
        // Vraća sve komentare za određeni blog
        List<BlogCommentDto> GetByBlogId(long blogId);

        // Kreira novi komentar
        BlogCommentDto Create(long authorId, CreateCommentDto dto);
    }
}