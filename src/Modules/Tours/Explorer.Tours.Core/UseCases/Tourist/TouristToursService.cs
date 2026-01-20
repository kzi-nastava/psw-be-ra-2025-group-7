using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TouristToursService : ITouristToursService
    {
        private readonly ITourRepository _tours;
        private readonly IMapper _mapper;
        private readonly ITourReviewRepository _reviews;

        public TouristToursService(ITourRepository tours, IMapper mapper, ITourReviewRepository reviews)
        {
            _tours = tours;
            _mapper = mapper;
            _reviews = reviews;
        }

        public List<TourPreviewDto> GetPublishedTours()
        {
            var tours = _tours.GetAll()
                .Where(t => t.Status == TourStatus.Published)
                .ToList();
        
            var result = new List<TourPreviewDto>();

            foreach (var tour in tours)
            {
                var allReviews = _reviews.GetAll()
                    .Where(r => r.TourId == tour.Id)
                    .ToList();

                var dto = new TourPreviewDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = tour.Difficulty.ToString(),
                    Tags = tour.Tags.ToList(),
                    Price = tour.Price,
                    FirstKeyPoint = tour.KeyPoints.Any()
                        ? _mapper.Map<KeyPointDto>(tour.KeyPoints.First())
                        : null,
                    KeyPointsCount = tour.KeyPoints.Count,
                    ShortestDurationMinutes = tour.TourDurations.Any()
                        ? tour.TourDurations.Min(d => d.Minutes)
                        : (int?)null,
                    AverageRating = allReviews.Any()
                        ? _reviews.GetAverageRatingForTour(tour.Id)
                        : (double?)null,
                    ReviewCount = allReviews.Count
                };

                // Ne šalješ sekrete
                if (dto.FirstKeyPoint != null)
                    dto.FirstKeyPoint.Secret = "";

                result.Add(dto);
            }
            return result;
        }
    }
}
