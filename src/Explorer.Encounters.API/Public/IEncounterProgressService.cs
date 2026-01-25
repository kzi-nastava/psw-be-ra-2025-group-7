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
        void Delete(long id);
        bool CheckEncounterProgress(long encounterId);
        void ActivateHiddenLocationForUser(long encounterId, long userId);
        void OnUserLocationChanged(long userId);
        HiddenLocationProgressDto GetHiddenLocationProgress(long encounterId, long userId);
        EncounterProgressDto ActivateSocialEncounter(long encounterId, long userId);
        bool IsUserAtLocation(long encounterId, long userId);
        bool HasActiveSocialEncounter(long userId);
        EncounterDto GetActiveSocial();

        EncounterProgressDto ActivateMiscEncounter(long encounterId, long userId);
        //List<int> CheckParticipantsLocation(long encounterId);
        //void FinishEncounterProgress(long encounterId, List<int> users);
    }
}
