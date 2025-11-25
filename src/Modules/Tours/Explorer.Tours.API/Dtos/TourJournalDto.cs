namespace Explorer.Tours.API.Dtos;

public class TourJournalDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string? City { get; set; }
    public string Country { get; set; }
}
