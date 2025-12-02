using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourDurationDto
    {
        public int Type { get; set; }   //enum
        public int Minutes { get; set; }
    }

}
