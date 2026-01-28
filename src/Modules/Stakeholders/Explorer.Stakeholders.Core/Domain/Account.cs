using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Account
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        
        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        
        public string Role { get; set; } = string.Empty;

       
        public bool IsBlocked { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
       // public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
