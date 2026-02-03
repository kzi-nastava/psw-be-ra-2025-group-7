using AutoMapper;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class LeaderboardService : ILeaderBoardService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public LeaderboardService(IUserProfileRepository userProfileRepository, IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public List<LeaderboardUserDto> GetTopUsersByXp(int count)
        {
            var topProfiles = _userProfileRepository.GetTopByXp(count);
            return _mapper.Map<List<LeaderboardUserDto>>(topProfiles);
        }

        public List<LeaderboardUserDto> GetAllUsersByXp()
        {
            var allProfiles = _userProfileRepository.GetAllByXp();
            return _mapper.Map<List<LeaderboardUserDto>>(allProfiles);
        }
    }
}
