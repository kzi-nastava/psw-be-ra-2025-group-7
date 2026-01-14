using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourReviewService : ITourReviewService
{
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseTokenRepository _purchaseTokenRepository;
    private readonly IMapper _mapper;

    public TourReviewService(
        ITourReviewRepository reviewRepository,
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        ITourPurchaseTokenRepository purchaseTokenRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _purchaseTokenRepository = purchaseTokenRepository;
        _mapper = mapper;
    }

    public TourReviewDto CreateReview(long touristId, CreateTourReviewDto dto)
    {
        // Provera da li je tura kupljena
        ValidateTourPurchase(touristId, dto.TourId);

        // Provera da li već postoji recenzija za ovu turu
        var existingReview = _reviewRepository.GetByTouristAndTour(touristId, dto.TourId);
        if (existingReview != null)
            throw new InvalidOperationException("You have already reviewed this tour. Use update instead.");

        // Učitaj izvršenje ture i proveri vlasništvo
        var execution = GetAndValidateExecution(dto.TourExecutionId, touristId);

        // Proveri da li može da ostavi recenziju - domain validation
        TourReview.ValidateEligibilityForCreation(execution);

        // Izračunaj procenat progresa u trenutku kreiranja recenzije
        var progressPercentage = execution.CalculateProgressPercentage();

        // Kreiraj recenziju
        var review = new TourReview(
            touristId,
            dto.TourId,
            dto.TourExecutionId,
            dto.Rating,
            dto.Comment,
            progressPercentage,
            dto.ImageUrls
        );

        var createdReview = _reviewRepository.Create(review);
        var result = _mapper.Map<TourReviewDto>(createdReview);
        result.Tour = _mapper.Map<TourDto>(execution.Tour);

        return result;
    }

    public TourReviewDto UpdateReview(long touristId, long reviewId, UpdateTourReviewDto dto)
    {
        var review = _reviewRepository.Get(reviewId);

        if (review.TouristId != touristId)
            throw new ForbiddenException("You can only update your own reviews.");

        // Učitaj izvršenje ture (sa Tour navigation property)
        var execution = _executionRepository.Get(review.TourExecutionId);

        // Proveri da li i dalje može da ažurira recenziju - domain validation
        TourReview.ValidateEligibilityForUpdate(execution);

        review.Update(dto.Rating, dto.Comment, dto.ImageUrls);
        var updatedReview = _reviewRepository.Update(review);

        var result = _mapper.Map<TourReviewDto>(updatedReview);
        result.Tour = _mapper.Map<TourDto>(execution.Tour);

        return result;
    }

    public void DeleteReview(long touristId, long reviewId)
    {
        var review = _reviewRepository.Get(reviewId);

        if (review.TouristId != touristId)
            throw new ForbiddenException("You can only delete your own reviews.");

        _reviewRepository.Delete(reviewId);
    }

    public TourReviewDto GetReviewByTouristAndTour(long touristId, long tourId)
    {
        var review = _reviewRepository.GetByTouristAndTour(touristId, tourId);
        if (review == null)
            throw new NotFoundException("Review not found.");

        var result = _mapper.Map<TourReviewDto>(review);
        var tour = _tourRepository.Get(tourId);
        result.Tour = _mapper.Map<TourDto>(tour);

        return result;
    }

    public PagedResult<TourReviewDto> GetReviewsForTour(long tourId, int page, int pageSize)
    {
        var pagedResult = _reviewRepository.GetByTourId(tourId, page, pageSize);
        
        var tour = _tourRepository.Get(tourId);
        var dtos = pagedResult.Results.Select(review =>
        {
            var dto = _mapper.Map<TourReviewDto>(review);
            dto.Tour = _mapper.Map<TourDto>(tour);
            return dto;
        }).ToList();
        
        return new PagedResult<TourReviewDto>(dtos, pagedResult.TotalCount);
    }

    public PagedResult<TourReviewDto> GetReviewsByTourist(long touristId, int page, int pageSize)
    {
        var pagedResult = _reviewRepository.GetByTouristId(touristId, page, pageSize);
        
        var dtos = pagedResult.Results.Select(review =>
        {
            var dto = _mapper.Map<TourReviewDto>(review);
            var tour = _tourRepository.Get(review.TourId);
            dto.Tour = _mapper.Map<TourDto>(tour);
            return dto;
        }).ToList();
        
        return new PagedResult<TourReviewDto>(dtos, pagedResult.TotalCount);
    }

    public double GetAverageRatingForTour(long tourId)
    {
        return _reviewRepository.GetAverageRatingForTour(tourId);
    }

    public bool CanLeaveReview(long touristId, long tourExecutionId)
    {
        var execution = GetAndValidateExecution(tourExecutionId, touristId);
        return execution.CanLeaveReview();
    }

    /// <summary>
    /// Validates that the tourist has purchased the tour.
    /// Throws InvalidOperationException if not purchased.
    /// </summary>
    private void ValidateTourPurchase(long touristId, long tourId)
    {
        var purchaseToken = _purchaseTokenRepository.GetByUserAndTour(touristId, tourId);
        if (purchaseToken == null)
            throw new InvalidOperationException("You must purchase the tour before leaving a review.");
    }

    /// <summary>
    /// Gets the tour execution and validates that it belongs to the tourist.
    /// The returned execution has the Tour navigation property loaded.
    /// Throws ForbiddenException if the execution doesn't belong to the tourist.
    /// </summary>
    private TourExecution GetAndValidateExecution(long tourExecutionId, long touristId)
    {
        var execution = _executionRepository.Get(tourExecutionId);
        
        if (execution.TouristId != touristId)
            throw new ForbiddenException("You can only review your own tour executions.");
        
        // Repository should load Tour with KeyPoints via Include
        return execution;
    }
}
