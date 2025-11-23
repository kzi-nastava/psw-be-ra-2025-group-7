using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public.Tourist
{
    public interface ILocationService
    {
        void UpdateLocation(long userId, double latitude, double longitude);
        TouristLocationDto GetLocation(long userId);
    }
}
