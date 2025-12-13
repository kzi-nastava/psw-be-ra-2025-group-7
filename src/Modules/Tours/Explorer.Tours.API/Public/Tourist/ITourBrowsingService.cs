using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourBrowsingService
{
    PagedResult<TourPreviewDto> GetPublishedTourPreviews(int page, int pageSize);
    TourDto GetFullTourDetails(long tourId, long userId);
}
