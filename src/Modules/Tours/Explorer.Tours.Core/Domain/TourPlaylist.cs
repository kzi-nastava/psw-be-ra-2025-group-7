using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourPlaylist : Entity
{
    public long TourExecutionId { get; init; }
    public TourExecution TourExecution { get; init; }

    public List<string> SelectedGenres { get; private set; }
    public bool IncludeWeather { get; private set; }
    public string? WeatherCondition { get; private set; }
    public double? Temperature { get; private set; }

    public List<PlaylistTrack> Tracks { get; private set; }
    public int TotalDurationMinutes { get; private set; }

    public DateTime CreatedAt { get; init; }

    // EF Core constructor
    private TourPlaylist()
    {
        SelectedGenres = new List<string>();
        Tracks = new List<PlaylistTrack>();
    }

    public TourPlaylist(
        long tourExecutionId,
        List<string> selectedGenres,
        bool includeWeather,
        string? weatherCondition,
        double? temperature,
        List<PlaylistTrack> tracks)
    {
        if (selectedGenres == null || selectedGenres.Count == 0)
            throw new ArgumentException("At least one genre must be selected.", nameof(selectedGenres));

        if (selectedGenres.Count > 5)
            throw new ArgumentException("Maximum 5 genres allowed.", nameof(selectedGenres));

        if (tracks == null || tracks.Count == 0)
            throw new ArgumentException("Playlist must contain at least one track.", nameof(tracks));

        TourExecutionId = tourExecutionId;
        SelectedGenres = selectedGenres;
        IncludeWeather = includeWeather;
        WeatherCondition = weatherCondition;
        Temperature = temperature;
        Tracks = tracks;
        TotalDurationMinutes = CalculateTotalDuration(tracks);
        CreatedAt = DateTime.UtcNow;
    }

    private int CalculateTotalDuration(List<PlaylistTrack> tracks)
    {
        var totalMs = tracks.Sum(t => t.DurationMs);
        return (int)Math.Ceiling(totalMs / 60000.0); // Convert to minutes
    }
}