using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/wheel")]
    public class WheelOfFortuneController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IWalletInternalService _walletService;

        public WheelOfFortuneController(IUserProfileService userProfileService, IWalletInternalService walletService)
        {
            _userProfileService = userProfileService;
            _walletService = walletService;
        }

        [HttpGet("can-spin")]
        public ActionResult<bool> CanSpin()
        {
            try
            {
                var userId = long.Parse(User.FindFirst("id")!.Value);
                var topUsers = _userProfileService.GetTopUsersByXp(3);

                bool isEligible = topUsers.Any(u => u.UserId == userId);

                return Ok(isEligible);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("spin")]
        public ActionResult<decimal> Spin()
        {
            try
            {
                var userId = long.Parse(User.FindFirst("id")!.Value);
                var topUsers = _userProfileService.GetTopUsersByXp(3);

                if (!topUsers.Any(u => u.UserId == userId))
                {
                    return BadRequest("You are not eligible to spin the wheel.");
                }

                // Random reward
                var amounts = new[] { 50, 100, 200, 500, 1000, 1500 };
                var reward = amounts[Random.Shared.Next(amounts.Length)];

                _walletService.AddFunds(userId, reward);

                return Ok(reward);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
