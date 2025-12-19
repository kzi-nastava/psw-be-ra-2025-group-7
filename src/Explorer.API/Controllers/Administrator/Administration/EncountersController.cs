using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [ApiController]
    [Route("api/administrator/encounters")]
    public class EncountersController : ControllerBase
    {
        private readonly IEncounterService _service;

        public EncountersController(IEncounterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] CreateEncounterDto dto)
            => Ok(_service.Create(dto));

        [HttpPut("{id:long}")]
        public ActionResult<EncounterDto> Update(long id, [FromBody] UpdateEncounterDto dto)
            => Ok(_service.Update(id, dto));

        [HttpDelete("{id:long}")]

        public IActionResult Delete(long id)
        {
            _service.Delete(id);
            return NoContent();
        }


        [HttpPut("{id:long}/status")]
        public ActionResult<EncounterDto> ChangeStatus(long id, [FromBody] ChangeEncounterStatusDto dto)
            => Ok(_service.ChangeStatus(id, dto.Status));

        [HttpGet("{id:long}")]
        public ActionResult<EncounterDto> Get(long id)
            => Ok(_service.Get(id));

        [HttpGet]
        public ActionResult<IEnumerable<EncounterDto>> Get([FromQuery] string? status, [FromQuery] string? type)
        {
            var result = _service.Get(status, type);
            return Ok(result);
        }

    }
}
