namespace Explorer.Tours.API.Dtos;

public class KeyPointProximityCheckResultDto
{
    public bool IsNearKeyPoint { get; set; }
    public int? KeyPointIndex { get; set; }
    public string? KeyPointName { get; set; }
    public double? DistanceInMeters { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public bool WasAlreadyUnlocked { get; set; }
    public TourExecutionDto TourExecution { get; set; }
}
