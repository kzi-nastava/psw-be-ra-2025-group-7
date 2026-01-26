namespace Explorer.Tours.API.Dtos;

public class PlaylistTrackDto
{
    public string SpotifyTrackId { get; set; }
    public string Name { get; set; }
    public string Artist { get; set; }
    public string SpotifyUri { get; set; }
    public int DurationMs { get; set; }
}