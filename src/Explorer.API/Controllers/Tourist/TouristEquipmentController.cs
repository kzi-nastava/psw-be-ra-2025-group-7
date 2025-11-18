using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[ApiController]
[Route("api/tourists")]
//[Authorize(Policy = "touristPolicy")]
public class TouristEquipmentController : ControllerBase
{
    private readonly ITouristEquipmentService _service;

    public TouristEquipmentController(ITouristEquipmentService service)
    {
        _service = service;
    }

    // GET api/tourists/equipment
    // Vraća SVU opremu (za listanje u dropdown-u na frontu)
    [HttpGet("equipment")]
    [AllowAnonymous]
    public ActionResult<List<EquipmentDto>> GetAll()
    {
        var result = _service.GetAllEquipment();
        return Ok(result);
    }

    // GET api/tourists/{touristId}/equipment
    // Also support legacy: GET api/tourists/equipment/{touristId}
    [HttpGet("{touristId:long}/equipment")]
    [HttpGet("equipment/{touristId:long}")]
    [AllowAnonymous]
    public ActionResult<List<TouristEquipmentDto>> GetByTourist(long touristId)
    {
        var result = _service.GetByTourist(touristId);
        return Ok(result);
    }

    // POST api/tourists/{touristId}/equipment/{equipmentId}
    [HttpPost("{touristId:long}/equipment/{equipmentId:long}")]
    public IActionResult AddEquipmentToTourist(long touristId, long equipmentId)
    {
        _service.AddEquipmentToTourist(touristId, equipmentId);
        return Ok();
    }

    // DELETE api/tourists/equipment/{touristEquipmentId}
    [HttpDelete("equipment/{touristEquipmentId:long}")]
    public IActionResult RemoveEquipmentFromTourist(long touristEquipmentId)
    {
        _service.RemoveEquipmentFromTourist(touristEquipmentId);
        return Ok();
    }



    // PUT api/tourists/equipment
    [HttpPut("equipment")]
    public ActionResult<List<TouristEquipmentDto>> Update(
        [FromBody] UpdateTouristEquipmentDto request)
    {
        var result = _service.UpdateForTourist(request);
        return Ok(result);
    }
}
