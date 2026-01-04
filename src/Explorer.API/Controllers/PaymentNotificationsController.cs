using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/payments/notifications")]
    [ApiController]
    public class PaymentNotificationsController : ControllerBase
    {
        private readonly IPaymentNotificationService _notificationService;
        public PaymentNotificationsController(
            IPaymentNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 🔹 Sve notifikacije
        [HttpGet]
        public ActionResult<List<PaymentNotificationDto>> GetAll()
        {
            var userId = User.PersonId();
            var notifications = _notificationService.GetAll(userId);
            return Ok(notifications);
        }

        // 🔹 Nepročitane notifikacije
        [HttpGet("unread")]
        public ActionResult<List<PaymentNotificationDto>> GetUnread()
        {
            var userId = User.PersonId();
            var notifications = _notificationService.GetUnread(userId);
            return Ok(notifications);
        }

        // 🔹 Obeleži kao pročitano
        [HttpPut("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            var userId = User.PersonId();
            _notificationService.MarkAsRead(id, userId);
            return NoContent();
        }
    }
}
