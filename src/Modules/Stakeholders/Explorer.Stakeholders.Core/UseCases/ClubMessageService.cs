using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class ClubMessageService : IClubMessageService
{
    private readonly IClubMessageRepository _messageRepository;
    private readonly IClubRepository _clubRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public ClubMessageService(
        IClubMessageRepository messageRepository,
        IClubRepository clubRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _clubRepository = clubRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public ClubMessageDto CreateMessage(ClubMessageDto messageDto)
    {
        // Parse ResourceType if provided
        ResourceType? resourceType = null;
        if (!string.IsNullOrEmpty(messageDto.ResourceType))
        {
            if (!Enum.TryParse<ResourceType>(messageDto.ResourceType, out var parsedType))
                throw new ArgumentException($"Invalid ResourceType: {messageDto.ResourceType}");
            resourceType = parsedType;
        }

        var message = new ClubMessage(
            messageDto.ClubId,
            messageDto.AuthorId,
            messageDto.Content,
            messageDto.ResourceId,
            resourceType
        );

        var created = _messageRepository.Create(message);
        var createdDto = _mapper.Map<ClubMessageDto>(created);

        // Preserve author and club info for notification content
        createdDto.AuthorName = messageDto.AuthorName;
        createdDto.AuthorSurname = messageDto.AuthorSurname;
        createdDto.ClubName = messageDto.ClubName;

        // Create notifications for all club members
        var memberIds = _clubRepository.GetMemberIds(messageDto.ClubId);
        if (memberIds.Any())
        {
            _notificationService.CreateClubMessageNotifications(createdDto, memberIds);
        }

        return createdDto;
    }

    public ClubMessageDto UpdateMessage(ClubMessageDto messageDto, long requesterId)
    {
        var existingMessage = _messageRepository.Get(messageDto.Id);

        // Only the author can update their message
        if (existingMessage.AuthorId != requesterId)
            throw new UnauthorizedAccessException("You can only update your own messages");

        // Parse ResourceType if provided
        ResourceType? resourceType = null;
        if (!string.IsNullOrEmpty(messageDto.ResourceType))
        {
            if (!Enum.TryParse<ResourceType>(messageDto.ResourceType, out var parsedType))
                throw new ArgumentException($"Invalid ResourceType: {messageDto.ResourceType}");
            resourceType = parsedType;
        }

        existingMessage.Update(messageDto.Content, messageDto.ResourceId, resourceType);
        
        var updated = _messageRepository.Update(existingMessage);

        return _mapper.Map<ClubMessageDto>(updated);
    }

    public void DeleteMessage(long messageId, long requesterId)
    {
        var message = _messageRepository.Get(messageId);
        var club = _messageRepository.GetClub(message.ClubId);

        // Only club owner can delete messages
        if (club.CreatedBy != requesterId)
            throw new UnauthorizedAccessException("Only club owner can delete messages");

        // Delete associated notifications first
        _notificationService.DeleteClubMessageNotifications(messageId);

        _messageRepository.Delete(messageId);
    }

    public PagedResult<ClubMessageDto> GetClubMessages(long clubId, int page, int pageSize)
    {
        var result = _messageRepository.GetByClub(clubId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<ClubMessageDto>).ToList();
        
        return new PagedResult<ClubMessageDto>(items, result.TotalCount);
    }
}
