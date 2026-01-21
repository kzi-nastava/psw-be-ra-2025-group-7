using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Route("api/sales")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _service;

        public SaleController(ISaleService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("active")]
        public IActionResult GetAllActive()
        {
            return Ok(_service.GetAllActive());
        }

        [HttpPost]
        public IActionResult Create([FromBody] SaleDto sale)
        {
            var authorId = (int)User.PersonId();
            sale.AuthorId = authorId;
            return Ok(_service.Create(sale));
        }

        [HttpPut]
        public IActionResult Update([FromBody] SaleDto sale)
        {
            var authorId = (int)User.PersonId();
            sale.AuthorId = authorId;
            return Ok(_service.Update(sale));
        }

        [HttpPut("{id}/activate")]
        public IActionResult Activate(long id)
        {
            return Ok(_service.Activate(id));
        }

        [HttpDelete("{id}")]
        public IActionResult Archive(long id)
        {
            _service.Archive(id);
            return Ok();
        }
    }
}
