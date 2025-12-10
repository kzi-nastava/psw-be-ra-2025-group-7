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

        public TouristToursService(ITourRepository tours, IMapper mapper)
        {
            _tours = tours;
            _mapper = mapper;
        }

        public List<TourPreviewDto> GetPublishedTours()
        {
            var tours = _tours.GetAll()
                .Where(t => t.Status == TourStatus.Published)
                .ToList();
        
            var result = new List<TourPreviewDto>();

            foreach (var tour in tours)
            {
                var dto = new TourPreviewDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = (int)tour.Difficulty,
                    Tags = tour.Tags.ToList(),
                    FirstKeyPoint = tour.KeyPoints.Any()
                        ? _mapper.Map<KeyPointDto>(tour.KeyPoints.First())
                        : null
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
