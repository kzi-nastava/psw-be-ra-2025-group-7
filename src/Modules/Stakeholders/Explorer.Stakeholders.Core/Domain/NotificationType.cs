namespace Explorer.Stakeholders.Core.Domain
{
    public enum NotificationType
    {
        // Club notifications
        JoinRequestAccepted = 0,
        JoinRequestRejected = 1,

        // Follower system notifications
        FollowerMessage = 2,
        ClubActivity = 3
    }
}
