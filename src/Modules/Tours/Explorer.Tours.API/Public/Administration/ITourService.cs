using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPagedByAuthor(int page, int pageSize, long authorId);
        TourDto Create(TourDto tour);
        TourDto Update(TourDto tour);
        void Delete(long id, long authorId);

        // Kartica 3 – ključne tačke
        TourDto AddKeyPoint(long tourId, long authorId, KeyPointDto keyPoint);
        TourDto UpdateKeyPoint(long tourId, long authorId, int index, KeyPointDto keyPoint);
        TourDto RemoveKeyPoint(long tourId, long authorId, int index);

        // Životni ciklus ture (priča člana 1)
        TourDto Publish(long id, long authorId);
        TourDto Archive(long id, long authorId);
        TourDto Reactivate(long id, long authorId, int newStatus);
    }
}
