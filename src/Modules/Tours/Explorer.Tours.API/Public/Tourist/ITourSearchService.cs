using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourSearchService
    {
        List<TourDto> SearchByLocation(TourLocationSearchDto query);
    }
}
