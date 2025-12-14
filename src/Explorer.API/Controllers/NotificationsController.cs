using Explorer.Notifications.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.API.Controllers
{
    [Authorize]  // svaki ulogovan korisnik
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public ActionResult<List<NotificationDto>> GetAll([FromQuery] bool onlyUnread = false)
        {
            var userId = (int)User.PersonId();
            var items = _notificationRepository.GetForUser(userId, onlyUnread);

            // ako želiš DTO, napravi mapiranje; za sada možeš i direktno
            var result = items.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Preview = n.Preview,
                ProblemId = n.ProblemId,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            return Ok(result);
        }

        [HttpPut("{id:int}/read")]
        public IActionResult MarkAsRead(int id)
        {
            var userId = (int)User.PersonId();
            _notificationRepository.MarkAsRead(id, userId);
            return NoContent();
        }
    }

    public class NotificationDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Preview { get; set; }
        public int ProblemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
