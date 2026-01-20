using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Tours
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tour-requests")]
    [ApiController]
    public class AuthorTourRequestController : ControllerBase
    {
        private readonly IAuthorTourRequestService _service;

        public AuthorTourRequestController(IAuthorTourRequestService service)
        {
            _service = service;
        }

        [HttpGet("open")]
        public ActionResult<PagedResult<AuthorTourRequestListItemDto>> GetOpen(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] decimal? minBudget = null,
            [FromQuery] decimal? maxBudget = null,
            [FromQuery] int? difficulty = null)
        {
            var authorId = User.PersonId();
            var result = _service.GetOpen(page, pageSize, authorId, minBudget, maxBudget, difficulty);
            return Ok(result);
        }



        [HttpGet("{id:long}")]
        public ActionResult<AuthorTourRequestDetailsDto> GetDetails(long id)
        {
            try
            {
                var authorId = User.PersonId();
                var result = _service.GetDetails(id, authorId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        [HttpPost("{requestId:long}/responses")]
        public ActionResult<TourRequestResponseDto> CreateResponse(long requestId, [FromBody] CreateTourRequestResponseDto dto)
        {
            try
            {
                if (dto == null) return BadRequest(new { message = "Body is required." });
                dto.TourRequestId = requestId; 

                var authorId = User.PersonId();
                var result = _service.CreateResponse(dto, authorId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-responses")]
        public ActionResult<List<AuthorResponseItemDto>> GetMyResponses()
        {
            var authorId = User.PersonId();
            var result = _service.GetMyResponses(authorId);
            return Ok(result);
        }


        [HttpPut("my-responses/{responseId:long}")]
        public ActionResult<TourRequestResponseDto> UpdateMyResponse(long responseId, [FromBody] UpdateMyResponseDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                var result = _service.UpdateMyResponse(responseId, authorId, dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("my-responses/{responseId:long}")]
        public IActionResult DeleteMyResponse(long responseId)
        {
            try
            {
                var authorId = User.PersonId();
                _service.DeleteMyResponse(responseId, authorId);
                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }




    }
}
