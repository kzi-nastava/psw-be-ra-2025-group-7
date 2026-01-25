using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Internal
{
    public interface IEncounterInternalService
    {
        IEnumerable<EncounterDto> GetByLocation(double Latitude, double Longitude);
    }
}
