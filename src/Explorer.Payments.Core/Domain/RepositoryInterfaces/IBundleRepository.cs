using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IBundleRepository
    {
        Bundle Get(long id);
        List<Bundle> GetByAuthor(long authorId);
        Bundle Create(Bundle bundle);
        Bundle Update(Bundle bundle);
        void Delete(long id);
        void Save(Bundle bundle);
        List<Bundle> GetPublished();
    }
}
