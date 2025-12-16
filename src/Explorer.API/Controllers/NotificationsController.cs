using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<List<NotificationDto>> GetAll()
        {
            var userId = User.PersonId();
            var result = _notificationService.GetForUser(userId, onlyUnread: false);
            return Ok(result);
        }

        [HttpGet("unread")]
        public ActionResult<List<NotificationDto>> GetUnread()
        {
            var userId = User.PersonId();
            var result = _notificationService.GetForUser(userId, onlyUnread: true);
            return Ok(result);
        }

        [HttpGet("unread/count")]
        public ActionResult<int> GetUnreadCount()
        {
            var userId = User.PersonId();
            var count = _notificationService.GetUnreadCount(userId);
            return Ok(count);
        }

        [HttpPut("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            var userId = User.PersonId();
            _notificationService.MarkAsRead(id, userId);
            return NoContent();
        }

        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            var userId = User.PersonId();
            _notificationService.MarkAllAsRead(userId);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            var userId = User.PersonId();
            _notificationService.Delete(id, userId);
            return NoContent();
        }
    }
}
