using Explorer.Encounters.API.Internal;
using Explorer.Encounters.API.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class UserLocationChangedNotifier : IUserLocationChangedNotifier
    {
        private readonly IEncounterProgressService _progressService;

        public UserLocationChangedNotifier(IEncounterProgressService progressService)
        {
            _progressService = progressService;
        }

        public void UserLocationChanged(long userId)
        {
            _progressService.OnUserLocationChanged((int)userId);
        }
    }
}
