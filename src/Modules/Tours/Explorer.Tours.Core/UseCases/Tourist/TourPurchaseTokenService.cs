using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourPurchaseTokenService : ITourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourPurchaseTokenService(
        ITourPurchaseTokenRepository repository, 
        ITourRepository tourRepository,
        IMapper mapper)
    {
        _repository = repository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public TourPurchaseTokenDto Create(long userId, long tourId)
    {
        // Check if already purchased
        if (_repository.HasUserPurchasedTour(userId, tourId))
        {
            throw new InvalidOperationException("User has already purchased this tour.");
        }

        // Get the tour and validate it can be purchased
        var tour = _tourRepository.Get(tourId);
        
        // Use factory method to enforce business rules
        var token = TourPurchaseToken.CreateForTour(userId, tour);
        
        var result = _repository.Create(token);
        return _mapper.Map<TourPurchaseTokenDto>(result);
    }

    public TourPurchaseTokenDto? GetByUserAndTour(long userId, long tourId)
    {
        var token = _repository.GetByUserAndTour(userId, tourId);
        return token != null ? _mapper.Map<TourPurchaseTokenDto>(token) : null;
    }

    public PagedResult<TourPurchaseTokenDto> GetPagedByUser(int page, int pageSize, long userId)
    {
        var result = _repository.GetPagedByUser(page, pageSize, userId);
        return new PagedResult<TourPurchaseTokenDto>(
            _mapper.Map<List<TourPurchaseTokenDto>>(result.Results),
            result.TotalCount
        );
    }

    public bool HasUserPurchasedTour(long userId, long tourId)
    {
        return _repository.HasUserPurchasedTour(userId, tourId);
    }
}
