using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IFollowerService
{
    // Follow/Unfollow
    FollowerDto Follow(long followerId, long followedId);
    void Unfollow(long followerId, long followedId);
    
    // Get followers and following
    PagedResult<FollowerDto> GetFollowers(long userId, int page, int pageSize);
    PagedResult<FollowerDto> GetFollowing(long userId, int page, int pageSize);
    
    // Check if following
    bool IsFollowing(long followerId, long followedId);
}
