using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourProblemService
    {
        PagedResult<TourProblemDto> GetByAuthor(int authorId,int page, int pageSize);
        PagedResult<TourProblemDto> GetTouristProblemsPages(int touristid, int page, int pageSize);
        TourProblemDto Create(TourProblemDto tourProblem, int touristId);
        TourProblemDto Update(TourProblemDto tourProblem, int touristId);
        TourProblemDto AddAuthorReply(int tourProblemId, int authorId, string message);
        void Delete(int id, int touristId);
    }
}
