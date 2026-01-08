using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-executions")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _tourExecutionService;

    public TourExecutionController(ITourExecutionService tourExecutionService)
    {
        _tourExecutionService = tourExecutionService;
    }

    [HttpPost("start")]
    public ActionResult<TourExecutionDto> StartTour([FromBody] StartTourExecutionDto dto)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.StartTour(touristId, dto);
        return Ok(result);
    }

    [HttpPut("{executionId:long}/complete")]
    public ActionResult<TourExecutionDto> CompleteTour(long executionId)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.CompleteTour(touristId, executionId);
        return Ok(result);
    }

    [HttpPut("{executionId:long}/abandon")]
    public ActionResult<TourExecutionDto> AbandonTour(long executionId)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.AbandonTour(touristId, executionId);
        return Ok(result);
    }

    [HttpPost("{executionId:long}/unlock-keypoint")]
    public ActionResult<TourExecutionDto> UnlockKeyPoint(long executionId, [FromBody] UnlockKeyPointDto dto)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.UnlockKeyPoint(touristId, executionId, dto);
        return Ok(result);
    }

    [HttpPost("{executionId:long}/check-proximity")]
    public ActionResult<KeyPointProximityCheckResultDto> CheckKeyPointProximity(long executionId, [FromBody] CheckKeyPointProximityDto dto)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.CheckKeyPointProximity(touristId, executionId, dto);
        return Ok(result);
    }

    [HttpPut("{executionId:long}/update-activity")]
    public ActionResult<TourExecutionDto> UpdateLastActivity(long executionId)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.UpdateLastActivity(touristId, executionId);
        return Ok(result);
    }

    [HttpGet("{executionId:long}/progress")]
    public ActionResult<object> GetProgressPercentage(long executionId)
    {
        var touristId = User.PersonId();
        var progressPercentage = _tourExecutionService.GetProgressPercentage(touristId, executionId);
        return Ok(new { executionId, progressPercentage });
    }

    [HttpGet("active/{tourId:long}")]
    public ActionResult<TourExecutionDto> GetActiveExecution(long tourId)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.GetActiveExecution(touristId, tourId);
        return Ok(result);
    }

    [HttpGet("history")]
    public ActionResult<PagedResult<TourExecutionDto>> GetExecutionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.GetExecutionHistory(touristId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{executionId:long}/keypoint/{keyPointIndex:int}/secret")]
    public ActionResult<object> GetKeyPointSecret(long executionId, int keyPointIndex)
    {
        var touristId = User.PersonId();
        var secret = _tourExecutionService.GetKeyPointSecret(touristId, executionId, keyPointIndex);
        return Ok(new { secret });
    }

    [HttpGet("{executionId:long}/map-keypoints")]
    public ActionResult<List<TouristKeyPointMapDto>> GetKeyPointsForMap(long executionId)
    {
        var touristId = User.PersonId();
        var result = _tourExecutionService.GetKeyPointsForMap(touristId, executionId);
        return Ok(result);
    }

}
