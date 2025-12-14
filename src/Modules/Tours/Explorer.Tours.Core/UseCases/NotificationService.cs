using Explorer.Notifications.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Notifications.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public void CreateProblemMessageNotification(
            long recipientUserId,
            int problemId,
            string messagePreview,
            DateTime createdAt)
        {
            var notification = new Notification(
                userId: recipientUserId,
                title: "Nova poruka na prijavljenom problemu",
                preview: messagePreview,
                problemId: problemId,
                createdAt: createdAt
            );

            _notificationRepository.Create(notification);
        }
    }
}
