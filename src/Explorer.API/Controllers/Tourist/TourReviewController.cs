using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-reviews")]
[ApiController]
public class TourReviewController : ControllerBase
{
    private readonly ITourReviewService _tourReviewService;

    public TourReviewController(ITourReviewService tourReviewService)
    {
        _tourReviewService = tourReviewService;
    }

    [HttpPost]
    public ActionResult<TourReviewDto> CreateReview([FromBody] CreateTourReviewDto dto)
    {
        var touristId = User.PersonId();
        var result = _tourReviewService.CreateReview(touristId, dto);
        return Ok(result);
    }

    [HttpPut("{reviewId:long}")]
    public ActionResult<TourReviewDto> UpdateReview(long reviewId, [FromBody] UpdateTourReviewDto dto)
    {
        var touristId = User.PersonId();
        var result = _tourReviewService.UpdateReview(touristId, reviewId, dto);
        return Ok(result);
    }

    [HttpDelete("{reviewId:long}")]
    public ActionResult DeleteReview(long reviewId)
    {
        var touristId = User.PersonId();
        _tourReviewService.DeleteReview(touristId, reviewId);
        return Ok();
    }

    [HttpGet("my-reviews/tour/{tourId:long}")]
    public ActionResult<TourReviewDto> GetMyReviewForTour(long tourId)
    {
        var touristId = User.PersonId();
        var result = _tourReviewService.GetReviewByTouristAndTour(touristId, tourId);
        return Ok(result);
    }

    [HttpGet("tour/{tourId:long}")]
    [AllowAnonymous]
    public ActionResult<PagedResult<TourReviewDto>> GetReviewsForTour(long tourId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _tourReviewService.GetReviewsForTour(tourId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("my-reviews")]
    public ActionResult<PagedResult<TourReviewDto>> GetMyReviews([FromQuery] int page, [FromQuery] int pageSize)
    {
        var touristId = User.PersonId();
        var result = _tourReviewService.GetReviewsByTourist(touristId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("tour/{tourId:long}/average-rating")]
    [AllowAnonymous]
    public ActionResult<object> GetAverageRating(long tourId)
    {
        var averageRating = _tourReviewService.GetAverageRatingForTour(tourId);
        return Ok(new { tourId, averageRating });
    }

    [HttpGet("can-review/{tourExecutionId:long}")]
    public ActionResult<object> CanLeaveReview(long tourExecutionId)
    {
        var touristId = User.PersonId();
        var canReview = _tourReviewService.CanLeaveReview(touristId, tourExecutionId);
        return Ok(new { canReview });
    }
}
