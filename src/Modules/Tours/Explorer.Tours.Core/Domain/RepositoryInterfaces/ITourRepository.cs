using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    PagedResult<Tour> GetPagedByAuthor(int page, int pageSize, long authorId);
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);
    List<Tour> GetAll();
    IEnumerable<Tour> GetPublishedWithKeyPoints();
}