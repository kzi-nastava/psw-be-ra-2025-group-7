using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
        public interface ITourPreferencesService
        {
            TourPreferencesDto GetByTouristId(long TouristId);
            TourPreferencesDto Create(TourPreferencesDto dto);
            TourPreferencesDto Update(TourPreferencesDto dto);
            void Delete(long id);
            //List<TourPreferencesDto> GetAll(); // pripazi
        }
    
}
