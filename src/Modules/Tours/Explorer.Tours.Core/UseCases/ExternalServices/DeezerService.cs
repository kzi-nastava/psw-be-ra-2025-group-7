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
        var tracksPerGenre = (int)Math.Ceiling((double)limit / genres.Count) + 10; // +10 rezerve

        foreach (var genre in genres.Take(3)) // Max 3 žanra
        {
            var genreId = GetGenreId(genre);
            var genreTracks = await GetPopularTracksForGenre(genreId, tracksPerGenre);
            tracks.AddRange(genreTracks);
        }

        // Shuffle i uzmi traženi broj
        return tracks
            .OrderBy(_ => Random.Shared.Next())
            .Take(limit)
            .ToList();
    }

    private async Task<List<PlaylistTrackDto>> GetPopularTracksForGenre(int genreId, int limit)
    {
        try
        {
            var tracks = new List<PlaylistTrackDto>();

            // 1. Uzmi TOP izvođače iz žanra (najpopularnije)
            var artistsUrl = $"genre/{genreId}/artists?limit=10";
            var artistsResponse = await _httpClient.GetAsync(artistsUrl);

            if (!artistsResponse.IsSuccessStatusCode)
                return new List<PlaylistTrackDto>();

            var artistsContent = await artistsResponse.Content.ReadAsStringAsync();
            var artistsData = JsonSerializer.Deserialize<DeezerArtistsResponse>(artistsContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (artistsData?.Data == null || artistsData.Data.Count == 0)
                return new List<PlaylistTrackDto>();

            // 2. Za svakog izvođača uzmi njihove TOP pesme (najslušanije)
            var tracksPerArtist = Math.Max(3, limit / artistsData.Data.Count);

            foreach (var artist in artistsData.Data)
            {
                var tracksUrl = $"artist/{artist.Id}/top?limit={tracksPerArtist}";
                var tracksResponse = await _httpClient.GetAsync(tracksUrl);

                if (!tracksResponse.IsSuccessStatusCode)
                    continue;

                var tracksContent = await tracksResponse.Content.ReadAsStringAsync();
                var tracksData = JsonSerializer.Deserialize<DeezerTracksResponse>(tracksContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tracksData?.Data == null)
                    continue;

                tracks.AddRange(tracksData.Data.Select(track => new PlaylistTrackDto
                {
                    SpotifyTrackId = track.Id.ToString(),
                    Name = track.Title,
                    Artist = track.Artist?.Name ?? "Unknown Artist",
                    SpotifyUri = track.Link,
                    DurationMs = track.Duration * 1000
                }));

                if (tracks.Count >= limit)
                    break;
            }

            return tracks;
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

    // ===========================
    // JSON Deserializacija klase
    // ===========================

    private class DeezerArtistsResponse
    {
        public List<DeezerArtistInfo> Data { get; set; }
    }

    private class DeezerArtistInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

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