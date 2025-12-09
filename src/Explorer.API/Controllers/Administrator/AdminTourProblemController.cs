using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/admin/tour-problems")]
    public class AdminTourProblemController : ControllerBase
    {
        private readonly ITourProblemService _service;

        public AdminTourProblemController(ITourProblemService service)
        {
            _service = service;
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

            return Ok(_service.SetResolveDue(id, dto.ResolveDue));
        }

        [HttpPut("{id}/penalty")]
        public ActionResult<TourProblemDto> SetPenalty(int id)
        {
            var result = _service.SetPenalty(id);
            return Ok(result);
        }


    }
}
