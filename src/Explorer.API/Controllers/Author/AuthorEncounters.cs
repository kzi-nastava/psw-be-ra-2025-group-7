using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [ApiController]
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/encounters")]
    public class AuthorEncounters : ControllerBase
    {
        private readonly IEncounterService _service;
        public AuthorEncounters(IEncounterService service)
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
    }
}
