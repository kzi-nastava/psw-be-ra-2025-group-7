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

        public List<NotificationDto> GetForTourist(long touristId)
        {
            var list = _repository.GetForTourist(touristId);
            return list.Select(_mapper.Map<NotificationDto>).ToList();
        }
        public void MarkAsRead(long notificationId, long touristId)
        {
            var notification = _repository.Get(notificationId);

            if (notification == null || notification.TouristId != touristId)
            {
                throw new NotFoundException("Notification not found.");
            }

            if (!notification.IsRead)
            {
                notification.MarkAsRead();
                _repository.Update(notification);
            }
        }
    }
}
