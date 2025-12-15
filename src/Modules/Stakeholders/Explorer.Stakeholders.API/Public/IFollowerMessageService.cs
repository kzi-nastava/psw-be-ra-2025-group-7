using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IFollowerMessageService
{
    // Kreiranje poruke za sve pratioce
    FollowerMessageDto SendMessageToFollowers(FollowerMessageDto messageDto);
    
    // Pregled poruka koje je korisnik poslao svojim pratiocima
    PagedResult<FollowerMessageDto> GetMyMessages(long authorId, int page, int pageSize);
    
    // Brisanje poruke
    void DeleteMessage(long messageId, long authorId);
}
