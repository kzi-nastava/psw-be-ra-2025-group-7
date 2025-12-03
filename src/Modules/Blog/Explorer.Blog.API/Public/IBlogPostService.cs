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
 
        PagedResult<BlogPostDto> GetByAuthor(long authorId, int page, int pageSize);    //pregledanje svojih blogova

        BlogPostDto Create(long authorId, CreateBlogPostDto dto);  //kreiranje bloga

        //BlogPostDto Update(long authorId, UpdateBlogPostDto dto);

        BlogPostDto UpdateDraft(long authorId, UpdateBlogPostDto dto);
        
        BlogPostDto UpdatePublishedDescription(long authorId, UpdatePublishedBlogDescriptionDto dto);

        BlogPostDto Publish(long authorId, long blogId);
        BlogPostDto Archive(long authorId, long blogId);

        PagedResult<BlogPostDto> GetPublic(int page, int pageSize);

        BlogPostDto Get(long id);
        BlogVoteDto Vote(long blogPostId, long userId, int value);

    }
}
