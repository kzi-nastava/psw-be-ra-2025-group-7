using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourReview : Entity
{
    public long TouristId { get; init; }
    public long TourId { get; init; }
    public long TourExecutionId { get; init; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }
    public double TourProgressPercentage { get; init; }
    
    private readonly List<string> _imageUrls;
    public IReadOnlyList<string> ImageUrls => _imageUrls.AsReadOnly();

    // Navigation properties
    public Tour Tour { get; init; }
    public TourExecution TourExecution { get; init; }

    // EF Core constructor
    private TourReview()
    {
        _imageUrls = new List<string>();
    }

    public TourReview(long touristId, long tourId, long tourExecutionId, int rating, string comment, 
                      double progressPercentage, List<string> imageUrls = null)
    {
        TouristId = touristId;
        TourId = tourId;
        TourExecutionId = tourExecutionId;
        Rating = rating;
        Comment = comment ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        TourProgressPercentage = progressPercentage;
        _imageUrls = imageUrls ?? new List<string>();
        
        Validate();
    }

    public void Update(int rating, string comment, List<string> imageUrls = null)
    {
        Rating = rating;
        Comment = comment ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
        
        if (imageUrls != null)
        {
            _imageUrls.Clear();
            _imageUrls.AddRange(imageUrls);
        }
        
        Validate();
    }

    /// <summary>
    /// Validates if a tourist is eligible to create a review based on tour execution progress and activity.
    /// Throws InvalidOperationException with detailed message if not eligible.
    /// </summary>
    /// <param name="execution">The tour execution to validate against (must have Tour navigation property loaded)</param>
    public static void ValidateEligibilityForCreation(TourExecution execution)
    {
        if (execution == null)
            throw new ArgumentNullException(nameof(execution));
        
        if (execution.Tour == null)
            throw new InvalidOperationException("Tour navigation property must be loaded for validation.");

        if (execution.CanLeaveReview())
            return; // Validation passed

        // If we reach here, validation failed - provide detailed error message
        var progress = execution.CalculateProgressPercentage();
        var daysSinceLastActivity = (DateTime.UtcNow - execution.LastActivity).TotalDays;

        if (progress <= 35.0)
        {
            throw new InvalidOperationException(
                $"You must complete at least 35% of the tour to leave a review. " +
                $"Current progress: {progress:F1}%");
        }

        if (daysSinceLastActivity > 7.0)
        {
            throw new InvalidOperationException(
                $"More than 7 days have passed since your last activity on this tour. " +
                $"Reviews can only be left within 7 days of last activity.");
        }

        // Fallback error (should not reach here if CanLeaveReview logic is correct)
        throw new InvalidOperationException(
            "You are not eligible to leave a review for this tour at this time.");
    }

    /// <summary>
    /// Validates if a review can be updated based on tour execution activity.
    /// Throws InvalidOperationException if more than 7 days have passed since last activity.
    /// </summary>
    /// <param name="execution">The tour execution to validate against (must have Tour navigation property loaded)</param>
    public static void ValidateEligibilityForUpdate(TourExecution execution)
    {
        if (execution == null)
            throw new ArgumentNullException(nameof(execution));
        
        if (execution.Tour == null)
            throw new InvalidOperationException("Tour navigation property must be loaded for validation.");

        if (execution.CanLeaveReview())
            return; // Validation passed

        // If we reach here, the 7-day window has passed
        var daysSinceLastActivity = (DateTime.UtcNow - execution.LastActivity).TotalDays;
        throw new InvalidOperationException(
            $"More than 7 days have passed since your last activity on this tour. " +
            $"Reviews can no longer be modified.");
    }

    private void Validate()
    {
        if (Rating < 1 || Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");
        
        if (string.IsNullOrWhiteSpace(Comment))
            throw new ArgumentException("Comment is required.");
        
        if (Comment.Length > 2000)
            throw new ArgumentException("Comment cannot exceed 2000 characters.");
        
        if (TourProgressPercentage < 0 || TourProgressPercentage > 100)
            throw new ArgumentException("Tour progress percentage must be between 0 and 100.");
        
        if (_imageUrls.Count > 10)
            throw new ArgumentException("Maximum 10 images allowed per review.");
        
        // Validate image URLs
        foreach (var url in _imageUrls)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.");
            
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException($"Invalid image URL: {url}");
            }
        }
    }
}
