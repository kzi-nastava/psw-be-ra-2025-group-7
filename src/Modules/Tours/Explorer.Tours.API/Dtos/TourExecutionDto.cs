namespace Explorer.Tours.API.Dtos;

public class TourExecutionDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? AbandonedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public List<int> UnlockedKeyPointIndices { get; set; } = new();
    public TourDto? Tour { get; set; }
}
