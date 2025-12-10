using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Tours
{
    [ApiController]
    [Route("api/tourist/tours")]
    public class TouristToursController : ControllerBase
    {
        private readonly ITouristToursService _service;

        public TouristToursController(ITouristToursService service)
        {
            _service = service;
        }

        [HttpGet("published")]
        public ActionResult<List<TourPreviewDto>> GetPublishedTours()
        {
            return Ok(_service.GetPublishedTours());
        }
    }

}
