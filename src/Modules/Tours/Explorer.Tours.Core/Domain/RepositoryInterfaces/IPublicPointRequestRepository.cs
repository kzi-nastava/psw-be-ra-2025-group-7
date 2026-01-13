using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IPublicPointRequestRepository
    {
        PublicPointRequest Create(PublicPointRequest entity);
        PublicPointRequest Update(PublicPointRequest entity);

        PublicPointRequest? Get(long id);

        // da spreči duple Pending requestove za istu tačku
        PublicPointRequest? GetPending(long tourId, int keyPointIndex);

        // admin lista (pending/approved/rejected)
        PagedResult<PublicPointRequest> GetPaged(int page, int pageSize, PublicPointRequestStatus? status = null);

        // opcionalno: sve za jednog autora (da autor vidi istoriju)
        PagedResult<PublicPointRequest> GetByAuthor(long authorId, int page, int pageSize);
    }
}
