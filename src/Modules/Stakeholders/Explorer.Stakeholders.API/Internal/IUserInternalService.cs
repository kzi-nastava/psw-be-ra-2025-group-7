using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Internal
{
    public interface IUserInternalService
    {
        UserBasicInfoDto GetUserBasicInfo(long userId);

        UserContactInfoDto GetUserContactInfo(long userId);
    }
}
