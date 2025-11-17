using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[ApiController]
[Route("api/tourists/equipment")]
[Authorize(Policy = "touristPolicy")]
public class TouristEquipmentController : ControllerBase
{
    private readonly ITouristEquipmentService _service;

    public TouristEquipmentController(ITouristEquipmentService service)
    {
        _service = service;
    }

    // GET api/tourists/equipment/{touristId}
    [HttpGet("{touristId:long}")]
    public ActionResult<List<TouristEquipmentDto>> GetByTourist(long touristId)
    {
        var result = _service.GetByTourist(touristId);
        return Ok(result);
    }

    // PUT api/tourists/equipment
    [HttpPut]
    public ActionResult<List<TouristEquipmentDto>> Update(
        [FromBody] UpdateTouristEquipmentDto request)
    {
        var result = _service.UpdateForTourist(request);
        return Ok(result);
    }
}
