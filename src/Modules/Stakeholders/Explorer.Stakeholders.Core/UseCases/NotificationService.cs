using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<NotificationDto> GetForUser(long userId, bool onlyUnread = false)
        {
            var list = _repository.GetForUser(userId, onlyUnread);
            return list.Select(_mapper.Map<NotificationDto>).ToList();
        }

        public int GetUnreadCount(long userId)
        {
            return _repository.GetUnreadCount(userId);
        }

        public void MarkAsRead(long notificationId, long userId)
        {
            var notification = _repository.Get(notificationId);

            if (notification == null || notification.UserId != userId)
            {
                throw new NotFoundException("Notification not found.");
            }

            if (!notification.IsRead)
            {
                notification.MarkAsRead();
                _repository.Update(notification);
            }
        }

        public void MarkAllAsRead(long userId)
        {
            _repository.MarkAllAsRead(userId);
        }

        public void Delete(long notificationId, long userId)
        {
            var notification = _repository.Get(notificationId);

            if (notification == null || notification.UserId != userId)
            {
                throw new NotFoundException("Notification not found.");
            }

            _repository.Delete(notificationId);
        }
    }
}
