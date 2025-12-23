using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Tours
{
    [ApiController]
    [Route("api/tourist/tours")]
    public class TouristToursController : ControllerBase
    {
        private readonly ITouristToursService _service;
        private readonly IEnhancedReviewService _enhancedReviewService;


        public TouristToursController(ITouristToursService service, IEnhancedReviewService enhancedReviewService)
        {
            _service = service;
            _enhancedReviewService = enhancedReviewService;
        }

        [HttpGet("published")]
        public ActionResult<List<TourPreviewDto>> GetPublishedTours()
        {
            return Ok(_service.GetPublishedTours());
        }

        [HttpPost("{tourId}/enhanced-reviews")]
        [Authorize(Policy = "touristPolicy")]
        public async Task<IActionResult> CreateReview(long tourId, [FromBody] EnhancedReviewDto dto)
        {
            var touristId = long.Parse(User.FindFirst("id")!.Value);
            await _enhancedReviewService.CreateReview(tourId, dto, touristId);
            return Ok();
        }

        [HttpGet("{tourId}/enhanced-reviews")]
        public async Task<ActionResult<List<EnhancedReviewDto>>> GetReviews(long tourId)
        {
            var result = await _enhancedReviewService.GetReviews(tourId);
            return Ok(result);
        }
    }

}
