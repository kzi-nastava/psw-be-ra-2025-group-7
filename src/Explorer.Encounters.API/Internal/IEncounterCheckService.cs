using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Internal
{
   
    public interface IEncounterCheckService
    {
        bool HasMandatoryEncounter(long keyPointId);
        bool IsMandatoryEncounterCompleted(long keyPointId, long userId);
    }

}
