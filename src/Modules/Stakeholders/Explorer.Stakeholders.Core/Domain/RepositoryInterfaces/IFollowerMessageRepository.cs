using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IFollowerMessageRepository
{
    FollowerMessage Create(FollowerMessage message);
    FollowerMessage Get(long id);
    PagedResult<FollowerMessage> GetByAuthor(long authorId, int page, int pageSize);
    void Delete(long id);
}
