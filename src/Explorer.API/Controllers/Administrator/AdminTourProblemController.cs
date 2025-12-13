using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Notifications.API.Public;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.UseCases.Administration;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/admin/tour-problems")]
    public class AdminTourProblemController : ControllerBase
    {
        private readonly ITourProblemService _service;
        private readonly INotificationService _notificationService;
        private readonly ITourService _tourService;

        public AdminTourProblemController(ITourProblemService service, INotificationService notificationService, ITourService tourService)
        {
            _service = service;
            _notificationService = notificationService;
            _tourService = tourService;
        }

        [HttpGet("all")]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult<List<TourProblemDto>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpPut("{id}/resolve-due")]
        public ActionResult<TourProblemDto> SetResolveDue(int id, [FromBody] ResolveDueDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResolveDue))
                return BadRequest("ResolveDue is required.");

            // 1️⃣ ažuriraj problem
            var updatedProblem = _service.SetResolveDue(id, dto.ResolveDue);

            // 2️⃣ pošalji notifikaciju
            _notificationService.CreateProblemMessageNotification(
                recipientUserId: (_tourService.GetById(updatedProblem.TourId)).AuthorId,
                problemId: updatedProblem.Id,
                messagePreview: $"Rok za rešavanje problema je postavljen na {dto.ResolveDue}.",
                createdAt: DateTime.UtcNow
            );

            return Ok(updatedProblem);
        }
        [HttpPost("{id:int}/reply")]
        public ActionResult<TourProblemDto> AdminReply(int id, [FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest("Message cannot be empty.");
            var adminId = (int)User.PersonId();
            try
            {
                var updated = _service.AddAuthorReply(id, adminId, message);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/penalty")]
        public ActionResult<TourProblemDto> SetPenalty(int id)
        {
            var result = _service.SetPenalty(id);
            return Ok(result);
        }

        [HttpPut("{id}/archive")]
        public ActionResult<TourProblemDto> ArchiveTour(int id)
        {
            var result = _service.ArchiveTour(id);
            return Ok(result);
        }


    }
}
