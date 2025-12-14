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
        public ActionResult<List<NotificationDto>> GetMine()
        {
            var touristId = User.PersonId();
            var result = _notificationService.GetForTourist(touristId);
            return Ok(result);
        }

        [HttpPatch("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            var touristId = User.PersonId();
            _notificationService.MarkAsRead(id, touristId);
            return Ok();
        }
    }
}
