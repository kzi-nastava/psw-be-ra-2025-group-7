using System.Collections.Generic;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogCommentRepository
    {
        // Vraća sve komentare za određeni blog
        List<BlogComment> GetByBlogId(long blogId);

        // Kreira novi komentar
        void Create(BlogComment comment);

        // Vraća jedan komentar po ID-u 
        BlogComment? Get(long id);

        // Ažurira komentar
        BlogComment Update(BlogComment comment);

        // Briše komentar
        void Delete(BlogComment comment);
    }
}