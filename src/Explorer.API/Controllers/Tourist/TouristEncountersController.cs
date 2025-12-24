using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounters")]
    public class TouristEncountersController : ControllerBase
    {
        private readonly IEncounterService _service;

        public TouristEncountersController(IEncounterService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EncounterDto>> GetActiveEncounters()
        {
            try
            {
                var encounters = _service.Get(status: "active", type: null);
                return Ok(encounters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to load active encounters.");
            }
        }
    }
}