using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    PagedResult<Tour> GetPagedByAuthor(int page, int pageSize, long authorId);
    PagedResult<Tour> GetPublishedTours(int page, int pageSize);
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);
    List<Tour> GetAll();
    IEnumerable<Tour> GetPublishedWithKeyPoints();
}