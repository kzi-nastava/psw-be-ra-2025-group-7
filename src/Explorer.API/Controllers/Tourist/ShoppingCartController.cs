using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public ActionResult<ShoppingCartDto> GetMyCart()
        {
            var touristId = GetTouristIdFromToken();
            var result = _shoppingCartService.GetByTouristId(touristId);
            return Ok(result);
        }

        [HttpPost("items")]
        public ActionResult<ShoppingCartDto> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                var touristId = GetTouristIdFromToken();
                var result = _shoppingCartService.AddToCart(touristId, dto.TourId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("items/{orderItemId:long}")]
        public ActionResult<ShoppingCartDto> RemoveFromCart(long orderItemId)
        {
            try
            {
                var touristId = GetTouristIdFromToken();
                var result = _shoppingCartService.RemoveFromCart(touristId, orderItemId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public ActionResult ClearCart()
        {
            var touristId = GetTouristIdFromToken();
            _shoppingCartService.ClearCart(touristId);
            return Ok();
        }

        /// <summary>
        /// Purchases all items in the cart by creating TourPurchaseTokens.
        /// This endpoint implements the DDD approach where the ShoppingCart aggregate
        /// validates the purchase and the service orchestrates token creation.
        /// All items are converted to purchase tokens and the cart is cleared.
        /// </summary>
        [HttpPost("purchase")]
        public ActionResult<List<TourPurchaseTokenDto>> PurchaseCart()
        {
            try
            {
                var touristId = GetTouristIdFromToken();
                var tokens = _shoppingCartService.PurchaseCart(touristId);
                return Ok(tokens);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private long GetTouristIdFromToken()
        {
            try
            {
                return User.PersonId();
            }
            catch
            {
                return 0;
            }
        }
    }
}
