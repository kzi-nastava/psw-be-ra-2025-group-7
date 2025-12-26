using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
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
    }
}
