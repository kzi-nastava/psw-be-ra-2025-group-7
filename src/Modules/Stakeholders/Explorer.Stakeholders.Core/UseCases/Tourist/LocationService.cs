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

        public LocationService(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public TouristLocationDto GetLocation(long userId)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            if (profile == null)
                throw new NotFoundException($"UserProfile for user {userId} not found.");

            return new TouristLocationDto
            {
                Latitude = profile.CurrentLatitude ?? 0,
                Longitude = profile.CurrentLongitude ?? 0
            };
        }

        public TouristLocationDto UpdateLocation(long userId, double latitude, double longitude)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            if (profile == null)
                throw new NotFoundException($"UserProfile for user {userId} not found.");

            profile.UpdateLocation(latitude, longitude);
            _userProfileRepository.Update(profile);

            return new TouristLocationDto
            {
                Latitude = profile.CurrentLatitude ?? 0,
                Longitude = profile.CurrentLongitude ?? 0
            };
        }
    }
}
