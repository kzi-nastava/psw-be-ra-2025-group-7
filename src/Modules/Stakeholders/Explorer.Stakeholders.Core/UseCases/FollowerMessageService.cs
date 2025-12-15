using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class FollowerMessageService : IFollowerMessageService
{
    private readonly IFollowerMessageRepository _messageRepository;
    private readonly IFollowerRepository _followerRepository;
    private readonly IMapper _mapper;

    public FollowerMessageService(
        IFollowerMessageRepository messageRepository,
        IFollowerRepository followerRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _followerRepository = followerRepository;
        _mapper = mapper;
    }

    public FollowerMessageDto SendMessageToFollowers(FollowerMessageDto messageDto)
    {
        // Parse ResourceType if provided
        ResourceType? resourceType = null;
        if (!string.IsNullOrEmpty(messageDto.ResourceType))
        {
            if (!Enum.TryParse<ResourceType>(messageDto.ResourceType, out var parsedType))
                throw new ArgumentException($"Invalid ResourceType: {messageDto.ResourceType}");
            resourceType = parsedType;
        }

        // Create the message
        var message = new FollowerMessage(
            messageDto.AuthorId,
            messageDto.Content,
            messageDto.ResourceId,
            resourceType
        );

        var created = _messageRepository.Create(message);

        // TODO (taèka 3): Ovde æe se kreirati notifikacije za sve pratioce
        // var followerIds = _followerRepository.GetFollowerIds(messageDto.AuthorId);
        // foreach (var followerId in followerIds)
        // {
        //     _notificationService.CreateFollowerMessageNotification(followerId, created);
        // }

        return _mapper.Map<FollowerMessageDto>(created);
    }

    public PagedResult<FollowerMessageDto> GetMyMessages(long authorId, int page, int pageSize)
    {
        var result = _messageRepository.GetByAuthor(authorId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<FollowerMessageDto>).ToList();
        
        return new PagedResult<FollowerMessageDto>(items, result.TotalCount);
    }

    public void DeleteMessage(long messageId, long authorId)
    {
        var message = _messageRepository.Get(messageId);
        
        if (message.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only delete your own messages");

        _messageRepository.Delete(messageId);

        // TODO (taèka 3): Ovde æe se obrisati povezane notifikacije
        // _notificationService.DeleteFollowerMessageNotifications(messageId);
    }
}
