using Explorer.Notes.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Notes.API.Public
{
    public interface INoteService
    {
        List<NoteDto> GetByUserId(long userId);
        List<NoteDto> GetFilteredByUserId(long userId, int? type, string? tag, long? tourId, string? search);
        List<string> GetUserTags(long userId);
        NoteDto Get(long id, long userId);
        NoteDto Create(long userId, CreateNoteDto dto);
        NoteDto Update(long userId, UpdateNoteDto dto);
        NoteDto TogglePin(long id, long userId);
        void Delete(long id, long userId);
    }
}