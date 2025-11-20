using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public UserProfileDto GetByUserId(long userId)
    {
        var profile = _userProfileRepository.GetByUserId(userId);
        if (profile == null)
        {
            throw new KeyNotFoundException($"User profile for UserId {userId} not found.");
        }
        return _mapper.Map<UserProfileDto>(profile);
    }

    public UserProfileDto Create(UserProfileDto profileDto)
    {
        var profile = _mapper.Map<UserProfile>(profileDto);
        var createdProfile = _userProfileRepository.Create(profile);
        return _mapper.Map<UserProfileDto>(createdProfile);
    }

    public UserProfileDto Update(UserProfileDto profileDto)
    {
        var existingProfile = _userProfileRepository.GetByUserId(profileDto.UserId);
        if (existingProfile == null)
        {
            throw new KeyNotFoundException($"User profile for UserId {profileDto.UserId} not found.");
        }

        existingProfile.UpdateProfile(
            profileDto.FirstName,
            profileDto.LastName,
            profileDto.ProfilePicture,
            profileDto.Biography,
            profileDto.Motto
        );

        var updatedProfile = _userProfileRepository.Update(existingProfile);
        return _mapper.Map<UserProfileDto>(updatedProfile);
    }
}