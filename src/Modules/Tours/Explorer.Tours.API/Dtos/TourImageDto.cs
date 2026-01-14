using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourImageDto
    {
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}