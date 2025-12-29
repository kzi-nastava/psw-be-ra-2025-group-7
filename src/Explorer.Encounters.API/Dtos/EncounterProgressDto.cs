using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class EncounterProgressDto
    {
        public long Id { get; set; }
        public long EncounterId { get; set; }
        public int UserId { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
