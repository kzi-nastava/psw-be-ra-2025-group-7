using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterService
    {
        EncounterDto Create(CreateEncounterDto dto);
        EncounterDto Update(long id, UpdateEncounterDto dto);
        void Delete(long id);

        EncounterDto Get(long id);
        IEnumerable<EncounterDto> Get(string? status, string? type);

        EncounterDto ChangeStatus(long id, string status);

        EncounterDto AcceptEncounter(long id, UpdateEncounterDto dto);
        EncounterDto DeclineEncounter(long id);
    }
}
