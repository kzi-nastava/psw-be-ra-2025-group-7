using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourExecution : Entity
{
    public long TouristId { get; init; }
    public long TourId { get; init; }
    public Tour Tour { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? AbandonedAt { get; private set; }
    public TourExecutionStatus Status { get; private set; }
    
    public double StartLatitude { get; init; }
    public double StartLongitude { get; init; }
    
    // Lista indeksa otključanih KeyPoint-ova
    private readonly List<int> _unlockedKeyPointIndices;
    public IReadOnlyList<int> UnlockedKeyPointIndices => _unlockedKeyPointIndices.AsReadOnly();
    
    // EF Core constructor
    private TourExecution() 
    {
        _unlockedKeyPointIndices = new List<int>();
    }

    public TourExecution(long touristId, long tourId, double latitude, double longitude)
    {        
        ValidateCoordinates(latitude, longitude);
        
        TouristId = touristId;
        TourId = tourId;
        StartedAt = DateTime.UtcNow;
        Status = TourExecutionStatus.Active;
        StartLatitude = latitude;
        StartLongitude = longitude;
        _unlockedKeyPointIndices = new List<int>();
    }

    public void Complete()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException($"Cannot complete tour execution with status {Status}.");
        
        Status = TourExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Abandon()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException($"Cannot abandon tour execution with status {Status}.");
        
        Status = TourExecutionStatus.Abandoned;
        AbandonedAt = DateTime.UtcNow;
    }

    public void UnlockKeyPoint(int keyPointIndex)
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Cannot unlock key points on inactive tour execution.");
        
        if (keyPointIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(keyPointIndex), "Key point index cannot be negative.");
        
        if (_unlockedKeyPointIndices.Contains(keyPointIndex))
            return; // Already unlocked, idempotent operation
        
        _unlockedKeyPointIndices.Add(keyPointIndex);
    }

    public bool IsKeyPointUnlocked(int keyPointIndex)
    {
        return _unlockedKeyPointIndices.Contains(keyPointIndex);
    }

    private void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
        
        if (longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
    }
}

public enum TourExecutionStatus
{
    Active,
    Completed,
    Abandoned
}
