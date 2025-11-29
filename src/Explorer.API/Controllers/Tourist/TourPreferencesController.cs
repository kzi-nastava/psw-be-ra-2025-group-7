using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/preferences")]
    [ApiController]
    public class TourPreferencesController : ControllerBase
    {
        private readonly ITourPreferencesService _service;

        public TourPreferencesController(ITourPreferencesService service)
        {
            _service = service;
        }

        // GET api/tourist/preferences/{touristId}
        [HttpGet("{touristId:long}")]
        public ActionResult<TourPreferencesDto> Get(long touristId)
        {
            var result = _service.GetByTouristId(touristId);
            return Ok(result);
        }

        // POST api/tourist/preferences
        [HttpPost]
        public ActionResult<TourPreferencesDto> Create([FromBody] TourPreferencesDto tourPreferencesDto)
        {
            var created = _service.Create(tourPreferencesDto);
            return Ok(created);
        }

        // PUT api/tourist/preferences/{id}
        [HttpPut("{id:long}")]
        public ActionResult<TourPreferencesDto> Update([FromBody] TourPreferencesDto tourPreferencesDto)
        {
            var updated = _service.Update(tourPreferencesDto);
            return Ok(updated);
        }

        // DELETE api/tourist/preferences/{id}
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _service.Delete(id);
            return Ok();

        }
    }
}
