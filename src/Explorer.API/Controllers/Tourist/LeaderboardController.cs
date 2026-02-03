using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderBoardService _leaderBoardService;

        public LeaderboardController(ILeaderBoardService leaderBoardService)
        {
            _leaderBoardService = leaderBoardService;
        }

        [HttpGet("top/{count:int}")]
        public ActionResult<List<LeaderboardUserDto>> GetTopUsersByXp(int count)
        {
            try
            {
                var topUsers = _leaderBoardService.GetTopUsersByXp(count);
                return Ok(topUsers);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to load top users by XP.");
            }
        }

        [HttpGet("all")]
        public ActionResult<List<LeaderboardUserDto>> GetAllUsersByXp()
        {
            try
            {
                var allUsers = _leaderBoardService.GetAllUsersByXp();
                return Ok(allUsers);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to load all users by XP.");
            }
        }

    }
}
