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
        public ActionResult<List<NoteDto>> GetMyNotes(
            [FromQuery] int? type,
            [FromQuery] string? tag,
            [FromQuery] long? tourId,
            [FromQuery] string? search)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            // If any filter is applied, use filtered query
            if (type.HasValue || !string.IsNullOrWhiteSpace(tag) || tourId.HasValue || !string.IsNullOrWhiteSpace(search))
            {
                var filteredNotes = _noteService.GetFilteredByUserId(userId, type, tag, tourId, search);
                return Ok(filteredNotes);
            }

            // Otherwise, use regular query
            var notes = _noteService.GetByUserId(userId);
            return Ok(notes);
        }

        [HttpGet("tags")]
        public ActionResult<List<string>> GetMyTags()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var tags = _noteService.GetUserTags(userId);
            return Ok(tags);
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

        [HttpPut("{id}/pin")]
        public ActionResult<NoteDto> TogglePin(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();
            var updated = _noteService.TogglePin(id, userId);
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
    }
}