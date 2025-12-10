using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/public-point-requests")]
    [ApiController]
    public class PublicPointRequestController : ControllerBase
    {
        private readonly IPublicPointRequestService _requestService;

        public PublicPointRequestController(IPublicPointRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public ActionResult<PagedResult<PublicPointRequestDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _requestService.GetPaged(page, pageSize);
            return Ok(result);
        }

        [HttpGet("pending")]
        public ActionResult<List<PublicPointRequestDto>> GetAllPending()
        {
            var result = _requestService.GetAllPending();
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public ActionResult<PublicPointRequestDto> Approve(long id, [FromBody] ProcessRequestDto dto)
        {
            var result = _requestService.ApproveRequest(id, dto.Comment);
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public ActionResult<PublicPointRequestDto> Reject(long id, [FromBody] ProcessRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comment))
            {
                return BadRequest("Comment is required when rejecting a request.");
            }

            var result = _requestService.RejectRequest(id, dto.Comment);
            return Ok(result);
        }
    }
}