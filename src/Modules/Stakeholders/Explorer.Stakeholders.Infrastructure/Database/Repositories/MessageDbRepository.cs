using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Infrastructure.Database;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class MessageDbRepository : IMessageRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<Message> _dbSet;

        public MessageDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Message>();
        }

        public Message Create(Message message)
        {
            _dbSet.Add(message);
            DbContext.SaveChanges();
            return message;
        }

        public void Delete(long messageId)
        {
            _dbSet.Remove(Get(messageId));
            DbContext.SaveChanges();
        }

        public Message Get(long id)
        {
            return _dbSet.Find(id) ?? throw new NotFoundException("Not found: " + id);
        }

        public PagedResult<Message> GetPagedByConversation(long recipientId, long senderId, int pageNumber, int pageSize)
        {
            var task = _dbSet
                .Where(m => (m.SentToUserId == recipientId && m.SentByUserId == senderId)
                         || (m.SentToUserId == senderId && m.SentByUserId == recipientId))
                .GetPagedById(pageNumber, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<Message> GetPagedRecent(long recipientId, int pageNumber, int pageSize)
        {
            var task = _dbSet
                .Where(m => m.SentToUserId == recipientId || m.SentByUserId == recipientId)
                .GetPagedById(pageNumber, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<long> GetPagedContacts(long userId, int pageNumber, int pageSize)
        {
            var task = _dbSet
                .Where(m => m.SentToUserId == userId || m.SentByUserId == userId)
                .GroupBy(m => m.SentByUserId == userId ? m.SentToUserId : m.SentByUserId)
                .Select(g => g.First())
                .GetPagedById(pageNumber, pageSize);
            task.Wait();
            var ids = task.Result.Results.Select(r => r.SentByUserId == userId ? r.SentToUserId : r.SentByUserId);
            return new PagedResult<long>(ids.ToList(), task.Result.TotalCount);
        }

        public Message Update(Message message)
        {
            try
            {
                DbContext.Update(message);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return message;
        }
    }
}
