using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterProgressRepository
    {
        EncounterProgress Create(EncounterProgress encounterProgress);
        EncounterProgress Update(EncounterProgress encounterProgress);
        void Delete(long id);
        EncounterProgress? Get(long id);
        public bool isCompleted(long encounterId, int participantId);
        List<EncounterProgress> GetAll();
    }
}
