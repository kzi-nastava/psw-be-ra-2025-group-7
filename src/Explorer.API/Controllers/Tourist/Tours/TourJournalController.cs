using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Stakeholders.Infrastructure.Authentication;
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

    [HttpGet]
    public ActionResult<PagedResult<TourJournalDto>> GetMyTourJournals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var touristId = User.PersonId();
        var result = _tourJournalService.GetPagedByTourist(touristId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TourJournalDto> Create([FromBody] TourJournalDto tourJournal)
    {
        var touristId = User.PersonId();
        tourJournal.TouristId = touristId;
        var result = _tourJournalService.Create(tourJournal);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourJournalDto> Update([FromBody] TourJournalDto tourJournal)
    {
        var touristId = User.PersonId();
        tourJournal.TouristId = touristId;
        var result = _tourJournalService.Update(tourJournal);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var touristId = User.PersonId();
        _tourJournalService.Delete(id, touristId);
        return Ok();
    }
}
