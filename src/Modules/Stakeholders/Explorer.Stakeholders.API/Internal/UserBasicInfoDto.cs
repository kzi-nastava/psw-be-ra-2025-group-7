using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public class UserBasicInfoDto
    {
        public long UserId { get; set; }
        public string DisplayName { get; set; } = "";
        public string? ProfilePicture { get; set; }

    }
}
