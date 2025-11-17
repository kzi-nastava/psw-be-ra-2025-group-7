using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize(Policy = "authorPolicy")]
[Route("api/user/profile")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("{userId:long}")]
    public ActionResult<UserProfileDto> GetByUserId(long userId)
    {
        var result = _userProfileService.GetByUserId(userId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<UserProfileDto> Create([FromBody] UserProfileDto profile)
    {
        var result = _userProfileService.Create(profile);
        return Ok(result);
    }

    [HttpPut]
    public ActionResult<UserProfileDto> Update([FromBody] UserProfileDto profile)
    {
        var result = _userProfileService.Update(profile);
        return Ok(result);
    }
}