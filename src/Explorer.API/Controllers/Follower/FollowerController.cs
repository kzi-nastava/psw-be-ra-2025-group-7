using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Follower;

[Authorize(Policy = "touristPolicy")]
[Route("api/followers")]
[ApiController]
public class FollowerController : ControllerBase
{
    private readonly IFollowerService _followerService;

    public FollowerController(IFollowerService followerService)
    {
        _followerService = followerService;
    }

    /// <summary>
    /// Prati korisnika
    /// </summary>
    [HttpPost("follow/{followedId:long}")]
    public ActionResult<FollowerDto> Follow(long followedId)
    {
        var followerId = User.PersonId();
        var result = _followerService.Follow(followerId, followedId);
        return Ok(result);
    }

    /// <summary>
    /// Prestani pratiti korisnika
    /// </summary>
    [HttpDelete("unfollow/{followedId:long}")]
    public ActionResult Unfollow(long followedId)
    {
        var followerId = User.PersonId();
        _followerService.Unfollow(followerId, followedId);
        return Ok();
    }

    /// <summary>
    /// Preuzmi pratioce trenutno ulogovanog korisnika (ko mene prati)
    /// </summary>
    [HttpGet("my-followers")]
    public ActionResult<PagedResult<FollowerDto>> GetMyFollowers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var userId = User.PersonId();
        var result = _followerService.GetFollowers(userId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Preuzmi korisnike koje trenutno ulogovani korisnik prati (koga ja pratim)
    /// </summary>
    [HttpGet("my-following")]
    public ActionResult<PagedResult<FollowerDto>> GetMyFollowing(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var userId = User.PersonId();
        var result = _followerService.GetFollowing(userId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Proveri da li trenutno ulogovani korisnik prati drugog korisnika
    /// </summary>
    [HttpGet("is-following/{followedId:long}")]
    public ActionResult<bool> IsFollowing(long followedId)
    {
        var followerId = User.PersonId();
        var result = _followerService.IsFollowing(followerId, followedId);
        return Ok(result);
    }
}
