using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tours")]
    [ApiController]
    public class TourAuthoringController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourAuthoringController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var authorId = User.PersonId();
            return Ok(_tourService.GetPagedByAuthor(page, pageSize, authorId));
        }

        [HttpPost]
        public ActionResult<TourDto> Create([FromBody] TourDto tour)
        {
            tour.AuthorId = User.PersonId();
            return Ok(_tourService.Create(tour));
        }

        [HttpPut("{id:long}")]
        public ActionResult<TourDto> Update(long id, [FromBody] TourDto tour)
        {
            tour.Id = id;
            tour.AuthorId = User.PersonId();
            return Ok(_tourService.Update(tour));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var authorId = User.PersonId();
            _tourService.Delete(id, authorId);
            return Ok();
        }

        // ============== Kartica 3 – ključne tačke ==============

        // POST api/author/tours/{tourId}/keypoints
        [HttpPost("{tourId:long}/keypoints")]
        public ActionResult<TourDto> AddKeyPoint(long tourId, [FromBody] KeyPointDto keyPoint)
        {
            var authorId = User.PersonId();
            var result = _tourService.AddKeyPoint(tourId, authorId, keyPoint);
            return Ok(result);
        }

        // PUT api/author/tours/{tourId}/keypoints/{index}
        [HttpPut("{tourId:long}/keypoints/{index:int}")]
        public ActionResult<TourDto> UpdateKeyPoint(long tourId, int index, [FromBody] KeyPointDto keyPoint)
        {
            var authorId = User.PersonId();
            var result = _tourService.UpdateKeyPoint(tourId, authorId, index, keyPoint);
            return Ok(result);
        }

        // DELETE api/author/tours/{tourId}/keypoints/{index}
        [HttpDelete("{tourId:long}/keypoints/{index:int}")]
        public ActionResult<TourDto> RemoveKeyPoint(long tourId, int index)
        {
            var authorId = User.PersonId();
            var result = _tourService.RemoveKeyPoint(tourId, authorId, index);
            return Ok(result);
        }
    }
}
