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
            var blogPost = _blogPostRepository.GetByCommentId(dto.CommentId);
            if (blogPost == null)
                throw new NotFoundException($"Comment with ID {dto.CommentId} not found");

            var comment = blogPost.Comments.First(c => c.Id == dto.CommentId);

            if (comment.UserId != dto.UserId)
                throw new UnauthorizedAccessException("Only the author can edit this comment");

            if ((DateTime.UtcNow - comment.CreatedAt).TotalMinutes > 15)
                throw new InvalidOperationException("Cannot edit comment after 15 minutes");

            comment.EditText(dto.NewText);

            _blogPostRepository.Update(blogPost);

            return _mapper.Map<BlogCommentDto>(comment);
        }


        public void Delete(long commentId, long userId)
        {
            var blogPost = _blogPostRepository.GetByCommentId(commentId);
            if (blogPost == null)
                throw new NotFoundException($"Comment with ID {commentId} not found");

            var comment = blogPost.Comments.First(c => c.Id == commentId);

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Only the author can delete this comment");

            if ((DateTime.UtcNow - comment.CreatedAt).TotalMinutes > 15)
                throw new InvalidOperationException("Cannot delete comment after 15 minutes");

            blogPost.DeleteComment(commentId, userId);
            _blogPostRepository.Update(blogPost);
        }
    }
}