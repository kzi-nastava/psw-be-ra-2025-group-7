using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

/// <summary>
/// Predstavlja relaciju izmeðu korisnika koji prati (FollowerId) i korisnika koji je praæen (FollowedId)
/// </summary>
public class Follower : Entity
{
    public long FollowerId { get; init; }      // Person.Id koji prati
    public long FollowedId { get; init; }      // Person.Id koji je praæen
    public DateTime FollowedAt { get; init; }

    public Follower(long followerId, long followedId)
    {
        if (followerId == 0) throw new ArgumentException("Invalid FollowerId");
        if (followedId == 0) throw new ArgumentException("Invalid FollowedId");
        if (followerId == followedId) throw new ArgumentException("Cannot follow yourself");

        FollowerId = followerId;
        FollowedId = followedId;
        FollowedAt = DateTime.UtcNow;
    }
}
