using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/admin/reviews")]
    public class AdminReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public AdminReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<ReviewDto>> GetAll()
        {
            return Ok(_service.GetAll());
        }
    }
}
