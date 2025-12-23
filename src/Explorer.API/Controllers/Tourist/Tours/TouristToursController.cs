using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateReview(long tourId, [FromForm] EnhancedReviewCreateRequest request)
        {
            var touristId = long.Parse(User.FindFirst("id")!.Value);

            if (string.IsNullOrWhiteSpace(request.Data))
                return BadRequest("The data field is required.");

            EnhancedReviewDto? dto;
            try
            {
                dto = JsonSerializer.Deserialize<EnhancedReviewDto>(
                    request.Data,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON in data: {ex.Message}");
            }

            if (dto == null) return BadRequest("Invalid review payload.");

            var images = request.Images;

            if (images != null)
            {
                if (images.Count > 5) return BadRequest("Max 5 images allowed.");
                if (images.Any(i => i.Length > 5 * 1024 * 1024)) return BadRequest("Each image must be <= 5MB.");
            }

            var uploaded = new List<EnhancedReviewImageDto>();

            if (images != null && images.Count > 0)
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var folder = Path.Combine(webRoot, "enhanced-review-images");
                Directory.CreateDirectory(folder);

                foreach (var file in images)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    // može i relativno ako hoćeš:
                    var url = $"{Request.Scheme}://{Request.Host}/enhanced-review-images/{fileName}";
                    uploaded.Add(new EnhancedReviewImageDto { Url = url, SizeBytes = file.Length });
                }
            }

            await _enhancedReviewService.CreateReview(tourId, dto, touristId, uploaded);
            return Ok();
        }

        public class EnhancedReviewCreateRequest
        {
            public string Data { get; set; } = string.Empty;
            public List<IFormFile>? Images { get; set; }
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

    }

}
