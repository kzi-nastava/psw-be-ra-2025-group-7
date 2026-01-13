using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/payments/purchase-notifications")]
    [ApiController]
    public class PurchaseNotificationsController : ControllerBase
    {
        private readonly IPurchaseNotificationService _notificationService;

        public PurchaseNotificationsController(IPurchaseNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<List<PurchaseNotificationDto>> GetAll()
        {
            var userId = User.PersonId();
            var notifications = _notificationService.GetAll(userId);
            return Ok(notifications);
        }

        [HttpGet("unread")]
        public ActionResult<List<PurchaseNotificationDto>> GetUnread()
        {
            var userId = User.PersonId();
            var notifications = _notificationService.GetUnread(userId);
            return Ok(notifications);
        }

        [HttpPut("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            var userId = User.PersonId();
            _notificationService.MarkAsRead(id, userId);
            return NoContent();
        }
    }
}
