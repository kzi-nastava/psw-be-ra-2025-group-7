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
        public DateTime? FinishedAt { get; set; }
        public long UserId { get; private set; }
        public EncounterProgressStatus Status { get; private set; }
        public DateTime? EnteredPhotoRadiusAt { get; private set; }

        private EncounterProgress() { } // EF

        public EncounterProgress(long encounterId, long userId, EncounterProgressStatus status)
        {
            EncounterId = encounterId;
            UserId = userId;
            Status = status;
            FinishedAt = null;
        }
        public EncounterProgress(long encounterId, long userId,DateTime dateTime)
        {
            EncounterId = encounterId;
            UserId = userId;
            Status = EncounterProgressStatus.Active;
            EnteredPhotoRadiusAt = dateTime;
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
            if (Status == EncounterProgressStatus.Completed) return;
            Status = EncounterProgressStatus.Completed;
            FinishedAt = DateTime.UtcNow;
        }
    }
}
