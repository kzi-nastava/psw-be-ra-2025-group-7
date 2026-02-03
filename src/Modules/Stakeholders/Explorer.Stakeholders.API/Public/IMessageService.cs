using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        /// <summary>
        ///     Gets a paged list of users who have previously sent/received messages with the specified user.
        /// </summary>
        PagedResult<ContactDto> GetPagedContacts(long userId, int pageNumber, int pageSize);
        PagedResult<MessageDto> GetPagedRecent(long recipientId, int pageNumber, int pageSize);
        PagedResult<MessageDto> GetPagedByConversation(long recipientId, long senderId, int pageNumber, int pageSize);
        MessageDto SendMessage(MessageDto messageDto);
        MessageDto EditMessage(MessageDto messageDto);
        void DeleteMessage(long messageId, long requesterId);
        
        /// <summary>
        ///     Searches for users by username for messaging purposes
        /// </summary>
        List<UserSearchResultDto> SearchUsers(string searchTerm, long requesterId);
        
        /// <summary>
        ///     Gets all active users for messaging purposes
        /// </summary>
        List<UserSearchResultDto> GetAllUsers(long requesterId);
    }
}
