using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
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

        public void CreateFollowerMessageNotifications(FollowerMessageDto message, List<long> followerIds)
        {
            // Parse ResourceType if provided
            ResourceType? resourceType = null;
            if (!string.IsNullOrEmpty(message.ResourceType))
            {
                Enum.TryParse<ResourceType>(message.ResourceType, out var parsedType);
                resourceType = parsedType;
            }

            var authorName = !string.IsNullOrEmpty(message.AuthorName) 
                ? $"{message.AuthorName} {message.AuthorSurname}".Trim() 
                : "Someone you follow";
            
            var preview = message.Content.Length > 50 
                ? message.Content.Substring(0, 50) + "..." 
                : message.Content;

            foreach (var followerId in followerIds)
            {
                var notification = new Notification(
                    userId: followerId,
                    type: NotificationType.FollowerMessage,
                    content: $"{authorName}: {preview}",
                    resourceId: message.ResourceId,
                    resourceType: resourceType,
                    sourceFollowerMessageId: message.Id,
                    sourceClubMessageId: null
                );

                _repository.Create(notification);
            }
        }

        public void CreateClubMessageNotifications(ClubMessageDto message, List<long> memberIds)
        {
            // Parse ResourceType if provided
            ResourceType? resourceType = null;
            if (!string.IsNullOrEmpty(message.ResourceType))
            {
                Enum.TryParse<ResourceType>(message.ResourceType, out var parsedType);
                resourceType = parsedType;
            }

            var clubName = !string.IsNullOrEmpty(message.ClubName) 
                ? message.ClubName 
                : "A club";
            
            var authorName = !string.IsNullOrEmpty(message.AuthorName) 
                ? $"{message.AuthorName} {message.AuthorSurname}".Trim() 
                : "A member";

            var preview = message.Content.Length > 50 
                ? message.Content.Substring(0, 50) + "..." 
                : message.Content;

            foreach (var memberId in memberIds)
            {
                // Don't notify the author of their own message
                if (memberId == message.AuthorId)
                    continue;

                var notification = new Notification(
                    userId: memberId,
                    type: NotificationType.ClubActivity,
                    content: $"{clubName} - {authorName}: {preview}",
                    resourceId: message.ResourceId,
                    resourceType: resourceType,
                    sourceFollowerMessageId: null,
                    sourceClubMessageId: message.Id
                );

                _repository.Create(notification);
            }
        }

        public void DeleteFollowerMessageNotifications(long followerMessageId)
        {
            _repository.DeleteByFollowerMessageId(followerMessageId);
        }

        public void DeleteClubMessageNotifications(long clubMessageId)
        {
            _repository.DeleteByClubMessageId(clubMessageId);
        }
    }
}
