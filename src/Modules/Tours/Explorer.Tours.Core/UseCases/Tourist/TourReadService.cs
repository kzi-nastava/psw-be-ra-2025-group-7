using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourReadService : ITourReadService
    {
        private readonly ITourReadRepository _repo;
        private readonly IMapper _mapper;

        public TourReadService(ITourReadRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<TourFullForTouristDto?> GetFullForTouristAsync(long tourId, CancellationToken cancellationToken = default)
        {
            var tour = await _repo.GetFullByIdAsync(tourId, cancellationToken);
            if (tour == null) return null;

            // Only allow published (active) tours to tourists
            if (tour.Status != TourStatus.Published) return null;

            return _mapper.Map<TourFullForTouristDto>(tour);
        }
    }
}