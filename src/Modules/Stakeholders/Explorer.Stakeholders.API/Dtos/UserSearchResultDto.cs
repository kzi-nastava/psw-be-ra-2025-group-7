namespace Explorer.Stakeholders.API.Dtos;

public class UserSearchResultDto
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string? ProfilePicture { get; set; }
    public string Role { get; set; }
}
