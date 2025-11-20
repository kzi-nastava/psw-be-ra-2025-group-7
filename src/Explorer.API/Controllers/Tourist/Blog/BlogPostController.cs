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

            // Ako nema tokena (Swagger scenario), ne crashuj
            if (idClaim == null)
                return 0; // SWAGGER MODE

            if (!long.TryParse(idClaim.Value, out var userId))
                return 0;

            return userId;
        }


        /// <summary>
        /// Vraća moje blog postove (paginirano).
        /// </summary>
        [HttpGet]
        public ActionResult<PagedResult<BlogPostDto>> GetMyBlogPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var authorId = GetCurrentUserId();
            var result = _blogPostService.GetByAuthor(authorId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Kreira novi blog post za trenutno ulogovanog korisnika.
        /// </summary>
        [HttpPost]
        public ActionResult<BlogPostDto> Create([FromBody] CreateBlogPostDto dto)
        {
            var authorId = GetCurrentUserId();
            var created = _blogPostService.Create(authorId, dto);
            return Ok(created);
        }

        /// <summary>
        /// Izmenjuje postojeći blog post (naslov, opis, slike) ako pripada trenutnom korisniku.
        /// </summary>
        [HttpPut]
        public ActionResult<BlogPostDto> Update([FromBody] UpdateBlogPostDto dto)
        {
            var authorId = GetCurrentUserId();
            var updated = _blogPostService.Update(authorId, dto);
            return Ok(updated);
        }
    }
}
