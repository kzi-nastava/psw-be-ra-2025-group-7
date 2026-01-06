using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepositoy;

        public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper, IUserRepository userRepositoy)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
            _userRepositoy = userRepositoy;
        }

        // Helper: kreira profil ako ne postoji
        private UserProfile GetOrCreateProfile(long userId)
        {
            var profile = _userProfileRepository.GetByUserId(userId);

            if (profile == null)
            {
                profile = new UserProfile(
                    userId,
                    "User",
                    "Profile",
                    null,
                    null,
                    null
                );

                var user = _userRepositoy.GetById(userId);
                if(user.Role == UserRole.Tourist)
                {
                    profile.XP = 0;
                    profile.Level = 1;
                } else
                {
                    profile.XP = null;
                    profile.Level = null;
                }

                    _userProfileRepository.Create(profile);
            }

            return profile;
        }

        public UserProfileDto GetByUserId(long userId)
        {
            var profile = GetOrCreateProfile(userId);
            return _mapper.Map<UserProfileDto>(profile);
        }

        public void AddXP(long userId, int XP)
        {
            var profile = GetOrCreateProfile(userId);

            if (profile.XP == null)
            {
                profile.XP = 0;
                profile.Level = 1;
            }

            profile.XP += XP;
            CheckLevel(profile);
            _userProfileRepository.Update(profile);
        }

        public void CheckLevel(UserProfile profile)
        {
            int xp = profile.XP ?? 0;
            int level = 1;

            int xpForNextLevel = 100;

            while (xp >= xpForNextLevel)
            {
                level++;
                xpForNextLevel += level * 100;
            }

            profile.Level = level;
        }


        public UserProfileDto Create(UserProfileDto profileDto)
        {
            var profile = _mapper.Map<UserProfile>(profileDto);
            var createdProfile = _userProfileRepository.Create(profile);
            return _mapper.Map<UserProfileDto>(createdProfile);
        }

        public UserProfileDto Update(UserProfileDto profileDto)
        {
            var profile = GetOrCreateProfile(profileDto.UserId);

            profile.UpdateProfile(
                profileDto.FirstName,
                profileDto.LastName,
                profileDto.ProfilePicture,
                profileDto.Biography,
                profileDto.Motto,
                profileDto.XP,
                profileDto.Level
            );

            var updatedProfile = _userProfileRepository.Update(profile);
            return _mapper.Map<UserProfileDto>(updatedProfile);
        }
    }
}
