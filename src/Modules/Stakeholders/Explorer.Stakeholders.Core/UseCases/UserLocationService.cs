using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserLocationService : IUserLocationService
    {
        private readonly IUserProfileRepository _userProfileRepo;

        public UserLocationService(IUserProfileRepository userProfileRepo)
        {
            _userProfileRepo = userProfileRepo;
        }

        public UserLocationDto GetLocation(int userId)
        {
            var user = _userProfileRepo.GetByUserId(userId)
                ?? throw new KeyNotFoundException("User not found");

            return new UserLocationDto
            {
                UserId = userId,
                Latitude = user.CurrentLatitude,
                Longitude = user.CurrentLongitude
            };
        }
    }
}
