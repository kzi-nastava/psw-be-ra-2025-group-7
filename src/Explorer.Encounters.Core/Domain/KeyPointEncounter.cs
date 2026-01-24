using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class KeyPointEncounter : Entity
    {
        public long EncounterId { get; private set; }
        public long KeyPointId { get; private set; }
        public bool IsMandatory { get; private set; }

        private KeyPointEncounter() { } // EF

        public KeyPointEncounter(long encounterId, long keyPointId, bool isMandatory)
        {
            EncounterId = encounterId;
            KeyPointId = keyPointId;
            IsMandatory = isMandatory;
        }
    }
}

