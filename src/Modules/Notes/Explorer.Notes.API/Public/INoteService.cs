using Explorer.Notes.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Notes.API.Public
{
    public interface INoteService
    {
        List<NoteDto> GetByUserId(long userId);
        NoteDto Get(long id, long userId);
        NoteDto Create(long userId, CreateNoteDto dto);
        NoteDto Update(long userId, UpdateNoteDto dto);
        void Delete(long id, long userId);
    }
}