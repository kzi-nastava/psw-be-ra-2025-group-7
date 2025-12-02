using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public
{
    public interface IBlogPostService
    {
 
        PagedResult<BlogPostDto> GetByAuthor(long authorId, int page, int pageSize);    //pregeledanje svojih blogova

        BlogPostDto Create(long authorId, CreateBlogPostDto dto);  //kreiranje bloga

        BlogPostDto Update(long authorId, UpdateBlogPostDto dto);

        BlogVoteDto Vote(long blogPostId, long userId, int value);

    }
}
