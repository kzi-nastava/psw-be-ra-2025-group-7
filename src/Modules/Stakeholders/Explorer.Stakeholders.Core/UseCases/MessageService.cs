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
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public PagedResult<long> GetPagedContacts(long userId, int pageNumber, int pageSize)
        {
            return _repository.GetPagedContacts(userId, pageNumber, pageSize);
        }

        public PagedResult<MessageDto> GetPagedByRecipientId(long recipientId, int pageNumber, int pageSize)
        {
            var res = _repository.GetPagedByRecipientId(recipientId, pageNumber, pageSize);
            var items = res.Results.Select(_mapper.Map<MessageDto>).ToList();
            return new PagedResult<MessageDto>(items, res.TotalCount);
        }

        public PagedResult<MessageDto> GetPagedByConversation(long recipientId, long senderId, int pageNumber, int pageSize)
        {
            var res = _repository.GetPagedByConversation(recipientId, senderId, pageNumber, pageSize);
            var items = res.Results.Select(_mapper.Map<MessageDto>).ToList();
            return new PagedResult<MessageDto>(items, res.TotalCount);
        }

        public MessageDto SendMessage(MessageDto messageDto)
        {
            messageDto.SentAt = DateTime.UtcNow;
            messageDto.EditedAt = DateTime.MinValue;
            var message = _mapper.Map<Message>(messageDto);
            var createdMessage = _repository.Create(message);
            return _mapper.Map<MessageDto>(createdMessage);
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
            return _mapper.Map<MessageDto>(updatedMessage);
        }

        public void DeleteMessage(long messageId, long requesterId)
        {
            if (_repository.Get(messageId).SentByUserId != requesterId)
            {
                throw new UnauthorizedAccessException("You are not allowed to delete this message.");
            }

            _repository.Delete(messageId);;
        }
    }
}
