using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterProgress : Entity
    {
        public enum EncounterProgressStatus
        {
            Active,
            Completed
        }
        public long EncounterId { get; private set; }
        public int UserId { get; private set; }

        public EncounterProgressStatus Status { get; private set; }

        private EncounterProgress() { } // EF

        public EncounterProgress(long encounterId, int userId, EncounterProgressStatus status)
        {
            EncounterId = encounterId;
            UserId = userId;
            Status = status;
        }

        public void SetCompleted()
        {
            Status = EncounterProgressStatus.Completed;
        }
    }
}
