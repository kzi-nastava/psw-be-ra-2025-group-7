using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

/// <summary>
/// Aggregate Root - Poruka koju korisnik šalje svim svojim pratiocima
/// </summary>
public class FollowerMessage : AggregateRoot
{
    public long AuthorId { get; init; }          // Person.Id koji šalje poruku
    public string Content { get; private set; }
    public DateTime CreatedAt { get; init; }
    
    // Resource linking (opciono)
    public long? ResourceId { get; private set; }
    public ResourceType? ResourceType { get; private set; }

    public FollowerMessage(long authorId, string content, long? resourceId = null, ResourceType? resourceType = null)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");
        if (content.Length > 280) throw new ArgumentException("Content cannot exceed 280 characters");
        
        // Validacija resursa
        if ((resourceId.HasValue && !resourceType.HasValue) || (!resourceId.HasValue && resourceType.HasValue))
        {
            throw new ArgumentException("ResourceId and ResourceType must both be provided or both be null");
        }

        AuthorId = authorId;
        Content = content;
        ResourceId = resourceId;
        ResourceType = resourceType;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty");
        if (content.Length > 280) throw new ArgumentException("Content cannot exceed 280 characters");
        
        Content = content;
    }

    public void UpdateResource(long? resourceId, ResourceType? resourceType)
    {
        if ((resourceId.HasValue && !resourceType.HasValue) || (!resourceId.HasValue && resourceType.HasValue))
        {
            throw new ArgumentException("ResourceId and ResourceType must both be provided or both be null");
        }

        ResourceId = resourceId;
        ResourceType = resourceType;
    }
}
