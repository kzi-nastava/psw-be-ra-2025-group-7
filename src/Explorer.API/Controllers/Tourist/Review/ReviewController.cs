using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        // POST: create review
        [HttpPost]
        public ActionResult<ReviewDto> Create([FromBody] CreateReviewDto dto)
        {
            return Ok(_service.CreateReview(dto));
        }

        // GET: get own review
        [HttpGet("{personId}")]
        public ActionResult<ReviewDto> GetMyReview(long personId)
        {
            return Ok(_service.GetMyReview(personId));
        }

        // PUT: update review
        [HttpPut("{reviewId}")]
        public ActionResult<ReviewDto> Update(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            return Ok(_service.UpdateReview(reviewId, dto));
        }

        // DELETE: delete review
        [HttpDelete("{reviewId}")]
        public IActionResult Delete(int reviewId)
        {
            _service.DeleteReview(reviewId);
            return Ok();
        }
    }
}
