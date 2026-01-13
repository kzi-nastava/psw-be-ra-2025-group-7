namespace Explorer.Tours.API.Dtos;

public class TourReviewDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public long TourExecutionId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public double TourProgressPercentage { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    
    // Optional navigation data
    public TourDto? Tour { get; set; }
}
