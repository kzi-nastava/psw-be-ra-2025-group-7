using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public.Tourist;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Stakeholders.Core.UseCases.Tourist
{
    public class LocationService : ILocationService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMonumentRepository _monumentRepository;

        public LocationService(IUserProfileRepository userProfileRepository, IMonumentRepository monumentRepository)
        {
            _userProfileRepository = userProfileRepository;
            _monumentRepository = monumentRepository;
        }

        private UserProfile GetOrCreateProfile(long userId)
        {
            var profile = _userProfileRepository.GetByUserId(userId);

            if (profile == null)
            {
                profile = new UserProfile(
                    userId,
                    "User",
                    "Profile"
                );

                _userProfileRepository.Create(profile);
            }

            return profile;
        }

        public TouristLocationDto GetLocation(long userId)
        {
            var profile = GetOrCreateProfile(userId);

            return new TouristLocationDto
            {
                Latitude = profile.CurrentLatitude ?? 0,
                Longitude = profile.CurrentLongitude ?? 0
            };
        }

        public TouristLocationDto UpdateLocation(long userId, double latitude, double longitude)
        {
            var profile = GetOrCreateProfile(userId);

            profile.UpdateLocation(latitude, longitude);
            _userProfileRepository.Update(profile);

            return new TouristLocationDto
            {
                Latitude = profile.CurrentLatitude ?? 0,
                Longitude = profile.CurrentLongitude ?? 0
            };
        }

        public List<MonumentDto> GetNearbyMonuments(long userId, int limit = 40)
        {
            var profile = GetOrCreateProfile(userId);

            if (!profile.CurrentLatitude.HasValue || !profile.CurrentLongitude.HasValue)
                throw new ArgumentException($"User {userId} does not have a location set.");

            var touristLat = profile.CurrentLatitude.Value;
            var touristLon = profile.CurrentLongitude.Value;

            var monuments = _monumentRepository.GetAll();

            var monumentsWithDistance = monuments
                .Select(m => new
                {
                    Monument = m,
                    Distance = CalculateDistance(touristLat, touristLon, m.Latitude, m.Longitude)
                })
                .OrderBy(x => x.Distance)
                .Take(limit)
                .Select(x => new MonumentDto
                {
                    Id = x.Monument.Id,
                    Name = x.Monument.Name,
                    Description = x.Monument.Description,
                    YearOfCreation = x.Monument.YearOfCreation,
                    Status = (int)x.Monument.Status,
                    Latitude = x.Monument.Latitude,
                    Longitude = x.Monument.Longitude
                })
                .ToList();

            return monumentsWithDistance;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
