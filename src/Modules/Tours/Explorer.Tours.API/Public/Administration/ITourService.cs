using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPagedByAuthor(int page, int pageSize, long authorId);
        TourDto Create(TourDto tour);
        TourDto Update(TourDto tour);
        void Delete(long id, long authorId);
        TourDto Publish(long id, long authorId);
        TourDto Archive(long id, long authorId);
        TourDto Reactivate(long id, long authorId, int newStatus);
    }
}
