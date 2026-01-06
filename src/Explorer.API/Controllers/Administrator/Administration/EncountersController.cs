using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [ApiController]
    [Route("api/administrator/encounters")]
    public class EncountersController : ControllerBase
    {
        private readonly IEncounterService _service;
        private readonly IEncounterProgressService _encounterProgressService;

        public EncountersController(IEncounterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] CreateEncounterDto dto)
        {
            var creatorId = (int)User.PersonId();
            var encounter = _service.Create(creatorId, dto);
            return Ok(encounter);
        }

        [HttpPut("{id:long}")]
        public ActionResult<EncounterDto> Update(long id, [FromBody] UpdateEncounterDto dto)
        {
            int creatorId = (int)User.PersonId();
            var encounter = _service.Update(id, creatorId, dto);
            return Ok(encounter);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            int creatorId = (int)User.PersonId();
            _service.Delete(id, creatorId);
            return NoContent();
        }


        [HttpPut("{id:long}/status")]
        public ActionResult<EncounterDto> ChangeStatus(long id, [FromBody] ChangeEncounterStatusDto dto)
            => Ok(_service.ChangeStatus(id, dto.Status));

        /* [HttpGet("{id:long}")]
         public ActionResult<EncounterDto> Get(long id)
             => Ok(_service.Get(id));*/


        [HttpGet("{id:long}")]
        public ActionResult Get(long id)
        {
            try
            {
                return Ok(_service.Get(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<EncounterDto>> Get([FromQuery] string? status, [FromQuery] string? type)
        {
            var result = _service.Get(status, type);
            return Ok(result);
        }
     

        [HttpPut("{id:long}/accept")]
        public ActionResult<EncounterDto> AcceptEncounter(long id, [FromBody] UpdateEncounterDto dto)
        {
            var result = _service.AcceptEncounter(id, dto);
            return Ok(result);
        }

        [HttpPut("{id:long}/decline")]
        public ActionResult<EncounterDto> DeclineEncounter(long id)
        {
            var result = _service.DeclineEncounter(id);
            return Ok(result);
        }

    }
}
