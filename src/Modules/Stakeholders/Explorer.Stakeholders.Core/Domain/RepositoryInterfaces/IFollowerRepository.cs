using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IFollowerRepository
{
    Follower Create(Follower follower);
    void Delete(long followerId, long followedId);
    Follower? Get(long followerId, long followedId);
    bool Exists(long followerId, long followedId);
    
    // Get all followers for a specific user (koji prate ovog korisnika)
    PagedResult<Follower> GetFollowers(long followedId, int page, int pageSize);
    
    // Get all users that specific user follows (koga prati ovaj korisnik)
    PagedResult<Follower> GetFollowing(long followerId, int page, int pageSize);
    
    // Get follower IDs for sending messages
    List<long> GetFollowerIds(long followedId);
}
