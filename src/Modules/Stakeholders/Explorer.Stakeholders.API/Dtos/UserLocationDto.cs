using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UserLocationDto
    {
        public int UserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public UserLocationDto(int userId, double? latitude, double? longitude)
        {
            UserId = userId;
            Latitude = latitude;
            Longitude = longitude;
        }

        public UserLocationDto() { }
    }
}
