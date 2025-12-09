using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;




namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-preferences")]
    [ApiController]
    public class TourPreferencesController : ControllerBase
    {
        private readonly ITourPreferencesService _service;

        public TourPreferencesController(ITourPreferencesService service)
        {
            _service = service;
        }

        private long GetAuthTouristId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "id" ||
                c.Type == "userId" ||
                c.Type == "sub");

            if (claim == null || !long.TryParse(claim.Value, out long touristId))
            {
                var allClaims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
                throw new UnauthorizedAccessException($"Invalid token. Available claims: {allClaims}");
            }

            return touristId;
        }


        // GET api/tourist/preferences/
        [HttpGet]
        public ActionResult<TourPreferencesDto> Get()
        {
            try
            {
                var touristId = GetAuthTouristId();
                var result = _service.GetByTouristId(touristId);

                if (result == null)
                {
                    return NotFound("You have zero preferences.");
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        // POST api/tourist/preferences
        [HttpPost]
        public ActionResult<TourPreferencesDto> Create([FromBody] TourPreferencesDto tourPreferencesDto)
        {
            try
            {
                var touristId = GetAuthTouristId();
                tourPreferencesDto.TouristId = touristId;

                var existing = _service.GetByTouristId(touristId);
                if (existing != null)
                {
                    return BadRequest("You already have that same preference.");
                }

                var created = _service.Create(tourPreferencesDto);
                return Ok(created);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT api/tourist/preferences/
        [HttpPut("{id:long}")]
        public ActionResult<TourPreferencesDto> Update([FromBody] TourPreferencesDto tourPreferencesDto)
        {
            try
            {
                var touristId = GetAuthTouristId();

                var existing = _service.GetByTouristId(touristId);
                if (existing == null)
                {
                    return NotFound("This preferences does not exist.");
                }

                tourPreferencesDto.Id = existing.Id;
                tourPreferencesDto.TouristId = touristId;

                var updated = _service.Update(tourPreferencesDto);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE api/tourist/preferences/
        [HttpDelete]
        public IActionResult Delete(long id)
        {
            try
            {
                var touristId = GetAuthTouristId();

                var existing = _service.GetByTouristId(touristId);
                if (existing != null)
                {
                    _service.Delete(existing.Id);
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

    }
}
