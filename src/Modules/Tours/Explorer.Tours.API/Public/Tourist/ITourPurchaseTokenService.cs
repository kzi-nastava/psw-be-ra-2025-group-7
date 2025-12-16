using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourPurchaseTokenService
{
    TourPurchaseTokenDto Create(long userId, long tourId);
    TourPurchaseTokenDto? GetByUserAndTour(long userId, long tourId);
    PagedResult<TourPurchaseTokenDto> GetPagedByUser(int page, int pageSize, long userId);
    bool HasUserPurchasedTour(long userId, long tourId);
}
