using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/annual-awards")]
[ApiController]
public class AnnualAwardController : ControllerBase
{
    private readonly IAnnualAwardService _annualAwardService;

    public AnnualAwardController(IAnnualAwardService annualAwardService)
    {
        _annualAwardService = annualAwardService;
    }

    [HttpGet]
    public ActionResult<PagedResult<AnnualAwardDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _annualAwardService.GetPaged(page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<AnnualAwardDto> Create([FromBody] CreateAnnualAwardDto dto)
    {
        var created = _annualAwardService.Create(dto);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<AnnualAwardDto> Update([FromRoute] long id, [FromBody] UpdateAnnualAwardDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var updated = _annualAwardService.Update(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete([FromRoute] long id)
    {
        _annualAwardService.Delete(id);
        return NoContent();
    }
}