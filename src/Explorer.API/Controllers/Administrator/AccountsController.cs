using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [ApiController]
    [Route("api/administrator/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public ActionResult<AccountDto> Create([FromBody] AccountCreateDto dto)
        {
            var created = _accountService.CreateAccount(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        [HttpGet]
        public ActionResult<IEnumerable<AccountDto>> GetAll()
        {
            var accounts = _accountService.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpPut("{id:int}/block")]
        public ActionResult<AccountDto> Block(int id)
        {
            var result = _accountService.BlockAccount(id);
            return Ok(result);
        }
    }
}
