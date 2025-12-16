using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<List<NotificationDto>> GetMine([FromQuery] bool onlyUnread = false)
        {
            var touristId = User.PersonId();
            var result = _notificationService.GetForUser(touristId, onlyUnread);
            return Ok(result);
        }

        [HttpGet("unread")]
        public ActionResult<List<NotificationDto>> GetUnread()
        {
            var touristId = User.PersonId();
            var result = _notificationService.GetForUser(touristId, onlyUnread: true);
            return Ok(result);
        }

        [HttpGet("unread/count")]
        public ActionResult<int> GetUnreadCount()
        {
            var touristId = User.PersonId();
            var count = _notificationService.GetUnreadCount(touristId);
            return Ok(count);
        }

        [HttpPatch("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            var touristId = User.PersonId();
            _notificationService.MarkAsRead(id, touristId);
            return Ok();
        }

        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            var touristId = User.PersonId();
            _notificationService.MarkAllAsRead(touristId);
            return Ok();
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            var touristId = User.PersonId();
            _notificationService.Delete(id, touristId);
            return Ok();
        }
    }
}
