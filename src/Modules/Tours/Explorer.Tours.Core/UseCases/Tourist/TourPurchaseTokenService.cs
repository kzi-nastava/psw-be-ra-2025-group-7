using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourPurchaseTokenService : ITourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly IWalletInternalService _walletInternalService;

    public TourPurchaseTokenService(
        ITourPurchaseTokenRepository repository,
        ITourRepository tourRepository,
        IMapper mapper,
        IWalletInternalService walletInternalService)
    {
        _repository = repository;
        _tourRepository = tourRepository;
        _mapper = mapper;
        _walletInternalService = walletInternalService;
    }

    public TourPurchaseTokenDto Create(long userId, long tourId)
    {
        // Check if already purchased
        if (_repository.HasUserPurchasedTour(userId, tourId))
        {
            throw new InvalidOperationException("User has already purchased this tour.");
        }

        // Get the tour and validate it can be purchased (domain validation first)
        var tour = _tourRepository.Get(tourId);
        tour.ValidatePurchase();

        // If tour has a price > 0, check wallet balance and withdraw
        if (tour.Price > 0)
        {
            var balance = _walletInternalService.GetBalance(userId);
            if (balance < tour.Price)
            {
                throw new InvalidOperationException("Insufficient funds");
            }

            // deduct funds
            _walletInternalService.Withdraw(userId, tour.Price);
        }

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
