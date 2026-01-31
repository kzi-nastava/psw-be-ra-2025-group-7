using Explorer.Notifications.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Notifications.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public NotificationService(INotificationRepository notificationRepository, IRealTimeNotificationService realTimeNotificationService)
        {
            _notificationRepository = notificationRepository;
            _realTimeNotificationService = realTimeNotificationService;
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
            _ = _realTimeNotificationService.SendToUserAsync(recipientUserId, notification.Title + ": " + notification.Preview);
        }
    }
}
