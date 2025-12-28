using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class CreateEncounterDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Radius { get; set; }
        public int Xp { get; set; }
        public string Type { get; set; } = "";
        public int? RequiredParticipants { get; set; }

        public string? Status { get; set; } 
    }
}
