using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [ApiController]
    [Route("api/administration/public-point-requests")]
    public class PublicPointRequestsController : ControllerBase
    {
        private readonly PublicPointRequestAdminService _adminService;
        private readonly IPublicPointRequestRepository _repo;

        public PublicPointRequestsController(
            PublicPointRequestAdminService adminService,
            IPublicPointRequestRepository repo)
        {
            _adminService = adminService;
            _repo = repo;
        }

        // ✅ LIST (Pending/Approved/Rejected)
        [HttpGet]
        public ActionResult<PagedResult<PublicPointRequest>> GetPaged(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] PublicPointRequestStatus? status = null)
        {
            return Ok(_repo.GetPaged(page, pageSize, status));
        }

        // ✅ APPROVE
        [HttpPut("{id:long}/approve")]
        public IActionResult Approve(long id, [FromBody] ApprovePublicPointRequestDto dto)
        {
            _adminService.Approve(id, dto?.Comment);
            return Ok();
        }

        // ✅ REJECT
        [HttpPut("{id:long}/reject")]
        public IActionResult Reject(long id, [FromBody] RejectPublicPointRequestDto dto)
        {
            _adminService.Reject(id, dto.Comment);
            return Ok();
        }
    }

    public class ApprovePublicPointRequestDto
    {
        public string? Comment { get; set; }
    }

    public class RejectPublicPointRequestDto
    {
        public string Comment { get; set; } = string.Empty;
    }
}
