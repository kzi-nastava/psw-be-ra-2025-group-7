using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Route("api/author/tour-problems")]
    [ApiController]
    public class AuthorTourProblemController : Controller
    {
      private readonly ITourProblemService _service;
        public AuthorTourProblemController(ITourProblemService service)
        {
            _service = service;
        }
        // returns ONLY logged-in author's problems
        [Authorize(Policy = "authorPolicy")]
        [HttpGet("author")]
        public ActionResult<PagedResult<TourProblemDto>> GetByAuthor(
           [FromQuery] int page,
           [FromQuery] int pageSize)
        {
            var authorId = (int)User.PersonId();
            var result = _service.GetByAuthor(authorId, page, pageSize);
            return Ok(result);
        }
        [Authorize(Policy = "authorPolicy")]
        [HttpPost("{id:int}/reply")]
        public ActionResult<TourProblemDto> AuthorReply(int id, [FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest("Message cannot be empty.");

            var authorId = (int)User.PersonId();
            try
            {
                var updated = _service.AddAuthorReply(id, authorId, message);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
