using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseTokenRepository _purchaseTokenRepository;
    private readonly IMapper _mapper;
    
    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        ITourPurchaseTokenRepository purchaseTokenRepository,
        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _purchaseTokenRepository = purchaseTokenRepository;
        _mapper = mapper;
    }

    public TourExecutionDto StartTour(long touristId, StartTourExecutionDto dto)
    {
        // Proveri da li tura postoji
        var tour = _tourRepository.Get(dto.TourId);

        // Proveri da li je tura kupljena
        var purchaseToken = _purchaseTokenRepository.GetByUserAndTour(touristId, dto.TourId);
        if (purchaseToken == null)
            throw new InvalidOperationException("Tour must be purchased before starting.");

        // Proveri da li je tura u odgovarajućem statusu (Published ili Archived)
        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Only published or archived tours can be started.");

        // Proveri da li već postoji aktivna sesija za ovu turu
        var existingExecution = _executionRepository.GetActiveExecutionForTourist(touristId, dto.TourId);
        if (existingExecution != null && existingExecution.TourId != dto.TourId)
            throw new InvalidOperationException("There is already an active tour execution for this tour.");

        // Kreiraj novu sesiju
        var execution = new TourExecution(touristId, dto.TourId, dto.Latitude, dto.Longitude);
        var createdExecution = _executionRepository.Create(execution);
        
        var result = _mapper.Map<TourExecutionDto>(createdExecution);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public TourExecutionDto CompleteTour(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only complete your own tour executions.");

        execution.Complete();
        var updatedExecution = _executionRepository.Update(execution);
        
        var result = _mapper.Map<TourExecutionDto>(updatedExecution);
        var tour = _tourRepository.Get(updatedExecution.TourId);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public TourExecutionDto AbandonTour(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only abandon your own tour executions.");

        execution.Abandon();
        var updatedExecution = _executionRepository.Update(execution);
        
        var result = _mapper.Map<TourExecutionDto>(updatedExecution);
        var tour = _tourRepository.Get(updatedExecution.TourId);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public TourExecutionDto UnlockKeyPoint(long touristId, long executionId, UnlockKeyPointDto dto)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only unlock key points on your own tour executions.");

        var tour = _tourRepository.Get(execution.TourId);

        // Proveri da li je indeks validan
        if (dto.KeyPointIndex < 0 || dto.KeyPointIndex >= tour.KeyPoints.Count)
            throw new ArgumentException("Invalid key point index.");

        var keyPoint = tour.KeyPoints[dto.KeyPointIndex];
        
        // Proveri da li je turista dovoljno blizu ključne tačke (100m)
        const double maxDistanceKm = 0.1; // 100 metara
        var distance = CalculateDistance(dto.Latitude, dto.Longitude, keyPoint.Latitude, keyPoint.Longitude);
        
        if (distance > maxDistanceKm)
            throw new InvalidOperationException($"You must be within 100 meters of the key point to unlock it. Current distance: {distance * 1000:F0} meters.");

        // Otključaj tačku
        execution.UnlockKeyPoint(dto.KeyPointIndex);
        var updatedExecution = _executionRepository.Update(execution);
        
        var result = _mapper.Map<TourExecutionDto>(updatedExecution);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public TourExecutionDto GetActiveExecution(long touristId, long tourId)
    {
        var execution = _executionRepository.GetActiveExecutionForTourist(touristId, tourId);
        if (execution == null)
            throw new NotFoundException("No active tour execution found for this tour.");

        var result = _mapper.Map<TourExecutionDto>(execution);
        var tour = _tourRepository.Get(execution.TourId);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public PagedResult<TourExecutionDto> GetExecutionHistory(long touristId, int page = 1, int pageSize = 20)
    {
        var pagedResult = _executionRepository.GetByTouristId(touristId, page, pageSize);
        
        var dtos = new List<TourExecutionDto>();
        foreach (var execution in pagedResult.Results)
        {
            var dto = _mapper.Map<TourExecutionDto>(execution);
            var tour = _tourRepository.Get(execution.TourId);
            dto.Tour = _mapper.Map<TourDto>(tour);
            dtos.Add(dto);
        }
        
        return new PagedResult<TourExecutionDto>(dtos, pagedResult.TotalCount);
    }

    public string GetKeyPointSecret(long touristId, long executionId, int keyPointIndex)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only access secrets from your own tour executions.");

        var tour = _tourRepository.Get(execution.TourId);

        // Proveri da li je indeks validan
        if (keyPointIndex < 0 || keyPointIndex >= tour.KeyPoints.Count)
            throw new ArgumentException("Invalid key point index.");

        // Proveri da li je tačka otključana
        if (!execution.IsKeyPointUnlocked(keyPointIndex))
            throw new InvalidOperationException("This key point has not been unlocked yet. You must reach the key point first.");

        var keyPoint = tour.KeyPoints[keyPointIndex];
        return keyPoint.Secret;
    }

    public TourExecutionDto UpdateLastActivity(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only update your own tour executions.");

        execution.UpdateLastActivity();
        var updatedExecution = _executionRepository.Update(execution);
        
        var result = _mapper.Map<TourExecutionDto>(updatedExecution);
        var tour = _tourRepository.Get(updatedExecution.TourId);
        result.Tour = _mapper.Map<TourDto>(tour);
        
        return result;
    }

    public double GetProgressPercentage(long touristId, long executionId)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only check progress on your own tour executions.");

        return execution.CalculateProgressPercentage();
    }

    public KeyPointProximityCheckResultDto CheckKeyPointProximity(long touristId, long executionId, CheckKeyPointProximityDto dto)
    {
        var execution = _executionRepository.Get(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only check proximity on your own tour executions.");

        // Proveri blizinu i automatski otključaj ako je blizu
        var proximityResult = execution.CheckProximityAndUnlock(dto.Latitude, dto.Longitude);

        // Sačuvaj promene (LastActivity je uvek ažurirano, možda i nova tačka otključana)
        var updatedExecution = _executionRepository.Update(execution);

        var result = new KeyPointProximityCheckResultDto
        {
            TourExecution = _mapper.Map<TourExecutionDto>(updatedExecution)
        };

        // Dodaj Tour u DTO
        result.TourExecution.Tour = _mapper.Map<TourDto>(execution.Tour);

        if (proximityResult.HasValue)
        {
            var (keyPointIndex, distanceKm, wasAlreadyUnlocked) = proximityResult.Value;
            var keyPoint = execution.Tour.KeyPoints[keyPointIndex];
            
            result.IsNearKeyPoint = true;
            result.KeyPointIndex = keyPointIndex;
            result.KeyPointName = keyPoint.Name;
            result.DistanceInMeters = distanceKm * 1000; // Convert to meters
            result.WasAlreadyUnlocked = wasAlreadyUnlocked;
            result.UnlockedAt = execution.GetKeyPointUnlockTime(keyPointIndex);
            result.KeyPointLatitude = keyPoint.Latitude;
            result.KeyPointLongitude = keyPoint.Longitude;
        }
        else
        {
            result.IsNearKeyPoint = false;
        }

        return result;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public List<TouristKeyPointMapDto> GetKeyPointsForMap(long touristId, long executionId)
    {
        var execution = _executionRepository.GetExecutionWithTourAndKeyPoints(touristId, executionId)
                        ?? throw new NotFoundException("Execution not found");

        if (execution.Tour?.KeyPoints == null || execution.Tour.KeyPoints.Count == 0)
            return new List<TouristKeyPointMapDto>();

        // Sledeca neotkljucana KP
        int? nextIndex = null;
        for (int i = 0; i < execution.Tour.KeyPoints.Count; i++)
        {
            if (!execution.IsKeyPointUnlocked(i))
            {
                nextIndex = i;
                break;
            }
        }

        // Mapiraj sve KP u DTO za mapu
        var result = execution.Tour.KeyPoints
            .Select((kp, index) => new TouristKeyPointMapDto
            {
                Index = index,
                Latitude = kp.Latitude,
                Longitude = kp.Longitude,
                Name = kp.Name,
                IsUnlocked = execution.IsKeyPointUnlocked(index),
                IsNext = index == nextIndex,
                Secret = execution.IsKeyPointUnlocked(index) ? kp.Secret : string.Empty
            })
            .ToList();

        return result;
    }

}
