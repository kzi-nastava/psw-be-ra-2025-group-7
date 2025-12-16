using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourPurchaseTokenRepository
{
    TourPurchaseToken Create(TourPurchaseToken token);
    TourPurchaseToken? GetByUserAndTour(long userId, long tourId);
    PagedResult<TourPurchaseToken> GetPagedByUser(int page, int pageSize, long userId);
    bool HasUserPurchasedTour(long userId, long tourId);
}
