using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-purchases")]
[ApiController]
public class TourPurchaseController : ControllerBase
{
    private readonly ITourPurchaseTokenService _tourPurchaseTokenService;

    public TourPurchaseController(ITourPurchaseTokenService tourPurchaseTokenService)
    {
        _tourPurchaseTokenService = tourPurchaseTokenService;
    }

    [HttpPost("{tourId:long}")]
    public ActionResult<TourPurchaseTokenDto> PurchaseTour(long tourId)
    {
        var userId = User.PersonId();
        var result = _tourPurchaseTokenService.Create(userId, tourId);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<PagedResult<TourPurchaseTokenDto>> GetMyPurchases([FromQuery] int page, [FromQuery] int pageSize)
    {
        var userId = User.PersonId();
        var result = _tourPurchaseTokenService.GetPagedByUser(page, pageSize, userId);
        return Ok(result);
    }

    [HttpGet("simple")]
    public ActionResult<List<SimpleTourDto>> GetMyPurchasedToursSimple()
    {
        var userId = User.PersonId();
        var result = _tourPurchaseTokenService.GetPagedByUser(0, 1000, userId);

        var simpleTours = result.Results
            .Where(p => p.Tour != null)
            .Select(p => new SimpleTourDto
            {
                Id = p.TourId,
                Name = p.Tour!.Name
            })
            .ToList();

        return Ok(simpleTours);
    }

    [HttpGet("check/{tourId:long}")]
    public ActionResult<bool> HasPurchased(long tourId)
    {
        var userId = User.PersonId();
        var result = _tourPurchaseTokenService.HasUserPurchasedTour(userId, tourId);
        return Ok(result);
    }
}
