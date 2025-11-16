using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourProblemService
    {
        PagedResult<TourProblemDto> GetTouristPages(int touristid,int page, int pageSize);
        TourProblemDto Create(TourProblemDto tourProblem,int touristId);
        TourProblemDto Update(TourProblemDto tourProblem,int touristId);
        void Delete(int id,int touristId);
    }
}
