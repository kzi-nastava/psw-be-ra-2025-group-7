namespace Explorer.Tours.API.Dtos;

public class UpdateTourReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
}
