using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;


namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IUserProfileRepository
    {
        UserProfile? GetByUserId(long userId);
        UserProfile Create(UserProfile profile);
        UserProfile Update(UserProfile profile);
    }
}
