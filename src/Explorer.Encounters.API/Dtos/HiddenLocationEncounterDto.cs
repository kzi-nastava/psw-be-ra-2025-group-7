using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class HiddenLocationEncounterDto
    {
        public string ImageUrl { get; set; } = "";
        public double ActivationLatitude { get; set; }
        public double ActivationLongitude { get; set; }
        public double ActivationRadiusMeters { get; set; }
        public double PhotoLatitude { get; set; }
        public double PhotoLongitude { get; set; }
    }

}
