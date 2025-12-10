using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourSearchService : ITourSearchService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourSearchService(ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public List<TourDto> SearchByLocation(TourLocationSearchDto query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (query.RadiusKm <= 0)
                throw new ArgumentException("Radius must be greater than zero.", nameof(query.RadiusKm));

            var tours = _tourRepository.GetPublishedWithKeyPoints();

            // Filtriramo ture koje imaju bar jedan KeyPoint u radijusu
            var filtered = tours
                .Where(t => t.KeyPoints != null && t.KeyPoints.Any(kp =>
                    DistanceInKm(query.Latitude, query.Longitude, kp.Latitude, kp.Longitude) <= query.RadiusKm))
                .ToList();

            return _mapper.Map<List<TourDto>>(filtered);
        }

        // Funkcija za računanje distance (Haversine formula)

        private static double DistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
    }
}