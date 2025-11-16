using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourProblemRepository
    {
        PagedResult<TourProblem> GetByTourist(int touristId, int page, int pageSize);
        TourProblem Get(int id);
        TourProblem Create(TourProblem entity);
        TourProblem Update(TourProblem entity);
        void Delete(int id);
    }
}
