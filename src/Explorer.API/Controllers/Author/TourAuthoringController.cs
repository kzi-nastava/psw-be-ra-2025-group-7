using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/tours")]
[ApiController]
public class TourAuthoringController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourAuthoringController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var authorId = User.PersonId();
        return Ok(_tourService.GetPagedByAuthor(page, pageSize, authorId));
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] CreateTourDto createDto)
    {
        var tourDto = new TourDto
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Difficulty = createDto.Difficulty,
            Tags = createDto.Tags,
            Price = 0,
            AuthorId = User.PersonId(),
            Status = 0, // Draft
            PublishedAt = null,
            ArchivedAt = null
        };

        return Ok(_tourService.Create(tourDto));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update([FromBody] TourDto tour)
    {
        tour.AuthorId = User.PersonId();
        return Ok(_tourService.Update(tour));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var authorId = User.PersonId();
        _tourService.Delete(id, authorId);
        return Ok();
    }

    [HttpPut("{id:long}/publish")]
    public ActionResult<TourDto> Publish(long id)
    {
        try
        {
            var authorId = User.PersonId();
            var result = _tourService.Publish(id, authorId);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:long}/archive")]
    public ActionResult<TourDto> Archive(long id)
    {
        try
        {
            var authorId = User.PersonId();
            var result = _tourService.Archive(id, authorId);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:long}/reactivate")]
    public ActionResult<TourDto> Reactivate(long id, [FromBody] ReactivateRequestDto request)
    {
        try
        {
            var authorId = User.PersonId();
            var result = _tourService.Reactivate(id, authorId, request.NewStatus);
            return Ok(result);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
}