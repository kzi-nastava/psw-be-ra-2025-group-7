using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public long PersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
