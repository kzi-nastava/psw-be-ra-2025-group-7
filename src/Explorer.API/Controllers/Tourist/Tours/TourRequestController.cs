using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist.Tours
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-requests")]
    [ApiController]
    public class TourRequestController : ControllerBase
    {
        private readonly ITourRequestService _tourRequestService;

        public TourRequestController(ITourRequestService tourRequestService)
        {
            _tourRequestService = tourRequestService;
        }


        [HttpPost]
        public ActionResult<TourRequestDto> Create([FromBody] CreateTourRequestDto dto)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _tourRequestService.Create(dto, touristId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        public ActionResult<TourRequestDto> Update(long id, [FromBody] UpdateTourRequestDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var touristId = User.PersonId();
                var result = _tourRequestService.Update(dto, touristId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            try
            {
                var touristId = User.PersonId();
                _tourRequestService.Delete(id, touristId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("my-requests")]
        public ActionResult<PagedResult<TourRequestDto>> GetMyRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var touristId = User.PersonId();
            var result = _tourRequestService.GetByTourist(page, pageSize, touristId);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<TourRequestDto> GetById(long id)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _tourRequestService.GetById(id, touristId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpPost("{id:long}/close")]
        public ActionResult<TourRequestDto> Close(long id)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _tourRequestService.Close(id, touristId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpGet("{id:long}/responses")]
        public ActionResult<List<TourRequestResponseDto>> GetResponses(long id)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _tourRequestService.GetResponses(id, touristId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("accept-response")]
        public ActionResult<TourRequestDto> AcceptResponse([FromBody] AcceptResponseDto dto)
        {
            try
            {
                var touristId = User.PersonId();
                var result = _tourRequestService.AcceptResponse(dto, touristId);
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
        }

        [HttpPost("decline-response/{responseId:long}")]
        public ActionResult DeclineResponse(long responseId)
        {
            try
            {
                var touristId = User.PersonId();
                _tourRequestService.DeclineResponse(responseId, touristId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}