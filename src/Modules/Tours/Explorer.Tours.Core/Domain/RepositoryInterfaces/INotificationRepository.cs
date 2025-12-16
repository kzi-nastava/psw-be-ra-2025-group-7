namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        List<Notification> GetForUser(long userId, bool onlyUnread);
        void MarkAsRead(int id, long userId);
    }
}
