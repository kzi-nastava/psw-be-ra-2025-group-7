using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class FollowerService : IFollowerService
{
    private readonly IFollowerRepository _followerRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMapper _mapper;

    public FollowerService(IFollowerRepository followerRepository, IPersonRepository personRepository, IMapper mapper)
    {
        _followerRepository = followerRepository;
        _personRepository = personRepository;
        _mapper = mapper;
    }

    public FollowerDto Follow(long followerId, long followedId)
    {
        if (followerId == followedId)
            throw new ArgumentException("Cannot follow yourself");

        // Check if already following
        if (_followerRepository.Exists(followerId, followedId))
            throw new InvalidOperationException("Already following this user");

        var follower = new Follower(followerId, followedId);
        var created = _followerRepository.Create(follower);
        
        return _mapper.Map<FollowerDto>(created);
    }

    public void Unfollow(long followerId, long followedId)
    {
        if (!_followerRepository.Exists(followerId, followedId))
            throw new NotFoundException("Not following this user");

        _followerRepository.Delete(followerId, followedId);
    }

    public PagedResult<FollowerDto> GetFollowers(long userId, int page, int pageSize)
    {
        var result = _followerRepository.GetFollowers(userId, page, pageSize);
        var items = result.Results.Select(f =>
        {
            var dto = _mapper.Map<FollowerDto>(f);
            // TODO: Optionally fetch follower name from Person repository
            return dto;
        }).ToList();
        
        return new PagedResult<FollowerDto>(items, result.TotalCount);
    }

    public PagedResult<FollowerDto> GetFollowing(long userId, int page, int pageSize)
    {
        var result = _followerRepository.GetFollowing(userId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<FollowerDto>).ToList();
        
        return new PagedResult<FollowerDto>(items, result.TotalCount);
    }

    public bool IsFollowing(long followerId, long followedId)
    {
        return _followerRepository.Exists(followerId, followedId);
    }
}
