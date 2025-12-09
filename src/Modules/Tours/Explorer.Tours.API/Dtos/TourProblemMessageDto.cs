using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourProblemMessageDto
    {
        public int CreatorId { get; set; }
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
