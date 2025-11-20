using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserProfileService
    {
        UserProfileDto GetByUserId(long userId);
        UserProfileDto Create(UserProfileDto profile);
        UserProfileDto Update(UserProfileDto profile);
    }
}
