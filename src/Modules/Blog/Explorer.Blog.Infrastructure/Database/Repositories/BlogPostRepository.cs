using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Infrastructure.Database.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly BlogContext _context;

    public BlogPostRepository(BlogContext context)
    {
        _context = context;
    }

    public (List<BlogPost> Items, int TotalCount) GetByAuthor(long authorId, int page, int pageSize)
    {
        IQueryable<BlogPost> query = _context.BlogPosts
            .Include(b => b.Images)
            .Where(b => b.AuthorId == authorId)
            .OrderByDescending(b => b.CreatedAt);

        var total = query.Count();

        if (page > 0 && pageSize > 0)
        {
            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        var items = query.ToList();
        return (items, total);
    }

    public BlogPost? Get(long id)
    {
        return _context.BlogPosts
            .Include(b => b.Images)
            .FirstOrDefault(b => b.Id == id);
    }

    public BlogPost Create(BlogPost blogPost)
    {
        _context.BlogPosts.Add(blogPost);
        _context.SaveChanges();
        return blogPost;
    }

    public BlogPost Update(BlogPost blogPost)
    {
        _context.BlogPosts.Update(blogPost);
        _context.SaveChanges();
        return blogPost;
    }
}


