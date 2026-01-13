using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


// Mozda kada se doda Tour Controller u Tourist delu, ovaj namespace treba da se promeni

namespace Explorer.API.Controllers.Tourist.Tours
{
    [ApiController]
    [Route("api/tourist/tours/search")]
    public class TourSearchController : ControllerBase
    {
        private readonly ITourSearchService _tourSearchService;

        public TourSearchController(ITourSearchService tourSearchService)
        {
            _tourSearchService = tourSearchService;
        }


        [HttpGet("by-location")]
        [AllowAnonymous]
        public ActionResult<List<TourDto>> SearchByLocation(
     [FromQuery] double lat,
     [FromQuery] double lon,
     [FromQuery] double radiusKm)
        {
            if (radiusKm <= 0)
            {
                return BadRequest("Radius must be greater than zero.");
            }

            var query = new TourLocationSearchDto
            {
                Latitude = lat,
                Longitude = lon,
                RadiusKm = radiusKm
            };

            var result = _tourSearchService.SearchByLocation(query);

            // ✅ Turisti vide samo odobrene public keypointove
            foreach (var tour in result)
            {
                if (tour.KeyPoints != null)
                {
                    tour.KeyPoints = tour.KeyPoints
                        .Where(kp => kp.IsPublic)
                        .ToList();
                }
            }

            return Ok(result);
        }

    }
}