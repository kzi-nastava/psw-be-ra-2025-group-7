using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Follower;

[Authorize(Policy = "touristPolicy")]
[Route("api/follower-messages")]
[ApiController]
public class FollowerMessageController : ControllerBase
{
    private readonly IFollowerMessageService _followerMessageService;

    public FollowerMessageController(IFollowerMessageService followerMessageService)
    {
        _followerMessageService = followerMessageService;
    }

    /// <summary>
    /// Pošalji poruku svim pratiocima
    /// </summary>
    [HttpPost]
    public ActionResult<FollowerMessageDto> SendMessageToFollowers([FromBody] FollowerMessageDto messageDto)
    {
        var authorId = User.PersonId();
        messageDto.AuthorId = authorId;
        
        var result = _followerMessageService.SendMessageToFollowers(messageDto);
        return Ok(result);
    }

    /// <summary>
    /// Preuzmi sve poruke koje sam poslao svojim pratiocima
    /// </summary>
    [HttpGet("my-messages")]
    public ActionResult<PagedResult<FollowerMessageDto>> GetMyMessages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var authorId = User.PersonId();
        var result = _followerMessageService.GetMyMessages(authorId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Obriši poruku
    /// </summary>
    [HttpDelete("{messageId:long}")]
    public ActionResult DeleteMessage(long messageId)
    {
        var authorId = User.PersonId();
        _followerMessageService.DeleteMessage(messageId, authorId);
        return Ok();
    }
}
