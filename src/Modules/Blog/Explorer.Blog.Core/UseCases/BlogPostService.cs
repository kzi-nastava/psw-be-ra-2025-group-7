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

        public BlogPostDto Create(long authorId, CreateBlogPostDto dto)
        {
            var images = _mapper.Map<IEnumerable<BlogImage>>(dto); // koristi mapu CreateBlogPostDto → IEnumerable<BlogImage>

            var blogPost = new BlogPost(authorId, dto.Title, dto.Description, images);

            var created = _repository.Create(blogPost);
            return _mapper.Map<BlogPostDto>(created);
        }

        public BlogPostDto Update(long authorId, UpdateBlogPostDto dto)
        {
            var existing = _repository.Get(dto.Id);
            if (existing == null)
            {
                throw new NotFoundException("Blog post not found.");
            }

            if (existing.AuthorId != authorId)
            {
                throw new ForbiddenException("You cannot edit this blog post.");
            }

            var images = _mapper.Map<IEnumerable<BlogImage>>(dto); // UpdateBlogPostDto → IEnumerable<BlogImage>

            existing.Edit(dto.Title, dto.Description, images, existing.CreatedAt);

            var updated = _repository.Update(existing);
            return _mapper.Map<BlogPostDto>(updated);
        }
    }

}
