using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public class FakeUserProfileLocationService : IUserProfileLocationService
    {
        public UserLocationDto GetLocation(int userId)
        {
            // svi korisnici su blizu encounter-a
            return new UserLocationDto
            {
                Latitude = 45.0001,
                Longitude = 19.0001
            };
        }
    }
}
