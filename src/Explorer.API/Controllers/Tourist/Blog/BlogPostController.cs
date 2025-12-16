using System.Security.Claims;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Explorer.API.Controllers.Tourist.Blog
{
    [Authorize(Policy = "touristAuthorPolicy")]
    [Route("api/touristauthor/blog-posts")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        // helper – uzima ID trenutno ulogovanog korisnika iz JWT tokena
        private long GetCurrentUserId()
        {
            var idClaim = User?.FindFirst("id") ?? User?.FindFirst(ClaimTypes.NameIdentifier);

          
            if (idClaim == null)
                return 0; 

            if (!long.TryParse(idClaim.Value, out var userId))
                return 0;

            return userId;
        }


        //vraca blog postove
        [HttpGet]
        public ActionResult<PagedResult<BlogPostDto>> GetMyBlogPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var authorId = GetCurrentUserId();
            var result = _blogPostService.GetByAuthor(authorId, page, pageSize);
            return Ok(result);
        }

       
        [HttpPost]
        public ActionResult<BlogPostDto> Create([FromBody] CreateBlogPostDto dto)
        {
            var authorId = GetCurrentUserId();
            var created = _blogPostService.Create(authorId, dto);
            return Ok(created);
        }

        [HttpPost("{blogPostId}/vote")]
        public ActionResult<BlogVoteDto> Vote(long blogPostId, [FromBody] BlogVoteDto dto)
        {
            var userId = GetCurrentUserId();
            _blogPostService.Vote(blogPostId, userId, dto.Value);

            var updatetPost = _blogPostService.Get(blogPostId);

            return Ok(updatetPost);
        }

        [HttpDelete("{blogPostId}/vote")]
        public ActionResult<BlogVoteDto> RemoveVote(long blogPostId)
        {
            var userId = GetCurrentUserId();
            _blogPostService.Vote(blogPostId, userId, 0); // 0 znači povlačenje glasa
            var updatetPost = _blogPostService.Get(blogPostId);

            return Ok(updatetPost);
        }


        // Izmena bloga dok je u pripremi (naslov, opis, slike).
        [HttpPut("draft")]
        public ActionResult<BlogPostDto> UpdateDraft([FromBody] UpdateBlogPostDto dto)
        {
            var authorId = GetCurrentUserId();
            var updated = _blogPostService.UpdateDraft(authorId, dto);
            return Ok(updated);
        }

       
        // Izmena samo opisa objavljenog bloga.
        [HttpPut("published/description")]
        public ActionResult<BlogPostDto> UpdatePublishedDescription(
            [FromBody] UpdatePublishedBlogDescriptionDto dto)
        {
            var authorId = GetCurrentUserId();
            var updated = _blogPostService.UpdatePublishedDescription(authorId, dto);
            return Ok(updated);
        }

        [HttpPost("{id:long}/publish")]
        public ActionResult<BlogPostDto> Publish(long id)
        {
            var authorId = GetCurrentUserId();
            var updated = _blogPostService.Publish(authorId, id);
            return Ok(updated);
        }

        [HttpPost("{id:long}/archive")]
        public ActionResult<BlogPostDto> Archive(long id)
        {
            var authorId = GetCurrentUserId();
            var updated = _blogPostService.Archive(authorId, id);
            return Ok(updated);
        }


        [HttpGet("public")]
        public ActionResult<PagedResult<BlogPostDto>> GetPublic([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _blogPostService.GetPublic(page, pageSize);
            return Ok(result);
        }

        [HttpGet("filtered")]
        public ActionResult<PagedResult<BlogPostDto>> GetFiltered(
                                                                    [FromQuery] bool? active,
                                                                    [FromQuery] bool? famous,
                                                                    [FromQuery] int page = 1,
                                                                    [FromQuery] int pageSize = 10)
        {
            var filter = new BlogFilterDto
            {
                Active = active,
                Famous = famous,
                Page = page,
                PageSize = pageSize
            };

            var result = _blogPostService.GetFiltered(filter);
            return Ok(result);
        }



    }
}
