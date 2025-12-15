namespace Explorer.Stakeholders.API.Dtos;

public class ClubMessageDto
{
    public long Id { get; set; }
    public long ClubId { get; set; }
    public long AuthorId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Resource linking
    public long? ResourceId { get; set; }
    public string? ResourceType { get; set; }  // "Tour" ili "BlogPost"
    
    // Extended info
    public string? AuthorName { get; set; }
    public string? AuthorSurname { get; set; }
    public string? ClubName { get; set; }
}
