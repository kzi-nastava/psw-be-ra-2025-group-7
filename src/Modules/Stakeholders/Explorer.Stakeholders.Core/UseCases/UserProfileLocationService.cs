using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserProfileLocationService : IUserProfileLocationService
    {
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileLocationService(IUserProfileRepository userProfileRepository, IMapper mapper) 
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }
        public UserLocationDto GetLocation(int userId)
        {
            var profile = _userProfileRepository.GetByUserId(userId);
            return _mapper.Map<UserLocationDto>(profile);
        }

        public UserProfile GetById(long userId)
        {
            return _userProfileRepository.GetByUserId(userId);

        }

        public void AddXP(long userId, int XP)
        {
            var profile = GetById(userId);
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
    }
}
