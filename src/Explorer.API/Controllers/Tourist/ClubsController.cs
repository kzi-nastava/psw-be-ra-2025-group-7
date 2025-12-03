using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.Exceptions;

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
        var userId = TryGetUserId();
        if (userId.HasValue)
        {
            dto.CreatedBy = userId.Value;
        }

        dto.CreatedAt = DateTime.UtcNow;

        var created = _clubService.Create(dto);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
    {
        dto.Id = id;

        var userId = TryGetUserId();

        var existing = _clubService.GetAll().FirstOrDefault(c => c.Id == id);
        if (existing == null)
            throw new NotFoundException("Club not found.");

        if (userId.HasValue && existing.CreatedBy != userId.Value)
            return Forbid();

        var updated = _clubService.Update(dto);
        return Ok(updated);
    }



    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var userId = TryGetUserId();

        var existing = _clubService.GetAll().FirstOrDefault(c => c.Id == id);
        if (existing == null)
            throw new NotFoundException("Club not found.");

        if (userId.HasValue && existing.CreatedBy != userId.Value)
            return Forbid();

        _clubService.Delete(id);
        return Ok();
    }



    private long? TryGetUserId()
    {
        var claim = User?.FindFirst("id");
        if (claim == null) return null;

        if (long.TryParse(claim.Value, out var id))
        {
            return id;
        }

        return null;
    }

}

