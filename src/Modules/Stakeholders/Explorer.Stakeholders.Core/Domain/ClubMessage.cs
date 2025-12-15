using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

/// <summary>
/// Aggregate Root - Poruka na stranici kluba koju postavljaju èlanovi
/// </summary>
public class ClubMessage : AggregateRoot
{
    public long ClubId { get; init; }
    public long AuthorId { get; init; }          // Person.Id èlana kluba koji je postavio poruku
    public string Content { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Resource linking (opciono)
    public long? ResourceId { get; private set; }
    public ResourceType? ResourceType { get; private set; }

    public ClubMessage(long clubId, long authorId, string content, long? resourceId = null, ResourceType? resourceType = null)
    {
        if (clubId == 0) throw new ArgumentException("Invalid ClubId");
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");
        if (content.Length > 280) throw new ArgumentException("Content cannot exceed 280 characters");
        
        // Validacija resursa
        if ((resourceId.HasValue && !resourceType.HasValue) || (!resourceId.HasValue && resourceType.HasValue))
        {
            throw new ArgumentException("ResourceId and ResourceType must both be provided or both be null");
        }

        ClubId = clubId;
        AuthorId = authorId;
        Content = content;
        ResourceId = resourceId;
        ResourceType = resourceType;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string content, long? resourceId, ResourceType? resourceType)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");
        if (content.Length > 280) throw new ArgumentException("Content cannot exceed 280 characters");
        
        if ((resourceId.HasValue && !resourceType.HasValue) || (!resourceId.HasValue && resourceType.HasValue))
        {
            throw new ArgumentException("ResourceId and ResourceType must both be provided or both be null");
        }

        Content = content;
        ResourceId = resourceId;
        ResourceType = resourceType;
        UpdatedAt = DateTime.UtcNow;
    }
}
