using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourJournalRepository
{
    PagedResult<TourJournal> GetPagedByTourist(long touristId, int page, int pageSize);
    TourJournal Create(TourJournal tourJournal);
    TourJournal Update(TourJournal tourJournal);
    void Delete(long id);
    TourJournal Get(long id);
}
