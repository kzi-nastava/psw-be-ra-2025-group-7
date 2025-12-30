using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounters")]
    public class TouristEncountersController : ControllerBase
    {
        private readonly IEncounterService _service;
        private readonly IEncounterProgressService _encounterProgressService;
        public TouristEncountersController(
            IEncounterService service,
            IEncounterProgressService encounterProgressService)
         {
            _service = service;
            _encounterProgressService = encounterProgressService;
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
        [HttpPost("{encounterId:long}/activate-hidden-location")]
        public IActionResult ActivateHiddenLocation(long encounterId)
        {
            try
            {
                var userId = User.PersonId();
                _encounterProgressService.ActivateHiddenLocationForUser(encounterId, userId);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}