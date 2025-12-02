using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _repository;
        private readonly IMapper _mapper;

        public BlogPostService(IBlogPostRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PagedResult<BlogPostDto> GetByAuthor(long authorId, int page, int pageSize)
        {
            var (items, total) = _repository.GetByAuthor(authorId, page, pageSize);
            var resultDtos = _mapper.Map<List<BlogPostDto>>(items);
            return new PagedResult<BlogPostDto>(resultDtos, total);
        }


        public PagedResult<BlogPostDto> GetPublic(int page, int pageSize)
        {
            var (items, total) = _repository.GetPublic(page, pageSize);

            var resultDtos = _mapper.Map<List<BlogPostDto>>(items);
            return new PagedResult<BlogPostDto>(resultDtos, total);
        }


        public BlogPostDto Create(long authorId, CreateBlogPostDto dto)
        {
            var images = _mapper.Map<IEnumerable<BlogImage>>(dto); // koristi mapu CreateBlogPostDto → IEnumerable<BlogImage>

            var blogPost = new BlogPost(authorId, dto.Title, dto.Description, images);

            var created = _repository.Create(blogPost);
            return _mapper.Map<BlogPostDto>(created);
        }

        public BlogPostDto UpdateDraft(long authorId, UpdateBlogPostDto dto)
        {
            var existing = LoadAndCheckOwnership(authorId, dto.Id);

            existing.EditDraft(
                dto.Title,
                dto.Description,
                _mapper.Map<IEnumerable<BlogImage>>(dto));

            var updated = _repository.Update(existing);
            return _mapper.Map<BlogPostDto>(updated);
        }

        public BlogPostDto UpdatePublishedDescription(long authorId, UpdatePublishedBlogDescriptionDto dto)
        {
            var existing = LoadAndCheckOwnership(authorId, dto.Id);

            existing.UpdatePublishedDescription(dto.Description);

            var updated = _repository.Update(existing);
            return _mapper.Map<BlogPostDto>(updated);
        }

        public BlogPostDto Publish(long authorId, long blogId)
        {
            var existing = LoadAndCheckOwnership(authorId, blogId);

            existing.Publish();

            var updated = _repository.Update(existing);
            return _mapper.Map<BlogPostDto>(updated);
        }

        public BlogPostDto Archive(long authorId, long blogId)
        {
            var existing = LoadAndCheckOwnership(authorId, blogId);

            existing.Archive();

            var updated = _repository.Update(existing);
            return _mapper.Map<BlogPostDto>(updated);
        }

        public BlogVoteDto Vote(long blogPostId, long userId, int value)
        {
            if (value != 1 && value != -1)
                throw new ArgumentException("Vote value must be +1 or -1.");


            var blogPost = _repository.Get(blogPostId);
            if (blogPost == null)
            {
                throw new NotFoundException("Blog post not found.");
            }

            blogPost.Vote(userId, value);
            _repository.Update(blogPost);

            var vote = blogPost.Votes.FirstOrDefault(v => v.UserId == userId); // trenutni glas korisnika
            return vote == null ? null : _mapper.Map<BlogVoteDto>(vote);
            // korisnik je povukao glas, vrati null
            // inace vrati njegov glas mapiran u DTO
        }

        // Helper
        private BlogPost LoadAndCheckOwnership(long authorId, long blogId)
        {
            var existing = _repository.Get(blogId);
            if (existing == null)
                throw new NotFoundException("Blog post not found.");

            if (existing.AuthorId != authorId)
                throw new ForbiddenException("You cannot edit this blog post.");

            return existing;
        }
    }

}
