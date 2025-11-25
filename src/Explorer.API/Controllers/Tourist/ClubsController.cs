using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/clubs")]
[ApiController]
public class ClubsController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubsController(IClubService clubService) 
    { 
        _clubService = clubService;
    }

    [HttpGet]
    public ActionResult<List<ClubDto>> GetAll()
    { 
        var result = _clubService.GetAll();
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<ClubDto> Create([FromBody] ClubDto dto)
    {
        var created = _clubService.Create(dto);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
    {
        dto.Id = id;
        var updated = _clubService.Update(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id) 
    {
        _clubService.Delete(id);
        return Ok();
    }
}

