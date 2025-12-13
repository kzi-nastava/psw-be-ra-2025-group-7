using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

}
