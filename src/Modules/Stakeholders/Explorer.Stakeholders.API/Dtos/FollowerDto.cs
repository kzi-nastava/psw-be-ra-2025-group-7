namespace Explorer.Stakeholders.API.Dtos;

public class FollowerDto
{
    public long Id { get; set; }
    public long FollowerId { get; set; }
    public long FollowedId { get; set; }
    public DateTime FollowedAt { get; set; }
    
    // Extended info for display
    public string? FollowerName { get; set; }
    public string? FollowerSurname { get; set; }
}
