namespace Explorer.Tours.API.Dtos;

public class CreateTourReviewDto
{
    public long TourId { get; set; }
    public long TourExecutionId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
}
