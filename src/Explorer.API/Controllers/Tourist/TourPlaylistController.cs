using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-executions/{executionId}/playlist")]
[ApiController]
public class TourPlaylistController : ControllerBase
{
    private readonly ITourPlaylistService _playlistService;

    public TourPlaylistController(ITourPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpPost]
    public async Task<ActionResult<TourPlaylistDto>> GeneratePlaylist(long executionId, [FromBody] GeneratePlaylistDto dto)
    {
        var touristId = User.PersonId();
        var result = await _playlistService.GeneratePlaylist(touristId, executionId, dto);
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<TourPlaylistDto> GetPlaylist(long executionId)
    {
        var touristId = User.PersonId();
        var result = _playlistService.GetPlaylist(touristId, executionId);
        return Ok(result);
    }

    [HttpDelete]
    public ActionResult DeletePlaylist(long executionId)
    {
        var touristId = User.PersonId();
        _playlistService.DeletePlaylist(touristId, executionId);
        return NoContent();
    }
}