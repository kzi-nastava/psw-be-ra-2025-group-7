using Explorer.Notes.Core.Domain;
using Explorer.Notes.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Notes.Infrastructure.Database.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly NotesContext _context;

        public NoteRepository(NotesContext context)
        {
            _context = context;
        }

        public List<Note> GetByUserId(long userId)
        {
            return _context.Notes
                .Where(n => n.UserId == userId)
                .ToList();
        }

        public Note? Get(long id)
        {
            return _context.Notes.FirstOrDefault(n => n.Id == id);
        }

        public Note Create(Note note)
        {
            _context.Notes.Add(note);
            _context.SaveChanges();
            return note;
        }

        public Note Update(Note note)
        {
            _context.Notes.Update(note);
            _context.SaveChanges();
            return note;
        }

        public void Delete(Note note)
        {
            _context.Notes.Remove(note);
            _context.SaveChanges();
        }
    }
}