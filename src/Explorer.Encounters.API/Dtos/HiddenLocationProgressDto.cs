using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class HiddenLocationProgressDto
    {
        public long EncounterId { get; set; }

        public bool IsInPhotoRadius { get; set; }

        public int SecondsRequired { get; set; }      // npr 30
        public int SecondsSpent { get; set; }          // npr 12

        public bool IsCompleted { get; set; }

        public string Message { get; set; }
    }

}
