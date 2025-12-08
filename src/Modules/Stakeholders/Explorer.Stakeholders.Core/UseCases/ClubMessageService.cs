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
    private readonly IMapper _mapper;

    public ClubMessageService(IClubMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
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

        // TODO: Verify that authorId is a member of the club
        // This would require ClubMember repository which might not exist yet
        
        var message = new ClubMessage(
            messageDto.ClubId,
            messageDto.AuthorId,
            messageDto.Content,
            messageDto.ResourceId,
            resourceType
        );

        var created = _messageRepository.Create(message);

        // TODO (taèka 3): Kreirati notifikacije za sve èlanove kluba
        // _notificationService.CreateClubMessageNotification(clubId, created);

        return _mapper.Map<ClubMessageDto>(created);
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

        _messageRepository.Delete(messageId);

        // TODO (taèka 3): Obrisati povezane notifikacije
        // _notificationService.DeleteClubMessageNotifications(messageId);
    }

    public PagedResult<ClubMessageDto> GetClubMessages(long clubId, int page, int pageSize)
    {
        var result = _messageRepository.GetByClub(clubId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<ClubMessageDto>).ToList();
        
        return new PagedResult<ClubMessageDto>(items, result.TotalCount);
    }
}
