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
    
    // Praćenje aktivnosti za recenzije
    public DateTime LastActivity { get; private set; }
    
    // Lista indeksa otključanih KeyPoint-ova
    private readonly List<int> _unlockedKeyPointIndices;
    public IReadOnlyList<int> UnlockedKeyPointIndices => _unlockedKeyPointIndices.AsReadOnly();
    
    // Mapiranje indeksa KeyPoint-a -> vreme otključavanja
    private readonly Dictionary<int, DateTime> _keyPointUnlockTimes;
    public IReadOnlyDictionary<int, DateTime> KeyPointUnlockTimes => _keyPointUnlockTimes;
    
    // EF Core constructor
    private TourExecution() 
    {
        _unlockedKeyPointIndices = new List<int>();
        _keyPointUnlockTimes = new Dictionary<int, DateTime>();
    }

    public TourExecution(long touristId, long tourId, double latitude, double longitude)
    {
        ValidateCoordinates(latitude, longitude);
        
        TouristId = touristId;
        TourId = tourId;
        StartedAt = DateTime.UtcNow;
        LastActivity = DateTime.UtcNow;
        Status = TourExecutionStatus.Active;
        StartLatitude = latitude;
        StartLongitude = longitude;
        _unlockedKeyPointIndices = new List<int>();
        _keyPointUnlockTimes = new Dictionary<int, DateTime>();
    }

    public void Complete()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException($"Cannot complete tour execution with status {Status}.");
        
        Status = TourExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        LastActivity = DateTime.UtcNow;
    }

    public void Abandon()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException($"Cannot abandon tour execution with status {Status}.");
        
        Status = TourExecutionStatus.Abandoned;
        AbandonedAt = DateTime.UtcNow;
        LastActivity = DateTime.UtcNow;
    }

    public void UnlockKeyPoint(int keyPointIndex)
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Cannot unlock key points on inactive tour execution.");
        
        if (keyPointIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(keyPointIndex), "Key point index cannot be negative.");
        
        if (_unlockedKeyPointIndices.Contains(keyPointIndex))
            return; // Already unlocked, idempotent operation
        
        var unlockTime = DateTime.UtcNow;
        _unlockedKeyPointIndices.Add(keyPointIndex);
        _keyPointUnlockTimes[keyPointIndex] = unlockTime;
        LastActivity = unlockTime;
    }

    public void UpdateLastActivity()
    {
        if (Status == TourExecutionStatus.Active)
        {
            LastActivity = DateTime.UtcNow;
        }
    }

    public bool IsKeyPointUnlocked(int keyPointIndex)
    {
        return _unlockedKeyPointIndices.Contains(keyPointIndex);
    }

    public DateTime? GetKeyPointUnlockTime(int keyPointIndex)
    {
        return _keyPointUnlockTimes.TryGetValue(keyPointIndex, out var unlockTime) ? unlockTime : null;
    }

    /// <summary>
    /// Checks if the tourist is near any key points and automatically unlocks them.
    /// Updates LastActivity regardless of result.
    /// </summary>
    /// <param name="latitude">Tourist's current latitude</param>
    /// <param name="longitude">Tourist's current longitude</param>
    /// <param name="maxDistanceKm">Maximum distance in kilometers (default 0.1 = 100m)</param>
    /// <returns>Tuple of (keyPointIndex, distanceKm, wasAlreadyUnlocked) if near a key point, otherwise null</returns>
    public (int keyPointIndex, double distanceKm, bool wasAlreadyUnlocked)? CheckProximityAndUnlock(
        double latitude, 
        double longitude, 
        double maxDistanceKm = 0.1)
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Cannot check proximity on inactive tour execution.");
        
        ValidateCoordinates(latitude, longitude);
        
        if (Tour?.KeyPoints == null || Tour.KeyPoints.Count == 0)
        {
            UpdateLastActivity();
            return null;
        }

        // Proveri sve ključne tačke
        for (int i = 0; i < Tour.KeyPoints.Count; i++)
        {
            var keyPoint = Tour.KeyPoints[i];
            var distance = CalculateDistance(latitude, longitude, keyPoint.Latitude, keyPoint.Longitude);
            
            if (distance <= maxDistanceKm)
            {
                var wasAlreadyUnlocked = IsKeyPointUnlocked(i);
                
                // Otključaj ako nije već otključana
                if (!wasAlreadyUnlocked)
                {
                    UnlockKeyPoint(i);
                }
                else
                {
                    // Samo ažuriraj LastActivity
                    UpdateLastActivity();
                }
                
                return (i, distance, wasAlreadyUnlocked);
            }
        }
        
        // Nije blizu nijedne tačke, samo ažuriraj LastActivity
        UpdateLastActivity();
        return null;
    }

    public double CalculateProgressPercentage()
    {
        if (Tour?.KeyPoints == null || Tour.KeyPoints.Count == 0)
            return 0;

        return ((double)_unlockedKeyPointIndices.Count / Tour.KeyPoints.Count) * 100.0;
    }

    public bool CanLeaveReview()
    {
        // Mora biti više od 35% pređeno
        var progressPercentage = CalculateProgressPercentage();
        if (progressPercentage <= 35.0)
            return false;

        // Nije prošlo više od 7 dana od poslednje aktivnosti
        var daysSinceLastActivity = (DateTime.UtcNow - LastActivity).TotalDays;
        return daysSinceLastActivity <= 7.0;
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
