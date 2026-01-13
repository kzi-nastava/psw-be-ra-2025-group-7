using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using StakeholdersNotificationRepository = Explorer.Stakeholders.Core.Domain.RepositoryInterfaces.INotificationRepository;
using ToursNotificationRepository = Explorer.Tours.Core.Domain.RepositoryInterfaces.INotificationRepository;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly StakeholdersNotificationRepository _repository;
        private readonly ToursNotificationRepository _tourNotificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(
            StakeholdersNotificationRepository repository,
            ToursNotificationRepository tourNotificationRepository,
            IMapper mapper)
        {
            _repository = repository;
            _tourNotificationRepository = tourNotificationRepository;
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

        // ============== UNIFIED NOTIFICATION METHODS ==============

        public List<UnifiedNotificationDto> GetAllNotificationsForUser(long userId, bool onlyUnread = false)
        {
            var unifiedNotifications = new List<UnifiedNotificationDto>();

            // Get Stakeholder notifications
            var stakeholderNotifications = _repository.GetForUser(userId, onlyUnread);
            foreach (var notification in stakeholderNotifications)
            {
                unifiedNotifications.Add(new UnifiedNotificationDto
                {
                    Id = notification.Id,
                    Source = "Stakeholder",
                    Title = GetTitleFromType(notification.Type),
                    Content = notification.Content,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead,
                    ClubId = notification.ClubId,
                    Type = (int)notification.Type,
                    ResourceId = notification.ResourceId,
                    ResourceType = notification.ResourceType.HasValue ? (int)notification.ResourceType.Value : null,
                    SourceFollowerMessageId = notification.SourceFollowerMessageId,
                    SourceClubMessageId = notification.SourceClubMessageId
                });
            }

            // Get Tour notifications
            var tourNotifications = _tourNotificationRepository.GetForUser(userId, onlyUnread);
            foreach (var notification in tourNotifications)
            {
                unifiedNotifications.Add(new UnifiedNotificationDto
                {
                    Id = notification.Id,
                    Source = "Tour",
                    Title = notification.Title,
                    Content = notification.Preview, // Tours use Preview as content
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead,
                    ProblemId = notification.ProblemId
                });
            }

            // Sort by creation date, newest first
            return unifiedNotifications
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public int GetAllUnreadCount(long userId)
        {
            var stakeholderCount = _repository.GetUnreadCount(userId);
            var tourCount = _tourNotificationRepository.GetForUser(userId, onlyUnread: true).Count;
            return stakeholderCount + tourCount;
        }

        public void MarkNotificationAsRead(long notificationId, string source, long userId)
        {
            if (source == "Stakeholder")
            {
                MarkAsRead(notificationId, userId);
            }
            else if (source == "Tour")
            {
                _tourNotificationRepository.MarkAsRead((int)notificationId, userId);
            }
            else
            {
                throw new ArgumentException($"Invalid notification source: {source}");
            }
        }

        public void MarkAllNotificationsAsRead(long userId)
        {
            // Mark all Stakeholder notifications as read
            _repository.MarkAllAsRead(userId);

            // Mark all Tour notifications as read
            var tourNotifications = _tourNotificationRepository.GetForUser(userId, onlyUnread: true);
            foreach (var notification in tourNotifications)
            {
                _tourNotificationRepository.MarkAsRead((int)notification.Id, userId);
            }
        }

        private string GetTitleFromType(NotificationType type)
        {
            return type switch
            {
                NotificationType.JoinRequestAccepted => "Club Join Request Accepted",
                NotificationType.JoinRequestRejected => "Club Join Request Rejected",
                NotificationType.FollowerMessage => "New Follower Message",
                NotificationType.ClubActivity => "Club Activity",
                _ => "Notification"
            };
        }
    }
}
