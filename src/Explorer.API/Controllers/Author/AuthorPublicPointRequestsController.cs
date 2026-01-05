using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Route("api/author/public-point-requests")]
    [ApiController]
    public class AuthorPublicPointRequestsController : ControllerBase
    {
        private readonly PublicPointRequestService _service;

        public AuthorPublicPointRequestsController(PublicPointRequestService service)
        {
            _service = service;
        }

        // ✅ CREATE request (Pending)
        [Authorize(Policy = "authorPolicy")]
        [HttpPost]
        public IActionResult Create([FromBody] CreatePublicPointRequestDto dto)
        {
            var authorId = (int)User.PersonId();

            var req = _service.Create(dto.TourId, dto.KeyPointIndex, authorId);

            return Ok(new
            {
                req.Id,
                req.TourId,
                req.KeyPointIndex,
                req.Status,
                req.CreatedAt
            });
        }
    }

    public class CreatePublicPointRequestDto
    {
        public long TourId { get; set; }
        public int KeyPointIndex { get; set; }
    }
}
