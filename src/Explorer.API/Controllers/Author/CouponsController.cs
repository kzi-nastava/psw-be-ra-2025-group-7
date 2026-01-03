using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/coupons")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public ActionResult<List<CouponDto>> GetAll()
        {
            var authorId = User.PersonId();
            return Ok(_couponService.GetByAuthor(authorId));
        }

        [HttpGet("active")]
        public ActionResult<List<CouponDto>> GetMyCoupons(bool onlyActive = false)
        {
            var authorId = User.PersonId();
            var coupons = _couponService.GetByAuthor(authorId);
            
            if (onlyActive)
            {
                coupons = coupons.Where(c => c.IsActive).ToList();
            }
            
            return Ok(coupons);
        }

        [HttpGet("{id:long}")]
        public ActionResult<CouponDto> Get(long id)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_couponService.Get(id, authorId));
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPost]
        public ActionResult<CouponDto> Create([FromBody] CreateCouponDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_couponService.Create(authorId, dto));
            }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPut("{id:long}")]
        public ActionResult<CouponDto> Update(long id, [FromBody] UpdateCouponDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_couponService.Update(id, authorId, dto));
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPatch("{id:long}/deactivate")]
        public ActionResult Deactivate(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _couponService.Deactivate(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPatch("{id:long}/activate")]
        public ActionResult Activate(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _couponService.Activate(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _couponService.Delete(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }
    }
}
