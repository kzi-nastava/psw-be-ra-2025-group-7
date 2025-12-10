using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IPublicPointRequestService
    {
        PagedResult<PublicPointRequestDto> GetPaged(int page, int pageSize);
        List<PublicPointRequestDto> GetAllPending();
        PublicPointRequestDto ApproveRequest(long requestId, string? comment);
        PublicPointRequestDto RejectRequest(long requestId, string comment);
    }
}
