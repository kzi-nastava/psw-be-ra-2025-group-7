namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourPlaylistRepository
{
    TourPlaylist? GetByTourExecutionId(long tourExecutionId);
    TourPlaylist Create(TourPlaylist playlist);
    void Delete(long tourExecutionId);
}