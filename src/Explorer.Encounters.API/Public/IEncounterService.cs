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
        EncounterDto Create(int creatorId,CreateEncounterDto dto);
        EncounterDto Update(long id,int creatorId, UpdateEncounterDto dto);
        void Delete(long id,int creatorId);

        EncounterDto Get(long id);
        IEnumerable<EncounterDto> Get(string? status, string? type);

        EncounterDto ChangeStatus(long id, string status);

        EncounterDto AcceptEncounter(long id, UpdateEncounterDto dto);
        EncounterDto DeclineEncounter(long id);

        public bool IsMandatoryEncounterCompleted(long keyPointId, long userId);
        public bool HasMandatoryEncounter(long keyPointId);
    }
}
