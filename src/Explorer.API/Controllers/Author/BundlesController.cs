using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/bundles")]
    [ApiController]
    public class BundlesController : ControllerBase
    {
        private readonly IBundleService _bundleService;

        public BundlesController(IBundleService bundleService)
        {
            _bundleService = bundleService;
        }

        [HttpGet]
        public ActionResult<List<BundleDto>> GetAll()
        {
            var authorId = User.PersonId();
            return Ok(_bundleService.GetByAuthor(authorId));
        }

        [HttpGet("{id:long}")]
        public ActionResult<BundleDto> Get(long id)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_bundleService.Get(id, authorId));
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPost]
        public ActionResult<BundleDto> Create([FromBody] CreateBundleDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_bundleService.Create(authorId, dto));
            }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPut("{id:long}")]
        public ActionResult<BundleDto> Update(long id, [FromBody] UpdateBundleDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_bundleService.Update(id, authorId, dto));
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _bundleService.Delete(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        // UI helper: vrati sumu cena izabranih tura (validira da su autorove)
        [HttpPost("preview-total")]
        public ActionResult<BundlePreviewResponseDto> PreviewTotal([FromBody] BundlePreviewRequestDto dto)
        {
            try
            {
                var authorId = User.PersonId();
                return Ok(_bundleService.PreviewTotal(authorId, dto));
            }
            catch (ArgumentException e) { return BadRequest(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }


        [HttpPut("{id:long}/publish")]
        public ActionResult Publish(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _bundleService.Publish(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }

        [HttpPut("{id:long}/archive")]
        public ActionResult Archive(long id)
        {
            try
            {
                var authorId = User.PersonId();
                _bundleService.Archive(id, authorId);
                return Ok();
            }
            catch (NotFoundException e) { return NotFound(e.Message); }
            catch (InvalidOperationException e) { return BadRequest(e.Message); }
        }
    }
}
