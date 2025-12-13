using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourBrowsingService : ITourBrowsingService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseTokenRepository _purchaseRepository;
    private readonly IMapper _mapper;

    public TourBrowsingService(
        ITourRepository tourRepository,
        ITourPurchaseTokenRepository purchaseRepository,
        IMapper mapper)
    {
        _tourRepository = tourRepository;
        _purchaseRepository = purchaseRepository;
        _mapper = mapper;
    }

    public PagedResult<TourPreviewDto> GetPublishedTourPreviews(int page, int pageSize)
    {
        // Get only published tours for browsing
        var result = _tourRepository.GetPublishedTours(page, pageSize);
        
        return new PagedResult<TourPreviewDto>(
            _mapper.Map<List<TourPreviewDto>>(result.Results),
            result.TotalCount
        );
    }

    public TourDto GetFullTourDetails(long tourId, long userId)
    {
        // Check if user has purchased the tour
        if (!_purchaseRepository.HasUserPurchasedTour(userId, tourId))
        {
            throw new UnauthorizedAccessException(
                "You must purchase this tour to view full details.");
        }

        var tour = _tourRepository.Get(tourId);
        return _mapper.Map<TourDto>(tour);
    }
}
