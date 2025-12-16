using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogPostRepository
    {
        // paginacija po autoru (vrati listu + ukupan count)
        (List<BlogPost> Items, int TotalCount) GetByAuthor(long authorId, int page, int pageSize);

        BlogPost? Get(long id);

        BlogPost Create(BlogPost blogPost);

        BlogPost Update(BlogPost blogPost);

        (IEnumerable<BlogPost> items, int total) GetPublic(int page, int pageSize);

        BlogPost? GetByCommentId(long commentId);
        (IList<BlogPost> Items, int TotalCount) GetFiltered(bool? active, bool? famous, int page, int pageSize);


    }
}
