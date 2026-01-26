using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourPlaylistDbRepository : ITourPlaylistRepository
{
    private readonly ToursContext _context;
    private readonly DbSet<TourPlaylist> _dbSet;

    public TourPlaylistDbRepository(ToursContext context)
    {
        _context = context;
        _dbSet = _context.Set<TourPlaylist>();
    }

    public TourPlaylist? GetByTourExecutionId(long tourExecutionId)
    {
        return _dbSet.FirstOrDefault(p => p.TourExecutionId == tourExecutionId);
    }

    public TourPlaylist Create(TourPlaylist playlist)
    {
        _dbSet.Add(playlist);
        _context.SaveChanges();
        return playlist;
    }

    public void Delete(long tourExecutionId)
    {
        var playlist = GetByTourExecutionId(tourExecutionId);
        if (playlist == null)
            throw new NotFoundException($"Playlist for execution {tourExecutionId} not found.");

        _dbSet.Remove(playlist);
        _context.SaveChanges();
    }
}