using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRequestRepository
    {
        TourRequest Get(long id);
        TourRequest Create(TourRequest tourRequest);
        TourRequest Update(TourRequest tourRequest);
        void Delete(long id);
        PagedResult<TourRequest> GetPagedByTourist(int page, int pageSize, long touristId);
        PagedResult<TourRequest> GetOpenRequests(int page, int pageSize);
        List<TourRequest> GetExpiringSoon(int daysThreshold);
        int GetRequestCountForToday(long touristId);
        TourRequestResponse GetResponse(long id);
        TourRequestResponse CreateResponse(TourRequestResponse response);
        TourRequestResponse UpdateResponse(TourRequestResponse response);
        void DeleteResponse(long id);
        List<TourRequestResponse> GetResponsesByRequest(long tourRequestId);
        TourRequestResponse GetResponseByAuthorAndRequest(long authorId, long tourRequestId);
        int GetResponseCount(long tourRequestId);
        void AcceptResponse(long responseId, long tourRequestId);

        PagedResult<TourRequest> GetOpenRequestsFiltered(int page, int pageSize, decimal? minBudget, decimal? maxBudget, int? difficulty);

        List<TourRequestResponse> GetResponsesByAuthor(long authorId);

    }
}