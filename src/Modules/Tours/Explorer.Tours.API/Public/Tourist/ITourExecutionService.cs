using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourExecutionService
{
    TourExecutionDto StartTour(long touristId, StartTourExecutionDto dto);
    TourExecutionDto CompleteTour(long touristId, long executionId);
    TourExecutionDto AbandonTour(long touristId, long executionId);
    TourExecutionDto UnlockKeyPoint(long touristId, long executionId, UnlockKeyPointDto dto);
    TourExecutionDto GetActiveExecution(long touristId, long tourId);
    PagedResult<TourExecutionDto> GetExecutionHistory(long touristId, int page, int pageSize);
    string GetKeyPointSecret(long touristId, long executionId, int keyPointIndex);
    TourExecutionDto UpdateLastActivity(long touristId, long executionId);
    double GetProgressPercentage(long touristId, long executionId);
    KeyPointProximityCheckResultDto CheckKeyPointProximity(long touristId, long executionId, CheckKeyPointProximityDto dto);
    List<TouristKeyPointMapDto> GetKeyPointsForMap(long touristId, long executionId);


}
