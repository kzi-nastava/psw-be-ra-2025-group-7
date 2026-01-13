using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/coupons")]
    [ApiController]
    public class TouristCouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public TouristCouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("validate/{code}")]
        public ActionResult<CouponDto> ValidateCoupon(string code)
        {
            var coupon = _couponService.ValidateCoupon(code);
            
            if (coupon == null)
                return NotFound(new { message = "Invalid or expired coupon code." });

            return Ok(coupon);
        }
    }
}
