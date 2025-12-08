using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly BlogContext _context;

        public BlogCommentRepository(BlogContext context)
        {
            _context = context;
        }

        public List<BlogComment> GetByBlogId(long blogId)
        {
            var blogPost = _context.BlogPosts
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Id == blogId);

            return blogPost?.Comments.OrderBy(c => c.CreatedAt).ToList() ?? new List<BlogComment>();
        }

        public void Create(BlogComment comment)
        {
            // Učitaj blog post sa komentarima
            var blogPost = _context.BlogPosts
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Id == comment.BlogPostId);

            if (blogPost == null)
                throw new KeyNotFoundException($"Blog post with ID {comment.BlogPostId} not found.");

            _context.SaveChanges();
        }

        public BlogComment? Get(long id)
        {
            // Owned entity se pristupa kroz parent agregat
            return _context.BlogPosts
                .SelectMany(b => b.Comments)
                .FirstOrDefault(c => c.Id == id);
        }

        public BlogComment Update(BlogComment comment)
        {
            var blogPost = _context.BlogPosts
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Comments.Any(c => c.Id == comment.Id));

            if (blogPost == null)
                throw new KeyNotFoundException("Blog post not found");

            var trackedComment = blogPost.Comments.First(c => c.Id == comment.Id);

            trackedComment.EditText(comment.Text);

            _context.SaveChanges();
            return trackedComment;
        }

        public void Delete(BlogComment comment)
        {
            var blogPost = _context.BlogPosts
                .Include(b => b.Comments)
                .FirstOrDefault(b => b.Id == comment.BlogPostId);

            if (blogPost == null)
                throw new KeyNotFoundException($"Blog post with ID {comment.BlogPostId} not found.");

            blogPost.Comments.Remove(comment);

            _context.SaveChanges();
        }
    }
}