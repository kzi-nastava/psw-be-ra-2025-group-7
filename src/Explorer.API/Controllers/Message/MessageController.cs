using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Message;

[Authorize(Policy = "messagePolicy")]
[Route("api/messages")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    [Route("contacts")]
    public ActionResult<PagedResult<MessageDto>> GetPagedContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = _messageService.GetPagedContacts(User.PersonId(), pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<PagedResult<MessageDto>> GetPagedByRecipientId([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30)
    {
        var result = _messageService.GetPagedByRecipientId(User.PersonId(), pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet]
    [Route("contacts/{contactId:long}")]
    public ActionResult<PagedResult<MessageDto>> GetPagedByConversation([FromRoute] long contactId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30)
    {
        var result = _messageService.GetPagedByConversation(User.PersonId(), contactId, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<MessageDto> SendMessage([FromBody] MessageDto messageDto)
    {
        messageDto.SentByUserId = User.PersonId();
        var result = _messageService.SendMessage(messageDto);
        return Ok(result);
    }

    [HttpPut]
    public ActionResult<MessageDto> EditMessage([FromBody] MessageDto messageDto)
    {
        messageDto.SentByUserId = User.PersonId();
        var result = _messageService.EditMessage(messageDto);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult DeleteMessage([FromRoute] long id)
    {
        _messageService.DeleteMessage(id, User.PersonId());
        return Ok();
    }
}