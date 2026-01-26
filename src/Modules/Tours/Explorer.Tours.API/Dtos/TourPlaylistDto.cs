namespace Explorer.Tours.API.Dtos;

public class TourPlaylistDto
{
    public long Id { get; set; }
    public long TourExecutionId { get; set; }
    public List<string> SelectedGenres { get; set; } = new();
    public bool IncludeWeather { get; set; }
    public string? WeatherCondition { get; set; }
    public double? Temperature { get; set; }
    public List<PlaylistTrackDto> Tracks { get; set; } = new();
    public int TotalDurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}