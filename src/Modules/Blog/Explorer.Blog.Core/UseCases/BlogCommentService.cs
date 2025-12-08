using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Collections.Generic;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly IBlogCommentRepository _commentRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IMapper _mapper;

        public BlogCommentService(
            IBlogCommentRepository commentRepository,
            IBlogPostRepository blogPostRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
            _mapper = mapper;
        }

        public List<BlogCommentDto> GetByBlogId(long blogId)
        {
            var comments = _commentRepository.GetByBlogId(blogId);
            return _mapper.Map<List<BlogCommentDto>>(comments);
        }

        public BlogCommentDto Create(long authorId, CreateCommentDto dto)
        {
            // Proveri da li blog postoji
            var blogPost = _blogPostRepository.Get(dto.BlogId);
            if (blogPost == null)
                throw new NotFoundException($"Blog post with ID {dto.BlogId} not found.");

            // Kreiraj novi komentar (validacija se dešava u konstruktoru)
            var comment = new BlogComment(dto.BlogId, authorId, dto.Text);

            // Sačuvaj komentar (AddComment metoda validira status i ažurira popularnost)
            blogPost.AddComment(comment);
            _commentRepository.Create(comment);

            return _mapper.Map<BlogCommentDto>(comment);
        }

        public BlogCommentDto Edit(EditCommentDto dto)
        {
            var comment = _commentRepository.Get(dto.CommentId);
            if (comment == null)
                throw new NotFoundException($"Comment with ID {dto.CommentId} not found.");

            var blogPost = _blogPostRepository.Get(comment.BlogPostId);
            if (blogPost == null)
                throw new NotFoundException($"Blog post with ID {comment.BlogPostId} not found.");

            blogPost.EditComment(dto.CommentId, dto.UserId, dto.NewText);

            var updated = _commentRepository.Update(comment);

            return _mapper.Map<BlogCommentDto>(updated);
        }

        public void Delete(long commentId, long userId)
        {
            var comment = _commentRepository.Get(commentId);
            if (comment == null)
                throw new NotFoundException($"Comment with ID {commentId} not found.");

            var blogPost = _blogPostRepository.Get(comment.BlogPostId);
            if (blogPost == null)
                throw new NotFoundException($"Blog post with ID {comment.BlogPostId} not found.");

            blogPost.DeleteComment(commentId, userId);

            _commentRepository.Delete(comment);
        }
    }
}