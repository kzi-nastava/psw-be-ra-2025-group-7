using Explorer.Encounters.API.Internal;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterCheckService : IEncounterCheckService
    {
        private readonly IEncounterRepository _kpEncounterRepo;
        private readonly IEncounterProgressRepository _progressRepo;

        public EncounterCheckService(
            IEncounterRepository kpEncounterRepo,
            IEncounterProgressRepository progressRepo)
        {
            _kpEncounterRepo = kpEncounterRepo;
            _progressRepo = progressRepo;
        }

        public bool HasMandatoryEncounter(long keyPointId)
        {
            var kpEncounter = _kpEncounterRepo.GetByKeyPointId(keyPointId);
            return kpEncounter != null && kpEncounter.IsMandatory;

        }

        public bool IsMandatoryEncounterCompleted(long keyPointId, long userId)
        {
            var mandatory = _kpEncounterRepo
                .GetByKeyPointId(keyPointId);

            if (mandatory == null) return true;

            return _progressRepo.isCompleted(mandatory.EncounterId,(int) userId);
        }
    }

}
