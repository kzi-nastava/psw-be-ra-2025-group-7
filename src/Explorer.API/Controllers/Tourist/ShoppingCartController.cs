using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
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
