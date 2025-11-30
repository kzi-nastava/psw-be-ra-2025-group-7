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
        PagedResult<TourProblem> GetByAuthor(int authorId,int pageNumber, int pageSize);
        PagedResult<TourProblem> GetByTourist(int touristId,int pageNumber, int pageSize);
        TourProblem Get(int id);
        TourProblem Create(TourProblem tourProblem);
        TourProblem Update(TourProblem tourProblem);
        void Delete(int id);
    }
}
