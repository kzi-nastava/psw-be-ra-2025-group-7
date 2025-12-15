using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IClubMessageService
{
    // Èlanovi kluba postavljaju/menjaju poruku
    ClubMessageDto CreateMessage(ClubMessageDto messageDto);
    ClubMessageDto UpdateMessage(ClubMessageDto messageDto, long requesterId);
    
    // Vlasnik kluba briše poruke
    void DeleteMessage(long messageId, long requesterId);
    
    // Pregled poruka na stranici kluba
    PagedResult<ClubMessageDto> GetClubMessages(long clubId, int page, int pageSize);
}
