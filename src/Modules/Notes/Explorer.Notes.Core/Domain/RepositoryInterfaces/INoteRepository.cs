using System.Collections.Generic;

namespace Explorer.Notes.Core.Domain.RepositoryInterfaces
{
    public interface INoteRepository
    {
        List<Note> GetByUserId(long userId);
        Note? Get(long id);
        Note Create(Note note);
        Note Update(Note note);
        void Delete(Note note);
    }
}