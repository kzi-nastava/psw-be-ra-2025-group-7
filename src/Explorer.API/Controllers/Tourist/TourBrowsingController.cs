using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tours")]
[ApiController]
public class TourBrowsingController : ControllerBase
{
    private readonly ITourBrowsingService _tourBrowsingService;

    public TourBrowsingController(ITourBrowsingService tourBrowsingService)
    {
        _tourBrowsingService = tourBrowsingService;
    }

    [HttpGet("previews")]
    public ActionResult<PagedResult<TourPreviewDto>> GetPublishedTourPreviews([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _tourBrowsingService.GetPublishedTourPreviews(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{tourId:long}/full-details")]
    public ActionResult<TourDto> GetFullTourDetails(long tourId)
    {
        var userId = User.PersonId();
        var result = _tourBrowsingService.GetFullTourDetails(tourId, userId);
        return Ok(result);
    }
}
