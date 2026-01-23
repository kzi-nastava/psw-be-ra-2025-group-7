using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourRequestService
    {
        TourRequestDto Create(CreateTourRequestDto dto, long touristId);
        TourRequestDto Update(UpdateTourRequestDto dto, long touristId);
        void Delete(long id, long touristId);

        PagedResult<TourRequestDto> GetByTourist(int page, int pageSize, long touristId);
        TourRequestDto GetById(long id, long touristId);

        TourRequestDto Close(long id, long touristId);

        List<TourRequestResponseDto> GetResponses(long tourRequestId, long touristId);
        TourRequestDto AcceptResponse(AcceptResponseDto dto, long touristId);
        void DeclineResponse(long responseId, long touristId);

        TourRequestDto ExpressInterest(long tourRequestId, long responseId, long touristId);
    }
}