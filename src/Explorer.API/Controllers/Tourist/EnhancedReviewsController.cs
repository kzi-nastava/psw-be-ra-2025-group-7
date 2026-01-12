using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tourist/enhanced-reviews")]
    [Authorize(Policy = "touristPolicy")]
    public class EnhancedReviewsController : ControllerBase
    {
        private readonly IEnhancedReviewService _service;

        public EnhancedReviewsController(IEnhancedReviewService service)
        {
            _service = service;
        }

        [HttpPut("{reviewId}/helpful")]
        public async Task<IActionResult> ToggleHelpful(long reviewId)
        {
            var touristId = long.Parse(User.FindFirst("id")!.Value);
            var count = await _service.ToggleHelpful(reviewId, touristId);
            return Ok(new { helpfulCount = count });
        }
    }
}
