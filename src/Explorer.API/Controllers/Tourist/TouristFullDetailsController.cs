using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.Core.UseCases.Tourist; // <- use core interface namespace




namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tours")]
    [ApiController]
    

    public class ToursFullDetailsController : ControllerBase
    {
        private readonly ITourReadService _tourReadService;
        public ToursFullDetailsController(ITourReadService tourReadService)
        {
            _tourReadService = tourReadService;
        }

        [HttpGet("{tourId:long}/details")]
        public async Task<ActionResult<TourFullForTouristDto>> GetFullDetails(long tourId)
        {
            var dto = await _tourReadService.GetFullForTouristAsync(tourId);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}