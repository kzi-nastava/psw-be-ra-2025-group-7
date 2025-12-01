namespace Explorer.Tours.API.Dtos;

public class TourPreviewDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Difficulty { get; set; }
    public List<string> Tags { get; set; }
    public decimal Price { get; set; }
    public bool IsPurchasable { get; set; }
}
