using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution? GetActiveExecutionForTourist(long touristId, long tourId);
    PagedResult<TourExecution> GetByTouristId(long touristId, int page, int pageSize);
    List<TourExecution> GetAllActiveExecutionsByTourist(long touristId);
    TourExecution Get(long id);
    TourExecution Create(TourExecution execution);
    TourExecution Update(TourExecution execution);
    void Delete(long id);
}
