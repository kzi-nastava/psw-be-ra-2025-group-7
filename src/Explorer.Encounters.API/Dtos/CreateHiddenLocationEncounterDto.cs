using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
   public class CreateHiddenLocationEncounterDto
    {
        public string ImageUrl { get; set; } = "";
        public double ActivationRadiusMeters { get; set; }
        public double PhotoLatitude { get; set; }
        public double PhotoLongitude { get; set; }
    }
}
