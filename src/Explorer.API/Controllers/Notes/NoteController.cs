using System.Security.Claims;
using Explorer.Notes.API.Dtos;
using Explorer.Notes.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist.Notes
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/notes")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User?.FindFirst("id") ?? User?.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !long.TryParse(idClaim.Value, out var userId))
                return 0;
            return userId;
        }

        [HttpGet]
        public ActionResult<List<NoteDto>> GetMyNotes()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var notes = _noteService.GetByUserId(userId);
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public ActionResult<NoteDto> Get(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var note = _noteService.Get(id, userId);
            return Ok(note);
        }

        [HttpPost]
        public ActionResult<NoteDto> Create([FromBody] CreateNoteDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var created = _noteService.Create(userId, dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public ActionResult<NoteDto> Update(long id, [FromBody] UpdateNoteDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            dto.Id = id;
            var updated = _noteService.Update(userId, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            _noteService.Delete(id, userId);
            return Ok(new { message = "Note deleted successfully." });
        }

        [HttpPut("{id}/pin")]
        public ActionResult<NoteDto> TogglePin(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var result = _noteService.TogglePin(id, userId);
            return Ok(result);
        }
    }
}