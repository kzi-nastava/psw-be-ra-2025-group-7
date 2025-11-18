using System.Security.Claims;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Tours;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-journals")]
[ApiController]
public class TourJournalController : ControllerBase
{
    private readonly ITourJournalService _tourJournalService;

    public TourJournalController(ITourJournalService tourJournalService)
    {
        _tourJournalService = tourJournalService;
    }

    private long GetCurrentUserId()
    {
        var idClaim = User.FindFirst("personId") ?? User.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim == null || !long.TryParse(idClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User personId is missing from token.");
        }

        return userId;
    }

    [HttpGet]
    public ActionResult<PagedResult<TourJournalDto>> GetMyTourJournals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var touristId = GetCurrentUserId();
        var result = _tourJournalService.GetPagedByTourist(touristId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TourJournalDto> Create([FromBody] TourJournalDto tourJournal)
    {
        var touristId = GetCurrentUserId();
        tourJournal.TouristId = touristId;
        var result = _tourJournalService.Create(tourJournal);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourJournalDto> Update([FromBody] TourJournalDto tourJournal)
    {
        var result = _tourJournalService.Update(tourJournal);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _tourJournalService.Delete(id);
        return Ok();
    }
}
