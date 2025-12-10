using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IPublicPointRequestRepository
    {
        PublicPointRequest Create(PublicPointRequest entity);
        PublicPointRequest Get(long id);
        PublicPointRequest Update(PublicPointRequest entity);
        PagedResult<PublicPointRequest> GetPaged(int page, int pageSize);
        List<PublicPointRequest> GetPendingRequests();
    }
}