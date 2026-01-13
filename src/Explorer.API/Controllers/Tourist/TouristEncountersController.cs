using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
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
        [HttpGet("{encounterId:long}/hidden-location-progress")]
        public ActionResult<HiddenLocationProgressDto> GetHiddenLocationProgress(long encounterId)
        {
            var userId = User.PersonId();
            var progress = _encounterProgressService.GetHiddenLocationProgress(encounterId, userId);
            return Ok(progress);
        }

        [HttpPost("{encounterId:long}/activate-social")]
        public IActionResult ActivateSocialEncounter(long encounterId)
        {
            try
            {
                var userId = User.PersonId();
                _encounterProgressService.ActivateSocialEncounter(encounterId, userId);
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

        [HttpGet("{encounterId:long}/check_location")]
        public ActionResult<bool> IsUserAtLocation(long encounterId)
        {
            var userId = User.PersonId();
            var progress = _encounterProgressService.IsUserAtLocation(encounterId, userId);
            return Ok(progress);
        }

        [HttpGet("has_active_encounter")]
        public ActionResult<bool> HasActiveSocialEncounter()
        {
            var userId = User.PersonId();
            var progress = _encounterProgressService.HasActiveSocialEncounter(userId);
            return Ok(progress);
        }

        [HttpGet("{encounterId:long}/check_progress")]
        public ActionResult<bool> CheckEncounterProgress(long encounterId)
        {
            var progress = _encounterProgressService.CheckEncounterProgress(encounterId);
            return Ok(progress);
        }

        [HttpGet("get_social")]
        public ActionResult<EncounterDto> GetActiveSocial()
        {
            var progress = _encounterProgressService.GetActiveSocial();
            return Ok(progress);
        }

    }
}