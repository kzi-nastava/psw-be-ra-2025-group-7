using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [ApiController]
    [Route("api/payments/crypto")]
    public class CryptoPaymentController : ControllerBase
    {
        private readonly ICryptoPaymentService _cryptoPaymentService;

        public CryptoPaymentController(ICryptoPaymentService cryptoPaymentService)
        {
            _cryptoPaymentService = cryptoPaymentService;
        }

        [HttpGet("wallet-info")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<CryptoWalletInfoDto> GetWalletInfo()
        {
            var walletInfo = _cryptoPaymentService.GetWalletInfo();
            return Ok(walletInfo);
        }

        [HttpPost("register-address")]
        [Authorize(Policy = "touristPolicy")]
        public IActionResult RegisterSolanaAddress([FromBody] RegisterSolanaAddressDto dto)
        {
            var userId = long.Parse(User.FindFirst("id")!.Value);
            _cryptoPaymentService.RegisterSolanaAddress(userId, dto.SolanaAddress);
            return Ok(new { message = "Solana address registered successfully." });
        }

        [HttpGet("my-solana-address")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<UserSolanaAddressDto> GetMySolanaAddress()
        {
            var userId = long.Parse(User.FindFirst("id")!.Value);
            var result = _cryptoPaymentService.GetUserSolanaAddress(userId);
            return Ok(result);
        }

        [HttpPost("submit-transaction")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<CryptoDepositRequestDto> SubmitTransaction([FromBody] SubmitCryptoTransactionDto dto)
        {
            var userId = long.Parse(User.FindFirst("id")!.Value);
            var result = _cryptoPaymentService.SubmitTransaction(userId, dto.TransactionId);
            return Ok(result);
        }

        [HttpGet("my-deposits")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<List<CryptoDepositRequestDto>> GetMyDeposits()
        {
            var userId = long.Parse(User.FindFirst("id")!.Value);
            var deposits = _cryptoPaymentService.GetUserDepositHistory(userId);
            return Ok(deposits);
        }

        [HttpPost("process-pending")]
        [Authorize(Policy = "administratorPolicy")]
        public async Task<IActionResult> ProcessPendingDeposits()
        {
            await _cryptoPaymentService.ProcessPendingDeposits();
            return Ok(new { message = "Pending deposits processed successfully." });
        }
    }
}
