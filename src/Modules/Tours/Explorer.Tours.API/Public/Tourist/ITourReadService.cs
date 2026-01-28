using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourReadService
    {
        
        //Returns full details for an active (published) tour or null if not found / not active.
        
        Task<TourFullForTouristDto?> GetFullForTouristAsync(long tourId, CancellationToken cancellationToken = default);
    }
}

