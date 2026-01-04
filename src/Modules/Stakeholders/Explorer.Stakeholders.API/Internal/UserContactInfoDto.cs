using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public class UserContactInfoDto
    {
        public long UserId { get; set; }
        public string Email { get; set; } = "";
        public string? Biography { get; set; }        // opciono
        public string? Motto { get; set; }            // opciono
        public string? ProfilePicture { get; set; }   // opciono, da prikaže avatar i tu
    }
}
