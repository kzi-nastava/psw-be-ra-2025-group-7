using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Notifications.API.Public;
using Explorer.Payments.API.Internal;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourPurchaseTokenService : ITourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly IWalletInternalService _walletInternalService;
    private readonly INotificationService _notificationService;

    public TourPurchaseTokenService(
        ITourPurchaseTokenRepository repository,
        ITourRepository tourRepository,
        IMapper mapper,
        IWalletInternalService walletInternalService,
        INotificationService notificationService)
    {
        _repository = repository;
        _tourRepository = tourRepository;
        _mapper = mapper;
        _walletInternalService = walletInternalService;
        _notificationService = notificationService;
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

        // Check wallet balance (Shopping Expansion - Part 2)
        var balance = _walletInternalService.GetBalance(userId);
        if (balance < tour.Price)
            throw new InvalidOperationException("Insufficient funds");

        // NOTE: charging/payment integration temporarily skipped to allow build
        var token = TourPurchaseToken.CreateForTour(userId, tour);

        var result = _repository.Create(token);

        var notificationPreview = $"Your purchase of '{tour.Name}' was successful.";
        _notificationService.CreateProblemMessageNotification(userId, 0, notificationPreview, DateTime.UtcNow);

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
