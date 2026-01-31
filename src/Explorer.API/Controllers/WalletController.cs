using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [ApiController]
    [Route("api/payments/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<WalletDto> GetMyWallet()
        {
            var userId = long.Parse(User.FindFirst("id")!.Value);
            return Ok(_walletService.GetWallet(userId));
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult<WalletDto> GetWalletByUserId(long userId)
        {
            return Ok(_walletService.GetWallet(userId));
        }

        [HttpPost("deposit")]
        [Authorize(Policy = "administratorPolicy")]
        public async Task<IActionResult> AddFunds([FromBody] WalletDepositDto dto)
        {
            await _walletService.AddFunds(dto.TouristUserId, dto.Amount);
            return Ok();
        }
    }
}
