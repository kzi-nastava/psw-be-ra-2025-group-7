using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository repository, IUserRepository userRepository, IUserProfileRepository profileRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public PagedResult<ContactDto> GetPagedContacts(long userId, int pageNumber, int pageSize)
        {
            var pagedUserIds = _repository.GetPagedContacts(userId, pageNumber, pageSize);
            
            var contacts = new List<ContactDto>();
            
            foreach (var contactUserId in pagedUserIds.Results)
            {
                var user = _userRepository.GetById(contactUserId);
                if (user == null) continue;
                
                var profile = _profileRepository.GetByUserId(contactUserId);
                
                var displayName = profile != null 
                    ? $"{profile.FirstName} {profile.LastName}"
                    : user.Username;
                
                // Get last message between users
                var lastMessages = _repository.GetPagedByConversation(userId, contactUserId, 1, 1);
                var lastMessage = lastMessages.Results.FirstOrDefault();
                
                contacts.Add(new ContactDto
                {
                    UserId = contactUserId,
                    Username = user.Username,
                    DisplayName = displayName,
                    ProfilePicture = profile?.ProfilePicture,
                    LastMessageContent = lastMessage?.Content,
                    LastMessageAt = lastMessage?.SentAt,
                    IsLastMessageByMe = lastMessage?.SentByUserId == userId
                });
            }
            
            return new PagedResult<ContactDto>(contacts, pagedUserIds.TotalCount);
        }

        public PagedResult<MessageDto> GetPagedRecent(long recipientId, int pageNumber, int pageSize)
        {
            var res = _repository.GetPagedRecent(recipientId, pageNumber, pageSize);
            var items = res.Results.Select(m => EnrichMessageDto(m)).ToList();
            return new PagedResult<MessageDto>(items, res.TotalCount);
        }

        public PagedResult<MessageDto> GetPagedByConversation(long recipientId, long senderId, int pageNumber, int pageSize)
        {
            var res = _repository.GetPagedByConversation(recipientId, senderId, pageNumber, pageSize);
            var items = res.Results.Select(m => EnrichMessageDto(m)).ToList();
            return new PagedResult<MessageDto>(items, res.TotalCount);
        }

        public MessageDto SendMessage(MessageDto messageDto)
        {
            messageDto.SentAt = DateTime.UtcNow;
            messageDto.EditedAt = null;
            var message = _mapper.Map<Message>(messageDto);
            var createdMessage = _repository.Create(message);
            return EnrichMessageDto(createdMessage);
        }

        public MessageDto EditMessage(MessageDto messageDto)
        {
            messageDto.EditedAt = DateTime.UtcNow;

            var message = _repository.Get(messageDto.Id);

            if(message.SentByUserId != messageDto.SentByUserId)
            {
                throw new UnauthorizedAccessException("You are not allowed to edit this message.");
            }

            // Only the content and EditedAt fields are updatable
            message.Edit(messageDto.Content, DateTime.UtcNow);

            var updatedMessage = _repository.Update(message);
            return EnrichMessageDto(updatedMessage);
        }

        public void DeleteMessage(long messageId, long requesterId)
        {
            if (_repository.Get(messageId).SentByUserId != requesterId)
            {
                throw new UnauthorizedAccessException("You are not allowed to delete this message.");
            }

            _repository.Delete(messageId);
        }

        public List<UserSearchResultDto> SearchUsers(string searchTerm, long requesterId)
        {
            var users = _userRepository.SearchByUsername(searchTerm);
            
            // Exclude the requester from results
            users = users.Where(u => u.Id != requesterId).ToList();

            return MapUsersToSearchResults(users);
        }

        public List<UserSearchResultDto> GetAllUsers(long requesterId)
        {
            var users = _userRepository.GetAllActiveUsers();
            
            // Exclude the requester from results
            users = users.Where(u => u.Id != requesterId).ToList();

            return MapUsersToSearchResults(users);
        }

        private MessageDto EnrichMessageDto(Message message)
        {
            var dto = _mapper.Map<MessageDto>(message);
            
            // Enrich sender info
            var senderUser = _userRepository.GetById(message.SentByUserId);
            var senderProfile = _profileRepository.GetByUserId(message.SentByUserId);
            
            if (senderUser != null)
            {
                dto.SentByUsername = senderUser.Username;
                dto.SentByDisplayName = senderProfile != null 
                    ? $"{senderProfile.FirstName} {senderProfile.LastName}"
                    : senderUser.Username;
                dto.SentByProfilePicture = senderProfile?.ProfilePicture;
            }
            
            // Enrich receiver info
            var receiverUser = _userRepository.GetById(message.SentToUserId);
            var receiverProfile = _profileRepository.GetByUserId(message.SentToUserId);
            
            if (receiverUser != null)
            {
                dto.SentToUsername = receiverUser.Username;
                dto.SentToDisplayName = receiverProfile != null 
                    ? $"{receiverProfile.FirstName} {receiverProfile.LastName}"
                    : receiverUser.Username;
                dto.SentToProfilePicture = receiverProfile?.ProfilePicture;
            }
            
            return dto;
        }

        private List<UserSearchResultDto> MapUsersToSearchResults(List<User> users)
        {
            var results = new List<UserSearchResultDto>();

            foreach (var user in users)
            {
                var profile = _profileRepository.GetByUserId(user.Id);
                
                var displayName = profile != null 
                    ? $"{profile.FirstName} {profile.LastName}"
                    : user.Username;

                results.Add(new UserSearchResultDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    DisplayName = displayName,
                    ProfilePicture = profile?.ProfilePicture,
                    Role = user.Role.ToString()
                });
            }

            return results;
        }
    }
}
