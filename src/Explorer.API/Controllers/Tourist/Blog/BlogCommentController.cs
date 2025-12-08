using System.Security.Claims;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist.Blog
{
    [Route("api/blog-comments")]
    [ApiController]
    public class BlogCommentController : ControllerBase
    {
        private readonly IBlogCommentService _blogCommentService;

        public BlogCommentController(IBlogCommentService blogCommentService)
        {
            _blogCommentService = blogCommentService;
        }

        // Helper – uzima ID trenutno ulogovanog korisnika iz JWT tokena
        private long GetCurrentUserId()
        {
            var idClaim = User?.FindFirst("id") ?? User?.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null || !long.TryParse(idClaim.Value, out var userId))
                return 0;

            return userId;
        }

        // Vraća sve komentare za određeni blog (dostupno svima)
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<BlogCommentDto>> GetByBlogId([FromQuery] long blogId)
        {
            var comments = _blogCommentService.GetByBlogId(blogId);
            return Ok(comments);
        }

        // Kreira novi komentar (mora biti prijavljen)
        [HttpPost]
        [Authorize]
        public ActionResult<BlogCommentDto> Create([FromBody] CreateCommentDto dto)
        {
            var authorId = dto.UserId;

            if (authorId == 0)
                return Unauthorized("You must be logged in to comment.");

            var created = _blogCommentService.Create(authorId, dto);
            return Ok(created);
        }

        [HttpPut("edit")]
        [Authorize]
        public ActionResult<BlogCommentDto> Edit([FromBody] EditCommentDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized("You must be logged in to edit comments.");

            dto.UserId = userId; // osiguravamo da korisnik menja samo svoj komentar

            var updated = _blogCommentService.Edit(dto);
            return Ok(updated);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete([FromQuery] long commentId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized("You must be logged in to delete comments.");

            _blogCommentService.Delete(commentId, userId);
            return Ok(new { message = "Comment deleted successfully." });
        }

    }
}