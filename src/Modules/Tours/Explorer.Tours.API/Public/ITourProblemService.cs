using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourProblemService
    {
        PagedResult<TourProblemDto> GetTouristProblemsPages(int touristid, int page, int pageSize);
        TourProblemDto Create(TourProblemDto tourProblem, int touristId);
        TourProblemDto Update(TourProblemDto tourProblem, int touristId);
        PagedResult<TourProblemDto> GetAllProblems(int page, int pageSize);
        void Delete(int id, int touristId);
    }
}
