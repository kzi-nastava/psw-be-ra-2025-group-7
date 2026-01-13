using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases.Internal
{
    public class UserInternalService : IUserInternalService
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IPersonRepository _personRepository;

        public UserInternalService(IUserProfileRepository profileRepository, IPersonRepository personRepository)
        {
            _profileRepository = profileRepository;
            _personRepository = personRepository;
        }

        public UserBasicInfoDto GetUserBasicInfo(long userId)
        {
            var profile = _profileRepository.GetByUserId(userId);

            return new UserBasicInfoDto
            {
                UserId = userId,
                DisplayName = $"{profile.FirstName} {profile.LastName}",
                ProfilePicture = profile.ProfilePicture

            };
        }

        public UserContactInfoDto GetUserContactInfo(long userId)
        {
            var profile = _profileRepository.GetByUserId(userId);
            var person = _personRepository.GetByUserId(userId); // 👈 mora da postoji

            return new UserContactInfoDto
            {
                UserId = userId,
                Email = person.Email,
                Biography = profile.Biography,
                Motto = profile.Motto,
                ProfilePicture = profile.ProfilePicture
            };
        }
    }
}
