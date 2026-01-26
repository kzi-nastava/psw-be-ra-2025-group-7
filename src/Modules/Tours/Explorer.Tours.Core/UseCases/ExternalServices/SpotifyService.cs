using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.Extensions.Configuration;

namespace Explorer.Tours.Core.UseCases.ExternalServices
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private string? _accessToken;
        private DateTime _tokenExpiry;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("Spotify ClientId not configured");
            _clientSecret = configuration["Spotify:ClientSecret"] ?? throw new InvalidOperationException("Spotify ClientSecret not configured");
        }

        public async Task<List<PlaylistTrackDto>> GetRecommendations(
            List<string> genres,
            double targetEnergy,
            double targetValence,
            int targetTempo,
            int limit)
        {
            await EnsureAccessToken();

            var genresParam = string.Join(",", genres.Take(5)); // Spotify dozvoljava najviše 5 seedova
            var url = $"https://api.spotify.com/v1/recommendations?" +
                      $"seed_genres={Uri.EscapeDataString(genresParam)}&" +
                      $"target_energy={targetEnergy:F2}&" +
                      $"target_valence={targetValence:F2}&" +
                      $"target_tempo={targetTempo}&" +
                      $"limit={limit}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Spotify API request failed: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var spotifyResponse = JsonSerializer.Deserialize<SpotifyRecommendationsResponse>(content);

            if (spotifyResponse?.Tracks == null)
                return new List<PlaylistTrackDto>();

            return spotifyResponse.Tracks.Select(track => new PlaylistTrackDto
            {
                SpotifyTrackId = track.Id,
                Name = track.Name,
                Artist = track.Artists?.FirstOrDefault()?.Name ?? "Unknown Artist",
                SpotifyUri = track.Uri,
                DurationMs = track.DurationMs
            }).ToList();
        }

        private async Task EnsureAccessToken()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
                return;

            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to authenticate with Spotify API");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(content);

            _accessToken = tokenResponse?.AccessToken ?? throw new InvalidOperationException("No access token received");
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); 
        }

        // Pomoćne klase za JSON deserializaciju
        private class SpotifyTokenResponse
        {
            public string AccessToken { get; set; }
            public int ExpiresIn { get; set; }
        }

        private class SpotifyRecommendationsResponse
        {
            public List<SpotifyTrack> Tracks { get; set; }
        }

        private class SpotifyTrack
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Uri { get; set; }
            public int DurationMs { get; set; }
            public List<SpotifyArtist> Artists { get; set; }
        }

        private class SpotifyArtist
        {
            public string Name { get; set; }
        }
    }
}