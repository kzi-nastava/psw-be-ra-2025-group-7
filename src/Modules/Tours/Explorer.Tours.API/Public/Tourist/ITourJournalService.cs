using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourJournalService
{
    PagedResult<TourJournalDto> GetPagedByTourist(long touristId, int page, int pageSize);
    TourJournalDto Create(TourJournalDto tourJournal);
    TourJournalDto Update(TourJournalDto tourJournal);
    void Delete(long id);
}
