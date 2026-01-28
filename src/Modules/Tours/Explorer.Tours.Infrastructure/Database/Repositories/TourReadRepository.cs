using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;


namespace Explorer.Tours.Infrastructure.Database.Repositories
{

    public class TourReadRepository : ITourReadRepository
    {
        private readonly ToursContext _db;
        public TourReadRepository(ToursContext db) => _db = db;

        public async Task<Tour?> GetFullByIdAsync(long tourId, CancellationToken cancellationToken = default)
        {
            return await _db.Tours
                .AsNoTracking()
                .Include(t => t.Images)
                .Include(t => t.RequiredEquipment)
                .Include(t => t.KeyPoints)
                .Include(t => t.TourDurations)
                .FirstOrDefaultAsync(t => t.Id == tourId, cancellationToken);
        }
    }
}