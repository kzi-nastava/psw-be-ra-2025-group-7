using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Club;

[Authorize(Policy = "touristPolicy")]
[Route("api/club-messages")]
[ApiController]
public class ClubMessageController : ControllerBase
{
    private readonly IClubMessageService _clubMessageService;

    public ClubMessageController(IClubMessageService clubMessageService)
    {
        _clubMessageService = clubMessageService;
    }

    /// <summary>
    /// Kreiraj poruku na stranici kluba (samo èlanovi kluba)
    /// </summary>
    [HttpPost]
    public ActionResult<ClubMessageDto> CreateMessage([FromBody] ClubMessageDto messageDto)
    {
        var authorId = User.PersonId();
        messageDto.AuthorId = authorId;
        
        var result = _clubMessageService.CreateMessage(messageDto);
        return Ok(result);
    }

    /// <summary>
    /// Ažuriraj poruku (samo autor može)
    /// </summary>
    [HttpPut("{messageId:long}")]
    public ActionResult<ClubMessageDto> UpdateMessage(long messageId, [FromBody] ClubMessageDto messageDto)
    {
        var requesterId = User.PersonId();
        messageDto.Id = messageId;
        
        var result = _clubMessageService.UpdateMessage(messageDto, requesterId);
        return Ok(result);
    }

    /// <summary>
    /// Obriši poruku (samo vlasnik kluba može)
    /// </summary>
    [HttpDelete("{messageId:long}")]
    public ActionResult DeleteMessage(long messageId)
    {
        var requesterId = User.PersonId();
        _clubMessageService.DeleteMessage(messageId, requesterId);
        return Ok();
    }

    /// <summary>
    /// Preuzmi sve poruke na stranici kluba
    /// </summary>
    [HttpGet("club/{clubId:long}")]
    public ActionResult<PagedResult<ClubMessageDto>> GetClubMessages(
        long clubId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = _clubMessageService.GetClubMessages(clubId, page, pageSize);
        return Ok(result);
    }
}
