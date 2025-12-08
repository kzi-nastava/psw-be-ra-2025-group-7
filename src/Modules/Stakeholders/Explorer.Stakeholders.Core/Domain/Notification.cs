using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

/// <summary>
/// PLACEHOLDER za taèku (3) - Sistem notifikacija
/// Implementacija æe biti završena u sledeæem zadatku
/// </summary>
public class Notification : Entity
{
    public long UserId { get; init; }                    // Person.Id primaoca
    public NotificationType Type { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; private set; }
    
    // Resource linking
    public long? ResourceId { get; init; }
    public ResourceType? ResourceType { get; init; }
    
    // Reference na izvornu poruku (ako postoji)
    public long? SourceFollowerMessageId { get; init; }
    public long? SourceClubMessageId { get; init; }

    public Notification(
        long userId, 
        NotificationType type, 
        string content,
        long? resourceId = null,
        ResourceType? resourceType = null,
        long? sourceFollowerMessageId = null,
        long? sourceClubMessageId = null)
    {
        UserId = userId;
        Type = type;
        Content = content;
        ResourceId = resourceId;
        ResourceType = resourceType;
        SourceFollowerMessageId = sourceFollowerMessageId;
        SourceClubMessageId = sourceClubMessageId;
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}

public enum NotificationType
{
    FollowerMessage = 0,        // Poruka od korisnika koje pratim
    ClubActivity = 1            // Aktivnost u klubovima èiji sam èlan
}
