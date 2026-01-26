using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ISpotifyService
{
    Task<List<PlaylistTrackDto>> GetRecommendations(
        List<string> genres,
        double targetEnergy,
        double targetValence,
        int targetTempo,
        int limit);
}