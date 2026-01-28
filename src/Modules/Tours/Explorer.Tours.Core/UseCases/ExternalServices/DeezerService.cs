using System.Text.Json;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Core.UseCases.ExternalServices;

public class DeezerService : IDeezerService
{
    private readonly HttpClient _httpClient;

    // Mapiranje žanrova na Deezer genre IDs
    private static readonly Dictionary<string, int> GENRE_MAPPING = new()
    {
        { "pop", 132 },
        { "rock", 152 },
        { "hip-hop", 116 },
        { "electronic", 106 },
        { "jazz", 129 },
        { "classical", 98 },
        { "latin", 143 },
        { "reggae", 144 },
        { "country", 100 },
        { "blues", 153 },
        { "metal", 464 },
        { "indie", 85 },
        { "folk", 466 },
        { "soul", 165 },
        { "funk", 169 },
        { "disco", 113 },
        { "house", 113 },
        { "techno", 113 },
        { "world", 173 },
        { "alternative", 85 },
        { "r&b", 165 }
    };

    public DeezerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.deezer.com/");
    }

    public async Task<List<PlaylistTrackDto>> GetRecommendations(
        List<string> genres,
        double targetEnergy,
        double targetValence,
        int targetTempo,
        int limit)
    {
        var tracks = new List<PlaylistTrackDto>();

        // Deezer nema direktnu pretragu po mood parametrima kao Spotify,
        // tako da koristimo žanrove i preuzimamo top pesme
        foreach (var genre in genres.Take(3)) // Max 3 žanra da ne pretrpamo API
        {
            var genreId = GetGenreId(genre);
            var genreTracks = await GetTopTracksForGenre(genreId, limit / genres.Count);
            tracks.AddRange(genreTracks);

            if (tracks.Count >= limit)
                break;
        }

        // Ako nismo dobili dovoljno pesama iz žanrova, dodaj popularne pesme
        if (tracks.Count < limit)
        {
            var chartTracks = await GetChartTracks(limit - tracks.Count);
            tracks.AddRange(chartTracks);
        }

        return tracks.Take(limit).ToList();
    }

    private async Task<List<PlaylistTrackDto>> GetTopTracksForGenre(int genreId, int limit)
    {
        try
        {
            // Deezer radio endpoint za žanr daje playlist pesama
            var url = $"radio/{genreId}/tracks?limit={limit}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<PlaylistTrackDto>();

            var content = await response.Content.ReadAsStringAsync();
            var deezerResponse = JsonSerializer.Deserialize<DeezerTracksResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (deezerResponse?.Data == null)
                return new List<PlaylistTrackDto>();

            return deezerResponse.Data.Select(track => new PlaylistTrackDto
            {
                SpotifyTrackId = track.Id.ToString(), // Koristimo isto polje za Deezer ID
                Name = track.Title,
                Artist = track.Artist?.Name ?? "Unknown Artist",
                SpotifyUri = track.Link, // Koristimo isto polje za Deezer link
                DurationMs = track.Duration * 1000 // Deezer vraća sekunde, konvertujemo u ms
            }).ToList();
        }
        catch
        {
            return new List<PlaylistTrackDto>();
        }
    }

    private async Task<List<PlaylistTrackDto>> GetChartTracks(int limit)
    {
        try
        {
            var url = $"chart/0/tracks?limit={limit}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<PlaylistTrackDto>();

            var content = await response.Content.ReadAsStringAsync();
            var deezerResponse = JsonSerializer.Deserialize<DeezerTracksResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (deezerResponse?.Data == null)
                return new List<PlaylistTrackDto>();

            return deezerResponse.Data.Select(track => new PlaylistTrackDto
            {
                SpotifyTrackId = track.Id.ToString(),
                Name = track.Title,
                Artist = track.Artist?.Name ?? "Unknown Artist",
                SpotifyUri = track.Link,
                DurationMs = track.Duration * 1000
            }).ToList();
        }
        catch
        {
            return new List<PlaylistTrackDto>();
        }
    }

    private int GetGenreId(string genre)
    {
        var normalizedGenre = genre.ToLower().Trim();

        if (GENRE_MAPPING.TryGetValue(normalizedGenre, out var genreId))
            return genreId;

        // Default na Pop ako žanr nije pronađen
        return 132;
    }

    // Pomoćne klase za JSON deserializaciju
    private class DeezerTracksResponse
    {
        public List<DeezerTrack> Data { get; set; }
    }

    private class DeezerTrack
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int Duration { get; set; } // u sekundama
        public DeezerArtist Artist { get; set; }
    }

    private class DeezerArtist
    {
        public string Name { get; set; }
    }
}