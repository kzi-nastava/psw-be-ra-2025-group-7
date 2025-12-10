using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IPublicPointRequestRepository
    {
        PublicPointRequest Create(PublicPointRequest entity);
        // Po potrebi kasnije dodaš Get/Update/Delete itd.
    }
}
