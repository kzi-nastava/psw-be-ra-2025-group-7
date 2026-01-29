using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourPlaylistService
{
    Task<TourPlaylistDto> GeneratePlaylist(long touristId, long executionId, GeneratePlaylistDto dto);
    TourPlaylistDto GetPlaylist(long touristId, long executionId);
    void DeletePlaylist(long touristId, long executionId);
}