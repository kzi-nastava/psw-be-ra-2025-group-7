using Explorer.Stakeholders.API.Dtos;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public.Tourist
{
    public interface ILocationService
    {
        TouristLocationDto UpdateLocation(long userId, double latitude, double longitude);
        TouristLocationDto GetLocation(long userId);
        List<MonumentDto> GetNearbyMonuments(long userId, int limit = 40);
    }
}
