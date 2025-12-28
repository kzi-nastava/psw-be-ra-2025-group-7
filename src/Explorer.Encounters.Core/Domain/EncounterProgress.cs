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
        public DateTime? EnteredPhotoRadiusAt { get; private set; }

        private EncounterProgress() { } // EF

        public EncounterProgress(long encounterId, int userId, EncounterProgressStatus status)
        {
            EncounterId = encounterId;
            UserId = userId;
            Status = status;
        }
        public void EnterPhotoRadius(DateTime now)
        {
            if (EnteredPhotoRadiusAt == null)
                EnteredPhotoRadiusAt = now;
        }

        public void ExitPhotoRadius()
        {
            EnteredPhotoRadiusAt = null;
        }

        public bool HasStayedLongEnough(int seconds, DateTime now)
        {
            return EnteredPhotoRadiusAt != null &&
                   (now - EnteredPhotoRadiusAt.Value).TotalSeconds >= seconds;
        }
        public void SetCompleted()
        {
            Status = EncounterProgressStatus.Completed;
        }
    }
}
