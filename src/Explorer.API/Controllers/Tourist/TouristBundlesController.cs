using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/bundles")]
    [ApiController]
    public class TouristBundlesController : ControllerBase
    {
        private readonly IBundleService _bundleService;
        private readonly IBundlePurchaseService _bundlePurchaseService;

        public TouristBundlesController(
            IBundleService bundleService,
            IBundlePurchaseService bundlePurchaseService)
        {
            _bundleService = bundleService;
            _bundlePurchaseService = bundlePurchaseService;
        }

        [HttpGet]
        public ActionResult<List<BundleDto>> GetPublished()
        {
            return Ok(_bundleService.GetPublished());
        }

        /// <summary>
        /// Purchase a bundle by its ID for the logged-in tourist
        /// </summary>
        [HttpPost("{bundleId:long}/purchase")]
        public ActionResult Purchase(long bundleId)
        {
            try
            {
                var touristId = User.PersonId(); // ID trenutno ulogovanog turiste
                _bundlePurchaseService.PurchaseBundle(touristId, bundleId);
                return Ok(new { Message = "Bundle purchased successfully." });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = "Internal server error.", Details = e.Message });
            }
        }
    }
}
