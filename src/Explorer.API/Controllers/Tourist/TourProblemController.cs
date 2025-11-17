using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication; // User.PersonId()
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-problems")]
    [ApiController]
    public class TourProblemController : ControllerBase
    {
        private readonly ITourProblemService _service;

        public TourProblemController(ITourProblemService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourProblemDto>> GetTouristProblems(
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            var touristId = (int)User.PersonId();
            var result = _service.GetTouristProblemsPages(touristId, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<TourProblemDto> Create([FromBody] TourProblemDto tourProblem)
        {
            var touristId = (int)User.PersonId();
            tourProblem.Id = 0;
            tourProblem.TouristId = touristId;

            var created = _service.Create(tourProblem, touristId);
            return Ok(created);
        }

        [HttpPut("{id:int}")]
        public ActionResult<TourProblemDto> Update(int id, [FromBody] TourProblemDto tourProblem)
        {
            var touristId = (int)User.PersonId();
            tourProblem.Id = id;
            tourProblem.TouristId = touristId;

            var updated = _service.Update(tourProblem, touristId);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var touristId = (int)User.PersonId();
            _service.Delete(id, touristId);
            return Ok();
        }
    }
}
