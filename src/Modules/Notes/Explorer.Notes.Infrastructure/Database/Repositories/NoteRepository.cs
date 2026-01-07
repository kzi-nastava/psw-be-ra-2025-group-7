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

        public List<Note> GetFilteredByUserId(long userId, int? type, string? tag, long? tourId, string? search)
        {
            var query = _context.Notes.Where(n => n.UserId == userId);

            // Filter by type
            if (type.HasValue)
            {
                query = query.Where(n => (int)n.Type == type.Value);
            }

            // Filter by tourId
            if (tourId.HasValue)
            {
                query = query.Where(n => n.TourId == tourId.Value);
            }

            // Search in title and content (case-insensitive)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(lowerSearch) ||
                    n.Content.ToLower().Contains(lowerSearch));
            }

            // Execute query to load into memory
            var results = query.ToList();

            // Filter by tag in memory (because JSONB column can't be translated by EF Core)
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var lowerTag = tag.ToLower();
                results = results.Where(n => n.Tags.Any(t => t.ToLower() == lowerTag)).ToList();
            }

            return results;
        }

        public List<string> GetUserTags(long userId)
        {
            // Load all notes into memory first, then extract tags
            var notes = _context.Notes
                .Where(n => n.UserId == userId)
                .ToList();

            return notes
                .SelectMany(n => n.Tags)
                .Distinct()
                .OrderBy(t => t)
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