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
    }
}
