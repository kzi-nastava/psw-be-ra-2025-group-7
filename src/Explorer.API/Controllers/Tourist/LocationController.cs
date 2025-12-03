using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public.Tourist;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        private long GetUserId()
        {
            return long.Parse(User.FindFirstValue("id"));
        }

        [HttpGet]
        public ActionResult<TouristLocationDto> Get()
        {
            var result = _locationService.GetLocation(GetUserId());
            return Ok(result);
        }

        [HttpPut]
        public IActionResult Update([FromBody] TouristLocationDto dto)
        {
            var result = _locationService.UpdateLocation(GetUserId(), dto.Latitude, dto.Longitude);
            return Ok(result);
        }

        [HttpGet("nearby-monuments")]
        public ActionResult<List<MonumentDto>> GetNearbyMonuments()
        {
            var result = _locationService.GetNearbyMonuments(GetUserId());
            return Ok(result);
        }
    }
}
