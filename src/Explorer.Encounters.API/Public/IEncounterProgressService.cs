using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterProgressService
    {
        EncounterProgressDto Create(EncounterProgressDto dto);
        EncounterDto Update(long id);
        EncounterProgressDto Delete(long id);
        bool CheckEncounterProgress(long encounterId);
    }
}
