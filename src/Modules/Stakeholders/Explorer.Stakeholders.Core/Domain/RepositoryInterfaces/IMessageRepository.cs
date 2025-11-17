using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        PagedResult<long> GetPagedContacts(long userId, int pageNumber, int pageSize);
        PagedResult<Message> GetPagedRecent(long recipientId, int pageNumber, int pageSize);
        PagedResult<Message> GetPagedByConversation(long recipientId, long senderId, int pageNumber, int pageSize);
        Message Get(long id);
        Message Create(Message message);
        Message Update(Message message);
        void Delete(long messageId);
        void Delete(Message message) { Delete(message.Id); }
    }
}
