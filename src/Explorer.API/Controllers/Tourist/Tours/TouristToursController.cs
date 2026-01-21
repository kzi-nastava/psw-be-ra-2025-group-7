using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Explorer.API.Controllers.Tourist.Tours
{
    [ApiController]
    [Route("api/tourist/tours")]
    public class TouristToursController : ControllerBase
    {
        private readonly ITouristToursService _service;
        private readonly IEnhancedReviewService _enhancedReviewService;
        private readonly IWebHostEnvironment _env;

        public TouristToursController(ITouristToursService service, IEnhancedReviewService enhancedReviewService, IWebHostEnvironment env)
        {
            _service = service;
            _enhancedReviewService = enhancedReviewService;
            _env = env;
        }

        [HttpGet("published")]
        public ActionResult<List<TourPreviewDto>> GetPublishedTours()
        {
            return Ok(_service.GetPublishedTours());
        }

        [HttpPost("{tourId}/enhanced-reviews")]
        [Authorize(Policy = "touristPolicy")]
        public async Task<ActionResult<long>> CreateReview(long tourId, [FromBody] EnhancedReviewDto dto)
        {
            var touristId = long.Parse(User.FindFirst("id")!.Value);
            var reviewId = await _enhancedReviewService.CreateReview(tourId, dto, touristId);
            return Ok(reviewId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("enhanced-reviews/{reviewId}/images")]
        [Authorize(Policy = "touristPolicy")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImages(long reviewId, [FromForm] List<IFormFile> images)
        {
            if (images == null || images.Count == 0) return BadRequest("No images uploaded.");
            if (images.Count > 5) return BadRequest("Max 5 images allowed.");
            if (images.Any(i => i.Length > 5 * 1024 * 1024)) return BadRequest("Each image must be <= 5MB.");

            var uploaded = new List<EnhancedReviewImageDto>();

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(webRoot, "enhanced-review-images");
            Directory.CreateDirectory(folder);

            foreach (var file in images)
            {
                var ext = Path.GetExtension(file.FileName);

                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { ".jpg", ".jpeg", ".png", ".webp"};

                if (!allowed.Contains(ext))
                    return BadRequest("Only .jpg, .jpeg, .png, .webp files are allowed.");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                // RELATIVAN URL (front može da prefiksuje baseUrl)
                var url = $"/enhanced-review-images/{fileName}";
                uploaded.Add(new EnhancedReviewImageDto { Url = url, SizeBytes = file.Length });
            }

            await _enhancedReviewService.AddImages(reviewId, uploaded);
            return Ok(uploaded.Select(x => x.Url).ToList());
        }

        [HttpGet("{tourId}/enhanced-reviews")]
        public async Task<ActionResult<List<EnhancedReviewDto>>> GetReviews(long tourId)
        {
            var result = await _enhancedReviewService.GetReviews(tourId);
            return Ok(result);
        }

        [HttpGet("{tourId}/review-summary")]
        public async Task<ActionResult<ReviewSummaryDto>> GetSummary(long tourId)
        {
            var result = await _enhancedReviewService.GetSummary(tourId);
            return Ok(result);
        }

        [HttpPut("enhanced-reviews/{reviewId}/helpful")]
        [Authorize(Policy = "touristPolicy")]
        public async Task<ActionResult<int>> ToggleHelpful(long reviewId)
        {
            var touristId = long.Parse(User.FindFirst("id")!.Value);
            var count = await _enhancedReviewService.ToggleHelpful(reviewId, touristId);
            return Ok(count);
        }


    }
}