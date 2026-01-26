namespace Explorer.Tours.Core.Domain;

public class PlaylistTrack
{
    public string SpotifyTrackId { get; set; }
    public string Name { get; set; }
    public string Artist { get; set; }
    public string SpotifyUri { get; set; }
    public int DurationMs { get; set; }

    // Prazan konstruktor za EF Core i JSON deserializaciju
    public PlaylistTrack() { }

    public PlaylistTrack(string spotifyTrackId, string name, string artist, string spotifyUri, int durationMs)
    {
        SpotifyTrackId = spotifyTrackId ?? throw new ArgumentNullException(nameof(spotifyTrackId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Artist = artist ?? throw new ArgumentNullException(nameof(artist));
        SpotifyUri = spotifyUri ?? throw new ArgumentNullException(nameof(spotifyUri));
        DurationMs = durationMs;
    }
}