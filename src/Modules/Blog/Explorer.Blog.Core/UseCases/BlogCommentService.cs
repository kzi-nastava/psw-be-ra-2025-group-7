using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly IBlogCommentRepository _commentRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IUserInternalService _userInternalService;
        private readonly IMapper _mapper;

        public BlogCommentService(
            IBlogCommentRepository commentRepository,
            IBlogPostRepository blogPostRepository,
            IUserInternalService userInternalService,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
            _userInternalService = userInternalService; 
            _mapper = mapper;
        }

        public List<BlogCommentDto> GetByBlogId(long blogId)
        {
            var comments = _commentRepository.GetByBlogId(blogId);
            var commentDtos = _mapper.Map<List<BlogCommentDto>>(comments);

            foreach (var dto in commentDtos)
            {
                dto.Username = _userInternalService.GetUsername(dto.UserId);
            }

            return commentDtos;
        }

        public BlogCommentDto Create(long authorId, CreateCommentDto dto)
        {
            var blogPost = _blogPostRepository.Get(dto.BlogId);
            if (blogPost == null)
                throw new NotFoundException($"Blog post with ID {dto.BlogId} not found.");

            var comment = new BlogComment(dto.BlogId, authorId, dto.Text);
            blogPost.AddComment(comment);
            _commentRepository.Create(comment);

            var result = _mapper.Map<BlogCommentDto>(comment);

            result.Username = _userInternalService.GetUsername(authorId);

            return result;
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

            var result = _mapper.Map<BlogCommentDto>(comment);

            result.Username = _userInternalService.GetUsername(dto.UserId);

            return result;
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