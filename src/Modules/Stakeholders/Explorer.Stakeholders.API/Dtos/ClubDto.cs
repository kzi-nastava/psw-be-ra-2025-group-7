using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
